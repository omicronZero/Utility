using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Contexts
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public class ContextInitializerAttribute : Attribute
    {
        public Type ContextInitializerType { get; }

        /// <summary>
        /// Gets a specific type that should be initialized instead of the queried type. The type must derive from the type on which this attribute was defined.
        /// This value is required on interfaces and abstract types.
        /// </summary>
        public Type PropagateType { get; }

        public ContextInitializerAttribute(Type contextInitializerType)
            : this(contextInitializerType, null)
        { }

        protected ContextInitializerAttribute(Type contextInitializerType, Type propagateType)
        {
            if (contextInitializerType != null && !typeof(ContextInitializer).IsAssignableFrom(contextInitializerType))
                throw new ArgumentException("Context initializer type or null expected for the context initializer type parameter.", nameof(contextInitializerType));

            ContextInitializerType = contextInitializerType;
            PropagateType = propagateType;
        }
    }
}
