using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Utility.ObjectDescription.Constraint;

namespace Utility.ObjectDescription.ComponentModel
{
    public class ReflectionMethodDescriptor : MethodDescriptor
    {
        public MethodInfo Method { get; }

        public override ObjectConstraint InstanceConstraint { get; }

        public ReflectionMethodDescriptor(MethodInfo method)
            : base(new ReflectionParameterDescripor(CheckMethodInfo(method).ReturnParameter), Array.ConvertAll(method.GetParameters(), (p) => new ReflectionParameterDescripor(p)))
        {
            Method = method;

            InstanceConstraint = TypeConstraintRule.CreateConstraint(method.DeclaringType);
        }

        public override Delegate CreateDelegate(Type delegateType, object instance)
        {
            if (delegateType == null)
                throw new ArgumentNullException(nameof(delegateType));

            return Method.CreateDelegate(delegateType, instance);
        }

        public override object Invoke(object instance, params object[] parameters)
        {
            return Method.Invoke(instance, parameters);
        }

        public override object[] GetAttributes(Type attributeType)
        {
            if (attributeType == null)
                throw new ArgumentNullException(nameof(attributeType));

            return Method.GetCustomAttributes(attributeType, true);
        }

        private static MethodInfo CheckMethodInfo(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            return methodInfo;
        }
    }
}
