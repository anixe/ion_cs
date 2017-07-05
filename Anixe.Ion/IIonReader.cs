using System;

namespace Anixe.Ion
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to ION data.
    /// </summary>
    public interface IIonReader : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on section header.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '['; otherwise, <c>false</c>.</value>
        bool IsSectionHeader { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on property.
        /// </summary>
        /// <value><c>true</c> if other boolean properties are false; otherwise, <c>false</c>.</value>
        bool IsProperty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on comment.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '#'; otherwise, <c>false</c>.</value>
        bool IsComment { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table headers row.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|' and current table header was not already passed; otherwise, <c>false</c>.</value>
        bool IsTableHeaderRow { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table row.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|'; otherwise, <c>false</c>.</value>
        bool IsTableRow { get;}

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table header separator row. IMPORTANT: we recognize this 
        /// property as a combination of first two characters as "|-". Fill your table rows without '-' character.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|-'; otherwise, <c>false</c>.</value>
        bool IsTableHeaderSeparatorRow { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table row with data.
        /// </summary>
        /// <value><c>true</c> if IonReader CurrentLine first character is equal to '|' and current table header was already passed; otherwise, <c>false</c>.</value>
        bool IsTableDataRow { get; }

        /// <summary>
        /// Gets a value indicating whether a IonReader CurrentLine is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <value><c>true</c> if string.IsNullOrWhiteSpace(CurrentLine); otherwise, <c>false</c>.</value>
        bool IsEmptyLine { get; }

        /// <summary>
        /// Gets the current line value.
        /// </summary>
        /// <value>The current line.</value>
        string CurrentLine { get; }

        /// <summary>
        /// Gets the name of current section. It is changing only when CurrentLine is on section header.
        /// </summary>
        /// <value>The current section.</value>
        string CurrentSection { get; }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <value>The current line number.</value>
        int CurrentLineNumber { get; }

        /// <summary>
        /// Read this instance.
        /// </summary>
        /// <returns><c>true</c> if the next line was read successfully; otherwise,<c>false</c>.</returns>
        bool Read();
    }
}

