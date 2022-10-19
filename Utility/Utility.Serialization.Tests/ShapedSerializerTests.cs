using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Utility.Serialization;
using DataSpecTests.Mocks;

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0051 // Remove unused private members
namespace DataSpecTests
{
    public class ShapedSerializerTests
    {
        [Fact]
        public void TestCallGetObject()
        {
            var serializer = Serialization.GetDefaultSerializer<CallTest>();
            Assert.Throws<SpecialException>(() => serializer.GetObject<CallTest>(null));
        }

        [Fact]
        public void TestCallGetObjectData()
        {
            var serializer = Serialization.GetDefaultSerializer<CallTest>();
            Assert.Throws<SpecialException>(() => serializer.GetObjectData<CallTest>(null, new CallTest()));
        }

        [Fact]
        public void TestByOrderGetObjectData()
        {
            var serializer = Serialization.GetDefaultSerializer<BindByOrderMethod>();
            var queueRw = new QueueReaderWriter();

            serializer.GetObjectData(queueRw, new BindByOrderMethod());

            Assert.Equal(new object[] { 1, "2" }, queueRw.GetObjects());
        }

        [Fact]
        public void TestByOrderGetObject()
        {
            var serializer = Serialization.GetDefaultSerializer<BindByOrderMethod>();
            var queueRw = new QueueReaderWriter();

            queueRw.Write(1);
            queueRw.Write("2");

            serializer.GetObject<BindByOrderMethod>(queueRw);

        }
        [Fact]
        public void TestByNameGetObjectData()
        {
            var serializer = Serialization.GetDefaultSerializer<BindByNameMethod>();
            var queueRw = new QueueReaderWriter();

            serializer.GetObjectData(queueRw, new BindByNameMethod());

            Assert.Equal(new object[] { 1, "2" }, queueRw.GetObjects());
        }

        [Fact]
        public void TestByNameGetObject()
        {
            var serializer = Serialization.GetDefaultSerializer<BindByNameMethod>();
            var queueRw = new QueueReaderWriter();

            queueRw.Write(1);
            queueRw.Write("2");

            serializer.GetObject<BindByNameMethod>(queueRw);

        }

        [Fact]
        public void TestByOrderGetObjectDataStaticSerialize()
        {
            var serializer = Serialization.GetDefaultSerializer<BindByOrderMethodStaticSerialize>();
            var queueRw = new QueueReaderWriter();

            serializer.GetObjectData(queueRw, new BindByOrderMethodStaticSerialize());

            Assert.Equal(new object[] { 1, "2" }, queueRw.GetObjects());
        }

        [Fact]
        public void TestByOrderGetObjectStaticSerialize()
        {
            var serializer = Serialization.GetDefaultSerializer<BindByOrderMethodStaticSerialize>();
            var queueRw = new QueueReaderWriter();

            queueRw.Write(1);
            queueRw.Write("2");

            serializer.GetObject<BindByOrderMethodStaticSerialize>(queueRw);

        }
        [Fact]
        public void TestByNameGetObjectDataStaticSerialize()
        {
            var serializer = Serialization.GetDefaultSerializer<BindByNameMethodStaticSerialize>();
            var queueRw = new QueueReaderWriter();

            serializer.GetObjectData(queueRw, new BindByNameMethodStaticSerialize());

            Assert.Equal(new object[] { 1, "2" }, queueRw.GetObjects());
        }

        [Fact]
        public void TestByNameGetObjectStaticSerialize()
        {
            var serializer = Serialization.GetDefaultSerializer<BindByNameMethodStaticSerialize>();
            var queueRw = new QueueReaderWriter();

            queueRw.Write(1);
            queueRw.Write("2");

            serializer.GetObject<BindByNameMethodStaticSerialize>(queueRw);
        }

        [ShapedSerializer]
        private class CallTest
        {
            public static CallTest Deserialize()
            {
                throw new SpecialException();
            }

            private void Serialize()
            {
                throw new SpecialException();
            }
        }

        [ShapedSerializer()]
        private class BindByOrderMethod
        {
            public static BindByOrderMethod Deserialize(int intValue, string stringValue)
            {
                Assert.Equal(1, intValue);
                Assert.Equal("2", stringValue);
                return new BindByOrderMethod();
            }

            private void Serialize(out int intValue, out string stringValue)
            {
                intValue = 1;
                stringValue = "2";
            }
        }

        [ShapedSerializer(true)]
        private class BindByNameMethod
        {
            public static BindByNameMethod Deserialize(string stringValue, int intValue)
            {
                Assert.Equal(1, intValue);
                Assert.Equal("2", stringValue);
                return new BindByNameMethod();
            }

            private void Serialize(out int intValue, out string stringValue)
            {
                intValue = 1;
                stringValue = "2";
            }
        }

        [ShapedSerializer()]
        private class BindByOrderCtr
        {
            public BindByOrderCtr(int intValue, string stringValue)
            {
                Assert.Equal(1, intValue);
                Assert.Equal("2", stringValue);
            }

            private void Serialize(out int intValue, out string stringValue)
            {
                intValue = 1;
                stringValue = "2";
            }
        }

        [ShapedSerializer(true)]
        private class BindByNameCtr
        {
            public BindByNameCtr(string stringValue, int intValue)
            {
                Assert.Equal(1, intValue);
                Assert.Equal("2", stringValue);
            }

            private void Serialize(out int intValue, out string stringValue)
            {
                intValue = 1;
                stringValue = "2";
            }
        }

        [ShapedSerializer()]
        private class BindByOrderMethodStaticSerialize
        {
            public static BindByOrderMethodStaticSerialize Deserialize(int intValue, string stringValue)
            {
                Assert.Equal(1, intValue);
                Assert.Equal("2", stringValue);
                return new BindByOrderMethodStaticSerialize();
            }

            private static void Serialize(BindByOrderMethodStaticSerialize instance, out int intValue, out string stringValue)
            {
                Assert.NotNull(instance);
                intValue = 1;
                stringValue = "2";
            }
        }

        [ShapedSerializer(true)]
        private class BindByNameMethodStaticSerialize
        {
            public static BindByNameMethodStaticSerialize Deserialize(string stringValue, int intValue)
            {
                Assert.Equal(1, intValue);
                Assert.Equal("2", stringValue);
                return new BindByNameMethodStaticSerialize();
            }

            private static void Serialize(BindByNameMethodStaticSerialize instance, out int intValue, out string stringValue)
            {
                Assert.NotNull(instance);
                intValue = 1;
                stringValue = "2";
            }
        }

        [ShapedSerializer()]
        private class BindByOrderCtrStaticSerialize
        {
            public BindByOrderCtrStaticSerialize(int intValue, string stringValue)
            {
                Assert.Equal(1, intValue);
                Assert.Equal("2", stringValue);
            }

            private static void Serialize(BindByOrderCtrStaticSerialize instance, out int intValue, out string stringValue)
            {
                Assert.NotNull(instance);
                intValue = 1;
                stringValue = "2";
            }
        }

        [ShapedSerializer(true)]
        private class BindByNameCtrStaticSerialize
        {
            public BindByNameCtrStaticSerialize(string stringValue, int intValue)
            {
                Assert.Equal(1, intValue);
                Assert.Equal("2", stringValue);
            }

            private static void Serialize(BindByNameCtrStaticSerialize instance, out int intValue, out string stringValue)
            {
                Assert.NotNull(instance);
                intValue = 1;
                stringValue = "2";
            }
        }
    }
}

#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore CA1822 // Mark members as static
