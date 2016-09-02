namespace Anixe.Ion
{
    internal class SectionHeaderReader
    {
        public string Read(string currentLine)
        {
            int indexOfOpeningCharacter = currentLine.IndexOf(Consts.IonSpecialChars.HeaderOpeningCharacter);
            int indexOfClosingCharacter = currentLine.IndexOf(Consts.IonSpecialChars.HeaderClosingCharacter);

            return currentLine.Substring(indexOfOpeningCharacter + 1, indexOfClosingCharacter - 1);
        }
    }
}

