using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections.Adapters;
using Xunit;

namespace Utility.Tests.Collection
{
    public class CollectionHelperTests
    {
        [Fact]
        public void TestMethods()
        {
            List<string> strings = new List<string>();
            List<string> comparison = new List<string>();

            CollectionAdapterNongeneric<string, List<string>> helper = new CollectionAdapterNongeneric<string, List<string>>(strings);

            helper.Add("Test1");
            comparison.Add("Test1");

            helper.Add("Test2");
            comparison.Add("Test2");

            helper.Add("Test3");
            comparison.Add("Test3");

            helper.Add("Test4");
            comparison.Add("Test4");

            Assert.True(strings.SequenceEqual(comparison));

            helper.Remove("Test2");
            comparison.Remove("Test2");

            Assert.True(strings.SequenceEqual(comparison));

            Assert.True(helper.Contains("Test3"));

            string[] testArray = new string[helper.Count];

            Assert.Equal(testArray.Length, strings.Count);
            helper.CopyTo(testArray, 0);

            Assert.True(testArray.SequenceEqual(comparison));

            string[,] invalidArray = new string[1, testArray.Length];


            Assert.Throws<ArgumentException>(() => helper.CopyTo(invalidArray, 0));

            Assert.Throws<ArgumentNullException>(() => helper.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => helper.CopyTo(testArray, -1));
            Assert.Throws<ArgumentException>(() => helper.CopyTo(testArray, 1));

            Assert.Throws<ArgumentException>(() => helper.Add(1));

            helper.Remove(1);
            Assert.False(helper.Contains(1));

            helper.Clear();
            comparison.Clear();

            Assert.Empty(helper);
        }

        [Fact]
        public void TestNull()
        {
            var c = new CollectionAdapterNongeneric<string, List<string>>();

            Assert.Throws<NullReferenceException>(() => c.Add("foo"));
            Assert.Throws<NullReferenceException>(() => c.Remove("foo"));
            Assert.Throws<NullReferenceException>(() => c.Contains("foo"));

            Assert.Throws<NullReferenceException>(() => c.Add(1));
            Assert.Throws<NullReferenceException>(() => c.Remove(1));
            Assert.Throws<NullReferenceException>(() => c.Contains(1));

            Assert.Throws<NullReferenceException>(() => c.Clear());
            Assert.Throws<NullReferenceException>(() => c.GetEnumerator());
            Assert.Throws<NullReferenceException>(() => c.IsReadOnly);
            Assert.Throws<NullReferenceException>(() => c.SyncRoot);
            Assert.Throws<NullReferenceException>(() => c.IsSynchronized);
            Assert.Throws<NullReferenceException>(() => c.CopyTo(new string[0], 0));
            Assert.Throws<NullReferenceException>(() => c.CopyTo(new string[0, 0], 0));
            Assert.Throws<NullReferenceException>(() => c.Count);
        }
    }
}
