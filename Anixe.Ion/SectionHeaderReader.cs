using System;

namespace Anixe.Ion
{
    internal sealed class SectionHeaderReader
    {
        public ArraySegment<char> Read(ArraySegment<char> currentLine)
        {
            var array = currentLine.Array!;
            var indexOfOpeningCharacter = Array.IndexOf(array, Consts.IonSpecialChars.HeaderOpeningCharacter, currentLine.Offset);
            var indexOfClosingCharacter = Array.IndexOf(array, Consts.IonSpecialChars.HeaderClosingCharacter, indexOfOpeningCharacter + 1);

            return new ArraySegment<char>(array, indexOfOpeningCharacter + 1, indexOfClosingCharacter - indexOfOpeningCharacter - 1);
        }
    }
}

