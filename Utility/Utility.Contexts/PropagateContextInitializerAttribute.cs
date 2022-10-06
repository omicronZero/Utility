using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Contexts
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public class PropagateContextInitializerAttribute : ContextInitializerAttribute
    {
        public PropagateContextInitializerAttribute(Type propagateType)
            : base(null, propagateType)
        { }
    }
}
