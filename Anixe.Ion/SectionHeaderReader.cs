using System;
using System.Collections.Generic;

namespace Anixe.Ion
{
    internal sealed class SectionHeaderReader
    {
        public ArraySegment<char> Read(ArraySegment<char> currentLine)
        {
            var indexOfOpeningCharacter = IndexOf(currentLine, Consts.IonSpecialChars.HeaderOpeningCharacter);
            var indexOfClosingCharacter = IndexOf(currentLine, Consts.IonSpecialChars.HeaderClosingCharacter);

            return currentLine.Slice(indexOfOpeningCharacter + 1, indexOfClosingCharacter - 1);
        }

        private static int IndexOf(ArraySegment<char> arr, char ch)
        {
            return ((IList<char>)arr).IndexOf(ch);
        }
    }
}

