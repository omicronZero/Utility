using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace Utility.Tests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void TestAsReadOnlyAutoEnumerable()
        {
            var enumeration = Enumerable.Range(0, 10).Select((i) => i * i);

            var readonlyEnumerable = enumeration.AsReadonlyAuto();

            if (enumeration is ICollection<int>)
                throw new InvalidOperationException("Bad test: Enumeration is a collection.");

            Assert.False(readonlyEnumerable is ICollection<int>);
        }

        [Fact]
        public void TestAsReadOnlyAutoCollection()
        {
            var enumeration = new HashSet<int>(Enumerable.Range(0, 10).Select((i) => i * i));

            var readonlyEnumerable = enumeration.AsReadonlyAuto();

            if (enumeration is IList<int>)
                throw new InvalidOperationException("Bad test: Enumeration is a list.");

            Assert.True(readonlyEnumerable is ICollection<int>);
            Assert.False(readonlyEnumerable is IList<int>);

            if (readonlyEnumerable is ICollection<int> collection)
                Assert.True(collection.IsReadOnly);
        }

        [Fact]
        public void TestAsReadOnlyAutoList()
        {
            var enumeration = Enumerable.Range(0, 10).Select((i) => i * i).ToList();

            var readonlyEnumerable = enumeration.AsReadonlyAuto();

            Assert.True(readonlyEnumerable is IList<int>);

            if (readonlyEnumerable is IList<int> collection)
                Assert.True(collection.IsReadOnly);
        }

        [Fact]
        public void TestAsReadOnlyDictionary()
        {
            var dictionary = new Dictionary<int, int>();

            dictionary.Add(0, 1);
            dictionary.Add(1, 2);
            dictionary.Add(2, 0);

            var readOnly = dictionary.AsReadonlyDictionary();

            Assert.True(readOnly.IsReadOnly);
        }

        [Fact]
        public void TestAsReadOnlyList()
        {
            int[] x = Enumerable.Range(0, 10).ToArray();

            TestList(x.AsReadonlyList(), x);
        }

        [Fact]
        public void TestAsReadOnlyCollection()
        {
            int[] x = Enumerable.Range(0, 10).ToArray();

            TestCollection(x.AsReadonlyCollection(), x);
        }

        [Fact]
        public void TestConcatCollections()
        {
            List<int[]> arrays = new List<int[]>();

            for (int i = 0; i < 10; i++)
                arrays.Add(Enumerable.Range(i * 10, 10).ToArray());

            var collection = EnumerableExtensions.ConcatCollections(arrays);

            TestCollection(collection, Enumerable.Range(0, 100).ToArray());
        }

        [Fact]
        public void TestConcatLists()
        {
            List<int[]> arrays = new List<int[]>();

            for (int i = 0; i < 10; i++)
                arrays.Add(Enumerable.Range(i * 10, 10).ToArray());

            var collection = EnumerableExtensions.ConcatLists(arrays);

            TestList(collection, Enumerable.Range(0, 100).ToArray());
        }

        [Fact]
        public void TestLazy()
        {
            int count = 0;

            var enumerable = EnumerableExtensions.Lazy(() =>
             {
                 count++;
                 return Enumerable.Range(0, 10);
             });

            Assert.Equal(0, count);

            enumerable.GetEnumerator();

            Assert.Equal(1, count);

            enumerable.GetEnumerator();

            Assert.Equal(1, count);
        }

        [Fact]
        public void TestLazyParallel()
        {
            List<Thread> threads = new List<Thread>();

            bool singleInstantiation = true;

            int count = 0;

            for (int i = 0; i < 100; i++)
            {
                var enumerable = EnumerableExtensions.Lazy(() =>
                {
                    Interlocked.Increment(ref count);
                    return Enumerable.Range(0, 10);
                }, true);

                for (int j = 0; j < 16; j++)
                {
                    var thread = new Thread(() =>
                    {
                        enumerable.GetEnumerator();

                        if (count > 1)
                            singleInstantiation = false; //assert in thread is not recognized
                    });

                    thread.Start();
                    threads.Add(thread);
                }

                foreach (Thread t in threads)
                    t.Join();
                threads.Clear();
                count = 0;
            }

            Assert.True(singleInstantiation);
        }

        [Fact]
        public void TestSelectTreeLeafs()
        {
            SingleLeafTree root = new SingleLeafTree(new SingleLeafTree(1, 2, 3), 4, new SingleLeafTree(5, 6, 7), new SingleLeafTree(new SingleLeafTree(8, 9, 10)));

            int[] leafs = root.SelectTreeLeafs((l) => l.Value).ToArray();
            Array.Sort(leafs);

            Assert.Equal(Enumerable.Range(1, 10), leafs.Where((n) => n != 0));
        }

        [Fact]
        public void TestSelectManyTreeLeafs()
        {
            MultiLeafTree root = new MultiLeafTree(new MultiLeafTree(new int[] { 1 }), new int[] { 2, 3, 4 }, new MultiLeafTree(new int[] { }), new MultiLeafTree(new MultiLeafTree(new int[] { 5, 6 }), new int[] { 7, 8, 9, 10 }));

            int[] leafs = root.SelectManyTreeLeafs((l) => l.Values).ToArray();
            Array.Sort(leafs);

            Assert.Equal(Enumerable.Range(1, 10), leafs);
        }

        [Fact]
        public void TestConcatenate()
        {
            int[] a = null;
            var b = new int[] { 0 };
            var c = new int[] { 1, 2 };

            Assert.Null(a.ConcatOrInvariant(a, false));
            Assert.Equal(b, a.ConcatOrInvariant(b));
            Assert.Equal(b, b.ConcatOrInvariant(a));
            Assert.Equal(b.Concat(c), b.ConcatOrInvariant(c));
        }

        //TODO: test ISetCollection variants for read-only collections/...

        [Fact]
        public void TestAsSetCollection()
        {
            var set = new HashSet<int>
            {
                1, 2, 3, 4, 5 
            };

            var collection = set.AsSetCollection();

            Assert.False(collection.Add(1));
            Assert.True(collection.Add(6));
            Assert.Equal(6, collection.Count);

            Assert.True(collection is ISet<int>);
            Assert.True(collection is ISetCollection<int>);
        }

        [Fact]
        public void TestSetCollectionAsReadOnly()
        {
            var set = new HashSet<int>
            {
                1, 2, 3, 4, 5
            };

            var collection = set.AsSetCollection();

            var readOnlyCollection = collection.AsReadonlyAuto();

            Assert.True(readOnlyCollection is ISetCollection<int>);

            readOnlyCollection = collection.AsReadonlyCollection();

            Assert.True(readOnlyCollection is ISetCollection<int>);
        }

        //TODO: test ISetCollection<int> on IList<T> for AsReadOnly*

        private static void TestEnumerable(IEnumerable<int> readOnly, IEnumerable<int> comparison)
        {
            Assert.Equal(comparison, readOnly);
        }

        private static void TestCollection(ICollection<int> readOnly, ICollection<int> comparison)
        {
            TestEnumerable(readOnly, comparison);

            Assert.Throws<InvalidOperationException>(() => readOnly.Add(1));
            Assert.Throws<InvalidOperationException>(() => readOnly.Remove(1));
            Assert.Throws<InvalidOperationException>(() => readOnly.Clear());

            readOnly.Contains(comparison.First());

            int[] v = new int[comparison.Count + 10];

            readOnly.CopyTo(v, 0);

            Assert.Equal(comparison.Count, readOnly.Count);

            Assert.Equal(comparison, v.Take(comparison.Count));
            Assert.Equal(Enumerable.Repeat(0, 10), v.Skip(comparison.Count));

            Array.Clear(v, 0, v.Length);

            readOnly.CopyTo(v, 5);

            Assert.Equal(Enumerable.Repeat(0, 5), v.Take(5));
            Assert.Equal(Enumerable.Repeat(0, 5), v.Skip(5 + comparison.Count));
            Assert.Equal(comparison, v.Skip(5).Take(comparison.Count));

            int[] p = v.ShallowCopy();

            Assert.Throws<ArgumentOutOfRangeException>(() => readOnly.CopyTo(v, -1));
            Assert.Throws<ArgumentException>(() => readOnly.CopyTo(v, 100));
            Assert.Throws<ArgumentException>(() => readOnly.CopyTo(v, 11));
        }

        private static void TestList(IList<int> readOnly, IList<int> comparison)
        {
            TestCollection(readOnly, comparison);

            Assert.Throws<InvalidOperationException>(() => readOnly.Insert(1, 1));
            Assert.Throws<InvalidOperationException>(() => readOnly.RemoveAt(1));
            Assert.Throws<InvalidOperationException>(() => readOnly[0] = 1);

            Assert.Equal(comparison[0], readOnly[0]);

            Assert.Equal(comparison.IndexOf(5), readOnly.IndexOf(5));
            Assert.Equal(comparison.IndexOf(1235), readOnly.IndexOf(1235));
        }

        private class SingleLeafTree : IEnumerable<SingleLeafTree>
        {
            public int Value { get; }
            public ReadOnlyCollection<SingleLeafTree> Children { get; }

            public SingleLeafTree(int value)
            {
                Value = value;
            }

            public SingleLeafTree(params SingleLeafTree[] children)
            {
                Children = children.ReadOnlyShallowCopy();
            }

            public static implicit operator SingleLeafTree(int value) => new SingleLeafTree(value);

            public IEnumerator<SingleLeafTree> GetEnumerator()
            {
                return Children?.GetEnumerator() ?? Enumerable.Empty<SingleLeafTree>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class MultiLeafTree : IEnumerable<MultiLeafTree>
        {
            public ReadOnlyCollection<int> Values { get; }
            public ReadOnlyCollection<MultiLeafTree> Children { get; }

            public MultiLeafTree(params int[] values)
            {
                Values = values.ReadOnlyShallowCopy();
            }

            public MultiLeafTree(params MultiLeafTree[] children)
            {
                Children = children.ReadOnlyShallowCopy();
            }

            public static implicit operator MultiLeafTree(int[] values) => new MultiLeafTree(values);

            public IEnumerator<MultiLeafTree> GetEnumerator()
            {
                return Children?.GetEnumerator() ?? Enumerable.Empty<MultiLeafTree>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
