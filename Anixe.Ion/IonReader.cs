using System;
using System.Buffers;
using System.Globalization;
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
        private bool leaveOpen;
        private StringBuilder sb;
        private ArrayPool<char> charPool;
        private readonly CurrentLineVerifier currentLineVerifier;
        private readonly SectionHeaderReader sectionHeaderReader;
        private char[] rentedCharBuffer;
        private byte[] byteBuff;
        private char[] charBuff;
        private byte[] preamble;
        private int buffIndex = 0;
        private int charsRead = 0;
        private Encoding enc = Encoding.UTF8;
        private bool checkBOM;


        /// <summary>
        /// It initialized IonReader
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="currentLineVerifier">Object which verifies state based on current line</param>
        /// <param name="sectionHeaderReader">Object which reads sections</param>
        /// <param name="leaveOpen">True if the stream should be open after Dispose()</param>
        /// <param name="charPool">Provide own System.Buffers.ArrayPool<char> instance. If null then System.Buffers.ArrayPool<char>.Shared will be used</param>
        public IonReader(Stream stream, CurrentLineVerifier currentLineVerifier, SectionHeaderReader sectionHeaderReader, bool leaveOpen, ArrayPool<char> charPool = null)
        {
            this.stream = stream;
            this.disposed = false;
            this.currentLineVerifier = currentLineVerifier;
            this.sectionHeaderReader = sectionHeaderReader;
            this.CurrentLineNumber = 0;
            this.leaveOpen = leaveOpen;
            this.sb = new StringBuilder();
            this.charPool = charPool ?? System.Buffers.ArrayPool<char>.Shared;
            this.rentedCharBuffer = null;
            this.byteBuff = new byte[1024];
            this.charBuff = new char[enc.GetMaxByteCount(byteBuff.Length)];
            this.preamble = enc.GetPreamble();
            this.checkBOM = this.stream.CanSeek ? this.stream.Position == 0 : true;
        }

        #region IIonReader members

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on section header.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '['; otherwise, <c>false</c>.</value>
        public bool IsSectionHeader { get { return this.currentLineVerifier.IsSectionHeader(CurrentRawLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on property.
        /// </summary>
        /// <value><c>true</c> if other boolean properties are false; otherwise, <c>false</c>.</value>
        public bool IsProperty { get { return this.currentLineVerifier.IsProperty(CurrentRawLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on comment.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '#'; otherwise, <c>false</c>.</value>
        public bool IsComment { get { return this.currentLineVerifier.IsComment(CurrentRawLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table row.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|'; otherwise, <c>false</c>.</value>
        public bool IsTableRow { get { return this.currentLineVerifier.IsTableRow(CurrentRawLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table headers row.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|' and current table header was not already passed; otherwise, <c>false</c>.</value>
        public bool IsTableHeaderRow { get { return this.currentLineVerifier.IsTableHeaderRow(CurrentRawLine, passedCurrentTableHeaderRow); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table header separator row. IMPORTANT: we recognize this 
        /// property as a combination of first two characters as "|-". Fill your table rows without '-' character.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|-'; otherwise, <c>false</c>.</value>
        public bool IsTableHeaderSeparatorRow { get { return this.currentLineVerifier.IsTableHeaderSeparatorRow(CurrentRawLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table row with data.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|' and current table header was already passed; otherwise, <c>false</c>.</value>
        public bool IsTableDataRow { get { return this.currentLineVerifier.IsTableDataRow(CurrentRawLine, passedCurrentTableHeaderRow); } }

        /// <summary>
        /// Gets a value indicating whether a IonReader CurrentLine is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <value><c>true</c> if string.IsNullOrWhiteSpace(CurrentRawLine); otherwise, <c>false</c>.</value>
        public bool IsEmptyLine { get { return this.currentLineVerifier.IsEmptyLine(CurrentRawLine); } }

        /// <summary>
        /// Gets the current line value. It allocates string from CurrentRawLine.
        /// </summary>
        /// <value>The current line.</value>
        public string CurrentLine
        {
            get
            {
                return new String(CurrentRawLine.Array, CurrentRawLine.Offset, CurrentRawLine.Count);
            }
        }

        /// <summary>
        /// Gets current line as array segment of characters.
        /// The value comes from rented buffer, copy it for private use or it will loose the state after next call of 'Read' method.
        /// </summary>
        /// <value>The current line</value>
        public ArraySegment<char> CurrentRawLine { get; private set; }

        /// <summary>
        /// Gets the name of current section. It is changing only when CurrentLine is on section header.
        /// </summary>
        /// <value>The current section.</value>
        public string CurrentSection { get; private set; }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <value>The current line number.</value>
        public int CurrentLineNumber { get; private set; }

        /// <summary>
        /// Read line from provide stream.
        /// </summary>
        /// <returns><c>true</c> if the next line was read successfully; otherwise,<c>false</c>.</returns>
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
                CurrentSection = new String(tmp.Array, tmp.Offset, tmp.Count);
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
                    var x = 0;
                    if(i > buffIndex)
                    {
                        x = charBuff[i - 1] == '\r' ? 1 : 0;
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
                if(this.rentedCharBuffer != null)
                {
                    charPool.Return(this.rentedCharBuffer, true);
                    this.rentedCharBuffer = null;
                }

                if(!this.leaveOpen && this.stream != null)
                {
                    this.stream.Dispose();
                    this.stream = null;
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
