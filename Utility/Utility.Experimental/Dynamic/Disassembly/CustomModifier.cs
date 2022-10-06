using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public struct CustomModifier
    {
        public int MetadataToken { get; }
        public bool Required { get; }

        public CustomModifier(int metadataToken, bool required)
        {
            MetadataToken = metadataToken;
            Required = required;
        }
    }
}
