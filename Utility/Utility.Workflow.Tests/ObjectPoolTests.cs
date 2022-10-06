using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace Utility.Tests
{
    public class ObjectPoolTests
    {
        [Fact]
        public void Test()
        {
            new ObjectPool<MockPoolObject>(() => new MockPoolObject(), 10, 10, ThreadingType.None).Dispose();
        }

        [Fact]
        public void TestNoThread()
        {
            new ObjectPool<MockPoolObject>(() => throw new InvalidOperationException(), 10, 10, ThreadingType.None).Dispose();
        }

        [Fact]
        public void TestBackgroundThread()
        {
            bool isBackground = false;
            using ManualResetEvent ev = new ManualResetEvent(false);

            var pool = new ObjectPool<MockPoolObject>(() =>
            {
                if (Thread.CurrentThread.IsBackground)
                    isBackground = true;

                ev.Set();

                return new MockPoolObject();
            }, 10, 10, ThreadingType.ThreadPool);

            ev.WaitOne();
            ev.Reset();

            Assert.False(isBackground);
        }

        [Fact]
        public void TestPreallocation()
        {
            int allocated = 0;

            ManualResetEvent waitHandle = new ManualResetEvent(false);

            using var pool = new ObjectPool<int>(() =>
            {
                int v = allocated++;

                if (v == 9)
                    waitHandle.Set();

                return v;
            }, 10, 100, ThreadingType.Thread);

            if (!waitHandle.WaitOne(5000))
                throw new InvalidOperationException("Test timeout or infinite loop.");

            Assert.Equal(10, allocated);

            Thread.Sleep(100);

            Assert.Equal(10, allocated);
        }

        [Fact]
        public void TestOverallocation()
        {
            using var pool = new ObjectPool<int>(() =>
            {
                throw new InvalidOperationException("Too many allocations.");
            }, 0, 100, ThreadingType.Thread);

            Thread.Sleep(100);
        }

        [Fact]
        public void TestExcess()
        {
            using var pool = new ObjectPool<object>(() => new object(), 0, 0, ThreadingType.None);

            List<IDisposable> handles = new List<IDisposable>();

            for (int i = 0; i < 100; i++)
            {
                object poolObject;
                handles.Add(pool.Allocate(out poolObject));
            }

            foreach (IDisposable d in handles)
                d.Dispose();

            Assert.Equal(0, pool.AllocatedInstances);
        }

        [Fact]
        public void TestPooling()
        {
            ManualResetEvent waitHandle = new ManualResetEvent(false);

            using var pool = new ObjectPool<object>(() => new object(), 1, 10, ThreadingType.None);

            HashSet<object> h = new HashSet<object>(ReferenceComparer<object>.Default);

            bool unique = true;

            for (int i = 0; i < 100; i++)
            {
                object poolObject;
                pool.Allocate(out poolObject).Dispose();
                unique &= h.Add(poolObject);
            }

            Assert.False(unique);
        }

        [Fact]
        public void TestRandomAccess()
        {
            using var pool = new ObjectPool<object>(() => new object(), 1, 10, ThreadingType.Thread);

            var objects = new List<IDisposable>();

            Random r = new Random(0);

            for (int i = 0; i < 1000; i++)
            {
                if (r.Next(0, 2) == 0)
                    objects.Add(pool.Allocate(out _));
                else
                    pool.Allocate(out _).Dispose();
            }

            foreach (IDisposable d in objects)
                d.Dispose();
        }

        [Fact]
        public void TestClear()
        {
            bool cleared = false;
            using var pool = new ObjectPool<object>(() => new object(), (instance) => cleared = true, 1, 10, ThreadingType.Thread);

            pool.Allocate(out _).Dispose();

            Assert.True(cleared);
        }

        private sealed class MockPoolObject
        { }
    }
}
