using System;
using System.IO;

namespace Anixe.Ion
{
    public class IonReader : IIonReader, IDisposable
    {
        private readonly StreamReader streamReader;

        internal IonReader(StreamReader streamReader)
        {
            this.streamReader = streamReader;
        }

        #region IIonReader members

        public bool IsSectionHeader           { get; }
        public bool IsProperty                { get; }
        public bool IsComment                 { get; }
        public bool IsTableRow                { get; }
        public bool IsTableHeaderSeparatorRow { get; }
        public bool IsEmptyLine               { get; }

        public string CurrentLine { get; }

        public bool Read()
        {
            return false;
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
        }

        #endregion
    }
}

