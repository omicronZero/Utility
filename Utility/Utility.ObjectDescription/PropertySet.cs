using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public abstract class PropertySet
    {
        public abstract PropertyAccessorTable PropertyTable { get; }

        public abstract bool IsReadonly { get; }

        public abstract T GetValue<T>(PropertyAccessor accessor);
        public abstract void SetValue<T>(PropertyAccessor accessor, T value);

        public abstract object GetValue(PropertyAccessor accessor);
        public abstract void SetValue(PropertyAccessor accessor, object value);

        public abstract Func<T> GetGetMethod<T>(PropertyAccessor accessor);
        public abstract Action<T> GetSetMethod<T>(PropertyAccessor accessor);
    }
}
