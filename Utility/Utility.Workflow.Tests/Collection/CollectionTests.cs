using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class CollectionTests
    {

        [Fact]
        public void TestOperations()
        {
            Collection<int> collection = new Collection<int>();
            List<int> comparison = new List<int>();

            collection.Add(0);
            comparison.Add(0);

            collection.Add(1);
            comparison.Add(1);

            collection.RemoveAt(0);
            comparison.RemoveAt(0);

            collection.Insert(1, 2);
            comparison.Insert(1, 2);

            collection.Insert(1, 3);
            comparison.Insert(1, 3);

            collection.Remove(2);
            comparison.Remove(2);

            Assert.Equal(collection.Count, comparison.Count);
            Assert.True(collection.SequenceEqual(comparison));

            collection.Clear();
            comparison.Clear();

            for (int i = 0; i < 10; i++)
            {
                collection.Add(i);
                comparison.Add(i);
            }

            //tests correct IEnumerable<T> behavior
            Assert.True(collection.SequenceEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));

            Assert.Contains(1, collection);
            Assert.Equal(1, collection.IndexOf(1));

            int[] result = new int[10];

            collection.CopyTo(result, 0);

            Assert.True(result.SequenceEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
        }

        [Fact]
        public void TestErrors()
        {
            Collection<string> collection = new Collection<string>();

            Assert.Throws<ArgumentNullException>(() => new Collection<string>(null));

            //should not throw an ArgumentNullException
            collection.Add(null);
            Assert.Contains(null, collection);
            Assert.Equal(0, collection.IndexOf(null));

            string[] result = new string[0];

            Assert.Throws<ArgumentException>(() => collection.CopyTo(result, 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(result, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(result, -1));

            result = new string[10];
            Assert.Throws<ArgumentException>(() => collection.CopyTo(result, 11));
        }

        [Fact]
        public void TestInternalList()
        {
            List<string> internalList = new List<string>();
            Collection<string> collection = new Collection<string>(internalList);

            collection.Add("foo");

            Assert.Single(internalList);
            Assert.Contains("foo", internalList);
        }
    }
}
