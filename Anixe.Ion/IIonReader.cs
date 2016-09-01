using System;

namespace Anixe.Ion
{
    internal interface IIonReader
    {
        bool IsSectionHeader           { get; }
        bool IsProperty                { get; }
        bool IsComment                 { get; }
        bool IsTableRow                { get; }
        bool IsTableHeaderSeparatorRow { get; }
        bool IsEmptyLine               { get; }

        string Line { get; }

        bool Read();
    }
}

