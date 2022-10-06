using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class ListAccessorTests
    {
        [Fact]
        public void TestBehavior()
        {
            ListAccessor<int> accessor = new ListAccessor<int>((i) => i, () => 50, true);

            Assert.Contains(12, accessor);
            Assert.True(accessor.SequenceEqual(Enumerable.Range(0, 50)));

            Assert.Throws<ArgumentOutOfRangeException>(() => accessor[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => accessor[accessor.Count + 1]);
        }
    }
}
