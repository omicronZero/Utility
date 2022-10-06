using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data
{
    [Serializable]
    public struct EmptyType
    {
        public static EmptyType Instance => default;
    }
}
