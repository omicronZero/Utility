using System;

namespace Utility.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate,
        AllowMultiple = false, Inherited = false)]
    public class ObjectSerializerAttribute : Attribute
    {
        public Type SerializerType { get; }

        public ObjectSerializerAttribute(Type serializerType)
        {
            if (serializerType == null)
                throw new ArgumentNullException(nameof(serializerType));

            if (!typeof(IObjectSerializer).IsAssignableFrom(serializerType))
                throw new ArgumentException($"{typeof(IObjectSerializer).FullName} expected.", nameof(serializerType));

            SerializerType = serializerType;
        }
    }
}
