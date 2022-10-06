using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utility.Contexts
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public class DefaultContextInitializerAttribute : ContextInitializerAttribute
    {
        public DefaultContextInitializerAttribute()
            : base(typeof(DefaultInitializer))
        { }

        private sealed class DefaultInitializer : ContextInitializer
        {
            protected override object CreateDefaultObjectCore(Type type)
            {
                return type.InvokeMember(".ctr",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DoNotWrapExceptions | BindingFlags.CreateInstance,
                    null,
                    null,
                    null);
            }
        }
    }
}
