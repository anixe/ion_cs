using System;

namespace Anixe.Ion
{
    public class IonReader : IIonReader
    {
        public bool IsSectionHeader           { get; }
        public bool IsProperty                { get; }
        public bool IsComment                 { get; }
        public bool IsTableRow                { get; }
        public bool IsTableHeaderSeparatorRow { get; }
        public bool IsEmptyLine               { get; }

        public string Line { get; }

        public bool Read()
        {
            return false;
        }
    }
}

