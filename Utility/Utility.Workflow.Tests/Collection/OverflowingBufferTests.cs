using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Workflow.Collections.Special;
using Xunit;

namespace Utility.Tests.Collection
{
    public class OverflowingBufferTests
    {
        [Fact]
        public void TestPushAndIteration()
        {
            OverflowingBuffer<int> buffer = new OverflowingBuffer<int>(10);

            Assert.Equal(0, (int)buffer.Count);

            buffer.Push(0); //buffer: 0

            Assert.Equal(1, (int)buffer.Count);

            for (int i = 1; i <= 3; i++) //buffer: 0, 1, 2, 3
            {
                buffer.Push(i);
            }

            buffer.Push(Enumerable.Range(4, 6).ToArray(), 0, 6);//buffer: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9

            Assert.Equal(10, buffer.Count);

            Assert.True(buffer.SequenceEqual(Enumerable.Range(0, 10)));

            buffer.Push(10); //buffer: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10

            Assert.True(buffer.SequenceEqual(Enumerable.Range(1, 10)));

            Assert.Equal(1, buffer[0]);
            Assert.Equal(10, buffer[9]);
        }

        [Fact]
        public void TestPushOverfullBuffer()
        {
            OverflowingBuffer<int> buffer = new OverflowingBuffer<int>(10);

            buffer.Push(Enumerable.Range(0, 100).ToArray());

            Assert.True(buffer.SequenceEqual(Enumerable.Range(90, 10)));

            buffer = new OverflowingBuffer<int>(10);

            buffer.Push(0);
            buffer.Push(Enumerable.Range(1, 99).ToArray());

            Assert.True(buffer.SequenceEqual(Enumerable.Range(90, 10)));
        }

        [Fact]
        public void TestListBehavior()
        {
            OverflowingBuffer<int> buffer = new OverflowingBuffer<int>(10);

            for (int startValue = 0; startValue < 20; startValue += 10)
            {
                buffer.Push(Enumerable.Range(startValue, 10).ToArray());

                for (int i = startValue; i < startValue + 10; i++)
                {
                    Assert.Equal(i, buffer[i - startValue]);
                }

                Assert.True((bool)buffer.Contains(startValue + 9));

                Assert.False((bool)buffer.Contains(startValue + 10));

                Assert.Equal(8, buffer.IndexOf(startValue + 8));
            }
        }

        [Fact]
        public void TestDequeue()
        {
            OverflowingBuffer<int> buffer = new OverflowingBuffer<int>(10);

            buffer.Push(1);
            buffer.Dequeue(1);

            Assert.Empty(buffer);

            buffer.Push(Enumerable.Range(0, 10).ToArray());

            buffer.Dequeue(4);

            Assert.Equal(6, buffer.Count);

            Assert.True(Enumerable.Range(0, 10).Skip(4).SequenceEqual(buffer));

            buffer.Push(1);
            Assert.True(Enumerable.Range(0, 10).Skip(4).Concat(new int[] { 1 }).SequenceEqual(buffer));
        }

        [Fact]
        public void TestDequeueClearing()
        {
            OverflowingBuffer<int> buffer = new OverflowingBuffer<int>(10);

            buffer.Push(1);

            Assert.True(buffer.InternalBuffer[0] == 1);

            buffer.Dequeue(1, true);

            Assert.True(buffer.InternalBuffer[0] == 0);

            Assert.Empty(buffer);

            int delimiter = buffer.InternalWritePosition;

            buffer.Push(Enumerable.Range(0, 10).ToArray());

            buffer.Dequeue(4, true);

            Assert.Equal(6, buffer.Count);

            Assert.True(Enumerable.Range(0, 10).Skip(4).SequenceEqual(buffer));

            buffer.Push(1);
            Assert.True(Enumerable.Range(0, 10).Skip(4).Concat(new int[] { 1 }).SequenceEqual(buffer));
        }

        [Fact]
        public void TestDequeueNonClearing()
        {
            OverflowingBuffer<int> buffer = new OverflowingBuffer<int>(10);

            buffer.Push(1);
            buffer.Dequeue(1, false);

            Assert.Empty(buffer);

            buffer.Push(Enumerable.Range(0, 10).ToArray());

            buffer.Dequeue(4, false);

            Assert.Equal(6, buffer.Count);

            Assert.True(Enumerable.Range(0, 10).Skip(4).SequenceEqual(buffer));

            buffer.Push(1);
            Assert.True(Enumerable.Range(0, 10).Skip(4).Concat(new int[] { 1 }).SequenceEqual(buffer));
        }

        [Fact]
        public void TestDeuqueCompare()
        {
            OverflowingBuffer<int> bufferNonCleared = new OverflowingBuffer<int>(10);
            OverflowingBuffer<int> bufferCleared = new OverflowingBuffer<int>(10);
            OverflowingBuffer<int> buffer = new OverflowingBuffer<int>(10);

            bufferCleared.Push(1);
            bufferNonCleared.Push(1);
            buffer.Push(1);

            bufferNonCleared.Dequeue(1, false);
            bufferCleared.Dequeue(1, true);
            buffer.Dequeue(1);

            Assert.True(bufferNonCleared.SequenceEqual(bufferCleared));
            Assert.True(buffer.SequenceEqual(bufferCleared));

            bufferNonCleared.Push(Enumerable.Range(0, 10).ToArray());
            bufferCleared.Push(Enumerable.Range(0, 10).ToArray());
            buffer.Push(Enumerable.Range(0, 10).ToArray());

            Assert.True(bufferNonCleared.SequenceEqual(bufferCleared));
            Assert.True(buffer.SequenceEqual(bufferCleared));

            bufferNonCleared.Dequeue(4, false);
            bufferCleared.Dequeue(4, true);
            buffer.Dequeue(4);

            Assert.True(bufferNonCleared.SequenceEqual(bufferCleared));
            Assert.True(buffer.SequenceEqual(bufferCleared));

            Assert.Equal(6, bufferNonCleared.Count);
            Assert.Equal(6, bufferCleared.Count);
            Assert.Equal(6, buffer.Count);

            Assert.True(Enumerable.Range(0, 10).Skip(4).SequenceEqual(bufferNonCleared));
            Assert.True(Enumerable.Range(0, 10).Skip(4).SequenceEqual(bufferCleared));
            Assert.True(Enumerable.Range(0, 10).Skip(4).SequenceEqual(buffer));

            Assert.True(bufferNonCleared.SequenceEqual(bufferCleared));
            Assert.True(buffer.SequenceEqual(bufferCleared));

            bufferNonCleared.Push(1);
            bufferCleared.Push(1);
            buffer.Push(1);

            Assert.True(Enumerable.Range(0, 10).Skip(4).Concat(new int[] { 1 }).SequenceEqual(bufferNonCleared));
            Assert.True(Enumerable.Range(0, 10).Skip(4).Concat(new int[] { 1 }).SequenceEqual(bufferNonCleared));
            Assert.True(Enumerable.Range(0, 10).Skip(4).Concat(new int[] { 1 }).SequenceEqual(buffer));

            Assert.True(bufferNonCleared.SequenceEqual(bufferCleared));
            Assert.True(buffer.SequenceEqual(bufferCleared));
        }
    }
}
