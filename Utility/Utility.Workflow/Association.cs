using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public abstract class Association
    {
        public Type InstanceType { get; }
        public object Owner { get; }
        public bool OwnerByReference { get; }

        protected abstract object GetValue();

        internal Association(Type instanceType, object owner, bool ownerByReference)
        {
            InstanceType = instanceType ?? throw new ArgumentNullException(nameof(instanceType));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            OwnerByReference = ownerByReference && !owner.GetType().IsValueType;
        }

        public bool CheckOwner(object owner, Type expectedType = null, bool throwUponMismatch = true)
        {
            if (OwnerByReference ? !object.ReferenceEquals(Owner, owner) : object.Equals(Owner, owner))
            {
                if (throwUponMismatch)
                    ThrowOwnershipMismatch();

                return false;
            }

            if (expectedType != null && expectedType.IsAssignableFrom(InstanceType))
            {
                if (throwUponMismatch)
                    ThrowTypeMismatch();

                return false;
            }

            return true;
        }

        public abstract Association WithNewOwner(object owner);

        public abstract bool TryGet<TValue>(object owner, out TValue value);

        public abstract TValue Get<TValue>(object owner);

        protected static void ThrowOwnershipMismatch()
        {
            throw new InvalidOperationException("The current instance is owned by a different object than the one trying to acquire the value stored with the current association.");
        }

        protected static void ThrowTypeMismatch()
        {
            throw new InvalidOperationException("The current instance does not provide a value of the required type.");
        }

        public static Association<T> Create<T>(object owner, T instance, bool ownerByReference = true) => new Association<T>(owner, instance, ownerByReference);

        public static Association Create(object owner, object instance, bool ownerByReference = true, Type instanceType = null)
        {
            if (instanceType == null)
                instanceType = instance?.GetType() ?? typeof(object);

            return (Association)Activator.CreateInstance(typeof(Association<>).MakeGenericType(instanceType), owner, instance, ownerByReference);
        }

        public static bool Cast<T>(object owner, object instance, out T value)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            if (!(instance is Association assoc))
            {
                value = default;
                return false;
            }

            return assoc.TryGet(owner, out value);
        }

        public static T Cast<T>(object owner, object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            if (!(instance is Association assoc))
                throw new ArgumentException($"An instance of type {typeof(Association)} associated with the current instance was expected.");

            return assoc.Get<T>(owner);
        }
    }

    public class Association<T> : Association
    {
        public T Instance { get; }

        public Association(object owner, T instance, bool ownerByReference = true)
            : base(typeof(T), owner, ownerByReference)
        {
            Instance = instance;
        }

        protected sealed override object GetValue()
        {
            return Instance;
        }

        public override bool TryGet<TValue>(object owner, out TValue value)
        {
            if (!CheckOwner(owner, throwUponMismatch: false))
            {
                value = default;
                return false;
            }

            if (!(Instance is TValue v))
            {
                value = default;
                return false;
            }

            value = v;

            return true;
        }

        public override TValue Get<TValue>(object owner)
        {
            CheckOwner(owner, throwUponMismatch: true);

            if (!(Instance is TValue v))
            {
                ThrowTypeMismatch();
                return default; //unreached
            }

            return v;
        }

        public override Association WithNewOwner(object owner)
        {
            return new Association<T>(owner, Instance, OwnerByReference);
        }
    }
}
