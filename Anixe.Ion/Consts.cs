using System;

namespace Anixe.Ion
{
    internal static class Consts
    {
        public const string False = "false";
        public const string True = "true";
        public static readonly char[] ProhibitedTableCellCharacters = new[] { '\n', '|' };

        public static class ErrorMessages
        {
            public const string UndefinedFilePath = "File path must be defined!";
            public const string FileDoesNotExist  = "File '{0}' does not exist!";
        }

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

