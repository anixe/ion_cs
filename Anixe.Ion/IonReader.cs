using System;
using System.IO;

namespace Anixe.Ion
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to ION data.
    /// </summary>
    internal class IonReader : IIonReader
    {
        private bool disposed;
        private bool passedCurrentTableHeaderRow;
        private StreamReader streamReader;

        private readonly CurrentLineVerifier currentLineVerifier;
        private readonly SectionHeaderReader sectionHeaderReader;

        public IonReader(StreamReader streamReader, CurrentLineVerifier currentLineVerifier, SectionHeaderReader sectionHeaderReader)
        {
            this.streamReader        = streamReader;
            this.disposed            = false;
            this.currentLineVerifier = currentLineVerifier;
            this.sectionHeaderReader = sectionHeaderReader;
            this.CurrentLineNumber   = 0;
        }

        #region IIonReader members

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on section header.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '['; otherwise, <c>false</c>.</value>
        public bool IsSectionHeader { get { return this.currentLineVerifier.IsSectionHeader(CurrentLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on property.
        /// </summary>
        /// <value><c>true</c> if other boolean properties are false; otherwise, <c>false</c>.</value>
        public bool IsProperty { get { return this.currentLineVerifier.IsProperty(CurrentLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on comment.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '#'; otherwise, <c>false</c>.</value>
        public bool IsComment { get { return this.currentLineVerifier.IsComment(CurrentLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table row.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|'; otherwise, <c>false</c>.</value>
        public bool IsTableRow { get { return this.currentLineVerifier.IsTableRow(CurrentLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table headers row.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|' and current table header was not already passed; otherwise, <c>false</c>.</value>
        public bool IsTableHeaderRow { get { return this.currentLineVerifier.IsTableHeaderRow(CurrentLine, passedCurrentTableHeaderRow); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table header separator row. IMPORTANT: we recognize this 
        /// property as a combination of first two characters as "|-". Fill your table rows without '-' character.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|-'; otherwise, <c>false</c>.</value>
        public bool IsTableHeaderSeparatorRow { get { return this.currentLineVerifier.IsTableHeaderSeparatorRow(CurrentLine); } }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table row with data.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|' and current table header was already passed; otherwise, <c>false</c>.</value>
        public bool IsTableDataRow { get { return this.currentLineVerifier.IsTableDataRow(CurrentLine, passedCurrentTableHeaderRow); } }

        /// <summary>
        /// Gets a value indicating whether a IonReader CurrentLine is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <value><c>true</c> if string.IsNullOrWhiteSpace(CurrentLine); otherwise, <c>false</c>.</value>
        public bool IsEmptyLine { get { return this.currentLineVerifier.IsEmptyLine(CurrentLine); } }

        /// <summary>
        /// Gets the current line value.
        /// </summary>
        /// <value>The current line.</value>
        public string CurrentLine { get; private set; }

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
        /// Read this instance.
        /// </summary>
        /// <returns><c>true</c> if the next line was read successfully; otherwise,<c>false</c>.</returns>
        public bool Read()
        {
            if(this.streamReader.EndOfStream)
            {
                ResetFields();
                return false;
            }

            CurrentLine = this.streamReader.ReadLine();
            CurrentLineNumber++;

            if(IsSectionHeader)
            {
                CurrentSection = this.sectionHeaderReader.Read(CurrentLine);
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
            
            return true;
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
                if(this.streamReader != null)
                {
                    this.streamReader.Dispose();
                    this.streamReader = null;
                }

                disposed = true;
            }
        }

        #endregion

        #region Private methods

        public void ResetFields()
        {
            CurrentLine = string.Empty;
            CurrentSection = string.Empty;
        }

        #endregion
    }
}

