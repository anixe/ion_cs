using System;

namespace Anixe.Ion
{
    internal static class Consts
    {
        public const string False = "false";
        public const string True = "true";

        public static class ErrorMessages
        {
            public const string UndefinedFilePath = "File path must be defined!";
            public const string FileDoesNotExist  = "File '{0}' does not exist!";
        }

        public static class IonSpecialChars
        {
            public readonly static string NewLineEscaped = Environment.NewLine.Length == 1 ? "\\n" : "\\r\\n";
            public const char EscapeCharacter               = '\\';
            public const char HeaderOpeningCharacter        = '[';
            public const char HeaderClosingCharacter        = ']';
            public const char CommentCharacter              = '#';
            public const char TableOpeningCharacter         = '|';
            public const char TableHeaderSeparatorCharacter = '-';
            public const char WriteSpaceCharacter = ' ';
            public const char QuotationCharacter = '"';
            public const char EqualsCharacter = '=';
        }
    }
}

