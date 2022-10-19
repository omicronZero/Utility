using System;

namespace Utility.Serialization
{
    public abstract class ObjectSerializer<TConstraint> : IObjectSerializer
    {
        public abstract T GetObject<T>(IObjectReader source)
            where T : TConstraint;

        public abstract void GetObjectData<T>(IObjectWriter target, T instance)
            where T : TConstraint;

        T IObjectSerializer.GetObject<T>(IObjectReader source)
        {
            return Invocation<T>.InvokeGetObject(this, source);
        }

        void IObjectSerializer.GetObjectData<T>(IObjectWriter target, T instance)
        {
            Invocation<T>.InvokeGetObjectData(this, target, instance);
        }

        private static class Invocation<T>
        {
            private static readonly Action<ObjectSerializer<TConstraint>, IObjectWriter, T>? _invokeGetObjectData;
            private static readonly Func<ObjectSerializer<TConstraint>, IObjectReader, T>? _invokeGetObject;

            static Invocation()
            {
                //T is constraint-less. Therefore, we try to create InvocationHelper<T> which has a constraint TConstraint on T
                //if the instantiation fails, we cannot pass the instances on to the serializer we attempt to invoke
                //this is an efficient way to type-check as we use reflection only here
                Type helperType;

                try
                {
                    helperType = typeof(InvocationHelper<>).MakeGenericType(typeof(TConstraint), typeof(T));
                }
                catch (ArgumentException)
                {
                    _invokeGetObject = null;
                    _invokeGetObjectData = null;
                    return;
                }

                _invokeGetObject = (Func<ObjectSerializer<TConstraint>, IObjectReader, T>)helperType.GetMethod("InvokeGetObject")!.CreateDelegate(typeof(Func<ObjectSerializer<TConstraint>, IObjectReader, T>));
                _invokeGetObjectData = (Action<ObjectSerializer<TConstraint>, IObjectWriter, T>)helperType.GetMethod("InvokeGetObjectData")!.CreateDelegate(typeof(Action<ObjectSerializer<TConstraint>, IObjectWriter, T>));
            }

            public static void InvokeGetObjectData(ObjectSerializer<TConstraint> serializer, IObjectWriter target, T instance)
            {
                var getObjectData = _invokeGetObjectData;

                if (getObjectData == null)
                    throw new ArgumentException("The indicated type does not match the constraint.");

                getObjectData(serializer, target, instance);
            }

            public static T InvokeGetObject(ObjectSerializer<TConstraint> serializer, IObjectReader source)
            {
                var getObject = _invokeGetObject;

                if (getObject == null)
                    throw new ArgumentException("The indicated type does not match the constraint.");

                return getObject(serializer, source);
            }
        }

        private static class InvocationHelper<T>
            where T : TConstraint
        {
            public static void InvokeGetObjectData(ObjectSerializer<TConstraint> serializer, IObjectWriter target, T instance)
            {
                serializer.GetObjectData(target, instance);
            }

            public static T InvokeGetObject(ObjectSerializer<TConstraint> serializer, IObjectReader source)
            {
                return serializer.GetObject<T>(source);
            }
        }
    }
}
