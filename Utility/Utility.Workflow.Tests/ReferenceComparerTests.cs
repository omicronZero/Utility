using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class ReferenceComparerTests
    {
        [Fact]
        public void TestReference()
        {
            var x = new object();
            var y = new object();

            var comp = ReferenceComparer<object>.Default;

            Assert.True(comp.Equals(x, x));
            Assert.False(comp.Equals(x, y));
            Assert.False(comp.Equals(y, x));

            Assert.Equal(comp.GetHashCode(x), comp.GetHashCode(x));

            bool equal = true;

            for (int i = 0; i < 100; i++)
            {
                equal &= comp.GetHashCode(x) == comp.GetHashCode(y);
            }

            Assert.False(equal); //is very unprobable to occur
        }

        [Fact]
        public void TestValue()
        {
            var x = 1;
            var y = 2;

            var comp = ReferenceComparer<int>.Default;

            Assert.True(comp.Equals(x, x));
            Assert.False(comp.Equals(x, y));
            Assert.False(comp.Equals(y, x));

            Assert.Equal(comp.GetHashCode(x), comp.GetHashCode(x));
            Assert.NotEqual(comp.GetHashCode(x), comp.GetHashCode(y));
        }
    }
}
