using System;
using System.Collections.ObjectModel;

namespace Utility.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false)]
    public class SerializationLayoutAttribute : Attribute
    {
        public ReadOnlyCollection<Type> Layout { get; }

        internal Type[] LayoutArray { get; }

        public SerializationLayoutAttribute(params Type[] layout)
        {
            if (layout == null)
                throw new ArgumentNullException(nameof(layout));

            LayoutArray = (Type[])layout.Clone();
            Layout = Array.AsReadOnly(LayoutArray);

            foreach (var t in LayoutArray)
            {
                if (t == null)
                    throw new ArgumentException("Layout entries must not be null.", nameof(layout));
            }
        }
    }
}
