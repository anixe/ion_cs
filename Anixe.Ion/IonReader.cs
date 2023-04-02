using Anixe.Ion.Helpers;
using System;
using System.Buffers;
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
        private readonly byte[] preamble;
        private int buffIndex = 0;
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
            this.charBuff = ArrayPool<char>.Shared.Rent(enc.GetMaxByteCount(byteBuff.Length));
            this.preamble = enc.GetPreamble();
            this.checkBOM = this.stream.CanSeek ? this.stream.Position == 0 : true;
        }

        #region IIonReader members

        public bool IsSectionHeader => this.currentLineVerifier.IsSectionHeader(CurrentRawLine);

        public bool IsProperty => this.currentLineVerifier.IsProperty(CurrentRawLine);

        public bool IsComment => this.currentLineVerifier.IsComment(CurrentRawLine);

        public bool IsTableRow => this.currentLineVerifier.IsTableRow(CurrentRawLine);

        public bool IsTableHeaderRow => this.currentLineVerifier.IsTableHeaderRow(CurrentRawLine, passedCurrentTableHeaderRow);

        public bool IsTableHeaderSeparatorRow => this.currentLineVerifier.IsTableHeaderSeparatorRow(CurrentRawLine);

        public bool IsTableDataRow => this.currentLineVerifier.IsTableDataRow(CurrentRawLine, passedCurrentTableHeaderRow);

        public bool IsEmptyLine => this.currentLineVerifier.IsEmptyLine(CurrentRawLine);

        public string CurrentLine => new string(CurrentRawLine.Array, CurrentRawLine.Offset, CurrentRawLine.Count);

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
                CurrentSection = new string(headerSegment.Array, headerSegment.Offset, headerSegment.Count);
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
                var offset = GetIndex();
                this.charsRead = enc.GetChars(byteBuff, offset, bytesRead - offset, charBuff, 0);
                if(CopyTillEOL())
                {
                    return !endOfFile;
                }
            }
            return !endOfFile;
        }

        private int GetIndex()
        {
            var retval = 0;
            if(!this.checkBOM)
            {
                retval = 0;
            }
            else
            {
                if(this.byteBuff[0] == this.preamble[0] &&
                this.byteBuff[1] == this.preamble[1] &&
                this.byteBuff[2] == this.preamble[2])
                {
                    retval = this.preamble.Length;
                }
            }
            this.checkBOM = false;
            return retval;
        }

        private bool CopyTillEOL()
        {
            var startIdx = buffIndex;
            for (int i = buffIndex; i < this.charsRead; i++)
            {
                var character = charBuff[i];
                if (character == '\n')
                {
                    if (i > buffIndex)
                    {
                        int carriageReturnLength = charBuff[i - 1] == '\r' ? 1 : 0;
                        this.lineBufferWriter.Write(charBuff, buffIndex, i - buffIndex - carriageReturnLength);
                    }
                    buffIndex = i + 1;
                    return true;
                }
            }
            this.lineBufferWriter.Write(this.charBuff, startIdx, this.charsRead - startIdx);
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
        }

        #endregion
    }
}
