using Anixe.Ion.Exceptions;
using System;
#if !NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
#endif

namespace Anixe.Ion.Helpers
{
    internal static class ThrowHelper
    {
#if !NETSTANDARD2_0
        [DoesNotReturn]
#endif
        public static void Throw_InvalidTableCellDataException()
        {
            throw new InvalidTableCellDataException("Table cell contains prohibited character");
        }

#if !NETSTANDARD2_0
        [DoesNotReturn]
#endif
        public static void Throw_ArgumentNullException(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
