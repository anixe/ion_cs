using System;

namespace Anixe.Ion
{
    internal static class Consts
    {
        public static class ErrorMessages
        {
            public const string UndefinedFilePath = "File path must be defined!";
            public const string FileDoesNotExist  = "File '{0}' does not exist!";
        }

        public static class IonSpecialChars
        {
            public const char HeaderOpeningCharacter        = '[';
            public const char HeaderClosingCharacter        = ']';
            public const char CommentCharacter              = '#';
            public const char TableOpeningCharacter         = '|';
            public const char TableHeaderSeparatorCharacter = '-';
        }
    }
}

