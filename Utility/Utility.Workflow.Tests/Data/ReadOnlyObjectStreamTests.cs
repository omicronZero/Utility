using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Data;
using Xunit;

namespace Utility.Tests.Data
{
    public class ReadOnlyObjectStreamTests
    {
        [Fact]
        public void TestReadArray()
        {
            ReadonlyObjectStream<long> stream = new MockStream(true, true, 100);

            long[] buffer = new long[10];

            Assert.Equal(0, stream.Position);
            Assert.Equal(buffer.Length, stream.Read(buffer, 0, buffer.Length));
            Assert.Equal(buffer.Length, stream.Position);

            Assert.Equal(Enumerable.Range(0, buffer.Length).Select((i) => (long)i), buffer);

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
            ReadonlyObjectStream<long> stream = new MockStream(true, true, 100);

            long[] buffer = new long[10];

            var span = new Span<long>(buffer, 0, 10);

            Assert.Equal(buffer.Length, stream.Read(span));
            Assert.Equal(span.Length, stream.Position);

            Assert.Equal(Enumerable.Range(0, span.Length).Select((i) => (long)i), span.ToArray());
        }

        [Fact]
        public void TestWriteArray()
        {
            ReadonlyObjectStream<long> stream = new MockStream(true, true, 100);

            long[] buffer = new long[10];

            Assert.Throws<NotSupportedException>(() => stream.Write(buffer, 0, buffer.Length));

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
            ReadonlyObjectStream<long> stream = new MockStream(true, true, 100);

            long[] buffer = new long[10];

            Assert.Throws<NotSupportedException>(() => stream.Write(new Span<long>(buffer, 0, 10)));
        }

        private sealed class MockStream : ReadonlyObjectStream<long>
        {
            public override bool CanRead { get; }

            public override bool CanSeek { get; }

            public override long Position { get; set; }

            public override long Length { get; }

            public MockStream(bool canRead, bool canSeek, long length)
            {
                CanRead = canRead;
                CanSeek = canSeek;
                Length = length;
            }

            protected override int ReadCore(Span<long> span)
            {
                if (!CanRead)
                    throw new NotImplementedException(); //Thrown just for test

                int count = (int)Math.Min(span.Length, Length - Position);

                for (int i = 0; i < count; i++)
                    span[i] = Position + i;

                Position += count;

                return count;
            }
        }
    }
}
