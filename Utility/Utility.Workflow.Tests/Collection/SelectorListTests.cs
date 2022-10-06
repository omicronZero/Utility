using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class SelectorListTests
    {
        [Fact]
        public void TestListWithoutConverter()
        {
            var underlyingList = Enumerable.Range(0, 10).ToList();
            var comparison = underlyingList.Select((i) => i.ToString());

            var collections = new SelectorList<string, int>[]
            {
                new SelectorList<string, int>(underlyingList, (i) => i.ToString()),
                SelectorList.SelectList(underlyingList, (i) => i.ToString())
            };

            foreach (var collection in collections)
            {
                Assert.True(collection.SequenceEqual(comparison));

                Assert.Contains("5", collection);
                Assert.Equal(5, collection.IndexOf("5"));
                Assert.DoesNotContain("-1", collection);
                Assert.DoesNotContain("Not a numeric string", collection);
                Assert.Equal(underlyingList.Count, collection.Count);

                Assert.Throws<InvalidOperationException>(() => collection.Add("12"));
                Assert.True(collection.SequenceEqual(comparison));

                Assert.Throws<InvalidOperationException>(() => collection.Remove("12"));
                Assert.True(collection.SequenceEqual(comparison));

                Assert.Throws<InvalidOperationException>(() => collection.Clear());
                Assert.True(collection.SequenceEqual(comparison));

                string[] array = new string[4 + collection.Count];

                collection.CopyTo(array, 2);

                Assert.True(array.SequenceEqual(new string[2].Concat(collection).Concat(new string[2])));
            }
        }

        [Fact]
        public void TestArrayWithoutConverter()
        {
            var underlyingList = Enumerable.Range(0, 10).ToArray();
            var comparison = underlyingList.Select((i) => i.ToString());

            var collections = new SelectorList<string, int>[]
            {
                new SelectorList<string, int>(underlyingList, (i) => i.ToString()),
                SelectorList.SelectList(underlyingList, (i) => i.ToString())
            };

            foreach (var collection in collections)
            {
                Assert.True(collection.SequenceEqual(comparison));

                Assert.Contains("5", collection);
                Assert.Equal(5, collection.IndexOf("5"));
                Assert.DoesNotContain("-1", collection);
                Assert.DoesNotContain("Not a numeric string", collection);
                Assert.Equal(underlyingList.Length, collection.Count);

                Assert.Throws<InvalidOperationException>(() => collection.Add("12"));
                Assert.True(collection.SequenceEqual(comparison));

                Assert.Throws<InvalidOperationException>(() => collection.Remove("12"));
                Assert.True(collection.SequenceEqual(comparison));

                Assert.Throws<InvalidOperationException>(() => collection.Insert(1, "12"));
                Assert.True(collection.SequenceEqual(comparison));

                Assert.Throws<InvalidOperationException>(() => collection.RemoveAt(1));
                Assert.True(collection.SequenceEqual(comparison));

                Assert.Throws<InvalidOperationException>(() =>
                {
                    var c = collection;
                    c[1] = "2";
                });
                Assert.True(collection.SequenceEqual(comparison));

                Assert.Throws<InvalidOperationException>(() => collection.Clear());
                Assert.True(collection.SequenceEqual(comparison));

                string[] array = new string[4 + collection.Count];

                collection.CopyTo(array, 2);

                Assert.True(array.SequenceEqual(new string[2].Concat(collection).Concat(new string[2])));
            }
        }

        [Fact]
        public void TestListWithConverter()
        {
            var range = Enumerable.Range(0, 10);
            var underlyingList = range.ToList();
            var comparison = underlyingList.Select((i) => i.ToString()).ToList();

            var collection = new SelectorList<string, int>(underlyingList, (i) => i.ToString(), int.Parse);

            Assert.True(collection.SequenceEqual(comparison));

            Assert.Contains("5", collection);
            Assert.DoesNotContain("-1", collection);
            Assert.DoesNotContain("Not a numeric string", collection);
            Assert.Equal(underlyingList.Count, collection.Count);

            collection.Add("12");
            Assert.Equal(12, underlyingList.Last());
            Assert.True(collection.SequenceEqual(comparison.Concat(new string[] { "12" })));

            Assert.True(collection.Remove("12"));
            Assert.True(collection.SequenceEqual(comparison));

            collection.Clear();
            Assert.Empty(underlyingList);
            underlyingList.Clear();
            underlyingList.AddRange(range);

            string[] array = new string[4 + collection.Count];

            collection.CopyTo(array, 2);

            Assert.True(array.SequenceEqual(new string[2].Concat(collection).Concat(new string[2])));

            collection = SelectorList.SelectList(underlyingList, (i) => i.ToString(), int.Parse);

            Assert.True(collection.SequenceEqual(comparison));

            Assert.Contains("5", collection);
            Assert.Equal(5, collection.IndexOf("5"));
            Assert.DoesNotContain("-1", collection);
            Assert.DoesNotContain("Not a numeric string", collection);
            Assert.Equal(underlyingList.Count, collection.Count);

            collection.Add("12");
            Assert.Equal(12, underlyingList.Last());
            Assert.True(collection.SequenceEqual(comparison.Concat(new string[] { "12" })));

            Assert.True(collection.Remove("12"));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<ArgumentException>(() => collection.Add("Not a numeric string"));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.False(collection.Remove("Not a numeric string"));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<ArgumentException>(() => collection.Insert(0, "Not a numeric string"));
            Assert.True(collection.SequenceEqual(comparison));

            collection.Clear();
            Assert.Empty(underlyingList);
            underlyingList.Clear();
            underlyingList.AddRange(range);

            array = new string[4 + collection.Count];

            collection.CopyTo(array, 2);

            Assert.True(array.SequenceEqual(new string[2].Concat(collection).Concat(new string[2])));
        }

        [Fact]
        public void TestArrayWithConverter()
        {
            var range = Enumerable.Range(0, 10);
            var underlyingList = range.ToArray();
            var comparison = underlyingList.Select((i) => i.ToString()).ToList();

            var collection = new SelectorList<string, int>(underlyingList, (i) => i.ToString(), int.Parse);

            Assert.True(collection.SequenceEqual(comparison));

            Assert.Contains("5", collection);
            Assert.DoesNotContain("-1", collection);
            Assert.DoesNotContain("Not a numeric string", collection);
            Assert.Equal(underlyingList.Length, collection.Count);

            Assert.Throws<InvalidOperationException>(() => collection.Add("12"));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => collection.Remove("12"));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => collection.Insert(1, "12"));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => collection.RemoveAt(1));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() =>
            {
                var c = collection;
                c[1] = "2";
            });
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => collection.Clear());
            Assert.True(collection.SequenceEqual(comparison));

            string[] array = new string[4 + collection.Count];

            collection.CopyTo(array, 2);

            Assert.True(array.SequenceEqual(new string[2].Concat(collection).Concat(new string[2])));

            collection = SelectorList.SelectList(underlyingList, (i) => i.ToString(), int.Parse);

            Assert.True(collection.SequenceEqual(comparison));
            Assert.Contains("5", collection);
            Assert.Equal(5, collection.IndexOf("5"));
            Assert.DoesNotContain("-1", collection);
            Assert.DoesNotContain("Not a numeric string", collection);
            Assert.Equal(underlyingList.Length, collection.Count);

            Assert.Throws<InvalidOperationException>(() => collection.Add("12"));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => collection.Remove("12"));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => collection.Insert(1, "12"));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => collection.RemoveAt(1));
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() =>
            {
                var c = collection;
                c[1] = "2";
            });
            Assert.True(collection.SequenceEqual(comparison));

            Assert.Throws<InvalidOperationException>(() => collection.Clear());
            Assert.True(collection.SequenceEqual(comparison));

            array = new string[4 + collection.Count];

            collection.CopyTo(array, 2);

            Assert.True(array.SequenceEqual(new string[2].Concat(collection).Concat(new string[2])));
        }

        [Fact]
        public void TestNullReferences()
        {
            var range = Enumerable.Range(0, 10);
            var underlyingList = range.ToArray();
            var comparison = underlyingList.Select((i) => i.ToString()).ToList();

            var collection = new SelectorList<string, int>();

            Assert.Throws<NullReferenceException>(() => collection.Count);
            Assert.Throws<NullReferenceException>(() => collection.Contains("80"));
            Assert.Throws<NullReferenceException>(() => collection.IsReadOnly);

            Assert.Throws<NullReferenceException>(() => collection.Add("1"));
            Assert.Throws<NullReferenceException>(() => collection.Remove("1"));
            Assert.Throws<NullReferenceException>(() => collection.Clear());

            Assert.Throws<NullReferenceException>(() => collection.Insert(0, "1"));
            Assert.Throws<NullReferenceException>(() => collection.RemoveAt(0));
            Assert.Throws<NullReferenceException>(() => collection[0] = "1");
            Assert.Throws<NullReferenceException>(() => collection[0]);
            Assert.Throws<NullReferenceException>(() => collection.IndexOf("0"));

            Assert.Throws<NullReferenceException>(() => collection.CopyTo(new string[3], 2));
            Assert.Throws<NullReferenceException>(() => collection.CopyTo(new string[100], 1));
            Assert.Throws<NullReferenceException>(() => collection.CopyTo(new string[300], -1));
            Assert.Throws<NullReferenceException>(() => collection.CopyTo(new string[3], -1));
            Assert.Throws<NullReferenceException>(() => collection.CopyTo(null, 0));
            Assert.Throws<NullReferenceException>(() => collection.CopyTo(null, -1));
        }

        [Fact]
        public void TestInvalidArguments()
        {
            var range = Enumerable.Range(0, 10);
            var underlyingList = range.ToArray();
            var comparison = underlyingList.Select((i) => i.ToString()).ToList();

            Assert.Throws<ArgumentNullException>(() => new SelectorList<string, int>(null, (i) => i.ToString(), int.Parse));
            Assert.Throws<ArgumentNullException>(() => new SelectorList<string, int>(null, (i) => i.ToString()));
            Assert.Throws<ArgumentNullException>(() => new SelectorList<string, int>(null, null, int.Parse));
            Assert.Throws<ArgumentNullException>(() => new SelectorList<string, int>(null, null, null));

            Assert.Throws<ArgumentNullException>(() => SelectorList.SelectList<string, int>(null, (i) => i.ToString(), int.Parse));
            Assert.Throws<ArgumentNullException>(() => SelectorList.SelectList<string, int>(null, (i) => i.ToString()));
            Assert.Throws<ArgumentNullException>(() => SelectorList.SelectList<string, int>(null, null, int.Parse));
            Assert.Throws<ArgumentNullException>(() => SelectorList.SelectList<string, int>(null, null, null));

            var collection = new SelectorList<string, int>(underlyingList, (i) => i.ToString(), int.Parse);

            Assert.Throws<ArgumentException>(() => collection.CopyTo(new string[3], 2));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new string[underlyingList.Length], 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new string[300], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new string[3], -1));
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, -1));

            Assert.Throws<ArgumentOutOfRangeException>(() => collection.Insert(-1, "1"));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.Insert(collection.Count + 1, "1"));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.RemoveAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.RemoveAt(collection.Count));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => collection[collection.Count]);
            Assert.Throws<ArgumentOutOfRangeException>(() => collection[-1] = "0");
            Assert.Throws<ArgumentOutOfRangeException>(() => collection[collection.Count] = "0");
        }

        //TODO: test IList-behavior
    }
}
