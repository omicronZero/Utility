using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Utility.Tests
{
   public  class DisposableTests
    {
        [Fact]
        public void TestCreateCall()
        {
            int disposeCount = 0;
            Disposable.Create(() => disposeCount++).Dispose();

            Assert.Equal(1, disposeCount);
        }

        [Fact]
        public void TestCreateCallTwice()
        {
            int disposeCount = 0;
            var disposable = Disposable.Create(() => disposeCount++, true);

            disposable.Dispose();
            disposable.Dispose();

            Assert.Equal(1, disposeCount);
        }

        [Fact]
        public void TestCreateCallTwiceUncaughtSubsequentCalls()
        {
            int disposeCount = 0;
            var disposable = Disposable.Create(() => disposeCount++, false);

            disposable.Dispose();
            disposable.Dispose();

            Assert.Equal(2, disposeCount);
        }

        [Fact]
        public void TestCallEmpty()
        {
            Disposable.Empty.Dispose();
        }

        [Fact]
        public void TestJoin()
        {
            int disposeCount = 0;
            var disposables = Enumerable.Range(0, 100).Select((s) => Disposable.Create(() => disposeCount++)).ToArray();

            Disposable.Join(disposables).Dispose();

            Assert.Equal(100, disposeCount);
        }

        [Fact]
        public void TestJoinException()
        {
            var disposables = Enumerable.Range(0, 100).Select((s) => Disposable.Create(() => throw new InvalidOperationException())).ToArray();

           Assert.Throws<AggregateException>(() => Disposable.Join(disposables).Dispose());
        }
    }
}
