namespace Anixe.Ion
{
    internal class CurrentLineVerifier
    {
        public bool IsSectionHeader(string currentLine)
        {
            return !IsEmptyLine(currentLine)
                && currentLine[0] == Consts.IonSpecialChars.HeaderOpeningCharacter;
        }

        public bool IsProperty(string currentLine)
        {
            return !IsEmptyLine(currentLine)
                && !IsSectionHeader(currentLine)
                && !IsComment(currentLine)
                && !IsTableRow(currentLine)
                && !IsTableHeaderSeparatorRow(currentLine);
        }

        public bool IsComment(string currentLine)
        {
            return !IsEmptyLine(currentLine)
                && currentLine[0] == Consts.IonSpecialChars.CommentCharacter;
        }

        public bool IsTableRow(string currentLine)
        {
            return !IsEmptyLine(currentLine)
                && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter;
        }

        public bool IsTableHeaderSeparatorRow(string currentLine)
        {
            return !IsEmptyLine(currentLine)
                && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter
                && currentLine[1] == Consts.IonSpecialChars.TableHeaderSeparatorCharacter;
        }

        public bool IsEmptyLine(string currentLine)
        {
            return string.IsNullOrWhiteSpace(currentLine);
        }
    }
}

