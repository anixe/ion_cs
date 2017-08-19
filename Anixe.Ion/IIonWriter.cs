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
        void WriteEmptyLine();
        void Flush();
    }
}
