using Anixe.Ion.Helpers;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Anixe.Ion
{
    internal sealed class IonWriter : IIonWriter
    {
        private readonly TextWriter tw;
        private WriterState state;
        private string[]? lastTableColumns;
        private bool firstTableCell = true;
        internal WriterState State => this.state;
        private readonly WriterOptions options;

        public IonWriter(TextWriter tw, WriterOptions options)
        {
            this.options = options;
            this.tw = tw ?? throw new ArgumentNullException(nameof(tw));
        }

        public IonWriter(TextWriter tw)
        : this(tw, new WriterOptions())
        {
        }

        #region IIonWriter Members

        public void WriteSection(string name)
        {
            if (state != WriterState.None)
            {
                WriteLine();
                this.state = WriterState.Section;
            }
            ValidateWriteSection(name);
            ClearState();
            this.tw.Write(Consts.IonSpecialChars.HeaderOpeningCharacter);
            this.tw.Write(name);
            WriteLine(Consts.IonSpecialChars.HeaderClosingCharacter);
            this.state |= WriterState.Section;
        }

        private static void ValidateWriteSection(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Section name must be provided");
            }
        }

        public void WriteProperty(string name, string? value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            WriteQuotationCharacter();
            this.tw.Write(value);
            WriteQuotationCharacter(newLine: true);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, Action<TextWriter> writeValueAction)
        {
            if (writeValueAction == null)
            {
                throw new ArgumentNullException($"Provide {nameof(writeValueAction)} parameter");
            }
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            writeValueAction(this.tw);
            WriteLine();
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, bool value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            WriteLine(value ? Consts.True : Consts.False);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, int value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            WriteLine(value);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, double value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            WriteLine(value.ToString(CultureInfo.InvariantCulture));
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, float value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            WriteLine(value.ToString(CultureInfo.InvariantCulture));
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, char value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            WriteQuotationCharacter();
            this.tw.Write(value);
            WriteQuotationCharacter(newLine: true);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, decimal value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            WriteLine(value.ToString(CultureInfo.InvariantCulture));
            this.state |= WriterState.Property;
        }

        private void ValidateWriteProperty(string name)
        {
            if (this.state == WriterState.None)
            {
                throw new InvalidOperationException("Only section can be at the top of document");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException($"Property {nameof(name)} must be provided");
            }
        }

        public void WriteTableHeader(params string[] columns)
        {
            ValidateWriteTableHeader(columns);
            ClearState();
            WriteTableRow(columns, WriteCol);
            WriteSqueezedTableRow(columns, WriteHeaderSeparator);
            this.state &= ~WriterState.Property;
            this.state |= WriterState.TableHeader;
            this.lastTableColumns = columns;
        }

        private void ValidateWriteTableHeader(string[] columns)
        {
            if (this.state == WriterState.None)
            {
                throw new InvalidOperationException("Only section can be at the top of document");
            }
            if (this.state.HasBitFlags(WriterState.TableHeader))
            {
                throw new InvalidOperationException("Table can have ony one header");
            }
            if (columns == null || columns.Length == 0)
            {
                throw new ArgumentNullException("Cannot create empty table header");
            }
        }

        public void WriteTableRow(params string[] data)
        {
            if (this.lastTableColumns == null && !this.state.HasBitFlags(WriterState.TableHeader))
            {
                this.lastTableColumns = data;
            }
            ValidateWriteTableRow(data);
            WriteTableRow(data, WriteCol);
            this.state &= ~WriterState.Property;
            this.state &= ~WriterState.TableHeader;
            this.state |= WriterState.TableRow;
        }

        private void ValidateWriteTableRow(string[] columns)
        {
            if (columns == null || columns.Length == 0)
            {
                throw new ArgumentNullException("Cannot create empty table row");
            }
            if (columns.Length != this.lastTableColumns!.Length)
            {
                throw new ArgumentException("Must provide the same number of columns within the same table");
            }
        }

        public void WriteTableCell<TContext>(TContext context, Action<TextWriter, TContext> writeCellAction, bool lastCellInRow = false)
        {
            ValidateWriteTableCell(writeCellAction);
            WriteTableCellBefore();
            writeCellAction(this.tw, context);
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(Action<TextWriter> writeCellAction, bool lastCellInRow = false)
        {
            ValidateWriteTableCell(writeCellAction);
            WriteTableCellBefore();
            writeCellAction(this.tw);
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(int value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            tw.Write(value);
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
            if (Array.IndexOf(Consts.ProhibitedTableCellCharacters, value) != -1)
            {
                ThrowHelper.Throw_InvalidTableCellDataException();
            }
            tw.Write(value);
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(double value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            tw.Write(value);
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(decimal value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            tw.Write(value);
            WriteTableCellAfter(lastCellInRow);
        }

        public void WriteTableCell(long value, bool lastCellInRow = false)
        {
            WriteTableCellBefore();
            tw.Write(value);
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
            WriteTableCellBefore();
            if (buffer != null)
            {
                Validate(buffer, index, count);
            }
            tw.Write(buffer, index, count);
            WriteTableCellAfter(lastCellInRow);
        }

        private void WriteTableCellBefore()
        {
            if (this.firstTableCell)
            {
                tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
                this.firstTableCell = false;
            }
            tw.Write(Consts.IonSpecialChars.WriteSpaceCharacter);
        }

        private void WriteTableCellAfter(bool lastCellInRow = false)
        {
            tw.Write(Consts.IonSpecialChars.WriteSpaceCharacter);
            tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
            if (lastCellInRow)
            {
                WriteLine();
                this.firstTableCell = true;
            }
            this.state &= ~WriterState.Property;
            this.state &= ~WriterState.TableHeader;
            this.state |= WriterState.TableRow;
        }

        private static void ValidateWriteTableCell(Action<TextWriter> writeCellAction)
        {
            if (writeCellAction == null)
            {
                throw new ArgumentNullException("Must provide Action<TextWriter>");
            }
        }

        private static void ValidateWriteTableCell<TContext>(Action<TextWriter, TContext> writeCellAction)
        {
            if (writeCellAction == null)
            {
                throw new ArgumentNullException("Must provide Action<TextWriter, TContext>");
            }
        }

        private void WriteHeaderSeparator(string col)
        {
            for (int j = -1; j <= col.Length; j++)
            {
                tw.Write(Consts.IonSpecialChars.TableHeaderSeparatorCharacter);
            }
        }

        private void WriteCol(string? col)
        {
            if (col != null && col.IndexOfAny(Consts.ProhibitedTableCellCharacters) != -1)
            {
                ThrowHelper.Throw_InvalidTableCellDataException();
            }
            this.tw.Write(col);
        }

        private void WriteTableRow(string[] columns, Action<string> onItemAction)
        {
            tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
            for (int i = 0; i < columns.Length; i++)
            {
                tw.Write(Consts.IonSpecialChars.WriteSpaceCharacter);
                onItemAction(columns[i]);
                tw.Write(Consts.IonSpecialChars.WriteSpaceCharacter);
                tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
            }
            WriteLine();
        }

        private void WriteSqueezedTableRow(string[] columns, Action<string> onItemAction)
        {
            tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
            for (int i = 0; i < columns.Length; i++)
            {
                onItemAction(columns[i]);
                tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
            }
            WriteLine();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Validate(char[] buffer, int index, int count)
        {
            var end = index + count;
            for (int i = index; i < end; i++)
            {
                if (Array.IndexOf(Consts.ProhibitedTableCellCharacters, buffer[i]) != -1)
                {
                    ThrowHelper.Throw_InvalidTableCellDataException();
                }
            }
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
            if (!this.options.LeaveOpen)
            {
                this.tw.Dispose();
            }
        }

        #endregion

        private void ClearState()
        {
            this.lastTableColumns = null;
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
                WriteLine(Consts.IonSpecialChars.QuotationCharacter);
            }
            else
            {
                this.tw.Write(Consts.IonSpecialChars.QuotationCharacter);
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
