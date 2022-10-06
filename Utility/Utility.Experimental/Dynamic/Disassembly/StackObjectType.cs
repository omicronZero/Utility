using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public enum StackObjectType
    {
        //TODO: find a better name than "Computed" (also in comments)
        /// <summary>
        /// Indicates that the stack's top element is a value that has been computed in the current during the current method's execution.
        /// </summary>
        Computed = 0,
        Inferred,
        Constant,
        MetadataToken,
        Argument,
        Local,
        Return,
        Field,
        LocalDynamicPoolMemory,
        /// <summary>
        /// Determines that the stack object type is an address to an object.
        /// </summary>
        Address = 0x10000,
        ReferencesToThis = 0x1000000,
        Modifiers = ~0xffff,
    }
}
