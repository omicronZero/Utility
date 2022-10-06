using System;
using System.Collections.Generic;

namespace Utility.ObjectDescription
{
    public abstract class PropertyAccessorTable
    {
        protected abstract IList<PropertyAccessor> PropertiesCore { get; }
        protected abstract PropertyAccessor GetAccessorCore(string name);

        public abstract bool IsProperty(PropertyAccessor accessor);

        public PropertyAccessor GetAccessor(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return GetAccessorCore(name);
        }

        public IList<PropertyAccessor> Properties => PropertiesCore;
    }
}