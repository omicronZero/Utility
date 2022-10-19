using Utility.Serialization;
using DataSpecTests.Mocks;
using System;
using Xunit;

namespace DataSpecTests
{
    public class ObjectSerializerTests
    {
        [Fact]
        public void TestGetObjectValidExact()
        {
            bool received = false;

            DummyBase GetObject(IObjectReader reader)
            {
                received = true;
                return new DummyBase();
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
                throw new SpecialException();
            }

            var mock = new Mock(GetObject, GetObjectData);

            mock.GetObject<DummyBase>(null);

            Assert.True(received);
        }

        [Fact]
        public void TestGetObjectDataValidExact()
        {
            bool received = false;

            DummyBase GetObject(IObjectReader reader)
            {
                throw new SpecialException();
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
                received = true;
            }

            var mock = new Mock(GetObject, GetObjectData);

            mock.GetObjectData<DummyBase>(null, null);

            Assert.True(received);
        }

        [Fact]
        public void TestGetObjectDataValidType()
        {
            DummyBase GetObject(IObjectReader reader)
            {
                throw new SpecialException();
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
            }

            var mock = new Mock(GetObject, GetObjectData);

            mock.GetObjectData(null, new Dummy());
        }

        [Fact]
        public void TestGetObjectDataExactType()
        {
            DummyBase GetObject(IObjectReader reader)
            {
                throw new SpecialException();
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
            }

            var mock = new Mock(GetObject, GetObjectData);

            mock.GetObjectData(null, new DummyBase());
        }

        [Fact]
        public void TestGetObjectDataReaderWriterReceived()
        {
            var rw = new DummyReadWriter();

            DummyBase GetObject(IObjectReader reader)
            {
                Assert.Equal(rw, reader);
                return null;
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
                Assert.Equal(rw, writer);
            }

            var mock = new Mock(GetObject, GetObjectData);

            mock.GetObject<DummyBase>(rw);
            mock.GetObjectData(rw, new DummyBase());
        }

        [Fact]
        public void TestGenericMatchingGetObject()
        {
            bool received = false;

            DummyBase GetObject(IObjectReader reader)
            {
                received = true;
                return null;
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
                throw new SpecialException();
            }

            IObjectSerializer mock = new Mock(GetObject, GetObjectData);

            mock.GetObject<DummyBase>(null);

            Assert.True(received);
        }

        [Fact]
        public void TestGenericMatchingGetObjectData()
        {
            bool received = false;

            DummyBase GetObject(IObjectReader reader)
            {
                throw new SpecialException();
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
                received = true;
            }

            IObjectSerializer mock = new Mock(GetObject, GetObjectData);

            mock.GetObjectData(null, new DummyBase());

            Assert.True(received);
        }

        [Fact]
        public void TestGenericMismatchingGetObjectDataNonDummy()
        {
            DummyBase GetObject(IObjectReader reader)
            {
                throw new SpecialException();
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            { }

            IObjectSerializer mock = new Mock(GetObject, GetObjectData);

            Assert.Throws<ArgumentException>(() => mock.GetObjectData(null, new object()));
        }

        [Fact]
        public void TestGenericMismatchingGetObjectNonDummy()
        {
            DummyBase GetObject(IObjectReader reader)
            {
                return null;
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
                throw new SpecialException();
            }

            IObjectSerializer mock = new Mock(GetObject, GetObjectData);

            Assert.Throws<ArgumentException>(() => mock.GetObject<object>(null));
        }

        [Fact]
        public void TestGenericValidGetObjectDataInheritance()
        {
            DummyBase GetObject(IObjectReader reader)
            {
                throw new SpecialException();
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            { }

            IObjectSerializer mock = new Mock(GetObject, GetObjectData);

            mock.GetObjectData(null, new Dummy());
        }

        [Fact]
        public void TestGenericValidGetObjectInheritance()
        {
            DummyBase GetObject(IObjectReader reader)
            {
                return null;
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
                throw new SpecialException();
            }

            IObjectSerializer mock = new Mock(GetObject, GetObjectData);

            mock.GetObject<Dummy>(null);
        }

        private class Mock : ObjectSerializer<DummyBase>
        {
            public Func<IObjectReader, DummyBase> GetObjectCall { get; }
            public Action<IObjectWriter, DummyBase> GetObjectDataCall { get; }

            public Mock(Func<IObjectReader, DummyBase> getObjectCall, Action<IObjectWriter, DummyBase> getObjectDataCall)
            {
                GetObjectCall = getObjectCall ?? throw new ArgumentNullException(nameof(getObjectCall));
                GetObjectDataCall = getObjectDataCall ?? throw new ArgumentNullException(nameof(getObjectDataCall));
            }

            public override T GetObject<T>(IObjectReader source)
            {
                return (T)GetObjectCall(source);
            }

            public override void GetObjectData<T>(IObjectWriter target, T instance)
            {
                GetObjectDataCall(target, instance);
            }
        }
    }
}
