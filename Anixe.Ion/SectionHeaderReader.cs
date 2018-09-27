using System;

namespace Anixe.Ion
{
    internal class SectionHeaderReader
    {
        public ArraySegment<char> Read(ArraySegment<char> currentLine)
        {
            var indexOfOpeningCharacter = IndexOf(currentLine, Consts.IonSpecialChars.HeaderOpeningCharacter);
            var indexOfClosingCharacter = IndexOf(currentLine, Consts.IonSpecialChars.HeaderClosingCharacter);

            return currentLine.Slice(indexOfOpeningCharacter + 1, indexOfClosingCharacter - 1);
        }

        private int IndexOf(ArraySegment<char> arr, char ch)
        {
            for (int i = arr.Offset; i < arr.Count; i++)
            {
                if(arr[i] == ch)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}

