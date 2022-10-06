using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class ListSegmentTests
    {
        [Fact]
        public void TestListBehavior()
        {
            IList<string>[] underlyingLists = new IList<string>[]{
                new List<string>(Enumerable.Range(0, 100).Select((s) => s.ToString())),
                new System.Collections.ObjectModel.Collection<string>(new List<string>(new List<string>(Enumerable.Range(0, 100).Select((s) => s.ToString()))))
            };

            foreach (IList<string> underlyingList in underlyingLists)
            {
                int listInitialCount = underlyingList.Count;

                int startIndex = 10;
                int count = 40;

                string lastBeforeSegment = underlyingList[startIndex - 1];
                string firstAfterSegment = underlyingList[startIndex + count];

                var segment = new ListSegment<string>(underlyingList, startIndex, count);

                Assert.Equal(underlyingList.IsReadOnly, segment.IsReadOnly);

                Assert.Equal(underlyingList[startIndex], segment[0]);
                Assert.Equal(underlyingList[startIndex + count / 2 - 1], segment[count / 2 - 1]);
                Assert.Equal(underlyingList[startIndex + count - 1], segment[count - 1]);

                string v = segment[3];

                segment[3] = "foo";

                Assert.Equal("foo", underlyingList[startIndex + 3]);

                segment[3] = v;

                segment.Add("Test1");

                count++;
                Assert.Equal(listInitialCount + 1, underlyingList.Count);
                Assert.Equal(count, segment.Count);
                Assert.Equal("Test1", segment[count - 1]);
                Assert.Equal(count - 1, segment.IndexOf("Test1"));

                segment.Insert(4, "Test2");
                count++;

                Assert.Equal(count, segment.Count);
                Assert.Equal("Test2", segment[4]);
                Assert.Equal("Test2", underlyingList[startIndex + 4]);

                Assert.Contains("Test2", underlyingList);
                Assert.Contains("Test2", segment);

                segment.RemoveAt(4);
                count--;

                Assert.Equal(count, segment.Count);

                Assert.DoesNotContain("Test2", underlyingList);
                Assert.DoesNotContain("Test2", segment);

                Assert.True(segment.Remove("Test1"));
                count--;

                Assert.DoesNotContain("Test1", underlyingList);
                Assert.DoesNotContain("Test1", segment);

                for (int i = 0; i < segment.Count; i++)
                    Assert.Equal(underlyingList[startIndex + i], segment[i]);

                segment.Clear();

                Assert.Empty(segment);
                Assert.Equal(listInitialCount - count, underlyingList.Count);

                Assert.Equal(lastBeforeSegment, underlyingList[startIndex - 1]);
                Assert.Equal(firstAfterSegment, underlyingList[startIndex]);
            }
        }

        [Fact]
        public void TestArrayBehavior()
        {
            string[] underlyingList = Enumerable.Range(0, 100).Select((i) => i.ToString()).ToArray();

            int startIndex = 10;
            int count = 40;

            var segment = new ListSegment<string>(underlyingList, startIndex, count);

            Assert.True(segment.IsReadOnly);

            Assert.Equal(underlyingList[startIndex], segment[0]);
            Assert.Equal(underlyingList[startIndex + count / 2 - 1], segment[count / 2 - 1]);
            Assert.Equal(underlyingList[startIndex + count - 1], segment[count - 1]);

            segment.Clear();

            for (int i = 0; i < underlyingList.Length; i++)
            {
                Assert.Equal(i >= startIndex && i < startIndex + count ? null : i.ToString(), underlyingList[i]);
            }

            Assert.Equal(count, segment.Count);
        }


        [Fact]
        public void TestCopyTo()
        {
            string[] buffer = Enumerable.Range(0, 100).Select((i) => i.ToString()).ToArray();
            var underlyingLists = new IList<string>[] { buffer, new List<string>(buffer), new System.Collections.ObjectModel.Collection<string>(buffer) };

            int startIndex = 10;
            int count = 40;

            string[] result = new string[count + 4];

            foreach (var underlyingList in underlyingLists)
            {
                var segment = new ListSegment<string>(underlyingList, startIndex, count);

                segment.CopyTo(result, 2);

                Assert.True(result[0] == null && result[1] == null);
                Assert.True(result[result.Length - 1] == null && result[result.Length - 2] == null);

                for (int i = 0; i < segment.Count; i++)
                {
                    Assert.Equal(segment[i], result[i + 2]);
                }

                Array.Clear(result, 0, result.Length);
            }
        }

        [Fact]
        public void TestListExceptions()
        {
            string[] buffer = Enumerable.Range(0, 100).Select((i) => i.ToString()).ToArray();
            var underlyingLists = new IList<string>[] { new List<string>(buffer), new System.Collections.ObjectModel.Collection<string>(buffer), Array.AsReadOnly(buffer) };

            int startIndex = 10;
            int count = 40;

            string[] result = new string[count + 4];

            foreach (var underlyingList in underlyingLists)
            {
                var segment = new ListSegment<string>(underlyingList, startIndex, count);

                Assert.Throws<ArgumentOutOfRangeException>(() => segment[-1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => segment[count]);
                Assert.Throws<ArgumentOutOfRangeException>(() => segment.CopyTo(new string[100], -1));
                Assert.Throws<ArgumentException>(() => segment.CopyTo(new string[1], 0));
                Assert.Throws<ArgumentNullException>(() => segment.CopyTo(null, -1));
                Assert.Throws<ArgumentNullException>(() => segment.CopyTo(null, 0));

                //fixed size is not tested here
                if (segment.IsReadOnly)
                {
                    Assert.Throws<InvalidOperationException>(() => segment.Add("foo"));
                    Assert.Throws<InvalidOperationException>(() => segment.Remove("foo"));
                    Assert.Throws<InvalidOperationException>(() => segment.Insert(0, "foo"));
                    Assert.Throws<InvalidOperationException>(() => segment.RemoveAt(0));
                }
                else
                {
                    Assert.Throws<ArgumentOutOfRangeException>("index", () => segment.Insert(-1, "foo"));
                    Assert.Throws<ArgumentOutOfRangeException>("index", () => segment.Insert(segment.Count + 1, "foo"));
                    Assert.Throws<ArgumentOutOfRangeException>("index", () => segment.RemoveAt(-1));
                    Assert.Throws<ArgumentOutOfRangeException>("index", () => segment.RemoveAt(segment.Count));
                    Assert.Throws<ArgumentOutOfRangeException>("index", () => segment[segment.Count] = "foo");
                    Assert.Throws<ArgumentOutOfRangeException>("index", () => segment[-1] = "foo");
                }
            }
        }

        [Fact]
        public void TestFixedSizeExceptions()
        {
            string[] buffer = Enumerable.Range(0, 100).Select((i) => i.ToString()).ToArray();
            var underlyingLists = new FixedSizeMock<string>[] { new FixedSizeMock<string>(buffer, false), new FixedSizeMock<string>(buffer, true) };

            int startIndex = 10;
            int count = 40;

            string[] result = new string[count + 4];

            foreach (var underlyingList in underlyingLists)
            {
                var segment = new ListSegment<string>(underlyingList, startIndex, count);

                Assert.True(underlyingList.IsFixedSize());
                Assert.Equal(underlyingList.IsSetterReadonly(), underlyingList.DenyItemChanges);

                Assert.Throws<ArgumentOutOfRangeException>(() => segment[-1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => segment[count]);
                Assert.Throws<ArgumentOutOfRangeException>(() => segment.CopyTo(new string[100], -1));
                Assert.Throws<ArgumentException>(() => segment.CopyTo(new string[1], 0));
                Assert.Throws<ArgumentNullException>(() => segment.CopyTo(null, -1));
                Assert.Throws<ArgumentNullException>(() => segment.CopyTo(null, 0));

                Assert.Throws<InvalidOperationException>(() => segment.Add("foo"));
                Assert.Throws<InvalidOperationException>(() => segment.Remove("foo"));
                Assert.Throws<InvalidOperationException>(() => segment.Insert(0, "foo"));
                Assert.Throws<InvalidOperationException>(() => segment.RemoveAt(0));

                if (segment.IsSetterReadonly())
                {
                    Assert.Throws<InvalidOperationException>(() => segment[0] = "foo");
                }
                else
                {
                    segment[0] = "foo";
                }
            }
        }

        [Fact]
        public void TestScopeChange()
        {
            string[] buffer = Enumerable.Range(0, 100).Select((i) => i.ToString()).ToArray();
            var underlyingLists = new IList<string>[] { new List<string>(buffer), new System.Collections.ObjectModel.Collection<string>(buffer), Array.AsReadOnly(buffer) };

            int startIndex = 10;
            int count = 40;


            foreach (var underlyingList in underlyingLists)
            {
                var segment = new ListSegment<string>(underlyingList, startIndex, count);

                Assert.Throws<ArgumentOutOfRangeException>(() => segment.SetScope(-1, count));
                Assert.Throws<ArgumentOutOfRangeException>(() => segment.SetScope(underlyingList.Count + 1, 0));
                Assert.Throws<ArgumentException>(() => segment.SetScope(startIndex, underlyingList.Count - startIndex + 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => segment.SetScope(startIndex, -1));

                segment.SetScope(10, 45);

                Assert.Equal(underlyingList[10], segment[0]);
            }
        }
    }
}
