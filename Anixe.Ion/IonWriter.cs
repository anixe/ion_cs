using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anixe.Ion
{
    internal class IonWriter : IIonWriter
    {
        private TextWriter tw;
        private WriterState state;
        private string[] lastTableColumns;
        private bool firstTableCell = true;
        private bool leaveOpen;

        internal WriterState State
        {
            get { return this.state; }
        }

        public IonWriter(TextWriter tw, bool leaveOpen = false)
        {
            this.tw = tw;
            this.leaveOpen = leaveOpen;
        }

        #region IIonWriter Members

        public void WriteSection(string name)
        {
            if(state != WriterState.None)
            {
                this.tw.WriteLine();
                this.state = WriterState.Section;
            }
            ValidateWriteSection(name);
            ClearState();
            this.tw.Write(Consts.IonSpecialChars.HeaderOpeningCharacter);
            this.tw.Write(name);
            this.tw.WriteLine(Consts.IonSpecialChars.HeaderClosingCharacter);
            this.state |= WriterState.Section;
        }

        private void ValidateWriteSection(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Section name must be provided");
            }
        }

        public void WriteProperty(string name, string value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            this.tw.Write(Consts.IonSpecialChars.QuotationCharacter);
            this.tw.Write(value);
            this.tw.WriteLine(Consts.IonSpecialChars.QuotationCharacter);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, Action<TextWriter> writeValueAction)
        {
            if (writeValueAction == null)
            {
                throw new ArgumentNullException("Provide writeValueAction parameter");
            }
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            writeValueAction(this.tw);
            this.tw.WriteLine();
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, bool value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            this.tw.WriteLine(value ? Consts.True : Consts.False);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, int value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            this.tw.WriteLine(value);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, double value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            this.tw.WriteLine(value.ToString(CultureInfo.InvariantCulture));
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, float value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            this.tw.WriteLine(value.ToString(CultureInfo.InvariantCulture));
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, char value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            this.tw.Write(Consts.IonSpecialChars.QuotationCharacter);
            this.tw.Write(value);
            this.tw.WriteLine(Consts.IonSpecialChars.QuotationCharacter);
            this.state |= WriterState.Property;
        }

        public void WriteProperty(string name, decimal value)
        {
            ValidateWriteProperty(name);
            ClearState();
            this.tw.Write(name);
            this.tw.Write(Consts.IonSpecialChars.EqualsCharacter);
            this.tw.WriteLine(value.ToString(CultureInfo.InvariantCulture));
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
                throw new ArgumentNullException("Property name must be provided");
            }
        }

        public void WriteTableHeader(params string[] columns)
        {
            ValidateWriteTableHeader(columns);
            ClearState();
            tw.WriteLine();
            WriteTableRow(columns, WriteCol);
            WriteTableRow(columns, WriteHeaderSeparator, true);
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
            if (columns.Length != this.lastTableColumns.Length)
            {
                throw new ArgumentException("Must provide the same number of columns within the same table");
            }
        }

        public void WriteTableCell(Action<TextWriter> writeCellAction, bool lastCellInRow = false)
        {
            ValidateWriteTableCell(writeCellAction);
            if(this.firstTableCell)
            {
                tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
                this.firstTableCell = false;
            }
            tw.Write(Consts.IonSpecialChars.WriteSpaceCharacter);
            writeCellAction(this.tw);
            tw.Write(Consts.IonSpecialChars.WriteSpaceCharacter);
            tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
            if(lastCellInRow)
            {
                this.tw.WriteLine();
                this.firstTableCell = true;
            }
            this.state &= ~WriterState.Property;
            this.state &= ~WriterState.TableHeader;
            this.state |= WriterState.TableRow;
        }

        private void ValidateWriteTableCell(Action<TextWriter> writeCellAction)
        {
            if(writeCellAction == null)
            {
                throw new ArgumentNullException("Must provide Action<TextWriter>");
            }
        }

        private void WriteHeaderSeparator(string col)
        {
            for (int j = -1; j <= col.Length; j++)
            {
                tw.Write(Consts.IonSpecialChars.TableHeaderSeparatorCharacter);
            }
        }

        private void WriteCol(string col)
        {
            this.tw.Write(col);
        }

        private void WriteTableRow(string[] columns, Action<string> onItemAction, bool squeeze = false)
        {
            tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
            for (int i = 0; i < columns.Length; i++)
            {
                var col = columns[i];
                if (!squeeze)
                {
                    tw.Write(Consts.IonSpecialChars.WriteSpaceCharacter);
                }
                onItemAction(col);
                if (!squeeze)
                {
                    tw.Write(Consts.IonSpecialChars.WriteSpaceCharacter);
                }
                tw.Write(Consts.IonSpecialChars.TableOpeningCharacter);
            }
            tw.WriteLine();
        }

        public void WriteEmptyLine()
        {
            ClearState();
            this.tw.WriteLine();
        }

        public void Flush()
        {
            this.tw.Flush();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.tw != null && !this.leaveOpen)
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
    }
}
