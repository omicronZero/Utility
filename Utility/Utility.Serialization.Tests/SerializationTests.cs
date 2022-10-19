using Utility.Serialization;
using DataSpecTests.Mocks;
using System;
using Xunit;

namespace DataSpecTests
{
    public class SerializationTests
    {
        [Fact]
        public void TestNoSerializerDefined()
        {
            Assert.Null(Serialization.GetDefaultSerializer<int>());
        }

        [Fact]
        public void TestSerializeExternal()
        {
            var rw = new QueueReaderWriter();
            Serialization.GetDefaultSerializer<ExternalSerializable>().GetObjectData(rw, new ExternalSerializable(1, "2"));

            Assert.Equal(new (Type, object)[] { (typeof(int), 1), (typeof(string), "2") }, rw);
        }

        [Fact]
        public void TestDeserializeExternal()
        {
            var rw = new QueueReaderWriter();
            var original = new ExternalSerializable(1, "2");
            Serialization.GetDefaultSerializer<ExternalSerializable>().GetObjectData(rw, original);

            var inst = Serialization.GetDefaultSerializer<ExternalSerializable>().GetObject<ExternalSerializable>(rw);

            Assert.Equal(original, inst);
        }

        [Fact]
        public void TestSerializeInternal()
        {
            var rw = new QueueReaderWriter();
            Serialization.GetDefaultSerializer<InternalSerializableNoLayout>().GetObjectData(rw, new InternalSerializableNoLayout(1, "2"));

            Assert.Equal(new (Type, object)[] { (typeof(int), 1), (typeof(string), "2") }, rw);
        }

        [Fact]
        public void TestDeserializeInternal()
        {
            var rw = new QueueReaderWriter();
            var original = new InternalSerializableNoLayout(1, "2");
            Serialization.GetDefaultSerializer<InternalSerializableNoLayout>().GetObjectData(rw, original);

            var inst = Serialization.GetDefaultSerializer<InternalSerializableNoLayout>().GetObject<InternalSerializableNoLayout>(rw);

            Assert.Equal(original, inst);
        }

        [Fact]
        public void TestSerializeInternalExplicitLayout()
        {
            var rw = new QueueReaderWriter();
            Serialization.GetDefaultSerializer<InternalSerializableExplicitLayout>().GetObjectData(rw, new InternalSerializableExplicitLayout(1, "2"));

            Assert.Equal(new (Type, object)[] { (typeof(int), 1), (typeof(string), "2") }, rw);
        }

        [Fact]
        public void TestDeserializeInternalExplicitLayout()
        {
            var rw = new QueueReaderWriter();
            var original = new InternalSerializableExplicitLayout(1, "2");
            Serialization.GetDefaultSerializer<InternalSerializableExplicitLayout>().GetObjectData(rw, original);

            var inst = Serialization.GetDefaultSerializer<InternalSerializableExplicitLayout>().GetObject<InternalSerializableExplicitLayout>(rw);

            Assert.Equal(original, inst);
        }

        [Fact]
        public void TestSerializerNotInherited()
        {
            Assert.NotNull(Serialization.GetDefaultSerializer<InheritanceSerializableBase>());
            Assert.Null(Serialization.GetDefaultSerializer<InheritanceSerializableChild>());
        }

        [Fact]
        public void TestSerializerAttributeInherited()
        {
            //test whether derived attributes are correctly recognized as ObjectSerializerAttributes

            Assert.NotNull(Serialization.GetDefaultSerializer<InheritingAttributeSerializable>());
        }

        [ObjectSerializer(typeof(SerializationProxy))]
        private struct ExternalSerializable
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }

            public ExternalSerializable(int intValue, string stringValue)
            {
                IntValue = intValue;
                StringValue = stringValue;
            }
        }

        private struct InternalSerializableNoLayout : IObjectSerializable
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }

            public InternalSerializableNoLayout(int intValue, string stringValue)
            {
                IntValue = intValue;
                StringValue = stringValue;
            }

            internal InternalSerializableNoLayout(IObjectReader reader)
            {
                IntValue = reader.Read<int>();
                StringValue = reader.Read<string>();
            }

            void IObjectSerializable.GetObjectData(IObjectWriter writer)
            {
                writer.Write(IntValue);
                writer.Write(StringValue);
            }
        }

        [SerializationLayout(typeof(int), typeof(string))]
        private struct InternalSerializableExplicitLayout : IObjectSerializable
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }

            public InternalSerializableExplicitLayout(int intValue, string stringValue)
            {
                IntValue = intValue;
                StringValue = stringValue;
            }

            internal InternalSerializableExplicitLayout(IObjectReader reader)
            {
                IntValue = reader.Read<int>();
                StringValue = reader.Read<string>();
            }

            void IObjectSerializable.GetObjectData(IObjectWriter writer)
            {
                writer.Write(IntValue);
                writer.Write(StringValue);
            }
        }

        private class SerializationProxy : IObjectSerializer
        {
            public T GetObject<T>(IObjectReader source)
            {
                if (typeof(T) != typeof(ExternalSerializable))
                    throw new ArgumentException("Unsupported type.", nameof(T));

                return (T)(object)new ExternalSerializable() { IntValue = source.Read<int>(), StringValue = source.Read<string>() };
            }

            public void GetObjectData<T>(IObjectWriter target, T instance)
            {
                if (!(instance is ExternalSerializable v))
                    throw new ArgumentException("Unsupported value type.", nameof(instance));

                target.Write(v.IntValue);
                target.Write(v.StringValue);
            }
        }

        [ObjectSerializer(typeof(InheritanceSerializationProxy))]
        private class InheritanceSerializableBase
        { }

        private class InheritanceSerializableChild : InheritanceSerializableBase
        { }

        private class InheritanceSerializationProxy : IObjectSerializer
        {
            public T GetObject<T>(IObjectReader source)
            {
                Assert.Equal(typeof(InheritanceSerializableBase), typeof(T));
                return default;
            }

            public void GetObjectData<T>(IObjectWriter target, T instance)
            {
                Assert.IsType<InheritanceSerializableBase>(instance);
            }
        }

        private class InheritingAttribute : ObjectSerializerAttribute
        {
            public InheritingAttribute()
                : base(typeof(SerializationProxy))
            { }
        }

        [Inheriting]
        private class InheritingAttributeSerializable
        {
        }
    }

}
