using System;
using System.Text;
using System.Collections.Generic;
using Xunit;
using Utility.Collections;
using System.Linq;

namespace Utility.Tests.Collection
{
    public class ReadOnlyCollectionTests
    {
        [Fact]
        public void Test()
        {
            var comparison = Enumerable.Range(0, 100);
            List<int> underlyingList = new List<int>(comparison);

            ICollection<int> readOnlyCollection = new ReadOnlyCollection<int>(underlyingList);

            Assert.True(readOnlyCollection.SequenceEqual(comparison));

            Assert.Equal(100, readOnlyCollection.Count);
            Assert.Contains(80, readOnlyCollection);
            Assert.DoesNotContain(-1, readOnlyCollection);
            Assert.True(readOnlyCollection.IsReadOnly);

            Assert.True(underlyingList.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => readOnlyCollection.Add(1));

            Assert.True(underlyingList.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => readOnlyCollection.Remove(1));

            Assert.True(underlyingList.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => readOnlyCollection.Clear());

            Assert.True(underlyingList.SequenceEqual(comparison));

            int[] array = new int[104];

            readOnlyCollection.CopyTo(array, 2);

            Assert.True(array.SequenceEqual(Enumerable.Repeat(0, 2).Concat(comparison).Concat(Enumerable.Repeat(0, 2))));
        }

        [Fact]
        public void TestNullReference()
        {
            ICollection<int> readOnlyCollection = new ReadOnlyCollection<int>();

            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Count);
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Contains(80));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.IsReadOnly);

            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Add(1));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Remove(1));
            Assert.Throws<NullReferenceException>(() => readOnlyCollection.Clear());

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
            Assert.Throws<ArgumentNullException>(() => new ReadOnlyCollection<int>(null));
            ICollection<int> readOnlyCollection = new ReadOnlyCollection<int>(underlyingList);

            Assert.Throws<ArgumentException>(() => readOnlyCollection.CopyTo(new int[3], 2));
            Assert.Throws<ArgumentException>(() => readOnlyCollection.CopyTo(new int[100], 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => readOnlyCollection.CopyTo(new int[300], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => readOnlyCollection.CopyTo(new int[3], -1));
            Assert.Throws<ArgumentNullException>(() => readOnlyCollection.CopyTo(null, 0));
            Assert.Throws<ArgumentNullException>(() => readOnlyCollection.CopyTo(null, -1));
        }

        //TODO: test ICollection-behavior
    }
}
