using System;
using System.Collections.Generic;

namespace Anixe.Ion
{
    internal sealed class CurrentLineVerifier
    {
        public bool IsSectionHeader(ArraySegment<char> currentLine)
        {
            return currentLine.Count != 0
                && currentLine[0] == Consts.IonSpecialChars.SectionHeaderOpening;
        }

        public bool IsTableHeaderRow(ArraySegment<char> currentLine, bool passedCurrentTableHeaderRow)
        {
          return !passedCurrentTableHeaderRow
              && currentLine.Count > 1
              && currentLine[1] != Consts.IonSpecialChars.TableHeaderSeparator
              && currentLine[0] == Consts.IonSpecialChars.TableColumnSeparator;
        }

        public bool IsTableDataRow(ArraySegment<char> currentLine, bool passedCurrentTableHeaderRow)
        {
          return passedCurrentTableHeaderRow
              && currentLine.Count > 1
              && currentLine[1] != Consts.IonSpecialChars.TableHeaderSeparator
              && currentLine[0] == Consts.IonSpecialChars.TableColumnSeparator;
        }

        public bool IsProperty(ArraySegment<char> currentLine)
        {
            if (currentLine.Count == 0)
            {
                return false;
            }

            char firstChar = currentLine[0];
            return firstChar != Consts.IonSpecialChars.TableColumnSeparator
                && firstChar != Consts.IonSpecialChars.SectionHeaderOpening
                && firstChar != Consts.IonSpecialChars.BeginCommentLine
                && !IsWhiteSpace(currentLine);
        }

        public bool IsComment(ArraySegment<char> currentLine)
        {
            return currentLine.Count != 0
                && currentLine[0] == Consts.IonSpecialChars.BeginCommentLine;
        }

        public bool IsTableRow(ArraySegment<char> currentLine)
        {
            return currentLine.Count != 0
                && currentLine[0] == Consts.IonSpecialChars.TableColumnSeparator;
        }

        public bool IsTableHeaderSeparatorRow(ArraySegment<char> currentLine)
        {
            return currentLine.Count > 1
                && currentLine[1] == Consts.IonSpecialChars.TableHeaderSeparator
                && currentLine[0] == Consts.IonSpecialChars.TableColumnSeparator;
        }

        public bool IsEmptyLine(ArraySegment<char> currentLine)
        {
            return currentLine.Count == 0 || IsWhiteSpace(currentLine);
        }

        private static bool IsWhiteSpace(ArraySegment<char> currentLine)
        {
            for (int i = currentLine.Offset; i < currentLine.Count; i++)
            {
                if (!char.IsWhiteSpace(currentLine[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}