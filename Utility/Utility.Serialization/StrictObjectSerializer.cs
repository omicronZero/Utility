using System;

namespace Utility.Serialization
{
    public abstract class StrictObjectSerializer<TConstraint> : IObjectSerializer
    {
        public abstract TConstraint GetObject(IObjectReader source);
        protected abstract void GetObjectDataCore(IObjectWriter target, TConstraint instance);

        public void GetObjectData(IObjectWriter target, TConstraint instance)
        {
            CheckGetObjectData(instance);

            GetObjectDataCore(target, instance);
        }

        T IObjectSerializer.GetObject<T>(IObjectReader source)
        {
            if (typeof(T) != typeof(TConstraint))
                throw new ArgumentException("Requested type does not match constraint type.");

            return Invocation<T>.InvokeGetObject(this, source);
        }

        void IObjectSerializer.GetObjectData<T>(IObjectWriter target, T instance)
        {
            if (typeof(T) != typeof(TConstraint))
                throw new ArgumentException("Instance type does not match constraint type.");

            Invocation<T>.InvokeGetObjectData(this, target, instance);
        }

        private static void CheckGetObjectData(TConstraint instance)
        {
            if (instance != null && instance.GetType() != typeof(TConstraint))
                throw new ArgumentException("Instance type does not match constraint type.");
        }

        private static class Invocation<T>
        {
            private static readonly Action<StrictObjectSerializer<TConstraint>, IObjectWriter, T>? _invokeGetObjectData;
            private static readonly Func<StrictObjectSerializer<TConstraint>, IObjectReader, T>? _invokeGetObject;

            static Invocation()
            {
                //similar to the way ObjectSerializer acts. Here, we do not allow child types of TConstraint
                //but only exact matches
                Type? helperType;

                if (typeof(T) != typeof(TConstraint))
                    helperType = null;
                else
                {
                    try
                    {
                        helperType = typeof(InvocationHelper<>).MakeGenericType(typeof(TConstraint), typeof(T));
                    }
                    catch (ArgumentException)
                    {
                        helperType = null;
                    }
                }

                if (helperType == null)
                {
                    _invokeGetObject = null;
                    _invokeGetObjectData = null;
                    return;
                }

                _invokeGetObject = (Func<StrictObjectSerializer<TConstraint>, IObjectReader, T>)helperType.GetMethod("InvokeGetObject")!.CreateDelegate(typeof(Func<StrictObjectSerializer<TConstraint>, IObjectReader, T>));
                _invokeGetObjectData = (Action<StrictObjectSerializer<TConstraint>, IObjectWriter, T>)helperType.GetMethod("InvokeGetObjectData")!.CreateDelegate(typeof(Action<StrictObjectSerializer<TConstraint>, IObjectWriter, T>));
            }

            public static void InvokeGetObjectData(StrictObjectSerializer<TConstraint> serializer, IObjectWriter target, T instance)
            {
                var getObjectData = _invokeGetObjectData;

                if (getObjectData == null)
                    throw new ArgumentException("The indicated type does not match the constraint.");

                getObjectData(serializer, target, instance);
            }

            public static T InvokeGetObject(StrictObjectSerializer<TConstraint> serializer, IObjectReader source)
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

            public static void InvokeGetObjectData(StrictObjectSerializer<TConstraint> serializer, IObjectWriter target, T instance)
            {
                serializer.GetObjectData(target, instance);
            }

            public static T InvokeGetObject(StrictObjectSerializer<TConstraint> serializer, IObjectReader source)
            {
                return (T)serializer.GetObject(source)!;
            }
        }
    }
}
