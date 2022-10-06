using System;
using System.Text;
using System.Collections.Generic;
using Xunit;
using Utility.Collections;
using System.Linq;

namespace Utility.Tests.Collection
{
    public class ReadOnlyListTests
    {
        [Fact]
        public void Test()
        {
            var comparison = Enumerable.Range(0, 100);
            List<int> underlyingList = new List<int>(comparison);

            IList<int> readOnlyCollection = new ReadOnlyList<int>(underlyingList);

            Assert.True(readOnlyCollection.SequenceEqual(comparison));

            Assert.Equal(100, readOnlyCollection.Count);
            Assert.Contains(80, readOnlyCollection);
            Assert.DoesNotContain(-1, readOnlyCollection);
            Assert.Equal(5, readOnlyCollection.IndexOf(5));
            Assert.Equal(-1, readOnlyCollection.IndexOf(-10));
            Assert.Equal(5, readOnlyCollection[5]);
            Assert.True(readOnlyCollection.IsReadOnly);

            Assert.True(underlyingList.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => readOnlyCollection.Add(1));

            Assert.True(underlyingList.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => readOnlyCollection.Remove(1));

            Assert.True(underlyingList.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => readOnlyCollection.Clear());

            Assert.True(underlyingList.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => readOnlyCollection.Insert(0, 1));

            Assert.True(underlyingList.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => readOnlyCollection.RemoveAt(0));

            Assert.True(underlyingList.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => readOnlyCollection[5] = -3);

            Assert.True(underlyingList.SequenceEqual(comparison));

            int[] array = new int[104];

            readOnlyCollection.CopyTo(array, 2);

            Assert.True(array.SequenceEqual(Enumerable.Repeat(0, 2).Concat(comparison).Concat(Enumerable.Repeat(0, 2))));
        }

        [Fact]
        public void TestNullReference()
        {
            IList<int> readOnlyCollection = new ReadOnlyList<int>();

            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Count);
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Contains(80));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.IsReadOnly);

            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Add(1));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Remove(1));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Clear());

            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Insert(0, 1));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.RemoveAt(0));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection[0] = 1);
            Assert.Throws<NullReferenceException>(() => readOnlyCollection[0]);
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.IndexOf(0));

            Assert.Throws<NullReferenceException>(() => readOnlyCollection.CopyTo(new int[3], 2));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.CopyTo(new int[100], 1));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.CopyTo(new int[300], -1));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.CopyTo(new int[3], -1));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.CopyTo(null, 0));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.CopyTo(null, -1));
        }

        [Fact]
        public void TestInvalidArguments()
        {
            var comparison = Enumerable.Range(0, 100);
            List<int> underlyingList = new List<int>(comparison);

            Assert.Throws<ArgumentNullException>(() => new ReadOnlyList<int>(null));

            IList<int> readOnlyCollection = new ReadOnlyList<int>(underlyingList);

            Assert.Throws<ArgumentException>(() => readOnlyCollection.CopyTo(new int[3], 2));
            Assert.Throws<ArgumentException>(() => readOnlyCollection.CopyTo(new int[100], 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => readOnlyCollection.CopyTo(new int[300], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => readOnlyCollection.CopyTo(new int[3], -1));
            Assert.Throws<ArgumentNullException>(() => readOnlyCollection.CopyTo(null, 0));
            Assert.Throws<ArgumentNullException>(() => readOnlyCollection.CopyTo(null, -1));

            Assert.Throws<ArgumentOutOfRangeException>(() => readOnlyCollection.Insert(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => readOnlyCollection.Insert(readOnlyCollection.Count + 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => readOnlyCollection.RemoveAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => readOnlyCollection.RemoveAt(readOnlyCollection.Count));
        }

        //TODO: test IList-behavior
    }
}
