using Anixe.Ion.Exceptions;

namespace Anixe.Ion.Helpers
{
    internal static class ThrowHelper
    {
        public static void Throw_InvalidTableCellDataException()
        {
            throw new InvalidTableCellDataException("Table cell contains prohibited character");
        }
    }
}
