using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class LabeledCollectionTests
    {
        [Fact]
        public void TestConstructors()
        {
            var collection = new LabeledCollection<string>();

            var l = new List<string>();
            collection = new LabeledCollection<string>(l);
        }

        [Fact]
        public void TestCollectionBehavior()
        {
            var l = new List<string>();
            var collection = new LabeledCollection<string>(l);

            //LabeledCollection inherits the Utility.Collections.Collection<T> and does not override the non-modifying methods Contains, Insert, etc.
            //thus only Add, Remove, Clear, etc. are tested here to check whether the behavior is delegated to the parent class correctly

            collection.Add("Test");

            Assert.Single(l);

            collection.Remove("Test");

            collection.Add("Foo");
            collection.Add("Foobar");

            collection.Clear();

            collection.Insert(0, "Foo");
            Assert.True(collection.SequenceEqual(new string[] { "Foo" }));

            collection[0] = "Foobar";
            Assert.True(collection.SequenceEqual(new string[] { "Foobar" }));

            collection.RemoveAt(0);

            Assert.Empty(collection);
        }

        [Fact]
        public void TestLabelBehavior()
        {
            var collection = new LabeledCollection<string>();

            Label endLabel = collection.CreateLabel(); // label on collection end

            Assert.Equal(0, endLabel.Index);
            Assert.True(endLabel.IsEnd);
            Assert.True(endLabel.IsAttached);

            collection.Add("Test");

            Assert.Equal(1, endLabel.Index);
            Assert.True(endLabel.IsEnd);

            //do twice to test whether the behavior is possible multiple times
            for (int i = 0; i < 2; i++)
            {
                Label itemLabel = collection.CreateLabel(0);

                Assert.Equal(0, itemLabel.Index);
                Assert.False(itemLabel.IsEnd);
                Assert.True(itemLabel.IsAttached);

                int changeCount = 0;

                itemLabel.LabelChanged += (s, e) => changeCount++;

                //collection: "Test"

                collection.Add("Test2"); //does not affect changeCount

                //collection: "Test", "Test2"

                Assert.Equal("Test", collection[itemLabel]);

                collection.Insert(0, "Test3"); //increments changeCount

                //collection: "Test3", "Test", "Test2"

                Assert.Equal("Test", collection[itemLabel]);

                collection.Insert(1, "Test4"); //increments changeCount

                //collection: "Test3", "Test4", "Test", "Test2"

                Assert.Equal("Test", collection[itemLabel]);

                Assert.Equal(2, changeCount);

                collection.RemoveAt(0);

                //collection: "Test4", "Test", "Test2"

                Assert.Equal("Test", collection[itemLabel]);

                collection.Remove("Test");

                Assert.NotEqual("Test", collection[itemLabel]);

                Assert.True(itemLabel.IsAttached);

                collection.Clear();

                //collection is empty

                Assert.True(itemLabel.IsAttached);
                Assert.Equal(0, itemLabel.Index);
                Assert.Equal(0, endLabel.Index);

                collection.RemoveLabel(itemLabel);

                Assert.False(itemLabel.IsAttached);
                Assert.True(endLabel.IsAttached);

                Assert.Throws<ArgumentException>(() => collection[itemLabel]);
                Assert.Throws<ArgumentException>(() => collection.RemoveLabel(itemLabel));

                //add "Test" for next for-iteration
                collection.Add("Test");
            }
        }

        [Fact]
        public void TestInsertionBeforeIteratedItem()
        {
            var collection = new LabeledCollection<int>(new List<int>(Enumerable.Range(0, 50)));

            Random rnd = new Random();

            var output = new List<int>();

            foreach (int i in collection.EnumerateIndicesLabeled())
            {
                int v = collection[i];
                output.Add(v);

                collection.Insert(rnd.Next(0, i), -1);
                collection.RemoveAt(rnd.Next(0, i + 1));
            }

            Assert.True(output.SequenceEqual(Enumerable.Range(0, 50)));
        }

        [Fact]
        public void TestInsertionAtIteratedItem()
        {
            var collection = new LabeledCollection<int>(new List<int>(Enumerable.Range(0, 3)));

            List<int> l = new List<int>();

            foreach (int i in collection.EnumerateIndicesLabeled())
            {
                l.Add(collection[i]);

                collection.Insert(i, 3);
            }

            Assert.True(l.SequenceEqual(Enumerable.Range(0, 3)));
        }

        [Fact]
        public void TestInsertionAfterIteratedItem()
        {
            var comparison = Enumerable.Range(0, 50).ToList();
            var collection = new LabeledCollection<int>(new List<int>(comparison));

            var result = new List<int>();

            using (var enr = collection.EnumerateIndicesLabeled().GetEnumerator())
            {
                enr.MoveNext(); //current index == 0
                result.Add(enr.Current);
                enr.MoveNext(); //current index == 1
                result.Add(enr.Current);

                collection.Insert(2, -1);
                collection.Insert(3, -1);
                collection.Insert(10, -1);

                while (enr.MoveNext())
                    result.Add(collection[enr.Current]);
            }

            comparison.Insert(2, -1);
            comparison.Insert(3, -1);
            comparison.Insert(10, -1);

            Assert.True(comparison.SequenceEqual(result));
        }

        [Fact]
        public void TestUpdateLabel()
        {
            LabeledCollection<int> collection = new LabeledCollection<int>();

            for (int i = 0; i < 10; i++)
                collection.Insert(0, 20 + i);

            var label = collection.CreateLabel(5);

            for (int i = 0; i < 10; i++)
                collection.Insert(i, 10 + i);

            Assert.Equal(15, label.Index);

            label.Index = 3;

            Assert.Equal(3, label.Index);

            for (int i = 0; i < 10; i++)
                collection.Insert(i, i);

            Assert.Equal(13, label.Index);

            collection.RemoveAt(4);

            Assert.Equal(12, label.Index);

            label.Index = 7;

            collection.RemoveAt(6);

            Assert.Equal(6, label.Index);
        }

        [Fact]
        public void TestRandom()
        {
            const int testCount = 1000;

            Random rnd = new Random(0);

            LabeledCollection<int> collection = new LabeledCollection<int>();
            List<Label> labels = new List<Label>();

            int l = 0;

            for (int i = 0; i < testCount; i++)
            {
                //clear with rather low probability
                Action a = (Action)(rnd.Next(24, 75) / 25);

                if (collection.Count == 0)
                    a = Action.Insert;

                if (a == Action.Clear)
                {
                    collection.Clear();

                    foreach (Label label in labels)
                    {
                        Assert.Equal(-1, label.Index);
                    }
                }
                else if (a == Action.Insert)
                {
                    int[] v = labels.Select((s) => s.Index).ToArray();
                    int ind = rnd.Next(0, collection.Count);
                    collection.Insert(ind, i);

                    for (int j = 0; j < labels.Count; j++)
                    {
                        if (v[j] >= ind)
                            Assert.Equal(v[j] + 1, labels[j].Index);
                    }

                    if (rnd.Next(0, 3) == 0)
                    {
                        collection.CreateLabel(ind);
                        l++;
                    }
                }
                else if (a == Action.Remove)
                {
                    int[] v = labels.Select((s) => s.Index).ToArray();
                    int ind = rnd.Next(0, collection.Count);

                    collection.RemoveAt(ind);

                    for (int j = 0; j < labels.Count; j++)
                    {
                        if (v[j] > ind)
                            Assert.Equal(v[j] - 1, labels[j].Index);
                        else if (v[j] == ind)
                        {
                            Assert.Equal(-1, labels[j].Index);
                            collection.RemoveLabel(labels[j]);
                        }
                    }
                }
            }
            if (l == 0)
                throw new InvalidOperationException("Bad implementation. Labels have not been created.");
        }

        private enum Action
        {
            Clear = 0,
            Insert,
            Remove,
        }
    }
}
