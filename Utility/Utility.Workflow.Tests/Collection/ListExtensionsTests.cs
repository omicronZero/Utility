using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class ListExtensionsTests
    {
        [Fact]
        public void TestIsFixedSize()
        {
            Assert.True(new int[1].IsFixedSize());
            Assert.Throws<ArgumentNullException>(() => ((int[])null).IsFixedSize());
        }

        [Fact]
        public void TestIsSetterReadonly()
        {
            Assert.False(new int[1].IsSetterReadonly());
            Assert.False(new List<int>().IsSetterReadonly());
            Assert.True(Array.AsReadOnly(new int[1]).IsSetterReadonly());
        }

        [Fact]
        public void TestCastIListOfT()
        {
            var objects = new List<object>();

            IList<string> strings = objects.CastList<object, string>();

            strings.Add("123");
            strings.Add("abc");

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>())); //Tests IEnumerable<T>.GetEnumerator

            strings.Insert(1, "Test");

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.Remove("Test");

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.RemoveAt(0);

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.Clear();

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.Insert(0, "Test");
            Assert.Equal(0, strings.IndexOf("Test"));

            Assert.True(strings.Contains("Test"));

            string[] targetStrings = new string[strings.Count];
            object[] targetObjects = new object[objects.Count];

            strings.CopyTo(targetStrings, 0);
            objects.CopyTo(targetObjects, 0);

            Assert.True(targetStrings.SequenceEqual(targetObjects));
        }

        [Fact]
        public void TestCastIListPropagated()
        {
            var objects = new List<object>();

            IList<string> strings = ListExtensions.CastList<string>((IList)objects);

            //this call to CallList recognizes that the TResult : TSource relation is given on IList<TSource> despite receiving an IList object
            //The test thus is the same as the one above
            Assert.True(strings.GetType() == objects.CastList<object, string>().GetType());

            strings.Add("123");
            strings.Add("abc");

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>())); //Tests IEnumerable<T>.GetEnumerator

            strings.Insert(1, "Test");

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.Remove("Test");

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.RemoveAt(0);

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.Clear();

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.Insert(0, "Test");
            Assert.Equal(0, strings.IndexOf("Test"));

            Assert.True(strings.Contains("Test"));

            string[] targetStrings = new string[strings.Count];
            object[] targetObjects = new object[objects.Count];

            strings.CopyTo(targetStrings, 0);
            objects.CopyTo(targetObjects, 0);

            Assert.True(targetStrings.SequenceEqual(targetObjects));
        }

        [Fact]
        public void TestCastIList()
        {
            var objects = new List<string>();

            IList<object> strings = ListExtensions.CastList<object>((IList)objects);

            //this call to CallList recognizes that the TResult : TSource relation is not given on IList<TSource> and thus
            //and thus behaves differently returning an object that is processed on IList
            Assert.True(strings.GetType() != new List<object>().CastList<object, string>().GetType());

            strings.Add("123");
            strings.Add("abc");

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>())); //Tests IEnumerable<T>.GetEnumerator

            strings.Insert(1, "Test");

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.Remove("Test");

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.RemoveAt(0);

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.Clear();

            Assert.Equal(objects.Count, strings.Count);
            Assert.True(objects.SequenceEqual(strings.Cast<string>()));

            strings.Insert(0, "Test");
            Assert.Equal(0, strings.IndexOf("Test"));

            Assert.True(strings.Contains("Test"));

            object[] targetStrings = new object[strings.Count];
            string[] targetObjects = new string[objects.Count];

            strings.CopyTo(targetStrings, 0);
            objects.CopyTo(targetObjects, 0);

            Assert.True(targetStrings.SequenceEqual(targetObjects));
        }

        [Fact]
        public void TestFixedSize()
        {
            var list = new FixedSizeMock(new int[10]);

            Assert.True(list.CastList<int>().IsReadOnly);
            list[0] = 1;
        }
    }
}
