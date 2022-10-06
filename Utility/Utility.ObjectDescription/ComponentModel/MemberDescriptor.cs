using System;
using System.Linq;

namespace Utility.ObjectDescription.ComponentModel
{
    public abstract class MemberDescriptor
    {
        public abstract object[] GetAttributes(Type attributeType);

        public virtual TAttribute[] GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            return (TAttribute[])GetAttributes(typeof(TAttribute));
        }

        public virtual object GetAttribute(Type attributeType)
        {
            return GetAttributes(attributeType).SingleOrDefault();
        }

        public virtual TAttribute GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return GetAttributes<TAttribute>().SingleOrDefault();
        }
    }
}