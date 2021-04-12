using Anixe.Ion.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Anixe.Ion.Helpers
{
    internal static class ThrowHelper
    {
        [DoesNotReturn]
        public static void Throw_InvalidTableCellDataException()
        {
            throw new InvalidTableCellDataException("Table cell contains prohibited character");
        }
    }
}
