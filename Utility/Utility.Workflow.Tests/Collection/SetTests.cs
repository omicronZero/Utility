using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Utility.Collections;
using Xunit;

namespace Utility.Tests.Collection
{
    public class SetTests
    {
        [DebuggerHidden]
        private static void AssertSetEquals<T>(IEnumerable<T> expected, ICollection<T> actual)
        {
            if (expected == null)
                throw new ArgumentNullException(nameof(expected));
            if (actual == null)
                throw new ArgumentNullException(nameof(actual));

            Assert.True(expected.ToHashSet().SetEquals(expected), $"Expected: {{{string.Join(", ", expected)}}}\nActual: {{{string.Join(", ", actual)}}}");
        }

        [Fact]
        public void TestDefault()
        {
            Set<int> set = default;

            Assert.Empty(set);
        }

        [Fact]
        public void TestSingleton()
        {
            var set = new Set<int>(1);

            Assert.True(set.IsSingleton);
            Assert.Equal(1, set.GetSingleton());
        }

        [Fact]
        public void TestList()
        {
            var set = new Set<int>(1, 2, 3);

            Assert.False(set.IsSingleton);
            AssertSetEquals(new int[] { 1, 2, 3 }, set);
        }

        [Fact]
        public void TestContains()
        {
            var set1 = new Set<int>(1);
            var set2 = new Set<int>(1, 2, 3);

            Assert.DoesNotContain(2, set1);
            Assert.DoesNotContain(expected: 1, Set<int>.Empty);

            Assert.Contains(1, set1);
            Assert.Contains(2, set2);
        }

        [Fact]
        public void TestToString()
        {
            var set1 = new Set<int>(1);
            var set2 = new Set<int>(1, 2, 3);

            Assert.Equal("{1}", set1.ToString());
            Assert.Equal("{1, 2, 3}", set2.ToString());
        }

        [Fact]
        public void TestUnion2Distinct()
        {
            var set1 = new Set<int>(1, 2, 3);
            var set2 = new Set<int>(4, 5, 6);

            AssertSetEquals(new int[] { 1, 2, 3, 4, 5, 6 }, Set.Union(set1, set2));
        }

        [Fact]
        public void TestUnion2Overlapping()
        {
            var set1 = new Set<int>(1, 2, 3);
            var set2 = new Set<int>(3, 5, 6);

            AssertSetEquals(new int[] { 1, 2, 3, 5, 6 }, Set.Union(set1, set2));
        }

        [Fact]
        public void TestIntersect2Distinct()
        {
            var set1 = new Set<int>(1, 2, 3);
            var set2 = new Set<int>(4, 5, 6);

            AssertSetEquals(new int[] { }, Set.Intersect(set1, set2));
        }

        [Fact]
        public void TestIntersect2OverlappingSingle()
        {
            var set1 = new Set<int>(1, 2, 3);
            var set2 = new Set<int>(3, 5, 6);
            var Intersect = Set.Intersect(set1, set2);

            AssertSetEquals(new int[] { 3 }, Intersect);
            Assert.True(Intersect.IsSingleton);
        }

        [Fact]
        public void TestIntersect2OverlappingMultiple()
        {
            var set1 = new Set<int>(1, 2, 3);
            var set2 = new Set<int>(2, 3, 6);
            var Intersect = Set.Intersect(set1, set2);

            AssertSetEquals(new int[] { 2, 3 }, Intersect);
            Assert.False(Intersect.IsSingleton);
        }

        [Fact]
        public void TestUnion3Distinct()
        {
            var set1 = new Set<int>(0, 1, 2);
            var set2 = new Set<int>(3, 4, 5);
            var set3 = new Set<int>(6, 7, 8);

            var Intersect = Set.Union(set1, set2, set3);

            AssertSetEquals(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, Intersect);
        }

        [Fact]
        public void TestUnion3Overlapping()
        {
            var set1 = new Set<int>(0, 1, 2);
            var set2 = new Set<int>(2, 3, 4);
            var set3 = new Set<int>(4, 5, 6);

            var Intersect = Set.Union(set1, set2, set3);

            AssertSetEquals(new int[] { 0, 1, 2, 3, 4, 5, 6 }, Intersect);
        }

        [Fact]
        public void TestUnionMultipleDistinct()
        {
            var set1 = new Set<int>(0, 1, 2);
            var set2 = new Set<int>(3, 4, 5);
            var set3 = new Set<int>(6, 7, 8);
            var set4 = new Set<int>(9, 10, 11);

            var Intersect = Set.Union(set1, set2, set3, set4);

            AssertSetEquals(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, Intersect);
        }

        [Fact]
        public void TestUnionMultipleOverlapping()
        {
            var set1 = new Set<int>(0, 1, 2);
            var set2 = new Set<int>(2, 3, 4);
            var set3 = new Set<int>(4, 5, 6);
            var set4 = new Set<int>(6, 7, 8);

            var Intersect = Set.Union(set1, set2, set3, set4);

            AssertSetEquals(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, Intersect);
        }

        [Fact]
        public void TestEquality()
        {
            var set1 = new Set<int>(0, 1, 2);
            var set2 = new Set<int>(0, 1, 2);
            var set3 = new Set<int>(2, 0, 1);

            Assert.Equal(set1, set1);
            Assert.Equal(set1, set2);
            Assert.Equal(set1, set3);
        }

        [Fact]
        public void TestEqualitySingleton()
        {
            var set1 = new Set<int>(1);
            var set2 = new Set<int>(1);

            Assert.Equal(set1, set1);
            Assert.Equal(set1, set2);
        }

        [Fact]
        public void TestInequality()
        {
            var sets = new Set<int>[]{
                new Set<int>(0, 1, 2),
                new Set<int>(1, 2, 3),
                new Set<int>(1),
                new Set<int>(2)
            };

            for (int i = 0; i < sets.Length; i++)
            {
                for (int j = 0; j < sets.Length; j++)
                {
                    if (i == j)
                        continue;

                    Assert.NotEqual(sets[i], sets[j]);
                }
            }
        }

        [Fact]
        public void TestSuperset()
        {
            var set1 = new Set<int>(0, 1, 2);
            var set2 = new Set<int>(1, 2, 3);
            var set3 = new Set<int>(1);
            var set4 = new Set<int>(2);
            var set5 = new Set<int>(0, 1, 2, 3);

            Assert.True(set5.IsSupersetOf(set1));
            Assert.True(set5.IsSupersetOf(set3));
            Assert.True(set5.IsSupersetOf(set5));
            Assert.True(set5.IsSupersetOf(set1));

            Assert.False(set1.IsSupersetOf(set2));
            Assert.False(set4.IsSupersetOf(set3));

            Assert.True(set5.IsSupersetOf((IEnumerable<int>)set1));
            Assert.True(set5.IsSupersetOf((IEnumerable<int>)set3));
            Assert.True(set5.IsSupersetOf((IEnumerable<int>)set5));
            Assert.True(set5.IsSupersetOf((IEnumerable<int>)set1));

            Assert.False(set1.IsSupersetOf((IEnumerable<int>)set2));
            Assert.False(set4.IsSupersetOf((IEnumerable<int>)set3));
        }

        [Fact]
        public void TestSubset()
        {
            var set1 = new Set<int>(0, 1, 2);
            var set2 = new Set<int>(1, 2, 3);
            var set3 = new Set<int>(1);
            var set4 = new Set<int>(2);
            var set5 = new Set<int>(0, 1, 2, 3);

            Assert.True(set1.IsSubsetOf(set5));
            Assert.True(set3.IsSubsetOf(set5));
            Assert.True(set5.IsSubsetOf(set5));
            Assert.True(set1.IsSubsetOf(set5));

            Assert.False(set2.IsSubsetOf(set1));
            Assert.False(set3.IsSubsetOf(set4));

            Assert.True(set1.IsSubsetOf((IEnumerable<int>)set5));
            Assert.True(set3.IsSubsetOf((IEnumerable<int>)set5));
            Assert.True(set5.IsSubsetOf((IEnumerable<int>)set5));
            Assert.True(set1.IsSubsetOf((IEnumerable<int>)set5));

            Assert.False(set2.IsSubsetOf((IEnumerable<int>)set1));
            Assert.False(set3.IsSubsetOf((IEnumerable<int>)set4));
        }

        private static readonly Set<int>[] _sets = new Set<int>[]{
            Set.Empty<int>(),
            Set.Create(1),
            Set.Create(4),
            Set.Create(1, 2, 3),
            Set.Create(1, 2, 3, 4),
        };

        [Fact]
        public void TestManyIntersectAndUnion()
        {
            foreach (var s1 in _sets)
            {
                foreach (var s2 in _sets)
                {
                    AssertSetEquals(s1
                        .Union(s2),
                        Set.Union(s1, s2));
                    AssertSetEquals(s1
                        .Intersect(s2),
                        Set.Intersect(s1, s2));

                    if (s1 == s2)
                        AssertSetEquals(s1
                            .Intersect(s2),
                            Set.Intersect(s1, s1));

                    foreach (var s3 in _sets)
                    {
                        AssertSetEquals(s1
                            .Union(s2)
                            .Union(s3),
                            Set.Union(s1, s2, s3));
                        AssertSetEquals(s1
                            .Intersect(s2)
                            .Intersect(s3),
                            Set.Intersect(s1, s2, s3));

                        if (s1 == s2)
                            AssertSetEquals(s1
                                .Intersect(s2),
                                Set.Intersect(s1, s1, s3));

                        if (s1 == s3)
                            AssertSetEquals(s1
                                .Intersect(s2),
                                Set.Intersect(s1, s2, s1));

                        if (s2 == s3)
                            AssertSetEquals(s1
                                .Intersect(s2),
                                Set.Intersect(s1, s2, s2));

                        if (s1 == s2 && s1 == s3)
                            AssertSetEquals(s1
                                .Intersect(s2),
                                Set.Intersect(s1, s1, s1));

                        foreach (var s4 in _sets)
                        {
                            AssertSetEquals(s1
                                .Union(s2)
                                .Union(s3)
                                .Union(s4),
                                Set.Union(s1, s2, s3, s4));
                            AssertSetEquals(s1
                                .Intersect(s2)
                                .Intersect(s3)
                                .Intersect(s4),
                                Set.Intersect(s1, s2, s3, s4));
                            AssertSetEquals(s1
                                .Intersect(s2)
                                .Intersect(s3)
                                .Intersect(s4),
                                Set.Intersect((IEnumerable<Set<int>>)new Set<int>[] { s1, s2, s3, s4 }));
                        }
                    }
                }
            }
        }
    }
}
