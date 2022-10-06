using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class CompositeListTests
    {
        [Fact]
        public void Test()
        {
            List<string>[] lists = new List<string>[3];
            int totalCount = 0;

            for (int i = 0; i < lists.Length; i++)
            {
                var l = new List<string>();
                string baseName = (i + 1).ToString();

                lists[i] = l;

                for (char c = 'A'; c < 'C'; c++)
                {
                    l.Add(baseName + c);
                    totalCount++;
                }
            }

            CompositeList<string> composite = new CompositeList<string>(lists);

            Assert.Equal(totalCount, composite.Count);

            Assert.Equal("1A", composite[0]);
            Assert.Equal("1B", composite[1]);

            int ind2B = lists.SelectMany((s) => s).TakeWhile((s) => s != "2B").Count();

            Assert.Equal("2B", composite[ind2B]);

            Array.Equals(ind2B, composite.IndexOf("2B"));

            IList<string> list = composite;
            Assert.Throws<InvalidOperationException>(() => list.Add("invalid"));
            Assert.Throws<InvalidOperationException>(() => list.Remove("invalid"));
            Assert.Throws<InvalidOperationException>(() => list.Insert(0, "invalid"));
            Assert.Throws<InvalidOperationException>(() => list.RemoveAt(0));
            Assert.Throws<InvalidOperationException>(() => list.Clear());
        }

        [Fact]
        public void TestNull()
        {
            var c = new CompositeList<string>();
            IList<string> clst = c;

            Assert.Throws<NullReferenceException>(() => clst.Add("foo"));
            Assert.Throws<NullReferenceException>(() => clst[0] = "foobar");
            Assert.Throws<NullReferenceException>(() => clst.Insert(0, "foo"));
            Assert.Throws<NullReferenceException>(() => clst.Remove("foo"));
            Assert.Throws<NullReferenceException>(() => clst.RemoveAt(0));
            Assert.Throws<NullReferenceException>(() => clst.Contains("foo"));
            Assert.Throws<NullReferenceException>(() => clst.Clear());

            Assert.Throws<NullReferenceException>(() => c.GetEnumerator());
            Assert.Throws<NullReferenceException>(() => c.GetEnumerator());
            Assert.Throws<NullReferenceException>(() => c.IsReadOnly);
            Assert.Throws<NullReferenceException>(() => c.CopyTo(new string[0], 0));
            Assert.Throws<NullReferenceException>(() => c.Count);
        }
    }
}