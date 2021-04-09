using System;

namespace Anixe.Ion
{
    /// <summary>
    /// Represents a reader that provides fast, non-cached, forward-only access to ION data.
    /// </summary>
    public interface IIonReader : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this instance of <see cref="IIonReader"/> is currently on section header.
        /// </summary>
        /// <value><see langword="true"/> if <see cref="IIonReader.CurrentLine"/> first character is equal to '['; otherwise, <see langword="false"/>.</value>
        bool IsSectionHeader { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on property.
        /// </summary>
        /// <value><see langword="true"/> if other boolean properties are false; otherwise, <see langword="false"/>.</value>
        bool IsProperty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on comment.
        /// </summary>
        /// <value><see langword="true"/> if <see cref="IIonReader.CurrentLine"/> first character is equal to '#'; otherwise, <see langword="false"/>.</value>
        bool IsComment { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of IonReader is currently on table headers row.
        /// </summary>
        /// <value><see langword="true"/> if <see cref="IIonReader.CurrentLine"/> first character is equal to '|' and current table header was not already passed; otherwise, <see langword="false"/>.</value>
        bool IsTableHeaderRow { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of <see cref="IIonReader"/> is currently on table row.
        /// </summary>
        /// <value><see langword="true"/> if <see cref="IIonReader.CurrentLine"/> first character is equal to '|'; otherwise, <see langword="false"/>.</value>
        bool IsTableRow { get;}

        /// <summary>
        /// Gets a value indicating whether this instance of <see cref="IIonReader"/> is currently on table header separator row. IMPORTANT: we recognize this
        /// property as a combination of first two characters as "|-". Fill your table rows without '-' character.
        /// </summary>
        /// <value><see langword="true"/> if <see cref="IIonReader.CurrentLine"/> first character is equal to '|-'; otherwise, <see langword="false"/>.</value>
        bool IsTableHeaderSeparatorRow { get; }

        /// <summary>
        /// Gets a value indicating whether this instance of <see cref="IIonReader"/> is currently on table row with data.
        /// </summary>
        /// <value><see langword="true"/> if <see cref="IIonReader.CurrentLine"/> first character is equal to '|' and current table header was already passed; otherwise, <see langword="false"/>.</value>
        bool IsTableDataRow { get; }

        /// <summary>
        /// Gets a value indicating whether a <see cref="IIonReader.CurrentLine"/> is <see langword="null"/>, empty, or consists only of white-space characters.
        /// </summary>
        /// <value><see langword="true"/> if string.IsNullOrWhiteSpace(CurrentLine); otherwise, <see langword="false"/>.</value>
        bool IsEmptyLine { get; }

        /// <summary>
        /// Gets the current line value. It allocates string from CurrentRawLine
        /// </summary>
        /// <value>The current line.</value>
        string CurrentLine { get; }

        /// <summary>
        /// Gets current line as array segment of characters. The value comes from rented buffer, copy it for private use.
        /// </summary>
        /// <value>The current line</value>
        ArraySegment<char> CurrentRawLine { get; }

        /// <summary>
        /// Gets the name of current section. It is changing only when <see cref="IIonReader.CurrentLine"/> is on section header. Returns <see langword="null"/> only if <see cref="IIonReader.Read"/> was not called yet.
        /// </summary>
        /// <value>The current section.</value>
        string? CurrentSection { get; }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <value>The current line number.</value>
        int CurrentLineNumber { get; }

        /// <summary>
        /// Read this instance.
        /// </summary>
        /// <returns><see langword="true"/> if the next line was read successfully; otherwise,<see langword="false"/>.</returns>
        bool Read();
    }
}

