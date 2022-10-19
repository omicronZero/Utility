using System;
using System.Collections.ObjectModel;

namespace Utility.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false)]
    public class SerializationLayoutAttribute : Attribute
    {
        public ReadOnlyCollection<Type> Layout { get; }

        public SerializationLayoutAttribute(params Type[] layout)
        {
            if (layout == null)
                throw new ArgumentNullException(nameof(layout));

            Layout = Array.AsReadOnly((Type[])layout.Clone());

            foreach (var t in Layout)
            {
                if (t == null)
                    throw new ArgumentException("Layout entries must not be null.", nameof(layout));
            }
        }
    }
}
