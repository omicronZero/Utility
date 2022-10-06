using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public abstract class ObjectProperty<T> : IObjectProperty, IObjectProperty<T>
    {
        public abstract T Value { get; set; }

        public abstract bool IsReadonly { get; }

        object IObjectProperty.Value
        {
            get => Value;
            set
            {
                if (value is T v)
                {
                    Value = v;
                }
                else
                    throw new ArgumentException($"Value of type { typeof(T).FullName } expected.");
            }
        }

        Type IObjectProperty.PropertyType => typeof(T);
    }
}
