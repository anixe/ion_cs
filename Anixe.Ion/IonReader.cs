using Anixe.Ion.Helpers;
using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Anixe.Ion
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to ION data.
    /// </summary>
    internal sealed class IonReader : IIonReader
    {
        private bool disposed;
        private bool passedCurrentTableHeaderRow;
        private Stream stream;
        private readonly bool leaveOpen;
        private readonly CurrentLineVerifier currentLineVerifier;
        private readonly SectionHeaderReader sectionHeaderReader;
        private BufferWriter lineBufferWriter;
        private readonly byte[] byteBuff;
        private readonly char[] charBuff;
        private int buffIndex = 0; // index into charBuff (not byteBuff) for remaining chars
        private int charsRead = 0;
        private readonly Encoding enc = Encoding.UTF8;
        private bool checkBOM;

        /// <summary>
        /// Initializes a new instance of <see cref="IonReader"/>.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="currentLineVerifier">Object which verifies state based on current line.</param>
        /// <param name="sectionHeaderReader">Object which reads sections.</param>
        /// <param name="leaveOpen"><see langword="true"/> if the stream should be open after <see cref="Dispose()"/>.</param>
        /// <param name="charPool">Provide own <see cref="ArrayPool{T}.Shared"/> instance. If <see langword="null"/> then <see cref="ArrayPool{T}.Shared"/> is used.</param>
        public IonReader(Stream stream, CurrentLineVerifier currentLineVerifier, SectionHeaderReader sectionHeaderReader, bool leaveOpen, ArrayPool<char>? charPool = null)
        {
            this.stream = stream;
            this.disposed = false;
            this.currentLineVerifier = currentLineVerifier;
            this.sectionHeaderReader = sectionHeaderReader;
            this.CurrentLineNumber = 0;
            this.leaveOpen = leaveOpen;
            this.lineBufferWriter = new BufferWriter(charPool ?? ArrayPool<char>.Shared);
            this.byteBuff = ArrayPool<byte>.Shared.Rent(1024);
            this.charBuff = ArrayPool<char>.Shared.Rent(enc.GetMaxCharCount(byteBuff.Length));
            this.checkBOM = this.stream.CanSeek ? this.stream.Position == 0 : true;
        }

        #region IIonReader members

        [MemberNotNullWhen(true, nameof(CurrentSection))]
        public bool IsSectionHeader => this.currentLineVerifier.IsSectionHeader(CurrentRawLine);

        public bool IsProperty => this.currentLineVerifier.IsProperty(CurrentRawLine);

        public bool IsComment => this.currentLineVerifier.IsComment(CurrentRawLine);

        public bool IsTableRow => this.currentLineVerifier.IsTableRow(CurrentRawLine);

        public bool IsTableHeaderRow => this.currentLineVerifier.IsTableHeaderRow(CurrentRawLine, passedCurrentTableHeaderRow);

        public bool IsTableHeaderSeparatorRow => this.currentLineVerifier.IsTableHeaderSeparatorRow(CurrentRawLine);

        public bool IsTableDataRow => this.currentLineVerifier.IsTableDataRow(CurrentRawLine, passedCurrentTableHeaderRow);

        public bool IsEmptyLine => this.currentLineVerifier.IsEmptyLine(CurrentRawLine);

        public string CurrentLine => new string(CurrentRawLine.Array!, CurrentRawLine.Offset, CurrentRawLine.Count);

        public ArraySegment<char> CurrentRawLine { get; private set; }

        public string? CurrentSection { get; private set; }

        public int CurrentLineNumber { get; private set; }

        public bool Read()
        {
            if(!this.stream.CanRead)
            {
                ResetFields();
                return false;
            }
            try
            {
                if(!ReadLine() && lineBufferWriter.Count == 0)
                {
                    ResetFields();
                    return false;
                }
                CurrentRawLine = this.lineBufferWriter.WrittenSegment;
                CurrentLineNumber++;
                ExposeData();
            }
            catch (Exception exception)
            {
                throw new Exception("File could not be parsed", exception);
            }
            return true;
        }

        private void ExposeData()
        {
            if(IsSectionHeader)
            {
                ArraySegment<char> headerSegment = this.sectionHeaderReader.Read(CurrentRawLine);
                CurrentSection = new string(headerSegment.Array!, headerSegment.Offset, headerSegment.Count);
            }

            if(passedCurrentTableHeaderRow)
            {
                if (!IsTableRow)
                {
                    passedCurrentTableHeaderRow = false;
                }
            }
            else
            {
                if (IsTableHeaderSeparatorRow)
                {
                    passedCurrentTableHeaderRow = true;
                }
            }
        }

        private bool ReadLine()
        {
            bool endOfFile = false;
            this.lineBufferWriter.Clear();
            if(buffIndex > 0)
            {
                if(!CopyTillEOL())
                {
                    buffIndex = 0;
                }
                else
                {
                    return !endOfFile;
                }
            }
            while (this.stream.CanRead && endOfFile == false)
            {
                int bytesRead = this.stream.Read(byteBuff, 0, byteBuff.Length);
                if (bytesRead == 0)
                {
                    endOfFile = true;
                    break;
                }
                var offset = GetIndex(bytesRead);
                this.charsRead = enc.GetChars(byteBuff.AsSpan(offset, bytesRead - offset), charBuff);
                if(CopyTillEOL())
                {
                    return !endOfFile;
                }
            }
            return !endOfFile;
        }

        private int GetIndex(int bytesRead)
        {
            if (!this.checkBOM)
            {
                return 0;
            }

            this.checkBOM = false;

            ReadOnlySpan<byte> preamble = this.enc.Preamble;

            if (!this.byteBuff.AsSpan(0, bytesRead).StartsWith(preamble))
            {
                return 0;
            }

            return preamble.Length;
        }

        /// <summary>
        /// Copies characters from the internal buffer to the line buffer writer until a newline character is
        /// encountered.
        /// </summary>
        /// <remarks>If no newline character is found, all available characters are written to the line
        /// buffer writer and the buffer index remains unchanged for the caller to manage. The method handles both '\n'
        /// and '\r\n' line endings, ensuring that carriage return characters are excluded from the written line when
        /// present. Callers should update the buffer index based on the return value to maintain correct buffer
        /// state.</remarks>
        /// <returns>true if a newline character is found and the line is successfully written; otherwise, false.</returns>

        private bool CopyTillEOL()
        {
            int available = this.charsRead - this.buffIndex;
            if (available <= 0)
            {
                return false;
            }

            var buffSlice = this.charBuff.AsSpan(this.buffIndex, available);
            for (int i = 0; i < buffSlice.Length; i++)
            {
                var character = buffSlice[i];
                if (character == '\n')
                {
                    if (i > 0)
                    {
                        int carriageReturnLength = buffSlice[i - 1] == '\r' ? 1 : 0;
                        // Write from the current buffIndex up to the character before newline (excluding CR if present)
                        this.lineBufferWriter.Write(this.charBuff.AsSpan(this.buffIndex, i - carriageReturnLength));
                    }
                    // Advance buffIndex to the position after the newline in the underlying char buffer
                    this.buffIndex = this.buffIndex + i + 1;
                    return true;
                }
            }

            // No newline found: write all remaining chars and leave buffIndex for caller to reset (or for next pass)
            this.lineBufferWriter.Write(this.charBuff, this.buffIndex, available);
            return false;
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if(!disposed)
            {
                ArrayPool<char>.Shared.Return(this.charBuff);
                ArrayPool<byte>.Shared.Return(this.byteBuff);

                this.lineBufferWriter.Dispose();

                if(!this.leaveOpen && this.stream != null)
                {
                    this.stream.Dispose();
                    this.stream = null!;
                }

                disposed = true;
            }
        }

        #endregion

        #region Private methods

        public void ResetFields()
        {
            CurrentRawLine = default(ArraySegment<char>);
            CurrentSection = string.Empty;
            CurrentSection = null;
        }

        #endregion
    }
}
