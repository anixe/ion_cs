using System;

namespace Anixe.Ion
{
    internal class CurrentLineVerifier
    {
        public bool IsSectionHeader(ArraySegment<char> currentLine)
        {
            return !IsEmptyLine(currentLine)
                && currentLine[0] == Consts.IonSpecialChars.HeaderOpeningCharacter;
        }

        public bool IsTableHeaderRow(ArraySegment<char> currentLine, bool passedCurrentTableHeaderRow)
        {
          return !passedCurrentTableHeaderRow
              && !IsEmptyLine(currentLine)
              && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter
              && currentLine[1] != Consts.IonSpecialChars.TableHeaderSeparatorCharacter;
        }

        public bool IsTableDataRow(ArraySegment<char> currentLine, bool passedCurrentTableHeaderRow)
        {
          return passedCurrentTableHeaderRow
              && !IsEmptyLine(currentLine)
              && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter
              && currentLine[1] != Consts.IonSpecialChars.TableHeaderSeparatorCharacter;
        }

        public bool IsProperty(ArraySegment<char> currentLine)
        {
            return !IsEmptyLine(currentLine)
                && !IsSectionHeader(currentLine)
                && !IsComment(currentLine)
                && !IsTableRow(currentLine)
                && !IsTableHeaderSeparatorRow(currentLine);
        }

        public bool IsComment(ArraySegment<char> currentLine)
        {
            return !IsEmptyLine(currentLine)
                && currentLine[0] == Consts.IonSpecialChars.CommentCharacter;
        }

        public bool IsTableRow(ArraySegment<char> currentLine)
        {
            return !IsEmptyLine(currentLine)
                && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter;
        }

        public bool IsTableHeaderSeparatorRow(ArraySegment<char> currentLine)
        {
            return !IsEmptyLine(currentLine)
                && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter
                && currentLine[1] == Consts.IonSpecialChars.TableHeaderSeparatorCharacter;
        }

        public bool IsEmptyLine(ArraySegment<char> currentLine)
        {
            return currentLine == default(ArraySegment<char>) || currentLine.Count == 0 || IsWhiteSpace(currentLine);
        }

        private bool IsWhiteSpace(ArraySegment<char> currentLine)
        {
            for (int i = currentLine.Offset; i < currentLine.Count; i++)
            {
                if(!char.IsWhiteSpace(currentLine[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}