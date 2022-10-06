using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public enum CallingConvention
    {
        Default = 0,
        C = 1,
        FastCall = 2,
        StdCall = 3,
        ThisCall = 4,
        VarArg = 5,
        HasThis = 0x20,
        ExplicitThis = 0x40
    }
}
