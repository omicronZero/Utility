using Utility.Serialization;
using DataSpecTests.Mocks;
using System;
using System.Collections.Generic;
using Xunit;

namespace DataSpecTests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void TestUnmanagedPrimitive()
        {
            Assert.True(typeof(int).IsUnmanaged());
        }

        [Fact]
        public void TestUnmanagedStruct()
        {
            Assert.True(typeof(TestObject).IsUnmanaged());
        }

        [Fact]
        public void TestUnmanagedStructNotUnmanaged()
        {
            Assert.False(typeof(TestObjectNotUnmanaged).IsUnmanaged());
        }

        [Fact]
        public void TestUnmanagedRefStruct()
        {
            Assert.False(typeof(TestObject).MakeByRefType().IsUnmanaged());
        }

        [Fact]
        public void TestUnmanagedRefStructNotUnmanaged()
        {
            Assert.False(typeof(TestObjectNotUnmanaged).MakeByRefType().IsUnmanaged());
        }

        [Fact]
        public void TestUnmanagedPtrStruct()
        {
            Assert.True(typeof(TestObject).MakePointerType().IsUnmanaged());
        }

        [Fact]
        public void TestUnmanagedPtrStructNotUnmanaged()
        {
            Assert.True(typeof(TestObjectNotUnmanaged).MakePointerType().IsUnmanaged());
        }

        [Fact]
        public void TestUnmanagedGenericPrimitive()
        {
            Assert.True(TypeExtensions.IsUnmanaged<int>());
        }

        [Fact]
        public void TestUnmanagedGenericStruct()
        {
            Assert.True(TypeExtensions.IsUnmanaged<TestObject>());
        }

        [Fact]
        public void TestUnmanagedGenericStructNotUnmanaged()
        {
            Assert.False(TypeExtensions.IsUnmanaged<TestObjectNotUnmanaged>());
        }

        [Fact]
        public void TestGetImplementationOfGenericInterfaceOnInterface()
        {
            Assert.Equal(new Type[] { typeof(IEnumerable<string>) }, TypeExtensions.GetImplementationOf(typeof(IList<string>), typeof(IEnumerable<>)));
        }

        [Fact]
        public void TestGetImplementationOfGenericInterfaceOnClass()
        {
            Assert.Equal(new Type[] { typeof(IEnumerable<string>) }, TypeExtensions.GetImplementationOf(typeof(List<string>), typeof(IEnumerable<>)));
        }

        [Fact]
        public void TestGetImplementationOfInterface()
        {
            Assert.Equal(new Type[] { typeof(System.Collections.IEnumerable) }, TypeExtensions.GetImplementationOf(typeof(List<string>), typeof(System.Collections.IEnumerable)));
        }

        [Fact]
        public void TestGetImplementationOfClass()
        {
            Assert.Equal(new Type[] { typeof(Dummy) }, TypeExtensions.GetImplementationOf(typeof(Dummy), typeof(Dummy)));
            Assert.Equal(new Type[] { typeof(DummyBase) }, TypeExtensions.GetImplementationOf(typeof(Dummy), typeof(DummyBase)));
        }

        [Fact]
        public void TestGetImplementationOfGenericGenericClass()
        {
            Assert.Equal(new Type[] { typeof(GenericClass1<string>) }, TypeExtensions.GetImplementationOf(typeof(GenericClass1<string>), typeof(GenericClass1<>)));
        }

        [Fact]
        public void TestGetImplementationOfGenericGenericBaseClass()
        {
            Assert.Equal(new Type[] { typeof(GenericClass1<string>) }, TypeExtensions.GetImplementationOf(typeof(GenericClassSubType1<string>), typeof(GenericClass1<>)));
        }

        [Fact]
        public void TestGetImplementationOfGenericGenericBaseBaseClass()
        {
            //Test whether non-direct inheritance is correctly recognized
            Assert.Equal(new Type[] { typeof(GenericClass1<string>) }, TypeExtensions.GetImplementationOf(typeof(GenericClassSubSubType1<string>), typeof(GenericClass1<>)));
        }

        [Fact]
        public void TestGetImplementationOfGenericGenericBaseClass2()
        {
            //base class with two different parameters where one is inherited
            Assert.Equal(new Type[] { typeof(GenericClass1<string>) }, TypeExtensions.GetImplementationOf(typeof(GenericClass2<string, int>), typeof(GenericClass1<>)));
        }

        [Fact]
        public void TestGetImplementationOfGenericGenericBaseClass2B()
        {
            //base class with two different parameters where no inheritance occurs
            Assert.Equal(new Type[] { typeof(GenericClass1<string>) }, TypeExtensions.GetImplementationOf(typeof(GenericClass2B<string, int>), typeof(GenericClass1<>)));
        }

        [Fact]
        public void TestGetImplementationOfGenericGenericClassUnassigned()
        {
            Assert.Equal(new Type[] { typeof(GenericClass1<>) }, TypeExtensions.GetImplementationOf(typeof(GenericClass1<>), typeof(GenericClass1<>)));
        }

        [Fact]
        public void TestGetImplementationOfGenericGenericBaseClassUnassigned()
        {
            Assert.Equal(new Type[] { typeof(GenericClass1<>) }, TypeExtensions.GetImplementationOf(typeof(GenericClassSubType1<>), typeof(GenericClass1<>)));
        }

        [Fact]
        public void TestGetImplementationOfGenericGenericBaseClass2Unassigned()
        {
            Assert.Equal(new Type[] { typeof(GenericClass1<>) }, TypeExtensions.GetImplementationOf(typeof(GenericClass2<,>), typeof(GenericClass1<>)));
        }

        [Fact]
        public void TestEnum()
        {
            Assert.Equal(new Type[] { typeof(DummyEnum) }, TypeExtensions.GetImplementationOf(typeof(DummyEnum), typeof(DummyEnum)));
        }

        [Fact]
        public void TestEnumBase()
        {
            Assert.Equal(new Type[] { typeof(int) }, TypeExtensions.GetImplementationOf(typeof(DummyEnum), typeof(int)));
        }

        [Fact]
        public void TestEnumValueType()
        {
            Assert.Equal(new Type[] { typeof(ValueType) }, TypeExtensions.GetImplementationOf(typeof(DummyEnum), typeof(ValueType)));
        }

        [Fact]
        public void TestDelegate()
        {
            Assert.Equal(new Type[] { typeof(Action<string>) }, TypeExtensions.GetImplementationOf(typeof(Action<string>), typeof(Action<>)));
        }

        [Fact]
        public void TestDelegateBase()
        {
            Assert.Equal(new Type[] { typeof(Delegate) }, TypeExtensions.GetImplementationOf(typeof(Action<string>), typeof(Delegate)));
        }

        [Fact]
        public void TestDelegateGeneric()
        {
            Assert.Equal(new Type[] { typeof(Action<>) }, TypeExtensions.GetImplementationOf(typeof(Action<>), typeof(Action<>)));
        }

        [Fact]
        public void TestDelegateGenericBase()
        {
            Assert.Equal(new Type[] { typeof(Delegate) }, TypeExtensions.GetImplementationOf(typeof(Action<>), typeof(Delegate)));
        }

        [Fact]
        public void TestDisjointClass()
        {
            Assert.Empty(TypeExtensions.GetImplementationOf(typeof(string), typeof(ValueType)));
        }

        [Fact]
        public void TestDisjointEnumerable()
        {
            Assert.Empty(TypeExtensions.GetImplementationOf(typeof(int), typeof(IEnumerable<>)));
        }

        [Fact]
        public void TestInterfaceMultiImplementationGeneric()
        {
            HashSet<Type> expected = new HashSet<Type>() { typeof(IBase<int>), typeof(IBase<string>) };
            HashSet<Type> actual = new HashSet<Type>(TypeExtensions.GetImplementationOf(typeof(IMulti), typeof(IBase<>)));

            Assert.True(expected.SetEquals(actual));
        }

        [Fact]
        public void TestClassMultiImplementationGeneric()
        {
            HashSet<Type> expected = new HashSet<Type>() { typeof(IBase<int>), typeof(IBase<string>) };
            HashSet<Type> actual = new HashSet<Type>(TypeExtensions.GetImplementationOf(typeof(MultiClass), typeof(IBase<>)));

            Assert.True(expected.SetEquals(actual));
        }

        [Fact]
        public void TestUnmanagedSizeInt32()
        {
            Assert.Equal(4, typeof(int).UnmanagedSize());
        }

        [Fact]
        public void TestUnmanagedSizeBoolean()
        {
            Assert.Equal(1, typeof(bool).UnmanagedSize());
        }

        [Fact]
        public void TestUnmanagedSizeClass()
        {
            Assert.Null(typeof(string).UnmanagedSize());
        }

        [Fact]
        public void TestUnmanagedSizeNotUnmanaged()
        {
            Assert.Null(typeof(TestObjectNotUnmanaged).UnmanagedSize());
        }

        [Fact]
        public void TestOrderIndependenceSizeThenUnmanaged()
        {
            Assert.Equal(4, typeof(int).UnmanagedSize());
            Assert.True(typeof(int).IsUnmanaged());
        }

        [Fact]
        public void TestOrderIndependenceUnmanagedThenSize()
        {
            Assert.True(typeof(int).IsUnmanaged());
            Assert.Equal(4, typeof(int).UnmanagedSize());
        }

        private interface IBase<T>
        {
        }

        private interface IMulti : IBase<int>, IBase<string>
        { }


        private class MultiClass : IBase<int>, IBase<string>
        { }

        private struct TestObject
        {
            public int Value { get; }
        }

        private struct TestObjectNotUnmanaged
        {
            public string Value { get; }
        }

        private class GenericClass1<T>
        {
        }

        private class GenericClassSubType1<T> : GenericClass1<T>
        {
        }

        private class GenericClass2<T, S> : GenericClass1<T>
        {
        }

        private class GenericClass2B<T, S> : GenericClass1<string>
        {
        }

        private class GenericClassSubSubType1<T> : GenericClassSubType1<T>
        {
        }

        public enum DummyEnum
        {
            None = 0
        }
    }
}
