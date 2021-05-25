using System;

namespace Anixe.Ion
{
    internal sealed class SectionHeaderReader
    {
        public ArraySegment<char> Read(ArraySegment<char> currentLine)
        {
            var indexOfOpeningCharacter = Array.IndexOf(currentLine.Array, Consts.IonSpecialChars.HeaderOpeningCharacter, currentLine.Offset);
            var indexOfClosingCharacter = Array.IndexOf(currentLine.Array, Consts.IonSpecialChars.HeaderClosingCharacter, indexOfOpeningCharacter + 1);

            return new ArraySegment<char>(currentLine.Array, indexOfOpeningCharacter + 1, indexOfClosingCharacter - indexOfOpeningCharacter - 1);
        }
    }
}

