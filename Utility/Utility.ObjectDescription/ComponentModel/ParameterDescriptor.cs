using System;
using System.Linq;
using System.Reflection;
using Utility.ObjectDescription.Constraint;

namespace Utility.ObjectDescription.ComponentModel
{
    public abstract class ParameterDescriptor : MemberDescriptor
    {
        public abstract bool IsIn { get; }
        public abstract bool IsOut { get; }
        public abstract bool IsRef { get; }
        public abstract bool IsReturn { get; }
        public abstract bool HasDefault { get; }

        public abstract object GetDefaultValue();

        public abstract string Name { get; }
        public abstract ObjectConstraint ParameterConstraint { get; }
    }
}