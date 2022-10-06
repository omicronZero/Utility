using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace Utility.Tests
{
    public class DisposableListTests
    {
        [Fact]
        public void TestSingleItemAddAndDispose()
        {
            var list = new DisposableList<int>();
            var handler = list.Add(1);

            Assert.Single(list);

            handler.Dispose();

            Assert.Empty(list);
        }

        [Fact]
        public void TestRandomAccess()
        {
            Random rnd = new Random(0);

            var list = new DisposableList<int>();
            var handles = new List<(int Item, IDisposable Handle)>();

            for (int i = 0; i < 100; i++)
            {
                PerformRandomAction(rnd, list, handles, i);
            }
        }

        private static void PerformRandomAction(Random random, DisposableList<int> list, List<(int Item, IDisposable Handle)> handles, int value)
        {
            if (list.Count == 0 || random.Next(0, 3) > 0)
                handles.Add((value, list.Add(value)));
            else
            {
                int j = random.Next(0, handles.Count);

                var item = handles[j];

                handles.RemoveAt(j);

                item.Handle.Dispose();

                Assert.Equal(list, handles.Select((s) => s.Item));
            }
        }

        [Fact]
        public void TestClear()
        {
            var list = new DisposableList<int>();
            int disposedCount = 0;

            for (int i = 0; i < 10; i++)
                list.Add(i, (c) => disposedCount++);

            list.Clear();

            Assert.Empty(list);
            Assert.Equal(10, disposedCount);
        }

        [Fact]
        public void TestParallelAccess()
        {
            var list = new DisposableList<int>(true);
            var threads = new List<Thread>();
            int threadCount = Math.Max(Environment.ProcessorCount, 4);

            Random r = new Random(0);

            List<int> resultValues = new List<int>();

            for (int i = 0; i < threadCount; i++)
            {
                int threadIndex = i;

                var thread = new Thread(() =>
                {
                    Random innerRandom;

                    lock (r)
                        innerRandom = new Random(r.Next());

                    var handles = new List<(int Item, IDisposable Handle)>();

                    const int iterations = 10000;

                    for (int j = 0; j < iterations; j++)
                    {
                        PerformRandomAction(innerRandom, list, handles, threadIndex * iterations + j);
                    }

                    lock (resultValues)
                        resultValues.AddRange(handles.Select((s) => s.Item));
                });

                threads.Add(thread);
            }

            Assert.True(new HashSet<int>(resultValues).SetEquals(new HashSet<int>(list)));
            list.Clear();
        }

        [Fact]
        public void TestEnumerableStabilityNewInsertions()
        {
            var list = new DisposableList<int>();

            list.Add(0);

            //test whether new insertions are recognized
            foreach (var i in list)
            {
                list.Add(i + 1);

                Assert.Equal(i + 2, list.Count);

                if (list.Count == 10)
                    break;
            }

            Assert.Equal(10, list.Count);
        }

        [Fact]
        public void TestEnumerableStabilityRemoveActiveItem()
        {
            var list = new DisposableList<int>();

            IDisposable two = null;

            for (int i = 0; i < 5; i++)
            {
                IDisposable handle = list.Add(i);

                if (i == 2)
                    two = handle;
            }

            //test whether removing an active item results in a stable iterator
            using (var enumerator = list.GetEnumerator())
            {
                bool successorReached = false;

                while (enumerator.MoveNext())
                {
                    int i = enumerator.Current;

                    if (i == 2)
                        two.Dispose();

                    successorReached |= i == 3; //sets successorReached to true upon reaching the successor of 2: 3

                    //current must be stable
                    Assert.Equal(i, enumerator.Current);
                }

                Assert.True(successorReached);
            }
        }

        [Fact]
        public void TestCompatibilityRemove()
        {
            var list = new DisposableList<int>();

            for (int i = 0; i < 10; i++)
                list.Add(i);

            ((ICollection<int>)list).Remove(4);

            Assert.Equal(list, Enumerable.Range(0, 10).Except(new int[] { 4 }));
        }

        [Fact]
        public void TestContains()
        {
            var list = new DisposableList<int>();

            for (int i = 0; i < 10; i++)
                list.Add(i);

            Assert.Contains(5, list);
        }

        [Fact]
        public void TestGetItem()
        {
            var list = new DisposableList<int>();
            var disposables = new List<IDisposable>();

            for (int i = 0; i < 10; i++)
                disposables.Add(list.Add(i));

            Assert.Equal(3, list.GetItem(disposables[3]));
        }

        [Fact]
        public void TestGetItemForeign()
        {
            var list = new DisposableList<int>();
            var disposables = new List<IDisposable>();

            for (int i = 0; i < 10; i++)
                disposables.Add(list.Add(i));

            var foreign = new DisposableList<int>();

            Assert.Throws<ArgumentException>(() => list.GetItem(foreign.Add(0)));
        }

        [Fact]
        public void TestGetItemDisposed()
        {
            var list = new DisposableList<int>();
            var disposables = new List<IDisposable>();

            for (int i = 0; i < 10; i++)
                disposables.Add(list.Add(i));


            disposables[3].Dispose();

            Assert.Throws<ObjectDisposedException>(() => list.GetItem(disposables[3]));
        }

        [Fact]
        public void TestGetItemTypeMismatch()
        {
            var list = new DisposableList<int>();
            var disposables = new List<IDisposable>();

            for (int i = 0; i < 10; i++)
                disposables.Add(list.Add(i));

            Assert.Throws<ArgumentException>(() => list.GetItem(Disposable.Empty));
        }

        [Fact]
        public void TestGetHandleIterator()
        {
            var list = new DisposableList<int>();

            for (int i = 0; i < 10; i++)
                list.Add(i);

            Assert.Equal(Enumerable.Range(0, 10), list.GetHandleIterator().Select((i) => i.Key));
            Assert.Equal(Enumerable.Range(0, 10), list.GetHandleIterator().Select((i) => list.GetItem(i.Value)));
        }

        [Fact]
        public void TestGetHandleIteratorNewInsertions()
        {
            var list = new DisposableList<int>();

            list.Add(0);

            //test whether new insertions are recognized
            foreach (var h in list.GetHandleIterator())
            {
                int i = h.Key;
                list.Add(i + 1);

                Assert.Equal(i + 2, list.Count);

                if (list.Count == 10)
                    break;
            }

            Assert.Equal(10, list.Count);
        }

        [Fact]
        public void TestGetHandleIteratorStabilityRemoveActiveItem()
        {
            var list = new DisposableList<int>();

            IDisposable two = null;

            for (int i = 0; i < 5; i++)
            {
                IDisposable handle = list.Add(i);

                if (i == 2)
                    two = handle;
            }

            //test whether removing an active item results in a stable iterator
            using (var enumerator = list.GetHandleIterator().GetEnumerator())
            {
                bool successorReached = false;

                while (enumerator.MoveNext())
                {
                    int i = enumerator.Current.Key;

                    if (i == 2)
                        two.Dispose();

                    successorReached |= i == 3; //sets successorReached to true upon reaching the successor of 2: 3

                    //current must be stable
                    Assert.Equal(i, enumerator.Current.Key);
                }

                Assert.True(successorReached);
            }
        }
    }
}
