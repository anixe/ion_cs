using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anixe.Ion
{
    public interface IIonWriter : IDisposable
    {
        void WriteSection(string name);
        void WriteProperty(string name, string value);
        void WriteProperty(string name, char value);
        void WriteProperty(string name, int value);
        void WriteProperty(string name, double value);
        void WriteProperty(string name, float value);
        void WriteProperty(string name, Action<TextWriter> writeValueAction);

        void WriteTableHeader(params string[] columns);
        void WriteTableRow(params string[] data);
        void WriteTableCell(Action<TextWriter> writeCellAction, bool lastCellInRow = false);
        void WriteTableCell<TContext>(TContext context, Action<TextWriter, TContext> writeCellAction, bool lastCellInRow = false);
        void WriteTableCell(int value, bool lastCellInRow = false);
        void WriteTableCell(string value, bool lastCellInRow = false);
        void WriteTableCell(char value, bool lastCellInRow = false);
        void WriteTableCell(double value, bool lastCellInRow = false);
        void WriteTableCell(decimal value, bool lastCellInRow = false);
        void WriteTableCell(long value, bool lastCellInRow = false);
        void WriteTableCell(bool value, bool lastCellInRow = false);
        void WriteTableCell(char[] buffer, int index, int count, bool lastCellInRow = false);
        void WriteEmptyLine();
        void Flush();
    }
}
