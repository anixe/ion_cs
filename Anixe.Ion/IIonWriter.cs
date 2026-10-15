using System;
using System.IO;

namespace Anixe.Ion
{
    /// <summary>Fast, non-cached, forward-only writer for ION data.</summary>
    public interface IIonWriter : IDisposable
    {
        /// <summary>Writes a section header (e.g. [SectionName]).</summary>
        /// <param name="name">Section name.</param>
        void WriteSection(string name);

        /// <summary>Writes a string property (key-value).</summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        void WriteProperty(string name, string? value);

        /// <summary>Writes a char property.</summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Character value.</param>
        void WriteProperty(string name, char value);

        /// <summary>Writes an int property.</summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Integer value.</param>
        void WriteProperty(string name, int value);

        /// <summary>Writes a double property.</summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Double value.</param>
        void WriteProperty(string name, double value);

        /// <summary>Writes a float property.</summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Float value.</param>
        void WriteProperty(string name, float value);

        /// <summary>Writes a property using a custom TextWriter action.</summary>
        /// <param name="name">Property name.</param>
        /// <param name="writeValueAction">Action that writes the value to a TextWriter.</param>
        void WriteProperty(string name, Action<TextWriter> writeValueAction);

        /// <summary>Writes a table header with the given column names.</summary>
        /// <param name="columns">Column names.</param>
        void WriteTableHeader(params string[] columns);

        /// <summary>Writes a table header with the given column names (span overload).</summary>
        /// <param name="columns">Column names.</param>
        void WriteTableHeader(params ReadOnlySpan<string> columns);

        /// <summary>Writes a table row with the provided cell values.</summary>
        /// <param name="data">Cell values.</param>
        void WriteTableRow(params string[] data);

        /// <summary>Writes a table row with the provided cell values (span overload).</summary>
        /// <param name="data">Cell values.</param>
        void WriteTableRow(params ReadOnlySpan<string> data);

        /// <summary>Writes a table cell using a TextWriter action.</summary>
        /// <param name="writeCellAction">Action that writes the cell to a TextWriter.</param>
        /// <param name="lastCellInRow">True if this is the last cell in the row.</param>
        void WriteTableCell(Action<TextWriter> writeCellAction, bool lastCellInRow = false);

        /// <summary>Writes a table cell using a TextWriter action with context.</summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">Context value passed to the action.</param>
        /// <param name="writeCellAction">Action that writes the cell with context.</param>
        /// <param name="lastCellInRow">True if this is the last cell in the row.</param>
        void WriteTableCell<TContext>(TContext context, Action<TextWriter, TContext> writeCellAction, bool lastCellInRow = false);

        /// <summary>Writes a Guid table cell; adds separator or newline if last in row.</summary>
        /// <param name="value">Value to write.</param>
        /// <param name="lastCellInRow">True to end the row after writing.</param>
        void WriteTableCell(Guid value, bool lastCellInRow = false);

        /// <summary>Writes an int table cell; adds separator or newline if last in row.</summary>
        /// <param name="value">Value to write.</param>
        /// <param name="lastCellInRow">True to end the row after writing.</param>
        void WriteTableCell(int value, bool lastCellInRow = false);

        /// <summary>Writes a string table cell; adds separator or newline if last in row.</summary>
        /// <param name="value">Value to write.</param>
        /// <param name="lastCellInRow">True to end the row after writing.</param>
        void WriteTableCell(string? value, bool lastCellInRow = false);


        /// <summary>Writes a char table cell; adds separator or newline if last in row.</summary>
        /// <param name="value">Value to write.</param>
        /// <param name="lastCellInRow">True to end the row after writing.</param>
        void WriteTableCell(char value, bool lastCellInRow = false);

        /// <summary>Writes a double table cell; adds separator or newline if last in row.</summary>
        /// <param name="value">Value to write.</param>
        /// <param name="lastCellInRow">True to end the row after writing.</param>
        void WriteTableCell(double value, bool lastCellInRow = false);

        /// <summary>Writes table cell value and adds table separators when needed.</summary>
        /// <param name="value">Value to write</param>
        /// <param name="lastCellInRow">Adds new line character when is set to <see langword="true"/></param>
        void WriteTableCell(decimal value, bool lastCellInRow = false);

        /// <summary>Writes table cell value and adds table separators when needed.</summary>
        /// <param name="value">Value to write</param>
        /// <param name="lastCellInRow">Adds new line character when is set to <see langword="true"/></param>
        void WriteTableCell(long value, bool lastCellInRow = false);

        /// <summary>Writes table cell value and adds table separators when needed.</summary>
        /// <param name="value">Value to write</param>
        /// <param name="lastCellInRow">Adds new line character when is set to <see langword="true"/></param>
        void WriteTableCell(bool value, bool lastCellInRow = false);

        /// <summary>Writes a table cell from a char buffer; adds separator or newline if last in row.</summary>
        /// <param name="buffer">Source character buffer.</param>
        /// <param name="index">Start index in buffer.</param>
        /// <param name="count">Number of chars to write.</param>
        /// <param name="lastCellInRow">True to end the row after writing.</param>
        void WriteTableCell(char[] buffer, int index, int count, bool lastCellInRow = false);

        /// <summary>Writes a table cell from a character span; adds separator or newline if last in row.</summary>
        /// <param name="span">Source character span.</param>
        /// <param name="lastCellInRow">True to end the row after writing.</param>
        void WriteTableCell(ReadOnlySpan<char> span, bool lastCellInRow = false);

        /// <summary>Writes an empty line to the output.</summary>
        void WriteEmptyLine();

        /// <summary>Flushes the writer and underlying stream.</summary>
        void Flush();
    }
}
