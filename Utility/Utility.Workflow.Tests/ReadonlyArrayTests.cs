using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class ReadonlyArrayTests
    {
        [Fact]
        public void TestIndex()
        {
            ReadonlyArray<int> array = new ReadonlyArray<int>(10, (i) => i);

            Assert.Equal(3, array[3]);

            for (int i = 0; i < array.Length; i++)
                Assert.Equal(i == 3, array.IsInitialized(i));
        }

        [Fact]
        public void TestEnumeration()
        {
            ReadonlyArray<int> array = new ReadonlyArray<int>(10, (i) => i);

            Assert.Equal(Enumerable.Range(0, 10), array);
        }

        [Fact]
        public void TestSerialize()
        {
            ReadonlyArray<int> array = new ReadonlyArray<int>(10, (i) => i);

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, array);

            ms.Position = 0;

            var deserialized = (ReadonlyArray<int>)formatter.Deserialize(ms);

            Assert.Equal(Enumerable.Range(0, 10), deserialized);
        }

        [Fact]
        public void TestContains()
        {
            ReadonlyArray<int> array = new ReadonlyArray<int>(10, (i) => i);

            Assert.Contains(1, array);
        }

        [Fact]
        public void TestIndexOf()
        {
            ReadonlyArray<int> array = new ReadonlyArray<int>(10, (i) => i);

            Assert.True(5 == array.IndexOf(5));
        }

        [Fact]
        public void TestIsInitialized()
        {
            ReadonlyArray<int> array = new ReadonlyArray<int>(10, (i) => i);

            Assert.False(array.IsInitialized(5));

            _ = array[5];

            Assert.True(array.IsInitialized(5));
        }

        [Fact]
        public void TestToArray()
        {
            ReadonlyArray<int> array = new ReadonlyArray<int>(10, (i) => i);

            Assert.Equal(Enumerable.Range(0, 10), array.ToArray());
        }
    }
}
