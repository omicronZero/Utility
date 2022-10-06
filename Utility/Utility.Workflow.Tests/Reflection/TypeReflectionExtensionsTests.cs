using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Utility.Reflection;
using System.Collections.ObjectModel;

namespace Utility.Tests.Reflection
{
    public class TypeReflectionExtensionsTests
    {
        [Fact]
        public void TestGetOperators()
        {
            foreach (Operators op in new Operators[] { Operators.Addition, Operators.Negation })
            {
                Assert.Single(TypeReflectionExtensions.GetOperators(typeof(decimal), op));
            }

            Assert.Empty(TypeReflectionExtensions.GetOperators(typeof(decimal), Operators.Xor));
        }

        [Fact]
        public void TestGetBinaryOperator()
        {
            var op = TypeReflectionExtensions.GetBinaryOperator(BinaryOperator.Addition, typeof(Mock), typeof(double));

            Assert.Equal(0.0, op.Invoke(null, new object[] { new Mock(), 0.0 }));

            Assert.Null(TypeReflectionExtensions.GetBinaryOperator(BinaryOperator.Addition, typeof(double), typeof(Mock)));

            op = TypeReflectionExtensions.GetBinaryOperator(BinaryOperator.Addition, typeof(Mock), typeof(double), typeof(double));

            Assert.Equal(0.0, op.Invoke(null, new object[] { new Mock(), 0.0 }));

            Assert.Null(TypeReflectionExtensions.GetBinaryOperator(BinaryOperator.Addition, typeof(Mock), typeof(double), typeof(Mock)));
        }

        [Fact]
        public void TestGetUnaryOperator()
        {
            var op = TypeReflectionExtensions.GetUnaryOperator(UnaryOperator.Negation, typeof(Mock));
            var mock = new Mock();

            Assert.Equal(mock, op.Invoke(null, new object[] { mock }));

            Assert.Null(TypeReflectionExtensions.GetUnaryOperator(UnaryOperator.Not, typeof(Mock)));
        }

        [Fact]
        public void TestGetConvert()
        {
            var convert = TypeReflectionExtensions.GetConvert<Mock, double>(false);

            Assert.Equal(0.0, convert(new Mock()));

            Assert.Null(TypeReflectionExtensions.GetConvert<Mock, string>(false));
            Assert.Null(TypeReflectionExtensions.GetConvert<Mock, int>(false));

            Assert.Equal(0, TypeReflectionExtensions.GetConvert<Mock, int>(true)(new Mock()));
        }

        [Fact]
        public void TestGetOperatorConvertFrom()
        {
            //AA : A
            //A -> B implicitly, defined on A
            //B -> AA explicitly, defined on AA

            //A -> B is defined on A, thus we will get null
            Assert.Empty(typeof(B).GetOperatorConvertFrom(typeof(A), includeExplicit: false, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false));
            //It should succeed when includeTargetOperators is true
            var op = typeof(B).GetOperatorConvertFrom(typeof(A), includeExplicit: false, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true).Single();
            Assert.IsType<B>(op.Invoke(null, new object[] { new A() }));

            //B -> AA is defined on AA, thus we will get an operator iif we include explicit operators
            op = typeof(AA).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false).Single();
            Assert.IsType<AA>(op.Invoke(null, new object[] { new B() }));
            op = typeof(AA).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true).Single();
            Assert.IsType<AA>(op.Invoke(null, new object[] { new B() }));

            //the operator B -> AA is explicit
            Assert.Empty(typeof(AA).GetOperatorConvertFrom(typeof(B), includeExplicit: false, includeImplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false));
            Assert.Empty(typeof(AA).GetOperatorConvertFrom(typeof(B), includeExplicit: false, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false));
            Assert.Empty(typeof(AA).GetOperatorConvertFrom(typeof(B), includeExplicit: false, includeImplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true));
            Assert.Empty(typeof(AA).GetOperatorConvertFrom(typeof(B), includeExplicit: false, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true));

            //we do not allow non-exact matches
            Assert.Empty(typeof(A).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false));
            Assert.Empty(typeof(A).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false));
            Assert.Empty(typeof(A).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true));
            Assert.Empty(typeof(A).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true));

            //this should then work if we allow non-exact matches
            Assert.Empty(typeof(A).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: false, exactMatch: false, includeInheritedOperators: false, includeTargetOperators: false));
            Assert.Empty(typeof(A).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: true, exactMatch: false, includeInheritedOperators: false, includeTargetOperators: false));
            Assert.Empty(typeof(A).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: false, exactMatch: false, includeInheritedOperators: false, includeTargetOperators: true));
            Assert.Empty(typeof(A).GetOperatorConvertFrom(typeof(B), includeExplicit: true, includeImplicit: true, exactMatch: false, includeInheritedOperators: false, includeTargetOperators: true));

            //this should work as well due to exactMatch being false, if we include inherited operators
            op = typeof(B).GetOperatorConvertFrom(typeof(AA), includeExplicit: false, includeImplicit: true, exactMatch: false, includeInheritedOperators: true, includeTargetOperators: true).Single();
            Assert.IsType<B>(op.Invoke(null, new object[] { new A() }));
            //but fail if we do not include them
            Assert.Empty(typeof(B).GetOperatorConvertFrom(typeof(AA), includeExplicit: false, includeImplicit: true, exactMatch: false, includeInheritedOperators: false, includeTargetOperators: true));
            //includeExplicit should not change the result of both operations
            op = typeof(B).GetOperatorConvertFrom(typeof(AA), includeExplicit: true, includeImplicit: true, exactMatch: false, includeInheritedOperators: true, includeTargetOperators: true).Single();
            Assert.IsType<B>(op.Invoke(null, new object[] { new A() }));
            Assert.Empty(typeof(B).GetOperatorConvertFrom(typeof(AA), includeExplicit: false, includeImplicit: true, exactMatch: false, includeInheritedOperators: false, includeTargetOperators: true));

            //but this should be empty
            Assert.Empty(typeof(B).GetOperatorConvertFrom(typeof(AA), includeExplicit: true, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true));
            //as well as this (operator is implicit)
            Assert.Empty(typeof(B).GetOperatorConvertFrom(typeof(AA), includeExplicit: true, includeImplicit: false, exactMatch: false, includeInheritedOperators: false, includeTargetOperators: true));

            Assert.Equal(0.0, TypeReflectionExtensions.GetOperatorConvertFrom<Mock, double>(false)(new Mock()));
            Assert.Null(TypeReflectionExtensions.GetOperatorConvertFrom<Mock, int>(false));
            Assert.Equal(0, TypeReflectionExtensions.GetOperatorConvertFrom<Mock, int>(true)(new Mock()));
        }

        [Fact]
        public void TestGetOperatorConvertTo()
        {
            //only operators defined on Mock
            var op = typeof(Mock).GetOperatorConvertTo(typeof(double), includeExplicit: false, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false).Single();

            Assert.Equal(0.0, op.Invoke(null, new object[] { new Mock() }));

            Assert.Empty(typeof(Mock).GetOperatorConvertTo(typeof(int), includeExplicit: false, includeImplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false));
            Assert.Empty(typeof(Mock).GetOperatorConvertTo(typeof(int), includeExplicit: false, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false));

            op = typeof(Mock).GetOperatorConvertTo(typeof(int), includeExplicit: true, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false).Single();
            Assert.Equal(0, op.Invoke(null, new object[] { new Mock() }));
            op = typeof(Mock).GetOperatorConvertTo(typeof(int), includeExplicit: true, includeImplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false).Single();
            Assert.Equal(0, op.Invoke(null, new object[] { new Mock() }));

            //the same with includeTargetOperators: true
            op = typeof(Mock).GetOperatorConvertTo(typeof(double), includeExplicit: false, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true).Single();

            Assert.Equal(0.0, op.Invoke(null, new object[] { new Mock() }));

            Assert.Empty(typeof(Mock).GetOperatorConvertTo(typeof(int), includeExplicit: false, includeImplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true));
            Assert.Empty(typeof(Mock).GetOperatorConvertTo(typeof(int), includeExplicit: false, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true));

            op = typeof(Mock).GetOperatorConvertTo(typeof(int), includeExplicit: true, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true).Single();
            Assert.Equal(0, op.Invoke(null, new object[] { new Mock() }));
            op = typeof(Mock).GetOperatorConvertTo(typeof(int), includeExplicit: true, includeImplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true).Single();
            Assert.Equal(0, op.Invoke(null, new object[] { new Mock() }));

            //Operator B -> A is defined on A.
            op = typeof(A).GetOperatorConvertTo(typeof(B), includeExplicit: true, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: true).Single();
            Assert.IsType<B>(op.Invoke(null, new object[] { new A() }));
            op = typeof(A).GetOperatorConvertTo(typeof(B), includeExplicit: true, includeImplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false).Single();
            Assert.IsType<B>(op.Invoke(null, new object[] { new A() }));
        }

        [Fact]
        public void TestGetCommonBaseType()
        {
            Assert.Equal(typeof(ValueType), TypeReflectionExtensions.GetCommonBaseType(new Type[] { typeof(int), typeof(long) }));
            Assert.Equal(typeof(MemberInfo), TypeReflectionExtensions.GetCommonBaseType(new Type[] { typeof(Type), typeof(MethodInfo) }));

            Assert.Throws<ArgumentException>(() => TypeReflectionExtensions.GetCommonBaseType(new Type[] { typeof(IEquatable<int>), typeof(string) }));
        }

        [Fact]
        public void TestGetCommonInterfaces()
        {
            Assert.Contains(typeof(IConvertible), TypeReflectionExtensions.GetCommonInterfaces(new Type[] { typeof(int), typeof(long) }));
            Assert.False(!TypeReflectionExtensions.GetCommonInterfaces(new Type[] { typeof(int), typeof(long) }).Contains(typeof(IConvertible)));
        }

        [Fact]
        public void TestGetOperatorExplicits()
        {
            var methods = TypeReflectionExtensions.GetOperatorExplicits(typeof(Mock), includeInheritedOperators: false);

            Assert.True(methods.All((v) => v.Name == "op_Explicit"));

            foreach (var member in typeof(Mock).GetMember("op_Explicit", BindingFlags.Public | BindingFlags.Static))
                Assert.Contains(member, methods);
        }

        [Fact]
        public void TestGetOperatorImplicits()
        {
            var methods = TypeReflectionExtensions.GetOperatorImplicits(typeof(Mock), includeInheritedOperators: false);

            Assert.True(methods.All((v) => v.Name == "op_Implicit"));

            foreach (var member in typeof(Mock).GetMember("op_Implicit", BindingFlags.Public | BindingFlags.Static))
                Assert.Contains(member, methods);
        }

        [Fact]
        public void TestGetConvertOperator()
        {
            var methods = TypeReflectionExtensions.GetConvertOperator(typeof(Mock), includeExplicit: true, includeImplicit: true, includeInheritedOperators: false);

            Assert.True(methods.All((v) => v.Name == "op_Implicit" || v.Name == "op_Explicit"));

            foreach (var member in typeof(Mock).GetMember("op_Implicit", BindingFlags.Public | BindingFlags.Static))
                Assert.Contains(member, methods);
        }

        [Fact]
        public void TestGetGenericImplementations()
        {
            //interfaces
            Assert.Equal(typeof(IEnumerable<char>), TypeReflectionExtensions.GetGenericImplementations(typeof(string), typeof(IEnumerable<>)).Single());
            Assert.Equal(typeof(IGenericBase<string>), TypeReflectionExtensions.GetGenericImplementations(typeof(IGenericChild<int>), typeof(IGenericBase<>)).Single());
            Assert.Equal(typeof(IEquatable<int>), TypeReflectionExtensions.GetGenericImplementations(typeof(IGenericChild<int>), typeof(IEquatable<>)).Single());

            Assert.Empty(TypeReflectionExtensions.GetGenericImplementations(typeof(IGenericChild<int>), typeof(IComparable<>)));

            Assert.Equal(typeof(GenericTypeBase<int>), TypeReflectionExtensions.GetGenericImplementations(typeof(GenericType), typeof(GenericTypeBase<>)).Single());
            Assert.Empty(TypeReflectionExtensions.GetGenericImplementations(typeof(GenericType), typeof(Collection<>)));

            Assert.Throws<ArgumentException>(() => TypeReflectionExtensions.GetGenericImplementations(typeof(string), typeof(GenericType)).ToArray()); //we expect a generic type as the second parameter
            Assert.Throws<ArgumentNullException>("baseType", () => TypeReflectionExtensions.GetGenericImplementations(typeof(string), null).ToArray());
            Assert.Throws<ArgumentNullException>("type", () => TypeReflectionExtensions.GetGenericImplementations(null, typeof(GenericTypeBase<>)).ToArray());
        }

        private sealed class Mock
        {
            public static implicit operator double(Mock mock) => 0;
            public static explicit operator int(Mock mock) => 0;

            public static double operator +(Mock left, double right) => right;

            public static Mock operator -(Mock mock) => mock;
        }

        private class A
        {
            public static implicit operator B(A value) => new B();
        }

        private class B
        {
        }

        private class AA : A
        {
            public static explicit operator AA(B value) => new AA();
        }

        private interface IGenericBase<T>
        {
        }

        private interface IGenericChild<T> : IGenericBase<string>, IEquatable<T>
        { }

        private class GenericTypeBase<T>
        { }

        private class GenericType : GenericTypeBase<int>
        { }
    }
}
