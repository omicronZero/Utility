using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class UncertainBooleanTests
    {
        [Fact]
        public void TestTrueFalse()
        {
            UncertainBoolean t = true;
            UncertainBoolean f = false;

            if (t) { } else throw new InvalidOperationException("IsTrue did not work.");

            if (f)
                throw new InvalidOperationException("IsFalse did not work.");

            Assert.False(t.IsUncertain);
            Assert.False(f.IsUncertain);
        }
        [Fact]
        public void TestNullableTrueFalse()
        {
            UncertainBoolean t = (bool?)true;
            UncertainBoolean f = (bool?)false;
            UncertainBoolean u = (bool?)null;

            Assert.True(t.IsTrue);
            Assert.False(t.IsFalse);

            Assert.False(t.IsUncertain);
            Assert.False(f.IsUncertain);

            Assert.True(u.IsUncertain);
            Assert.False(u.IsTrue);
            Assert.False(u.IsFalse);
        }

        [Fact]
        public void TestIsTrueIsFalseIsUncertain()
        {
            UncertainBoolean t = true;
            UncertainBoolean f = false;
            UncertainBoolean u = null;

            Assert.True(t.IsTrue);
            Assert.False(f.IsTrue);
            Assert.False(t.IsFalse);
            Assert.True(f.IsFalse);

            Assert.True(t.IsTrueOrUncertain);
            Assert.False(f.IsTrueOrUncertain);

            Assert.True(t.IsTrueOrUncertain);
            Assert.True(f.IsFalseOrUncertain);

            Assert.True(u.IsTrueOrUncertain);
            Assert.True(u.IsFalseOrUncertain);
        }

        [Fact]
        public void TestUncertain()
        {
            UncertainBoolean uncertain = null;

            if (uncertain) throw new InvalidOperationException("Uncertainity did not work.");
        }

        [Fact]
        public void TestLogicalOperators()
        {
            UncertainBoolean t = true;
            UncertainBoolean f = false;
            UncertainBoolean u = null;

            Assert.True((t | f).IsTrue);
            Assert.False((t & f).IsTrue);
            Assert.True((t ^ f).IsTrue);
            Assert.False((!t).IsTrue);
            Assert.True((!f).IsTrue);

            Assert.True((t | u).IsTrue);
            Assert.True((u | t).IsTrue);
            Assert.True((u | f).IsUncertain);
            Assert.True((f | u).IsUncertain);
            Assert.True((u | u).IsUncertain);

            Assert.True((t & u).IsUncertain);
            Assert.True((u & t).IsUncertain);
            Assert.True((u & f).IsFalse);
            Assert.True((f & u).IsFalse);
            Assert.True((u & u).IsUncertain);

            Assert.True((t ^ u).IsUncertain);
            Assert.True((u ^ t).IsUncertain);
            Assert.True((u ^ f).IsUncertain);
            Assert.True((f ^ u).IsUncertain);
            Assert.True((u ^ u).IsUncertain);

            Assert.True((!u).IsUncertain);
        }
    }
}
