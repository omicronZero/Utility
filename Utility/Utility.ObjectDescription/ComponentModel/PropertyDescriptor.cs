using System;
using System.Collections.Generic;
using System.Text;
using Utility.ObjectDescription.Constraint;

namespace Utility.ObjectDescription.ComponentModel
{
    public abstract class PropertyDescriptor : MethodGroupDescriptor
    {
        public MethodDescriptor GetMethod { get; }
        public MethodDescriptor SetMethod { get; }

        public ObjectConstraint Constraint { get; }

        public SetPropertyConventions SetMethodConventions { get; }

        public sealed override bool HasMethodRoles => true;

        public sealed override IList<MethodDescriptor> Methods { get; }

        public bool CanRead => GetMethod != null;
        public bool CanWrite => SetMethod != null;

        public bool IsStatic => GetMethod?.IsStatic ?? (SetMethod?.IsStatic ?? false);

        protected PropertyDescriptor(ObjectConstraint constraint, MethodDescriptor getMethod, MethodDescriptor setMethod, SetPropertyConventions setMethodConventions)
        {
            if (constraint == null)
                throw new ArgumentNullException(nameof(constraint));

            if (getMethod != null)
            {
                if (setMethod != null)
                {
                    if (getMethod.IsStatic != setMethod.IsStatic)
                        throw new ArgumentException("Get- and set-method must either be both static or both non-static.");
                }
            }

            int ind = -1;

            if (setMethod != null)
            {
                if (setMethod.ParameterCount < 1)
                    throw new ArgumentException("Set-method must have at least one parameter.", nameof(setMethod));


                if (setMethodConventions == SetPropertyConventions.First)
                    ind = 0;
                else if (setMethodConventions == SetPropertyConventions.Last)
                    ind = setMethod.ParameterCount - 1;
                else
                    throw new ArgumentException("Unsupported method conventions.", nameof(setMethodConventions));
            }

            //WARNING: correct order?
            if (getMethod != null)
            {
                if (!constraint.IsValid(getMethod.ReturnParameter.ParameterConstraint))
                    throw new ArgumentException("The get-method's return parameter constraint is invalid on the property constraint.");
            }

            if (setMethod != null)
            {
                if (!setMethod[ind].ParameterConstraint.IsValidConstraint(constraint))
                    throw new ArgumentException("The set-method's value parameter constraint is invalid on the property constraint.");
            }

            Constraint = constraint;
            GetMethod = getMethod;
            SetMethod = setMethod;
            SetMethodConventions = SetMethodConventions;

            var m = new MethodDescriptor[(getMethod == null ? 0 : 1) + (setMethod == null ? 0 : 1)];

            int i = 0;

            if (getMethod != null)
                m[i++] = getMethod;
            if (setMethod != null)
                m[i] = setMethod;

            Methods = Array.AsReadOnly(m);
        }
    }
}
