using Utility.Serialization;
using DataSpecTests.Mocks;
using System;
using Xunit;

namespace DataSpecTests
{
    public partial class StrictObjectSerializerTests
    {
        [Fact]
        public void TestGetObjectValid()
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

            mock.GetObject(null);

            Assert.True(received);
        }

        [Fact]
        public void TestGetObjectDataValid()
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

            mock.GetObjectData(null, null);

            Assert.True(received);
        }

        [Fact]
        public void TestGetObjectDataFailType()
        {
            DummyBase GetObject(IObjectReader reader)
            {
                throw new SpecialException();
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            {
            }

            var mock = new Mock(GetObject, GetObjectData);

            Assert.Throws<ArgumentException>(() => mock.GetObjectData(null, new Dummy()));
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

            mock.GetObject(rw);
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
        public void TestGenericMismatchingGetObjectDataInheritance()
        {
            DummyBase GetObject(IObjectReader reader)
            {
                throw new SpecialException();
            }

            void GetObjectData(IObjectWriter writer, DummyBase instance)
            { }

            IObjectSerializer mock = new Mock(GetObject, GetObjectData);

            Assert.Throws<ArgumentException>(() => mock.GetObjectData(null, new Dummy()));
        }

        [Fact]
        public void TestGenericMismatchingGetObjectInheritance()
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

            Assert.Throws<ArgumentException>(() => mock.GetObject<Dummy>(null));
        }

        private class Mock : StrictObjectSerializer<DummyBase>
        {
            public Func<IObjectReader, DummyBase> GetObjectCall { get; }
            public Action<IObjectWriter, DummyBase> GetObjectDataCall { get; }

            public Mock(Func<IObjectReader, DummyBase> getObjectCall, Action<IObjectWriter, DummyBase> getObjectDataCall)
            {
                GetObjectCall = getObjectCall ?? throw new ArgumentNullException(nameof(getObjectCall));
                GetObjectDataCall = getObjectDataCall ?? throw new ArgumentNullException(nameof(getObjectDataCall));
            }

            public override DummyBase GetObject(IObjectReader source)
            {
                return GetObjectCall(source);
            }

            protected override void GetObjectDataCore(IObjectWriter target, DummyBase instance)
            {
                if (instance != null && instance.GetType() != typeof(DummyBase))
                    throw new SpecialException();

                GetObjectDataCall(target, instance);
            }
        }
    }
}
