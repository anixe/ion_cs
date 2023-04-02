using System;

namespace Anixe.Ion
{
    internal sealed class SectionHeaderReader
    {
        public ArraySegment<char> Read(ArraySegment<char> currentLine)
        {
            var indexOfOpeningCharacter = Array.IndexOf(currentLine.Array, Consts.IonSpecialChars.SectionHeaderOpening, currentLine.Offset);
            var indexOfClosingCharacter = Array.IndexOf(currentLine.Array, Consts.IonSpecialChars.SectionHeaderClosing, indexOfOpeningCharacter + 1);

            return new ArraySegment<char>(currentLine.Array, indexOfOpeningCharacter + 1, indexOfClosingCharacter - indexOfOpeningCharacter - 1);
        }
    }
}

