using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class RangeTests
    {
        [Fact]
        public void TestIntersect()
        {
            Range<int> range = new Range<int>(0, 1);

            Assert.True(range.Intersects(range));
            Assert.True(range.Intersects(0, 2));
            Assert.True(range.Intersects(-1, 1));
            Assert.True(range.Intersects(-1, 2));

            Assert.False(range.Intersects(2, 3));
            Assert.False(range.Intersects(-2, -1));
        }

        [Fact]
        public void TestContainsValue()
        {
            Range<int> range = new Range<int>(0, 2);

            Assert.True(range.Contains(0));
            Assert.True(range.Contains(1));
            Assert.True(range.Contains(2));

            Assert.False(range.Contains(3));
            Assert.False(range.Contains(-1));
        }

        [Fact]
        public void TestContainsRange()
        {
            Range<int> range = new Range<int>(0, 2);

            Assert.True(range.Contains(range));
            Assert.True(range.Contains(0, 1));

            Assert.False(range.Contains(-1, 1));
            Assert.False(range.Contains(-1, 2));
            Assert.False(range.Contains(2, 3));
            Assert.False(range.Contains(-2, -1));
        }

        [Fact]
        public void TestNormalize()
        {
            Range<int> range = new Range<int>(2, 0);

            range.Normalize();

            Assert.True(range.StartComparable < range.EndComparable);
        }

        [Fact]
        public void TestCreateNormalized()
        {
            Range<int> interval = Range<int>.CreateNormalized(2, 0);

            Assert.True(interval.StartComparable < interval.EndComparable);
        }

        [Fact]
        public void TestSerialization()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            var instance = new Range<int>(1, 2);

            formatter.Serialize(ms, instance);

            ms.Position = 0;

            var result = (Range<int>)formatter.Deserialize(ms);

            Assert.Equal(instance, result);
        }
    }
}
