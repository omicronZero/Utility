using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class IntervalTests
    {
        [Fact]
        public void TestPrimitiveConstructors()
        {
            var interval = new Interval<int>(0, 1);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(0, 1, true, true);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(0, 1, true, false);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.False(interval.IncludeEnd);

            interval = new Interval<int>(0, 1, false, true);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.False(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(0, 1, 0);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.False(interval.IncludeStart);
            Assert.False(interval.IncludeEnd);

            interval = new Interval<int>(0, 1, IntervalBoundaryProperties.IncludeLowerBound);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.False(interval.IncludeEnd);

            interval = new Interval<int>(0, 1, IntervalBoundaryProperties.IncludeUpperBound);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.False(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(0, 1, IntervalBoundaryProperties.IncludeLowerBound | IntervalBoundaryProperties.IncludeUpperBound);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(1, 0, IntervalBoundaryProperties.IncludeLowerBound);

            Assert.Equal(1, interval.Start);
            Assert.Equal(0, interval.End);
        }

        [Fact]
        public void TestRangeConstructors()
        {
            Range<int> range = new Range<int>(0, 1);

            var interval = new Interval<int>(range);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(range, true, true);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(range, true, false);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.False(interval.IncludeEnd);

            interval = new Interval<int>(range, false, true);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.False(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(range, 0);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.False(interval.IncludeStart);
            Assert.False(interval.IncludeEnd);

            interval = new Interval<int>(range, IntervalBoundaryProperties.IncludeLowerBound);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.False(interval.IncludeEnd);

            interval = new Interval<int>(range, IntervalBoundaryProperties.IncludeUpperBound);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.False(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(range, IntervalBoundaryProperties.IncludeLowerBound | IntervalBoundaryProperties.IncludeUpperBound);

            Assert.Equal(0, interval.Start);
            Assert.Equal(1, interval.End);
            Assert.True(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);
        }

        [Fact]
        public void TestCreateNormalized()
        {
            var interval = Interval<int>.CreateNormalized(1, 0);

            Assert.True(interval.Start < interval.End);

            Assert.True(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = Interval<int>.CreateNormalized(1, 0, true, false);

            Assert.True(interval.Start < interval.End);

            Assert.False(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = Interval<int>.CreateNormalized(1, 0, false, true);

            Assert.True(interval.Start < interval.End);

            Assert.True(interval.IncludeStart);
            Assert.False(interval.IncludeEnd);

            interval = Interval<int>.CreateNormalized(0, 0, false, true);

            Assert.False(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = Interval<int>.CreateNormalized(0, 0, true, false);

            Assert.True(interval.IncludeStart);
            Assert.False(interval.IncludeEnd);
        }

        [Fact]
        public void TestCreateValue()
        {
            Assert.Equal(new Interval<int>(0, 0), Interval<int>.Value(0));
        }

        [Fact]
        public void TestIsEmpty()
        {
            Assert.False(new Interval<int>(0, 1).IsEmpty);
            Assert.False(new Interval<int>(0, 0).IsEmpty);

            Assert.True(new Interval<int>(0, 0, true, false).IsEmpty);
            Assert.True(new Interval<int>(0, 0, false, false).IsEmpty);
            Assert.True(new Interval<int>(0, 0, false, true).IsEmpty);
        }

        [Fact]
        public void TestContainsValue()
        {
            var interval = new Interval<int>(0, 100);

            Assert.True(interval.Contains(0));
            Assert.True(interval.Contains(100));

            Assert.True(interval.Contains(1));
            Assert.False(interval.Contains(-1));

            interval = new Interval<int>(0, 100, false, false);

            Assert.False(interval.Contains(0));
            Assert.False(interval.Contains(100));

            Assert.True(interval.Contains(1));
            Assert.False(interval.Contains(-1));

            interval = new Interval<int>(0, 100, true, false);

            Assert.True(interval.Contains(0));
            Assert.False(interval.Contains(100));

            Assert.True(interval.Contains(1));
            Assert.False(interval.Contains(-1));

            interval = new Interval<int>(0, 100, false, true);

            Assert.False(interval.Contains(0));
            Assert.True(interval.Contains(100));

            Assert.True(interval.Contains(1));
            Assert.False(interval.Contains(-1));
        }

        [Fact]
        public void TestContainsInterval()
        {
            var b = new Interval<int>(10, 50);
            var c = new Interval<int>(-10, -5);
            var d = new Interval<int>(-10, 5);
            var e = new Interval<int>(80, 120);
            var f = new Interval<int>(120, 140);

            var alu = new Interval<int>(0, 100);
            var al = new Interval<int>(0, 100, true, false);
            var au = new Interval<int>(0, 100, false, true);
            var a = new Interval<int>(0, 100, false, false);

            Assert.True(a.Contains(a));
            Assert.True(al.Contains(a));
            Assert.True(au.Contains(a));
            Assert.True(alu.Contains(a));

            Assert.False(a.Contains(al));
            Assert.False(a.Contains(au));
            Assert.False(a.Contains(alu));

            Assert.True(alu.Contains(au));
            Assert.True(alu.Contains(al));

            Assert.True(a.Contains(b));
            Assert.True(al.Contains(b));
            Assert.True(au.Contains(b));
            Assert.True(alu.Contains(b));
            Assert.False(a.Contains(c));
            Assert.False(a.Contains(d));

            Assert.True(alu.Contains(Interval<int>.Value(10)));
            Assert.True(alu.Contains(Interval<int>.Value(100)));
            Assert.True(alu.Contains(Interval<int>.Value(0)));

            Assert.False(a.Contains(Interval<int>.Value(0)));
            Assert.False(a.Contains(Interval<int>.Value(100)));

            Assert.False(a.Contains(e));
            Assert.False(a.Contains(f));
        }

        [Fact]
        public void TestEquals()
        {
            Interval<int> a = new Interval<int>(0, 1);
            Interval<int> b = new Interval<int>(0, 1, false, true);
            Interval<int> c = new Interval<int>(0, 1, true, false);
            Interval<int> d = new Interval<int>(0, 1, false, false);
            Interval<int> e = new Interval<int>(-1, 1);
            Interval<int> f = new Interval<int>(0, 2);
            Interval<int> g = new Interval<int>(-1, 2);

            Assert.Equal(a, a);
            Assert.NotEqual(b, a);
            Assert.NotEqual(c, a);
            Assert.NotEqual(d, a);
            Assert.NotEqual(e, a);
            Assert.NotEqual(f, a);
            Assert.NotEqual(g, a);
        }

        [Fact]
        public void TestIntersects()
        {
            var a = new Interval<int>(0, 5);
            var b = new Interval<int>(-5, 0, true, false);
            var c = new Interval<int>(-5, 0);
            var d = new Interval<int>(1, 2);
            var e = new Interval<int>(4, 7);
            var f = new Interval<int>(8, 10);

            Assert.False(a.Intersects(b));
            Assert.True(a.Intersects(c));
            Assert.True(a.Intersects(d));
            Assert.False(b.Intersects(d));
            Assert.True(a.Intersects(e));
            Assert.False(a.Intersects(f));
        }

        [Fact]
        public void TestIntersect()
        {
            var a = new Interval<int>(0, 5, false, true);
            var b = new Interval<int>(0, 5);


            var ab = a;
            ab.Intersect(b);
            Assert.False(ab.IncludeStart);
            Assert.True(ab.IncludeEnd);
            Assert.Equal(a.Start, ab.Start);
            Assert.Equal(a.End, ab.End);

            var bb = b;
            bb.Intersect(b);

            Assert.True(bb.IncludeStart);
            Assert.True(bb.IncludeEnd);
            Assert.Equal(b.Start, bb.Start);
            Assert.Equal(b.End, bb.End);

            Interval<int> empty1 = new Interval<int>(100, 100, false, false);
            Interval<int> empty2 = new Interval<int>(100, 100, true, false);
            Interval<int> empty3 = new Interval<int>(100, 100, false, true);


            var aempty1 = a;
            aempty1.Intersect(empty1);
            Assert.True(aempty1.IsEmpty);

            var aempty2 = a;
            aempty2.Intersect(empty2);
            Assert.True(aempty2.IsEmpty);

            var aempty3 = a;
            aempty3.Intersect(empty3);
            Assert.True(aempty3.IsEmpty);
        }

        [Fact]
        public void TestUnionRange()
        {
            Interval<int> a = new Interval<int>(0, 1);
            Interval<int> ab = a;
            Interval<int> ac = a;
            Interval<int> ad = a;
            Interval<int> ae = a;
            Interval<int> af = a;

            ab.Union(new Interval<int>(0, 2));
            ac.Union(new Interval<int>(0, 2, false, false));
            ad.Union(new Interval<int>(-1, 1, false, false));
            ae.Union(new Interval<int>(-1, 2, false, false));
            af.Union(new Interval<int>(-1, 2));

            Assert.Equal(ab, new Interval<int>(0, 2));
            Assert.Equal(ac, new Interval<int>(0, 2, true, false));
            Assert.Equal(ad, new Interval<int>(-1, 1, false, true));
            Assert.Equal(ae, new Interval<int>(-1, 2, false, false));
            Assert.Equal(af, new Interval<int>(-1, 2));

            Interval<int> empty1 = new Interval<int>(100, 100, false, false);
            Interval<int> empty2 = new Interval<int>(100, 100, true, false);
            Interval<int> empty3 = new Interval<int>(100, 100, false, true);

            var aempty1 = a;
            aempty1.Union(empty1);
            Assert.Equal(a, aempty1);

            var aempty2 = a;
            aempty2.Union(empty2);
            Assert.Equal(a, aempty2);

            var aempty3 = a;
            aempty3.Union(empty3);
            Assert.Equal(a, aempty3);
        }

        [Fact]
        public void TestUnionValue()
        {
            Interval<int> a = new Interval<int>(0, 2);

            Interval<int> ab = a;
            Interval<int> ac = a;
            Interval<int> ad = a;
            Interval<int> ae = a;
            Interval<int> af = a;

            ab.Union(-1);
            ac.Union(0);
            ad.Union(1);
            ae.Union(2);
            af.Union(3);

            Assert.Equal(new Interval<int>(-1, 2), ab);
            Assert.Equal(new Interval<int>(0, 2), ac);
            Assert.Equal(new Interval<int>(0, 2), ad);
            Assert.Equal(new Interval<int>(0, 2), ae);
            Assert.Equal(new Interval<int>(0, 3), af);

            a = new Interval<int>(0, 2, false, false);

            ab = a;
            ac = a;
            ad = a;
            ae = a;
            af = a;

            ab.Union(-1);
            ac.Union(0);
            ad.Union(1);
            ae.Union(2);
            af.Union(3);

            Assert.Equal(new Interval<int>(-1, 2, true, false), ab);
            Assert.Equal(new Interval<int>(0, 2, true, false), ac);
            Assert.Equal(new Interval<int>(0, 2, false, false), ad);
            Assert.Equal(new Interval<int>(0, 2, false, true), ae);
            Assert.Equal(new Interval<int>(0, 3, false, true), af);

            a = Interval<int>.Empty;

            a.Union(0);

            Assert.Equal(new Interval<int>(0, 0), a);
        }

        [Fact]
        public void TestEquality()
        {
            Assert.True(new Interval<int>(0, 1) == new Interval<int>(0, 1));
            Assert.False(new Interval<int>(0, 1) == new Interval<int>(0, 2));
            Assert.False(new Interval<int>(0, 1) == new Interval<int>(0, 1, false, true));
            Assert.False(new Interval<int>(0, 1) == new Interval<int>(0, 1, true, false));
            Assert.False(new Interval<int>(0, 1) == new Interval<int>(0, 1, false, false));
        }

        [Fact]
        public void TestUnequality()
        {
            Assert.False(new Interval<int>(0, 1) != new Interval<int>(0, 1));
            Assert.True(new Interval<int>(0, 1) != new Interval<int>(0, 2));
            Assert.True(new Interval<int>(0, 1) != new Interval<int>(0, 1, false, true));
            Assert.True(new Interval<int>(0, 1) != new Interval<int>(0, 1, true, false));
            Assert.True(new Interval<int>(0, 1) != new Interval<int>(0, 1, false, false));
        }

        [Fact]
        public void TestOperatorImplicitRange()
        {
            var v = (Interval<int>)new Range<int>(0, 1);

            Assert.Equal(0, v.Start);
            Assert.Equal(1, v.End);
            Assert.True(v.IncludeStart);
            Assert.True(v.IncludeEnd);
        }

        [Fact]
        public void TestNormalized()
        {
            Interval<int> interval = new Interval<int>(2, 0);

            interval.Normalize();

            Assert.True(interval.StartComparable < interval.EndComparable);

            interval = new Interval<int>(2, 0, true, false);

            interval.Normalize();

            Assert.True(interval.StartComparable < interval.EndComparable);
            Assert.False(interval.IncludeStart);
            Assert.True(interval.IncludeEnd);

            interval = new Interval<int>(2, 0, false, true);

            interval.Normalize();

            Assert.True(interval.StartComparable < interval.EndComparable);
            Assert.True(interval.IncludeStart);
            Assert.False(interval.IncludeEnd);
        }

        [Fact]
        public void TestSerialization()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            var instance = new Interval<int>(1, 2);

            formatter.Serialize(ms, instance);

            ms.Position = 0;

            var result = (Interval<int>)formatter.Deserialize(ms);

            Assert.Equal(instance, result);
        }
    }
}