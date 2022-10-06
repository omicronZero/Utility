using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Utility.ObjectDescription.Constraint;

namespace Utility.ObjectDescription.ComponentModel
{
    public class ReflectionParameterDescripor : ParameterDescriptor
    {
        public ParameterInfo Parameter { get; }

        private readonly ObjectConstraint _constraint;

        public ReflectionParameterDescripor(ParameterInfo parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            Parameter = parameter;

            _constraint = ObjectConstraintBuilder.Create().OfType(parameter.ParameterType);
        }

        public override bool IsIn => Parameter.IsIn;

        public override bool IsOut => Parameter.IsOut;

        public override bool IsRef => !Parameter.IsOut && Parameter.ParameterType.IsByRef;

        public override bool IsReturn => Parameter.IsRetval;

        public override bool HasDefault => Parameter.HasDefaultValue;

        public override string Name => Parameter.Name;

        public override ObjectConstraint ParameterConstraint => _constraint;

        public override object[] GetAttributes(Type attributeType)
        {
            if (attributeType == null)
                throw new ArgumentNullException(nameof(attributeType));

            return Parameter.GetCustomAttributes(attributeType, true);
        }

        public override object GetDefaultValue()
        {
            return Parameter.DefaultValue;
        }
    }
}
