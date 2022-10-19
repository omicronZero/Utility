using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Utility.Serialization
{
    public static class Serialization
    {
        public static ISerializerProvider DefaultProvider { get; } = new DefaultSerializerProvider();

        public static IObjectSerializer? GetDefaultSerializer<T>()
        {
            var exFactory = DefaultSerializer<T>.ExceptionFactory;

            if (exFactory != null)
                throw exFactory();

            return DefaultSerializer<T>.Serializer;
        }

        public static void GetObjectData<T>(IObjectWriter target, T instance)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var serializer = GetDefaultSerializer<T>();

            if (serializer == null)
                throw new InvalidOperationException($"A serializer could not be obtained for type {typeof(T).FullName}.");

            serializer.GetObjectData(target, instance);
        }

        public static T GetObject<T>(IObjectReader source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var serializer = GetDefaultSerializer<T>();

            if (serializer == null)
                throw new InvalidOperationException($"A serializer could not be obtained for type {typeof(T).FullName}.");

            return serializer.GetObject<T>(source);
        }

        private static readonly Type[] constructorArguments = new Type[] { typeof(IObjectReader) };

        private static class DefaultSerializer<T>
        {
            public static IObjectSerializer? Serializer { get; }
            public static Func<IObjectReader, T>? DeserializationConstructor { get; }

            public static Func<Exception>? ExceptionFactory { get; }

            static DefaultSerializer()
            {
                var type = typeof(T);

                var proxy = type.GetCustomAttribute<ObjectSerializerAttribute>();

                if (proxy == null)
                {
                    //must implement IObjectSerializable to be serializable
                    if (!typeof(IObjectSerializable).IsAssignableFrom(typeof(T)))
                    {
                        DeserializationConstructor = null;
                        Serializer = null;
                        return;
                    }

                    var ctr = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArguments, null);

                    //the deserialization constructor is required although not specifyable on the interface
                    if (ctr == null)
                    {
                        //here, we build the ctr signature by the use of the full names of the expected types
                        ExceptionFactory = () => new ArgumentException(
                            $"Missing initialization constructor with signature .ctr({string.Join(", ", constructorArguments.Select((s) => s.FullName))} on {typeof(IObjectSerializable).FullName} implementation.");

                        DeserializationConstructor = null;
                        Serializer = null;
                        return;
                    }

                    //we build an expression to make the constructor call more efficient

                    ParameterExpression readerParameter = Expression.Parameter(typeof(IObjectReader), "reader");
                    Expression<Func<IObjectReader, T>> ctrCall = Expression.Lambda<Func<IObjectReader, T>>(Expression.New(ctr, readerParameter), readerParameter);

                    DeserializationConstructor = ctrCall.Compile();

                    //if layout is not null, we have a strict layout our data has to obey
                    var layout = type.GetCustomAttribute<SerializationLayoutAttribute>();

                    if (layout == null)
                        Serializer = (IObjectSerializer?)Activator.CreateInstance(typeof(SerializableSerializer<>).MakeGenericType(typeof(T)));
                    else
                        Serializer = (IObjectSerializer?)Activator.CreateInstance(typeof(LayoutSerializableSerializer<>).MakeGenericType(typeof(T)), new object[] { layout.LayoutArray });
                }
                else
                {
                    //layout gets ignored
                    //we just attempt to initialize the serialization proxy

                    object? serializer = null;
                    try
                    {
                        serializer = Activator.CreateInstance(proxy.SerializerType);
                    }
                    catch (TargetInvocationException ex)
                    {
                        ExceptionFactory = () => new ArgumentException("Serialization proxy initialization failed. See inner exception.", ex.InnerException);
                    }
                    catch (MissingMemberException ex)
                    {
                        ExceptionFactory = () => new ArgumentException("Missing public empty constructor on proxy type.", ex);
                    }

                    Serializer = (IObjectSerializer?)serializer;
                }
            }
        }

        private sealed class LayoutSerializableSerializer<TConstraint> : ObjectSerializer<TConstraint>
            where TConstraint : IObjectSerializable
        {
            public Type[] Layout { get; }

            public LayoutSerializableSerializer(Type[] layout)
            {
                Layout = layout ?? throw new ArgumentNullException(nameof(layout));
            }

            public override T GetObject<T>(IObjectReader source)
            {
                if (source == null)
                    throw new ArgumentNullException(nameof(source));

                return (T)DefaultSerializer<TConstraint>.DeserializationConstructor!(new ReaderWrapper(source, Layout));
            }

            public override void GetObjectData<T>(IObjectWriter target, T instance)
            {
                if (target == null)
                    throw new ArgumentNullException(nameof(target));

                instance.GetObjectData(new WriterWrapper(target, Layout));
            }

            //we basically just wrap the reader and writer so that they can check whether the layout remains fulfilled
            private sealed class ReaderWrapper : IObjectReader
            {
                public IObjectReader Source { get; }
                public Type[] Layout { get; }

                private int _index;

                public ReaderWrapper(IObjectReader source, Type[] layout)
                {
                    Source = source;
                    Layout = layout;
                }

                public T Read<T>()
                {
                    if (_index >= Layout.Length)
                        throw new InvalidOperationException("End of layout reached.");
                    if (Layout[_index] != typeof(T))
                        throw new InvalidOperationException($"The submitted type does not match the expected type {Layout[_index].FullName}.");

                    var instance = Source.Read<T>();

                    _index++;

                    return instance;
                }
            }

            private sealed class WriterWrapper : IObjectWriter
            {
                public IObjectWriter Target { get; }
                public Type[] Layout { get; }

                private int _index;

                public WriterWrapper(IObjectWriter target, Type[] layout)
                {
                    Target = target;
                    Layout = layout;
                }

                public void Write<T>(T instance)
                {
                    if (_index >= Layout.Length)
                        throw new InvalidOperationException("End of layout reached.");
                    if (Layout[_index] != typeof(T))
                        throw new InvalidOperationException($"The submitted instance is not an exact instance of the expected type {Layout[_index].FullName}.");

                    Target.Write(instance);

                    _index++;
                }
            }
        }

        private sealed class SerializableSerializer<TConstraint> : ObjectSerializer<TConstraint>
            where TConstraint : IObjectSerializable
        {
            public override T GetObject<T>(IObjectReader source)
            {
                if (source == null)
                    throw new ArgumentNullException(nameof(source));

                return (T)DefaultSerializer<TConstraint>.DeserializationConstructor!(source);
            }

            public override void GetObjectData<T>(IObjectWriter target, T instance)
            {
                if (target == null)
                    throw new ArgumentNullException(nameof(target));
                if (instance == null)
                    throw new ArgumentNullException(nameof(instance));

                instance.GetObjectData(target);
            }
        }

        private sealed class DefaultSerializerProvider : ISerializerProvider
        {
            public IObjectSerializer? GetSerializer<T>()
            {
                return GetDefaultSerializer<T>();
            }
        }
    }
}
