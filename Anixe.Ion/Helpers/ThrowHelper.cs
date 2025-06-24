using System;
using System.Diagnostics.CodeAnalysis;

namespace Anixe.Ion.Helpers
{
    internal static class ThrowHelper
    {
        [DoesNotReturn]
        public static void Throw_ArgumentException_UndefinedFilePath()
        {
            throw new ArgumentException("File path must be defined!");
        }

        [DoesNotReturn]
        public static void Throw_ArgumentException_FileDoesNotExists(string filePath)
        {
            throw new ArgumentException(string.Format("File '{0}' does not exist!", filePath));
        }
    }
}
