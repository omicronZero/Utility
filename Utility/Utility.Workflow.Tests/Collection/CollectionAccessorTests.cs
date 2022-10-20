using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class CollectionAccessorTests
    {
        [Fact]
        public void TestWithoutContainsDelegate()
        {
            DelegateCollection<int> accessor = new DelegateCollection<int>(Enumerable.Range(0, 10), () => 10);

            Assert.Equal(10, accessor.Count);
            Assert.Contains(5, accessor);
            Assert.True(accessor.SequenceEqual(Enumerable.Range(0, 10)));

            Assert.True(accessor.IsReadOnly);
        }

        [Fact]
        public void TestWithContainsDelegate()
        {
            DelegateCollection<int> accessor = new DelegateCollection<int>(Enumerable.Range(0, 10), () => 10, (i) => i >= 0 && i < 10);

            Assert.Equal(10, accessor.Count);
            Assert.Contains(5, accessor);
            Assert.True(accessor.SequenceEqual(Enumerable.Range(0, 10)));

            Assert.True(accessor.IsReadOnly);
        }

        [Fact]
        public void TestExceptions()
        {
            DelegateCollection<int> accessor = new DelegateCollection<int>(Enumerable.Range(0, 10), () => 10, (i) => i >= 0 && i < 10);

            Assert.Throws<InvalidOperationException>(() => accessor.Add(11));
            Assert.Throws<InvalidOperationException>(() => accessor.Remove(1));
            Assert.Throws<InvalidOperationException>(() => accessor.Clear());
        }

        [Fact]
        public void TestCopyTo()
        {
            DelegateCollection<int> accessor = new DelegateCollection<int>(Enumerable.Range(0, 10), () => 10, (i) => i >= 0 && i < 10);

            int[] items = new int[10];

            accessor.CopyTo(items, 0);

            Assert.Equal(Enumerable.Range(0, 10), items);

            Assert.Throws<ArgumentOutOfRangeException>(() => accessor.CopyTo(items, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => accessor.CopyTo(items, 10));
            Assert.Throws<ArgumentException>(() => accessor.CopyTo(items, 1));
            Assert.Throws<ArgumentNullException>(() => accessor.CopyTo(null, 0));
            Assert.Throws<ArgumentException>(() => accessor.CopyTo(new int[1], 0));
        }
    }
}
