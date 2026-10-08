using System;

namespace Anixe.Ion
{
    internal sealed class SectionHeaderReader
    {
        public static readonly SectionHeaderReader Instance = new();

        public ArraySegment<char> Read(ArraySegment<char> currentLine)
        {
            var array = currentLine.Array!;
            var indexOfOpeningCharacter = Array.IndexOf(array, Consts.IonSpecialChars.SectionHeaderOpening, currentLine.Offset);
            var indexOfClosingCharacter = Array.IndexOf(array, Consts.IonSpecialChars.SectionHeaderClosing, indexOfOpeningCharacter + 1);

            if (indexOfOpeningCharacter < 0 || indexOfClosingCharacter < 0)
            {
                Throw_FormatException_InvalidHeaderFormat();
            }

            return new ArraySegment<char>(array, indexOfOpeningCharacter + 1, indexOfClosingCharacter - indexOfOpeningCharacter - 1);

            static void Throw_FormatException_InvalidHeaderFormat()
            {
                throw new FormatException("Invalid section header format.");
            }
        }
    }
}