using System;

namespace Anixe.Ion
{
    public interface IIonReader : IDisposable
    {
        bool IsSectionHeader           { get; }
        bool IsProperty                { get; }
        bool IsComment                 { get; }
        bool IsTableRow                { get; }
        bool IsTableHeaderSeparatorRow { get; }
        bool IsEmptyLine               { get; }

        string CurrentLine { get; }

        bool Read();
    }
}

