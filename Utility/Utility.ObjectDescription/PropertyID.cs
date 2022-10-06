using System;
using System.Collections.Generic;
using System.Reflection;
using Utility.ObjectDescription.Constraint;

namespace Utility.ObjectDescription
{
    public sealed class PropertyID : PropertyAccessor, IEquatable<PropertyID>
    {
        private int _hashCode;

        public override int Index { get; }
        public PropertyAccessorTable Table { get; }

        public PropertyInfo Property { get; }
        public override string Name => Property.Name;
        public Type PropertyType => Property.PropertyType;

        public override ObjectConstraint Constraint { get; }

        internal PropertyID(PropertyAccessorTable table, PropertyInfo property, int index)
        {
            Table = table;
            Property = property;

            _hashCode = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);

            Index = index;

            Constraint = new ObjectConstraint(TypeConstraintRule.Get(property.PropertyType));
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(PropertyID other)
        {
            return object.ReferenceEquals(this, other);
        }

        public override bool Equals(PropertyAccessor other)
        {
            return object.ReferenceEquals(this, other);
        }

        public MethodInfo GetMethod
        {
            get
            {
                MethodInfo m = Property.GetMethod;

                if (!m.IsPublic)
                    m = null;

                return m;
            }
        }

        public MethodInfo SetMethod
        {
            get
            {
                MethodInfo m = Property.SetMethod;

                if (!m.IsPublic)
                    m = null;

                return m;
            }
        }
    }
}
