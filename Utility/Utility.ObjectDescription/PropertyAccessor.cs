using System;
using System.Collections.Generic;
using System.Text;
using Utility.ObjectDescription.Constraint;

namespace Utility.ObjectDescription
{
    public abstract class PropertyAccessor : IEquatable<PropertyAccessor>
    {
        public abstract string Name { get; }
        public abstract int Index { get; }
        
        public abstract ObjectConstraint Constraint { get; }

        public abstract bool Equals(PropertyAccessor other);

        public abstract override int GetHashCode();

        public override string ToString()
        {
            return Name;
        }

        public sealed override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var o = obj as PropertyAccessor;

            if (o == null)
                return false;

            return Equals(o);
        }
    }
}
