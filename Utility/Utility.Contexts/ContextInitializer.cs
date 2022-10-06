using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utility.Contexts
{
    public class ContextInitializer
    {
        public virtual T CreateDefaultObject<T>()
        {
            return (T)CreateDefaultObject(typeof(T));
        }

        public object CreateDefaultObject(Type type)
        {
            return CreateDefaultObjectCore(type);
        }

        protected virtual object CreateDefaultObjectCore(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            try
            {
                return Activator.CreateInstance(type);
            }
            catch (MissingMethodException)
            {
                throw new NotSupportedException($"Type type { type.FullName } cannot be constructed without parameters.");
            }
        }

        public static T CreateDefault<T>() => (T)CreateDefault(typeof(T));

        public static object CreateDefault(Type type)
        {

            Type propagateType;

            ContextInitializer initializer = GetInitializer(type, out propagateType);

            if (propagateType != null)
            {             
                return CreateDefault(propagateType);
            }

            if (initializer == null)
                throw new ArgumentException($"A context initializer is not specified for type { type.FullName }.", nameof(type));

            return initializer.CreateDefaultObject(type);
        }

        public static ContextInitializer GetInitializer(Type type, out Type propagateType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            ContextInitializerAttribute att = type.GetCustomAttribute<ContextInitializerAttribute>(false);

            if (att == null)
            {
                propagateType = null;
                return null;
            }

            propagateType = att.PropagateType;

            if (propagateType != null)
            {
                if (!type.IsAssignableFrom(propagateType))
                    throw new ArgumentException("Bad type propagate. The specified propagate type does not strictly derive from the type on which it was defined.");

                return null;
            }

            if (att.ContextInitializerType == null)
                return null;

            try
            {
                return (ContextInitializer)Activator.CreateInstance(att.ContextInitializerType);
            }
            catch (MissingMethodException)
            {
                throw new ArgumentException("Bad context initializer format. Parameterless constructor expected.", nameof(type));
            }
        }
    }
}