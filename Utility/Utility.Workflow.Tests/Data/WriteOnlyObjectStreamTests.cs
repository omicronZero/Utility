using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Data;
using Xunit;

namespace Utility.Tests.Data
{
    public class WriteOnlyObjectStreamTests
    {
        [Fact]
        public void TestReadArray()
        {
            WriteonlyObjectStream<long> stream = new MockStream(true, true, 100);

            long[] buffer = new long[10];

            Assert.Throws<NotSupportedException>(() => stream.Read(buffer, 0, buffer.Length));

            Assert.Throws<ArgumentNullException>("buffer", () => stream.Read(null, 0, 10));

            Assert.Throws<ArgumentOutOfRangeException>("index", () => stream.Read(buffer, -1, 10));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => stream.Read(buffer, 20, 10));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => stream.Read(buffer, 20, 0));

            Assert.Throws<ArgumentOutOfRangeException>("count", () => stream.Read(buffer, 0, -1));
            Assert.Throws<ArgumentException>(() => stream.Read(buffer, 8, 30));
        }

        [Fact]
        public void TestReadRange()
        {
            WriteonlyObjectStream<long> stream = new MockStream(true, true, 100);

            long[] buffer = new long[10];

            Assert.Throws<NotSupportedException>(() => stream.Read(new Span<long>(buffer, 0, 10)));
        }

        [Fact]
        public void TestWriteArray()
        {
            ObjectStream<long> stream = new MockStream(true, true, 100);

            long[] buffer = new long[10];

            Assert.Equal(0, stream.Position);
            stream.Write(buffer, 0, buffer.Length);
            Assert.Equal(buffer.Length, stream.Position);

            Assert.Throws<ArgumentNullException>("buffer", () => stream.Write(null, 0, 10));

            Assert.Throws<ArgumentOutOfRangeException>("index", () => stream.Write(buffer, -1, 10));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => stream.Write(buffer, 20, 10));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => stream.Write(buffer, 20, 0));

            Assert.Throws<ArgumentOutOfRangeException>("count", () => stream.Write(buffer, 0, -1));
            Assert.Throws<ArgumentException>(() => stream.Write(buffer, 8, 30));
        }

        [Fact]
        public void TestWriteRange()
        {
            ObjectStream<long> stream = new MockStream(true, true, 100);

            long[] buffer = new long[10];

            var span = new Span<long>(buffer, 0, 10);

            stream.Write(span);

            Assert.Equal(span.Length, stream.Position);

            Assert.Equal(span.Length, stream.Position);
        }

        private sealed class MockStream : WriteonlyObjectStream<long>
        {

            public override bool CanWrite { get; }

            public override bool CanSeek { get; }

            public override long Position { get; set; }

            public override long Length { get; }

            public MockStream(bool canWrite, bool canSeek, long length)
            {
                CanWrite = canWrite;
                CanSeek = canSeek;
                Length = length;
            }

            protected override void WriteCore(ReadOnlySpan<long> span)
            {
                if (!CanWrite)
                    throw new NotImplementedException();

                if (span.Length + Position > Length)
                    throw new ArgumentException("The written input exceeds the stream's boundaries.", nameof(span));

                Position += span.Length;

                int count = (int)Math.Min(span.Length, Length - Position);
            }
        }
    }
}
