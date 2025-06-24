using System;

namespace Anixe.Ion
{
    internal sealed class SectionHeaderReader
    {
        public ArraySegment<char> Read(ArraySegment<char> currentLine)
        {
            var array = currentLine.Array!;
            var indexOfOpeningCharacter = Array.IndexOf(array, Consts.IonSpecialChars.SectionHeaderOpening, currentLine.Offset);
            var indexOfClosingCharacter = Array.IndexOf(array, Consts.IonSpecialChars.SectionHeaderClosing, indexOfOpeningCharacter + 1);

            if (indexOfOpeningCharacter < 0 || indexOfClosingCharacter < 0)
            {
                throw new InvalidOperationException("Invalid section header format.");
            }

            return new ArraySegment<char>(array, indexOfOpeningCharacter + 1, indexOfClosingCharacter - indexOfOpeningCharacter - 1);
        }
    }
}
