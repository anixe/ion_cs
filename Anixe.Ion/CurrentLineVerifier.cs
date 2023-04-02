using System;
using System.Collections.Generic;

namespace Anixe.Ion
{
    internal sealed class CurrentLineVerifier
    {
        public bool IsSectionHeader(ArraySegment<char> currentLine)
        {
            return currentLine.Count != 0
#if NETSTANDARD2_0
                && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.SectionHeaderOpening;
#else
                && currentLine[0] == Consts.IonSpecialChars.SectionHeaderOpening;
#endif
        }

        public bool IsTableHeaderRow(ArraySegment<char> currentLine, bool passedCurrentTableHeaderRow)
        {
          return !passedCurrentTableHeaderRow
              && currentLine.Count > 1
#if NETSTANDARD2_0
              && ((IList<char>)currentLine)[1] != Consts.IonSpecialChars.TableHeaderSeparator
              && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.TableColumnSeparator;
#else
              && currentLine[1] != Consts.IonSpecialChars.TableHeaderSeparator
              && currentLine[0] == Consts.IonSpecialChars.TableColumnSeparator;
#endif
        }

        public bool IsTableDataRow(ArraySegment<char> currentLine, bool passedCurrentTableHeaderRow)
        {
          return passedCurrentTableHeaderRow
              && currentLine.Count > 1
#if NETSTANDARD2_0
              && ((IList<char>)currentLine)[1] != Consts.IonSpecialChars.TableHeaderSeparator
              && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.TableColumnSeparator;
#else
              && currentLine[1] != Consts.IonSpecialChars.TableHeaderSeparator
              && currentLine[0] == Consts.IonSpecialChars.TableColumnSeparator;
#endif
        }

        public bool IsProperty(ArraySegment<char> currentLine)
        {
            if (currentLine.Count == 0)
            {
                return false;
            }

#if NETSTANDARD2_0
            char firstChar = ((IList<char>)currentLine)[0];
#else
            char firstChar = currentLine[0];
#endif
            return firstChar != Consts.IonSpecialChars.TableColumnSeparator
                && firstChar != Consts.IonSpecialChars.SectionHeaderOpening
                && firstChar != Consts.IonSpecialChars.BeginCommentLine
                && !IsWhiteSpace(currentLine);
        }

        public bool IsComment(ArraySegment<char> currentLine)
        {
            return currentLine.Count != 0
#if NETSTANDARD2_0
                && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.BeginCommentLine;
#else
                && currentLine[0] == Consts.IonSpecialChars.BeginCommentLine;
#endif
        }

        public bool IsTableRow(ArraySegment<char> currentLine)
        {
            return currentLine.Count != 0
#if NETSTANDARD2_0
                && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.TableColumnSeparator;
#else
                && currentLine[0] == Consts.IonSpecialChars.TableColumnSeparator;
#endif
        }

        public bool IsTableHeaderSeparatorRow(ArraySegment<char> currentLine)
        {
            return currentLine.Count > 1
#if NETSTANDARD2_0
                && ((IList<char>)currentLine)[1] == Consts.IonSpecialChars.TableHeaderSeparator
                && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.TableColumnSeparator;
#else
                && currentLine[1] == Consts.IonSpecialChars.TableHeaderSeparator
                && currentLine[0] == Consts.IonSpecialChars.TableColumnSeparator;
#endif
        }

        public bool IsEmptyLine(ArraySegment<char> currentLine)
        {
            return currentLine.Count == 0 || IsWhiteSpace(currentLine);
        }

        private static bool IsWhiteSpace(ArraySegment<char> currentLine)
        {
            for (int i = currentLine.Offset; i < currentLine.Count; i++)
            {
#if NETSTANDARD2_0
                if (!char.IsWhiteSpace(((IList<char>)currentLine)[i]))
#else
                if (!char.IsWhiteSpace(currentLine[i]))
#endif
                {
                    return false;
                }
            }
            return true;
        }
    }
}