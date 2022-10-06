using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class ArrayExtensionsTests
    {
        [Fact]
        public void TestShallowCopy()
        {
            int[] array = Enumerable.Range(0, 10).ToArray();
            int[] copy = ArrayExtensions.ShallowCopy(array);

            Assert.NotSame(array, copy);
            Assert.Equal(array, copy);
        }

        [Fact]
        public void TestReadOnlyShallowCopy()
        {
            int[] array = Enumerable.Range(0, 10).ToArray();

            var collection = ArrayExtensions.ReadOnlyShallowCopy(array);

            Assert.True(((ICollection<int>)collection).IsReadOnly);

            Assert.Equal(array, collection);
        }
    }
}
