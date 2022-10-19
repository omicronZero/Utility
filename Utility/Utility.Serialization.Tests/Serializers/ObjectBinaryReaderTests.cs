using Utility.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DataSpecTests.Mocks;

namespace DataSpecTests.Serializers
{
    public class ObjectBinaryReaderTests
    {
        [Fact]
        public void TestSinglePrimitive()
        {
            var ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(5), 0, 4);
            ms.Position = 0;

            var br = new ObjectBinaryReader(ms);

            Assert.Equal(5, br.Read<int>());
        }

        [Fact]
        public void TestMultiplePrimitive()
        {
            var ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(5), 0, 4);
            ms.Write(BitConverter.GetBytes(6L), 0, 8);
            ms.Position = 0;

            var br = new ObjectBinaryReader(ms);

            Assert.Equal(5, br.Read<int>());
            Assert.Equal(6, br.Read<long>());
        }
    }
}
