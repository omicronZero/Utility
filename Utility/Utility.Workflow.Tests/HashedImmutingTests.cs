using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class HashedImmutingTests
    {
        [Fact]
        public void TestGetHashCode()
        {
            HashedImmuting<string> test = new HashedImmuting<string>("bla");

            Assert.Equal("bla".GetHashCode(), test.GetHashCode());
        }

        [Fact]
        public void TestInitializeOnce()
        {
            GetHashCodeCounter counter = new GetHashCodeCounter();

            var hashedImmuting = new HashedImmuting<GetHashCodeCounter>(counter);

            hashedImmuting.GetHashCode();
            hashedImmuting.GetHashCode();

            Assert.Equal(1, counter.Counter);
        }

        [Fact]
        public void TestEquality()
        {
            var a = new HashedImmuting<int>(1);
            var b = new HashedImmuting<int>(1);

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
        }

        [Fact]
        public void TestInequality()
        {
            var a = new HashedImmuting<int>(1);
            var b = new HashedImmuting<int>(2);

            Assert.NotEqual(a, b);
            Assert.True(a != b);
            Assert.False(a == b);
        }

        private sealed class GetHashCodeCounter
        {
            public int Counter { get; private set; }

            public override int GetHashCode()
            {
                Counter++;
                return 1;
            }
        }
    }
}
