using System;

namespace Anixe.Ion
{
    internal static class Consts
    {
        public static class IonSpecialChars
        {
            public readonly static string NewLineEscaped = Environment.NewLine.Length == 1 ? "\\n" : "\\r\\n";
            public const char EscapeCharacter      = '\\';
            public const char SectionHeaderOpening = '[';
            public const char SectionHeaderClosing = ']';
            public const char BeginCommentLine     = '#';
            public const char TableColumnSeparator = '|';
            public const char TableHeaderSeparator = '-';
            public const char TableCellPadding     = ' ';
            public const char PropertyQuotation    = '"';
            public const char PropertyAssignment   = '=';
        }
    }
}
