using System;
using System.IO;

namespace Anixe.Ion
{
    public interface IIonWriter : IDisposable
    {
        void WriteSection(string name);
        void WriteProperty(string name, string? value);
        void WriteProperty(string name, char value);
        void WriteProperty(string name, int value);
        void WriteProperty(string name, double value);
        void WriteProperty(string name, float value);
        void WriteProperty(string name, Action<TextWriter> writeValueAction);

        void WriteTableHeader(params string[] columns);
        void WriteTableHeader(params ReadOnlySpan<string> columns);
        void WriteTableRow(params string[] data);
        void WriteTableRow(params ReadOnlySpan<string> data);
        void WriteTableCell(Action<TextWriter> writeCellAction, bool lastCellInRow = false);
        void WriteTableCell<TContext>(TContext context, Action<TextWriter, TContext> writeCellAction, bool lastCellInRow = false);

        /// <summary>Writes table cell value and adds table separators when needed.</summary>
        /// <param name="value">Value to write</param>
        /// <param name="lastCellInRow">Adds new line character when is set to <see langword="true"/></param>
        void WriteTableCell(int value, bool lastCellInRow = false);

        /// <summary>Writes table cell value and adds table separators when needed.</summary>
        /// <param name="value">Value to write.</param>
        /// <param name="lastCellInRow">Adds new line character when is set to <see langword="true"/></param>
        void WriteTableCell(string? value, bool lastCellInRow = false);

        /// <summary>Writes table cell value and adds table separators when needed.</summary>
        /// <param name="value">Value to write.</param>
        /// <param name="lastCellInRow">Adds new line character when is set to <see langword="true"/></param>
        void WriteTableCell(char value, bool lastCellInRow = false);

        /// <summary>Writes table cell value and adds table separators when needed.</summary>
        /// <param name="value">Value to write</param>
        /// <param name="lastCellInRow">Adds new line character when is set to <see langword="true"/></param>
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

        /// <summary>Writes table cell value and adds table separators when needed.</summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <param name="lastCellInRow">Adds new line character when is set to <see langword="true"/></param>
        void WriteTableCell(char[] buffer, int index, int count, bool lastCellInRow = false);

        /// <summary>
        /// Writes table cell value and adds table separators when needed.
        /// </summary>
        /// <param name="span">The span of characters to write data from.</param>
        /// <param name="lastCellInRow">Adds new line character when is set to <see langword="true"/></param>
        void WriteTableCell(ReadOnlySpan<char> span, bool lastCellInRow = false);

        void WriteEmptyLine();

        void Flush();
    }
}
