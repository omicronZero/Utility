using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Utility.Contexts;
using Xunit;

namespace Utility.Contexts.Tests
{
    public class ThreadContextualTests
    {
        [Fact]
        public void TestUninitializableLocalThreadBehaviorN1()
        {
            var mainThreadContext = new ContextMock(1);

            using (ThreadContextual<ContextMock>.Push(mainThreadContext))
            {
                Assert.Equal(mainThreadContext, ThreadContextual<ContextMock>.CurrentContext);
            }

            Assert.Null(ThreadContextual<ContextMock>.GetCurrentContext(false));
        }

        [Fact]
        public void TestUninitializableLocalThreadBehaviorN2()
        {
            var c1 = new ContextMock(1);

            using (ThreadContextual<ContextMock>.Push(c1))
            {
                var c2 = new ContextMock(2);

                using (ThreadContextual<ContextMock>.Push(c2))
                {
                    Assert.Equal(c2, ThreadContextual<ContextMock>.CurrentContext);
                }

                Assert.Equal(c1, ThreadContextual<ContextMock>.CurrentContext);
            }

            Assert.Null(ThreadContextual<ContextMock>.GetCurrentContext(false));
        }

        [Fact]
        public void TestUninitializableCrossThreadIndependenceBehaviorN1x0()
        {
            var mainThreadContext = new ContextMock(1);

            using (ThreadContextual<ContextMock>.Push(mainThreadContext))
            {
                new Thread(() =>
                {
                    var local = ThreadContextual<ContextMock>.GetCurrentContext(false);
                    Assert.NotEqual(mainThreadContext, local);
                }).Start();
            }
        }

        [Fact]
        public void TestUninitializableCrossThreadBehaviorN1x1()
        {
            var mainThreadContext = new ContextMock(1);

            using (ThreadContextual<ContextMock>.Push(mainThreadContext))
            {
                new Thread(() =>
                {
                    var local = ThreadContextual<ContextMock>.GetCurrentContext(false);

                    using (ThreadContextual<ContextMock>.Push(local))
                    {
                        Assert.Equal(local, ThreadContextual<ContextMock>.GetCurrentContext(false));
                    }

                    Assert.NotEqual(mainThreadContext, local);
                }).Start();
            }
        }
    }
}
