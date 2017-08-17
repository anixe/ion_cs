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
}
