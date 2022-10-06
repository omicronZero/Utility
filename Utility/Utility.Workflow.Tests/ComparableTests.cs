using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class ComparableTests
    {
        [Fact]
        public void TestEquals()
        {
            Comparable<int> first = 1;
            Comparable<int> second = 2;

            Assert.True(first.Equals(first));
            Assert.False(first.Equals(second));
            Assert.True(first.Equals((object)first));
            Assert.False(first.Equals((object)second));
        }

        [Fact]
        public void TestGetHashCode()
        {
            Comparable<int> first = 1;

            Assert.Equal(1.GetHashCode(), first.GetHashCode());
        }

        [Fact]
        public void TestComparisons()
        {
            Comparable<int> first = 1;
            Comparable<int> second = 2;
            Comparable<int> f = 1;

            Assert.True(first < second);
            Assert.True(second > first);
            Assert.True(first != second);
            Assert.True(first == f);
            Assert.True(second >= first);
            Assert.True(first <= second);
            Assert.True(f >= first);
            Assert.True(first <= f);

            Assert.False(second < first);
            Assert.False(first > second);
            Assert.False(first == second);
            Assert.False(first != f);
            Assert.False(first >= second);
            Assert.False(second <= first);
        }

        [Fact]
        public void TestMin()
        {
            Comparable<int> first = 1;
            Comparable<int> second = 2;

            Assert.Equal(first, Comparable<int>.Min(first, second));
        }

        [Fact]
        public void TestMax()
        {
            Comparable<int> first = 1;
            Comparable<int> second = 2;

            Assert.Equal(second, Comparable<int>.Max(first, second));
        }
    }
}
