using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace Anixe.Ion
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to ION data.
    /// </summary>
    internal class IonReader : IIonReader
    {
        private bool disposed;
        private bool passedCurrentTableHeaderRow;
        private Stream stream;
        private readonly bool leaveOpen;
        private readonly StringBuilder sb;
        private readonly ArrayPool<char> charPool;
        private readonly CurrentLineVerifier currentLineVerifier;
        private readonly SectionHeaderReader sectionHeaderReader;
        private char[]? rentedCharBuffer;
        private readonly byte[] byteBuff;
        private readonly char[] charBuff;
        private readonly byte[] preamble;
        private int buffIndex = 0;
        private int charsRead = 0;
        private readonly Encoding enc = Encoding.UTF8;
        private bool checkBOM;

        /// <summary>
        /// It initialized IonReader
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="currentLineVerifier">Object which verifies state based on current line</param>
        /// <param name="sectionHeaderReader">Object which reads sections</param>
        /// <param name="leaveOpen">True if the stream should be open after Dispose()</param>
        /// <param name="charPool">Provide own System.Buffers.ArrayPool<char> instance. If null then System.Buffers.ArrayPool<char>.Shared will be used</param>
        public IonReader(Stream stream, CurrentLineVerifier currentLineVerifier, SectionHeaderReader sectionHeaderReader, bool leaveOpen, ArrayPool<char>? charPool = null)
        {
            this.stream = stream;
            this.disposed = false;
            this.currentLineVerifier = currentLineVerifier;
            this.sectionHeaderReader = sectionHeaderReader;
            this.CurrentLineNumber = 0;
            this.leaveOpen = leaveOpen;
            this.sb = new StringBuilder();
            this.charPool = charPool ?? ArrayPool<char>.Shared;
            this.rentedCharBuffer = null;
            this.byteBuff = ArrayPool<byte>.Shared.Rent(1024);
            this.charBuff = ArrayPool<char>.Shared.Rent(enc.GetMaxByteCount(byteBuff.Length));
            this.preamble = enc.GetPreamble();
            this.checkBOM = this.stream.CanSeek ? this.stream.Position == 0 : true;
        }

        #region IIonReader members

        /// <inheritdoc/>
        public bool IsSectionHeader => this.currentLineVerifier.IsSectionHeader(CurrentRawLine);

        /// <inheritdoc/>
        public bool IsProperty => this.currentLineVerifier.IsProperty(CurrentRawLine);

        /// <inheritdoc/>
        public bool IsComment => this.currentLineVerifier.IsComment(CurrentRawLine);

        /// <inheritdoc/>
        public bool IsTableRow => this.currentLineVerifier.IsTableRow(CurrentRawLine);

        /// <inheritdoc/>
        public bool IsTableHeaderRow => this.currentLineVerifier.IsTableHeaderRow(CurrentRawLine, passedCurrentTableHeaderRow);

        /// <inheritdoc/>
        public bool IsTableHeaderSeparatorRow => this.currentLineVerifier.IsTableHeaderSeparatorRow(CurrentRawLine);

        /// <inheritdoc/>
        public bool IsTableDataRow => this.currentLineVerifier.IsTableDataRow(CurrentRawLine, passedCurrentTableHeaderRow);

        /// <inheritdoc/>
        public bool IsEmptyLine => this.currentLineVerifier.IsEmptyLine(CurrentRawLine);

        /// <inheritdoc/>
        public string CurrentLine => new string(CurrentRawLine.Array, CurrentRawLine.Offset, CurrentRawLine.Count);

        /// <inheritdoc/>
        public ArraySegment<char> CurrentRawLine { get; private set; }

        /// <inheritdoc/>
        public string? CurrentSection { get; private set; }

        /// <inheritdoc/>
        public int CurrentLineNumber { get; private set; }

        /// <inheritdoc/>
        public bool Read()
        {
            if(!this.stream.CanRead)
            {
                ResetFields();
                return false;
            }
            try
            {
                if(!ReadLine() && sb.Length == 0)
                {
                    ResetFields();
                    return false;
                }
                if(sb.Length == 0)
                {
                    CurrentRawLine = ArraySegment<char>.Empty;
                }
                else
                {
                    PrepareBuffer(sb.Length);
                    sb.CopyTo(0, rentedCharBuffer, 0, sb.Length);
                    CurrentRawLine = new ArraySegment<char>(rentedCharBuffer, 0, sb.Length);
                }
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
                var tmp = this.sectionHeaderReader.Read(CurrentRawLine);
                CurrentSection = new string(tmp.Array, tmp.Offset, tmp.Count);
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
            this.sb.Clear();
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
            var tmp = buffIndex;
            for (int i = buffIndex; i < this.charsRead; i++)
            {
                var character = charBuff[i];
                if (character == '\n')
                {
                    if (i > buffIndex)
                    {
                        int x = charBuff[i - 1] == '\r' ? 1 : 0;
                        this.sb.Append(charBuff, buffIndex, i - buffIndex - x);
                    }
                    buffIndex = i + 1;
                    return true;
                }
            }
            this.sb.Append(charBuff, tmp, this.charsRead - tmp);
            return false;
        }

        private void PrepareBuffer(int length)
        {
            if(this.rentedCharBuffer == null)
            {
                this.rentedCharBuffer = charPool.Rent(length);
            }
            else if(this.rentedCharBuffer.Length < length)
            {
                var tmp = this.rentedCharBuffer;
                this.rentedCharBuffer = charPool.Rent(length);
                charPool.Return(tmp, true);
            }
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

                if(this.rentedCharBuffer != null)
                {
                    charPool.Return(this.rentedCharBuffer, true);
                    this.rentedCharBuffer = null;
                }

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
