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
                && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.HeaderOpeningCharacter;
#else
                && currentLine[0] == Consts.IonSpecialChars.HeaderOpeningCharacter;
#endif
        }

        public bool IsTableHeaderRow(ArraySegment<char> currentLine, bool passedCurrentTableHeaderRow)
        {
          return !passedCurrentTableHeaderRow
              && currentLine.Count > 1
#if NETSTANDARD2_0
              && ((IList<char>)currentLine)[1] != Consts.IonSpecialChars.TableHeaderSeparatorCharacter
              && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.TableOpeningCharacter;
#else
              && currentLine[1] != Consts.IonSpecialChars.TableHeaderSeparatorCharacter
              && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter;
#endif
        }

        public bool IsTableDataRow(ArraySegment<char> currentLine, bool passedCurrentTableHeaderRow)
        {
          return passedCurrentTableHeaderRow
              && currentLine.Count > 1
#if NETSTANDARD2_0
              && ((IList<char>)currentLine)[1] != Consts.IonSpecialChars.TableHeaderSeparatorCharacter
              && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.TableOpeningCharacter;
#else
              && currentLine[1] != Consts.IonSpecialChars.TableHeaderSeparatorCharacter
              && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter;
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
            return firstChar != Consts.IonSpecialChars.TableOpeningCharacter
                && firstChar != Consts.IonSpecialChars.HeaderOpeningCharacter
                && firstChar != Consts.IonSpecialChars.CommentCharacter
                && !IsWhiteSpace(currentLine);
        }

        public bool IsComment(ArraySegment<char> currentLine)
        {
            return currentLine.Count != 0
#if NETSTANDARD2_0
                && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.CommentCharacter;
#else
                && currentLine[0] == Consts.IonSpecialChars.CommentCharacter;
#endif
        }

        public bool IsTableRow(ArraySegment<char> currentLine)
        {
            return currentLine.Count != 0
#if NETSTANDARD2_0
                && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.TableOpeningCharacter;
#else
                && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter;
#endif
        }

        public bool IsTableHeaderSeparatorRow(ArraySegment<char> currentLine)
        {
            return currentLine.Count > 1
#if NETSTANDARD2_0
                && ((IList<char>)currentLine)[1] == Consts.IonSpecialChars.TableHeaderSeparatorCharacter
                && ((IList<char>)currentLine)[0] == Consts.IonSpecialChars.TableOpeningCharacter;
#else
                && currentLine[1] == Consts.IonSpecialChars.TableHeaderSeparatorCharacter
                && currentLine[0] == Consts.IonSpecialChars.TableOpeningCharacter;
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