using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class RangePrimitiveTests
    {
        [Fact]
        public void TestIntersect()
        {
            RangeInt32 range = new RangeInt32(0, 1);

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
            RangeInt32 range = new RangeInt32(0, 2);

            Assert.True(range.Contains(0));
            Assert.True(range.Contains(1));
            Assert.True(range.Contains(2));

            Assert.False(range.Contains(3));
            Assert.False(range.Contains(-1));
        }

        [Fact]
        public void TestContainsRange()
        {
            RangeInt32 range = new RangeInt32(0, 2);

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
            RangeInt32 range = new RangeInt32(2, 0);

            range.Normalize();

            Assert.True(range.Start < range.End);
        }

        [Fact]
        public void TestCreateNormalized()
        {
            RangeInt32 interval = RangeInt32.CreateNormalized(2, 0);

            Assert.True(interval.Start < interval.End);
        }

        [Fact]
        public void TestSerialization()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            var instance = new RangeInt32(1, 2);

            formatter.Serialize(ms, instance);

            ms.Position = 0;

            var result = (RangeInt32)formatter.Deserialize(ms);

            Assert.Equal(instance, result);
        }
    }
}
