using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public abstract class Signature
    {
        public SignatureType SignatureType { get; }

        public abstract SignatureHelper ToSignatureHelper(ModuleBuilder module);

        internal Signature(SignatureType signatureType)
        {
            SignatureType = signatureType;
        }
    }
}
