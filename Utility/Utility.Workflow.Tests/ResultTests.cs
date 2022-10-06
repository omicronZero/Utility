using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class ResultTests
    {
        [Fact]
        public void TestResult()
        {
            var result = new Result<int>(10);

            Assert.Equal(10, result.Value);
        }

        [Fact]
        public void TestSuccess()
        {
            var result = new Result<int>(10);

            Assert.True(result.Success);
        }

        [Fact]
        public void TestFailed()
        {
            var result = new Result<int>();

            Assert.False(result.Success);
        }

        [Fact]
        public void TestConversion()
        {
            var result = (Result<int>)10;

            Assert.Equal(10, result.Value);
        }

        [Fact]
        public void TestStaticFailed()
        {
            var result = Result<int>.Failed;

            Assert.False(result.Success);
        }
    }
}
