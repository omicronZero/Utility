using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Utility.ObjectDescription
{
    public class GenericPropertySet : PropertySet
    {
        private readonly object[] _values;

        public override PropertyAccessorTable PropertyTable { get; }

        public GenericPropertySet(PropertyAccessorTable propertyTable, IDictionary<PropertyAccessor, object> initialValues)
            : this(
                  propertyTable,
                  initialValues == null ? throw new ArgumentNullException(nameof(initialValues)) : new Func<PropertyAccessor, object>((s) => initialValues[s])
                  )
        { }

        public GenericPropertySet(PropertyAccessorTable propertyTable, Func<PropertyAccessor, object> initializer)
        {
            if (propertyTable == null)
                throw new ArgumentNullException(nameof(propertyTable));
            if (initializer == null)
                throw new ArgumentNullException(nameof(initializer));

            PropertyTable = propertyTable;
            _values = new object[propertyTable.Properties.Count];

            for (int i = 0; i < _values.Length; i++)
            {
                _values[i] = initializer(propertyTable.Properties[i]);
            }
        }

        public override bool IsReadonly
        {
            get { return false; }
        }

        public override Func<T> GetGetMethod<T>(PropertyAccessor accessor)
        {
            CheckAccessor(accessor);

            int index = accessor.Index;

            return () => (T)_values[index];
        }

        public override Action<T> GetSetMethod<T>(PropertyAccessor accessor)
        {
            CheckReadonly();

            return GetSetMethodCore<T>(accessor);
        }

        public override T GetValue<T>(PropertyAccessor accessor)
        {
            CheckAccessor(accessor);

            if (_values[accessor.Index] is T v)
                return v;
            else
            {
                throw new ArgumentException($"The value is not of type { typeof(T).FullName }.", nameof(T));
            }
        }

        public override object GetValue(PropertyAccessor accessor)
        {
            CheckAccessor(accessor);

            return _values[accessor.Index];
        }

        public override void SetValue<T>(PropertyAccessor accessor, T value)
        {
            CheckReadonly();

            SetValueCore(accessor, value);
        }

        public override void SetValue(PropertyAccessor accessor, object value)
        {
            CheckReadonly();

            SetValueCore(accessor, value);
        }

        protected Action<T> GetSetMethodCore<T>(PropertyAccessor accessor)
        {
            CheckAccessor(accessor);

            int index = accessor.Index;

            return (value) => _values[index] = value;
        }

        protected void SetValueCore<T>(PropertyAccessor accessor, T value)
        {
            CheckAccessor(accessor);
            accessor.Constraint.PassArgument(value);

            _values[accessor.Index] = value;
        }

        protected void SetValueCore(PropertyAccessor accessor, object value)
        {
            CheckAccessor(accessor);
            accessor.Constraint.PassArgument(value);
            _values[accessor.Index] = value;
        }

        private void CheckReadonly()
        {
            if (IsReadonly)
                throw new NotSupportedException("The property set is read-only.");
        }

        private void CheckAccessor(PropertyAccessor accessor)
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));

            if (!PropertyTable.Properties.Contains(accessor))
            {
                throw new ArgumentException("The specified accessor is not a member of the current property set.");
            }
        }
    }
}
