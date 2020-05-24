using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anixe.Ion
{
    [Flags]
    internal enum WriterState
    {
        None = 0,
        Section = 1,
        Property = 2,
        TableHeader = 4,
        TableRow = 8,
    }

    internal static class WriterStateExtensions
    {
        public static bool HasBitFlags(this WriterState that, WriterState flag)
        {
            return (that & flag) == flag;
        }
    }
}
