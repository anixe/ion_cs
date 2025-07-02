using System;
using System.Buffers;
using System.Globalization;
using System.IO;

namespace Anixe.Ion
{
    internal sealed class IonWriter : IIonWriter
    {
        private static readonly SearchValues<char> SpecialCharsForTableCell = SearchValues.Create('|', '\n', '\\');

        private readonly TextWriter tw;
        private readonly WriterOptions options;
        private WriterState state;
        private int lastTableColumnsLength = -1; // -1 means no last table column yet
        private bool firstTableCell = true;

        public IonWriter(TextWriter tw, WriterOptions options)
        {
            ArgumentNullException.ThrowIfNull(tw);
            ArgumentNullException.ThrowIfNull(options);
            this.options = options;
            this.tw = tw;
        }

        public IonWriter(TextWriter tw)
        : this(tw, new WriterOptions())
        {
        }

        internal WriterState State => this.state;

        #region IIonWriter Members

        public void WriteSection(string name)
        {
            if (this.state != WriterState.None)
            {
                WriteLine();
                this.state = WriterState.Section;
            }
            ArgumentException.ThrowIfNullOrEmpty(name);
            ClearState();
            this.tw.Write(Consts.IonSpecialChars.SectionHeaderOpening);
            this.tw.Write(name);
            WriteLine(Consts.IonSpecialChars.SectionHeaderClosing);
            this.state |= WriterState.Section;
        }

        public void WriteProperty(string name, string? value)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ThrowIfTopOfDocument();
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.PropertyAssignment);
            WriteQuotationCharacter();
            this.tw.Write(value);
            WriteQuotationCharacter(newLine: true);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, Action<TextWriter> writeValueAction)
        {
            if (writeValueAction == null)
            {
                throw new ArgumentNullException(nameof(writeValueAction), $"Provide {nameof(writeValueAction)} parameter");
            }
            ArgumentException.ThrowIfNullOrEmpty(name);
            ThrowIfTopOfDocument();
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.PropertyAssignment);
            writeValueAction(this.tw);
            WriteLine();
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, bool value)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ThrowIfTopOfDocument();
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.PropertyAssignment);
            WriteLine(value ? "true" : "false");
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, int value)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ThrowIfTopOfDocument();
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.PropertyAssignment);
            WriteLine(value);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, double value)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ThrowIfTopOfDocument();
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.PropertyAssignment);
            WriteLine(value.ToString(CultureInfo.InvariantCulture));
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, float value)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ThrowIfTopOfDocument();
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.PropertyAssignment);
            WriteLine(value.ToString(CultureInfo.InvariantCulture));
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, char value)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ThrowIfTopOfDocument();
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.PropertyAssignment);
            WriteQuotationCharacter();
            this.tw.Write(value);
            WriteQuotationCharacter(newLine: true);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, decimal value)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ThrowIfTopOfDocument();
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.PropertyAssignment);
            WriteLine(value.ToString(CultureInfo.InvariantCulture));
            this.state |= WriterState.Property;
        }

        private void ThrowIfTopOfDocument()
        {
            if (this.state == WriterState.None)
            {
                Throw_InvalidOperationException();
            }

            static void Throw_InvalidOperationException() =>
                throw new InvalidOperationException("Only section can be at the top of document");
        }

        public void WriteTableHeader(params string[] columns)
        {
            ArgumentNullException.ThrowIfNull(columns);
            WriteTableHeader(columns.AsSpan());
        }

        public void WriteTableHeader(params ReadOnlySpan<string> columns)
        {
            ValidateWriteTableHeader(columns);
            ClearState();
            WriteTableRow(columns, col => WriteCol(col));
            WriteSqueezedTableRow(columns, WriteHeaderSeparator);
            this.state &= ~WriterState.Property;
            this.state |= WriterState.TableHeader;
            this.lastTableColumnsLength = columns.Length;
        }

        private void ValidateWriteTableHeader(ReadOnlySpan<string> columns)
        {
            ThrowIfTopOfDocument();
            if (this.state.HasBitFlags(WriterState.TableHeader))
            {
                throw new InvalidOperationException("Table can have only one header");
            }
            if (columns.Length == 0)
            {
                throw new ArgumentException("Cannot create empty table header", nameof(columns));
            }
        }

        public void WriteTableRow(params string[] data)
        {
            ArgumentNullException.ThrowIfNull(data);
            WriteTableRow(data.AsSpan());
        }

        public void WriteTableRow(params ReadOnlySpan<string> data)
        {
            if (this.lastTableColumnsLength == -1 && !this.state.HasBitFlags(WriterState.TableHeader))
            {
                this.lastTableColumnsLength = data.Length;
            }
            ValidateWriteTableRow(data);
            WriteTableRow(data, col => WriteCol(col));
            this.state &= ~WriterState.Property;
            this.state &= ~WriterState.TableHeader;
            this.state |= WriterState.TableRow;
        }

        private void ValidateWriteTableRow(ReadOnlySpan<string> columns)
        {
            if (columns.Length == 0)
            {
                Throw_ArgumentException_EmptyTableRow(nameof(columns));
            }

            if (columns.Length != this.lastTableColumnsLength)
            {
                Throw_ArgumentException_InvalidAmountOfColumns(nameof(columns));
            }

            static void Throw_ArgumentException_EmptyTableRow(string columnsName) =>
                throw new ArgumentException("Cannot create empty table row", columnsName);

            static void Throw_ArgumentException_InvalidAmountOfColumns(string columnsName) =>
                throw new ArgumentException("Must provide the same number of columns within the same table", columnsName);
        }

        public void WriteTableCell<TContext>(TContext context, Action<TextWriter, TContext> writeCellAction, bool lastCellInRow = false)
        {
            ArgumentNullException.ThrowIfNull(writeCellAction);
            WriteTableCellBefore();
            var sb = new System.Text.StringBuilder();

            using (var textWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                writeCellAction(textWriter, context);
            }

            WriteCol(sb.ToString().AsSpan());
            writeCellAction(this.tw, context);
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(Action<TextWriter> writeCellAction, bool lastCellInRow = false)
        {
            ArgumentNullException.ThrowIfNull(writeCellAction);
            WriteTableCellBefore();
            var sb = new System.Text.StringBuilder();

            using (var textWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                writeCellAction(textWriter);
            }

            WriteCol(sb.ToString().AsSpan());
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(int value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            tw.Write(value.ToString(CultureInfo.InvariantCulture));
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(string? value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            WriteCol(value);
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(char value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            WriteCol(new ReadOnlySpan<char>(in value));
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(double value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            tw.Write(value.ToString(CultureInfo.InvariantCulture));
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(decimal value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            tw.Write(value.ToString(CultureInfo.InvariantCulture));
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(long value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            tw.Write(value.ToString(CultureInfo.InvariantCulture));
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(bool value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            tw.Write(value);
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(char[] buffer, int index, int count, bool lastCellInRow = false)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            WriteTableCell(buffer.AsSpan(index, count), lastCellInRow);
        }

        public void WriteTableCell(ReadOnlySpan<char> span, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            WriteCol(span);
            WriteTableCellAfter(lastCellInRow);
        }

        private void WriteTableCellBefore()
        {
            if (this.firstTableCell)
            {
                tw.Write(Consts.IonSpecialChars.TableColumnSeparator);
                this.firstTableCell = false;
            }
            tw.Write(Consts.IonSpecialChars.TableCellPadding);
        }

        private void WriteTableCellAfter(bool lastCellInRow = false)
        {
            tw.Write(Consts.IonSpecialChars.TableCellPadding);
            tw.Write(Consts.IonSpecialChars.TableColumnSeparator);
            if (lastCellInRow)
            {
                WriteLine();
                this.firstTableCell = true;
            }
            this.state &= ~WriterState.Property;
            this.state &= ~WriterState.TableHeader;
            this.state |= WriterState.TableRow;
        }

        private void WriteHeaderSeparator(string col)
        {
            for (int j = -1; j <= col.Length; j++)
            {
                this.tw.Write(Consts.IonSpecialChars.TableHeaderSeparator);
            }
        }

        private void WriteCol(ReadOnlySpan<char> span)
        {
            var idx = span.IndexOfAny(SpecialCharsForTableCell);
            if (idx >= 0)
            {
                WriteEscaped(span, idx);
                return;
            }

            this.tw.Write(span);

            void WriteEscaped(ReadOnlySpan<char> span, int idx)
            {
                this.tw.Write(span[..idx]);

                for (var i = idx; i < span.Length; i++)
                {
                    var c = span[i];
                    if (c == '\n')
                    {
                        this.tw.Write('\\');
                        this.tw.Write('n');
                        continue;
                    }

                    if (c is '\\')
                    {
                        this.tw.Write('\\');
                    }

                    if (c is '|')
                    {
                        this.tw.Write('\\');
                    }

                    this.tw.Write(c);
                }
            }
        }

        private void WriteTableRow(ReadOnlySpan<string> columns, Action<string> onItemAction)
        {
            tw.Write(Consts.IonSpecialChars.TableColumnSeparator);
            foreach (var column in columns)
            {
                tw.Write(Consts.IonSpecialChars.TableCellPadding);
                onItemAction(column);
                tw.Write(Consts.IonSpecialChars.TableCellPadding);
                tw.Write(Consts.IonSpecialChars.TableColumnSeparator);
            }
            WriteLine();
        }

        private void WriteSqueezedTableRow(ReadOnlySpan<string> columns, Action<string> onItemAction)
        {
            tw.Write(Consts.IonSpecialChars.TableColumnSeparator);
            foreach (var column in columns)
            {
                onItemAction(column);
                tw.Write(Consts.IonSpecialChars.TableColumnSeparator);
            }
            WriteLine();
        }

        public void WriteEmptyLine()
        {
            ClearState();
            WriteLine();
        }

        public void Flush()
        {
            this.tw.Flush();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Flush();
            if (!this.options.LeaveOpen)
            {
                this.tw.Dispose();
            }
        }

        #endregion

        private void ClearState()
        {
            this.lastTableColumnsLength = -1;
            this.firstTableCell = true;
            this.state &= ~WriterState.Property;
            this.state &= ~WriterState.TableHeader;
            this.state &= ~WriterState.TableRow;
        }

        private void WriteQuotationCharacter(bool newLine = false)
        {
            if (this.options.EscapeQuotes)
            {
                this.tw.Write(Consts.IonSpecialChars.EscapeCharacter);
            }
            if (newLine)
            {
                WriteLine(Consts.IonSpecialChars.PropertyQuotation);
            }
            else
            {
                this.tw.Write(Consts.IonSpecialChars.PropertyQuotation);
            }
        }

        private void WriteLine()
        {
            if (this.options.EscapeNewLineChars)
            {
                this.tw.Write(Consts.IonSpecialChars.NewLineEscaped);
            }
            else
            {
                this.tw.WriteLine();
            }
        }

        private void WriteLine(string val)
        {
            this.tw.Write(val);
            WriteLine();
        }

        private void WriteLine(char val)
        {
            this.tw.Write(val);
            WriteLine();
        }

        private void WriteLine(int val)
        {
            this.tw.Write(val);
            WriteLine();
        }
    }
}
