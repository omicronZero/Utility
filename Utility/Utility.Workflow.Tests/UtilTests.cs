using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace Utility.Tests
{
    public class UtilTests
    {
        [Fact]
        public void TestUnsynchronized()
        {
            object syncObject = null;

            int v = 0;
            int iterations;

            v = CallSynchronizedTest(syncObject, v, out iterations);

            //probably true
            Assert.True(v < iterations);
        }

        [Fact]
        public void TestSynchronized()
        {
            object syncObject = new object();

            int v = 0;
            int iterations;

            v = CallSynchronizedTest(syncObject, v, out iterations);

            Assert.Equal(iterations, v);
        }

        [Fact]
        public void TestValidateIndex()
        {
            Util.ValidateIndex(0, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateIndex(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateIndex(1, 0));

            int[] array = new int[] { 0, 1 };
            Util.ValidateIndex(array, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateIndex(array, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateIndex(array, 2));
        }

        [Fact]
        public void TestValidateRange()
        {
            Util.ValidateRange(0, 2, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateRange(-1, 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateRange(3, 2, 2));

            int[] array = new int[] { 0, 1 };
            Util.ValidateRange(array, 0, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateRange(array, 1, -1));
            Assert.Throws<ArgumentException>(() => Util.ValidateRange(array, 1, 2));
        }

        [Fact]
        public void TestNamedValidateNamedIndex()
        {
            Util.ValidateNamedIndex(0, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateNamedIndex(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateNamedIndex(1, 0));

            int[] array = new int[] { 0, 1 };
            Util.ValidateNamedIndex(array, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateNamedIndex(array, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateNamedIndex(array, 3));
        }

        [Fact]
        public void TestNamedValidateNamedRange()
        {
            Util.ValidateNamedRange(0, 2, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateNamedRange(-1, 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateNamedRange(3, 2, 2));

            int[] array = new int[] { 0, 1 };
            Util.ValidateNamedRange(array, 0, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => Util.ValidateNamedRange(array, 1, -1));
            Assert.Throws<ArgumentException>(() => Util.ValidateNamedRange(array, 1, 2));
        }

        private static int CallSynchronizedTest(object syncObject, int v, out int totalIterations)
        {
            var threads = new List<Thread>();

            int pc = Environment.ProcessorCount;

            const int iterations = 10000;

            totalIterations = iterations * pc * 3;

            for (int i = 0; i < pc; i++)
            {
                var thread = new Thread(() =>
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        Util.CallSynchronized(syncObject, 1, (x) => { v++; });
                        Thread.Yield();
                    }
                });

                thread.Start();
                threads.Add(thread);

                thread = new Thread(() =>
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        Util.CallSynchronized(syncObject, () => { v++; return true; });
                        Thread.Yield();
                    }
                });

                thread.Start();
                threads.Add(thread);

                thread = new Thread(() =>
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        Util.CallSynchronized(syncObject, 1, (x) => { v++; return true; });
                        Thread.Yield();
                    }
                });

                thread.Start();
                threads.Add(thread);
            }

            foreach (Thread t in threads)
                t.Join();

            return v;
        }
    }
}
