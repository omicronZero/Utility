using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription.ComponentModel
{
    public abstract class MethodDescriptor : MemberDescriptor
    {
        public IList<ParameterDescriptor> Parameters { get; }
        public ParameterDescriptor ReturnParameter { get; }

        public abstract Constraint.ObjectConstraint InstanceConstraint { get; }

        public abstract object Invoke(object instance, params object[] parameters);
        public abstract Delegate CreateDelegate(Type delegateType, object instance);

        public bool IsStatic => InstanceConstraint != null;

        protected MethodDescriptor(ParameterDescriptor returnParameter, params ParameterDescriptor[] parameters)
        {
            if (returnParameter == null)
                throw new ArgumentNullException(nameof(returnParameter));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (!returnParameter.IsReturn)
                throw new ArgumentException("Return parameter must be marked as such.", nameof(returnParameter));

            parameters = (ParameterDescriptor[])parameters.Clone();

            foreach (ParameterDescriptor d in parameters)
            {
                if (d == null)
                    throw new ArgumentException("Parameters must not be null.", nameof(parameters));

                if (d.IsReturn)
                    throw new ArgumentException("Parameters must not be return parameters.", nameof(parameters));
            }

            ReturnParameter = returnParameter;
            Parameters = Array.AsReadOnly(parameters);
        }

        public TDelegate CreateDelegate<TDelegate>()
            where TDelegate : Delegate
        {
            return CreateDelegate<TDelegate>(null);
        }

        public virtual TDelegate CreateDelegate<TDelegate>(object instance)
            where TDelegate : Delegate
        {
            return (TDelegate)CreateDelegate(typeof(TDelegate), instance);
        }

        public Delegate CreateDelegate(Type delegateType)
        {
            return CreateDelegate(delegateType, null);
        }

        public ParameterDescriptor this[int index] => Parameters[index];

        public int ParameterCount => Parameters.Count;
    }
}
