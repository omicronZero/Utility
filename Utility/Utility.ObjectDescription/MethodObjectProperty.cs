using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.ObjectDescription.ComponentModel;
using Utility.ObjectDescription.Constraint;

namespace Utility.ObjectDescription
{
    public abstract class MethodObjectProperty : IObjectProperty
    {
        public MethodDescriptor GetMethod { get; }
        public MethodDescriptor SetMethod { get; }

        protected abstract object InstanceCore { get; }

        internal MethodObjectProperty(MethodDescriptor getMethod, MethodDescriptor setMethod)
        {
            if (getMethod == null)
                throw new ArgumentNullException(nameof(getMethod));

            GetMethod = getMethod;
            SetMethod = setMethod;
        }

        public bool IsStatic => IsStaticCore;

        internal virtual bool IsStaticCore => false;

        public object Value
        {
            get => ValueCore;
            set => ValueCore = value;
        }

        protected virtual object ValueCore
        {
            get => GetMethod.Invoke(InstanceCore, null);
            set => SetMethod.Invoke(InstanceCore, null);
        }

        public abstract bool IsReadonly { get; }

        public abstract Type PropertyType { get; }

        internal static void CheckGetterAndSetter<T>(MethodDescriptor getMethod, MethodDescriptor setMethod)
        {
            if (setMethod != null && setMethod.ParameterCount < 1)
                throw new ArgumentException("Set-method does not provide a parameter for value assignment.", nameof(setMethod));

            if (getMethod.ParameterCount != 0 || (setMethod != null && setMethod.ParameterCount != 1))
                throw new NotSupportedException("Index on get- and set-method not supported.");

            TypeConstraintRule constraint = TypeConstraintRule.Get(typeof(T));

            if (!constraint.IsValidConstraint(getMethod.ReturnParameter.ParameterConstraint))
                throw new ArgumentException("The specified property type is not allowed by the get-method constraint.");

            if (setMethod != null && !setMethod[0].ParameterConstraint.IsValidConstraint(constraint))
                throw new ArgumentException("The specified property type is not allowed by the set-method constraint.");
        }

        public static MethodObjectProperty Create(object instance, PropertyDescriptor property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (!property.CanRead)
                throw new ArgumentException("Property must support reading.", nameof(property));

            bool isStatic = property.IsStatic;
            bool hasConstraint;
            Type propertyType = property.Constraint.GetTypeConstraint(out hasConstraint);

            if (!hasConstraint)
                throw new ArgumentException("Type constraint expected on property constraint.", nameof(property));

            Type[] args = isStatic ? new Type[] { propertyType } : new Type[] { instance.GetType(), propertyType };

            if (isStatic)
            {
                return (MethodObjectProperty)typeof(MethodObjectProperty<,>).MakeGenericType(args).GetConstructor(args).Invoke(null, new object[] { instance, property });
            }
            else
            {
                return (MethodObjectProperty)typeof(MethodObjectProperty<>).MakeGenericType(args).GetConstructor(args).Invoke(null, new object[] { property });
            }
        }

        public static MethodObjectProperty Create(object instance, ObjectConstraint propertyConstraint, MethodDescriptor getMethod, MethodDescriptor setMethod)
        {
            if (getMethod == null)
                throw new ArgumentNullException(nameof(getMethod));

            if (setMethod != null && setMethod.ParameterCount < 1)
                throw new ArgumentException("Set-method does not provide a parameter for value assignment.", nameof(setMethod));

            bool isStatic = getMethod.IsStatic;
            bool hasConstraint;
            Type propertyType = propertyConstraint.GetTypeConstraint(out hasConstraint);

            if (!hasConstraint)
                throw new ArgumentException("Type constraint expected on property constraint.", nameof(propertyConstraint));

            Type[] args = isStatic ? new Type[] { propertyType } : new Type[] { instance.GetType(), propertyType };

            if (isStatic)
            {
                return (MethodObjectProperty)typeof(MethodObjectProperty<,>).MakeGenericType(args).GetConstructor(args).Invoke(null, new object[] { instance, getMethod, setMethod });
            }
            else
            {
                return (MethodObjectProperty)typeof(MethodObjectProperty<>).MakeGenericType(args).GetConstructor(args).Invoke(null, new object[] { getMethod, setMethod });
            }
        }
    }

    public class MethodObjectProperty<TInstance, T> : MethodObjectProperty, IObjectProperty<T>
    {
        private readonly TInstance _instance;
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        public MethodObjectProperty(TInstance instance, MethodDescriptor getMethod, MethodDescriptor setMethod)
            : base(getMethod, setMethod)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (getMethod == null)
                throw new ArgumentNullException(nameof(getMethod));

            if (typeof(TInstance).IsValueType && setMethod != null)
                throw new ArgumentException("Set-method unsupported on value type instances.");

            if (getMethod.IsStatic || (setMethod != null && setMethod.IsStatic))
                throw new ArgumentException("The specified get- and set-method must not be static.");

            if (!getMethod.InstanceConstraint.IsValid(instance) || (setMethod != null && !setMethod.InstanceConstraint.IsValid(instance)))
                throw new ArgumentException("The specified instance is not supported by the indicated methods.");

            CheckGetterAndSetter<T>(getMethod, setMethod);

            _instance = instance;

            _getter = getMethod.CreateDelegate<Func<T>>();
            _setter = setMethod?.CreateDelegate<Action<T>>();
        }

        public MethodObjectProperty(TInstance instance, PropertyDescriptor property)
            : base((property ?? throw new ArgumentNullException(nameof(property))).GetMethod, property.SetMethod)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (typeof(TInstance).IsValueType && property.SetMethod != null)
                throw new ArgumentException("Set-method unsupported on value type instances.");

            if (property.IsStatic)
                throw new ArgumentException("The specified property must not be static.", nameof(property));

            if (!property.Constraint.IsValid(instance))
                throw new ArgumentException("The specified instance is not supported by the indicated property.");

            CheckGetterAndSetter<T>(property.GetMethod, property.SetMethod);

            _instance = instance;

            _getter = property.GetMethod.CreateDelegate<Func<T>>();
            _setter = property.SetMethod?.CreateDelegate<Action<T>>();
        }

        internal override bool IsStaticCore => false;

        new public T Value
        {
            get => _getter();
            set => (_setter ?? throw new NotSupportedException("The property does not support setting."))(value);
        }

        public override bool IsReadonly => _setter == null;

        public override Type PropertyType => typeof(T);

        protected override object InstanceCore => _instance;
    }

    public class MethodObjectProperty<T> : MethodObjectProperty, IObjectProperty<T>
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        public MethodObjectProperty(MethodDescriptor getMethod, MethodDescriptor setMethod)
            : base(getMethod, setMethod)
        {
            if (getMethod == null)
                throw new ArgumentNullException(nameof(getMethod));

            if (!getMethod.IsStatic || (setMethod != null && !setMethod.IsStatic))
                throw new ArgumentException("The specified get- and set-method must be static.");

            CheckGetterAndSetter<T>(getMethod, setMethod);

            _getter = getMethod.CreateDelegate<Func<T>>();
            _setter = setMethod?.CreateDelegate<Action<T>>();
        }

        public MethodObjectProperty(PropertyDescriptor property)
            : base((property ?? throw new ArgumentNullException(nameof(property))).GetMethod, property.SetMethod)
        {
            CheckGetterAndSetter<T>(property.GetMethod, property.SetMethod);

            _getter = property.GetMethod.CreateDelegate<Func<T>>();
            _setter = property.SetMethod?.CreateDelegate<Action<T>>();
        }

        internal override bool IsStaticCore => false;

        new public T Value
        {
            get => _getter();
            set => (_setter ?? throw new NotSupportedException("The property does not support setting."))(value);
        }

        public override bool IsReadonly => _setter == null;

        public override Type PropertyType => typeof(T);

        protected override object InstanceCore => null;
    }
}
