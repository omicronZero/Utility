using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Utility.Contexts;
using Xunit;

namespace Utility.Contexts.Tests
{
    public class GlobalContextTests
    {
        private const int TestCount = 10000;

        public static object SyncRoot { get; } = new object();

        [Fact]
        public void TestUninitializableGlobalThreadBehaviorN1()
        {
            for (int i = 0; i < TestCount; i++)
                lock (SyncRoot)
                {
                    var mainThreadContext = new ContextMock(1);

                    using (GlobalContext<ContextMock>.Push(mainThreadContext))
                    {
                        Assert.Equal(mainThreadContext, GlobalContext<ContextMock>.CurrentContext);
                    }

                    Assert.Null(GlobalContext<ContextMock>.GetCurrentContext(false));
                }
        }

        [Fact]
        public void TestUninitializableGlobalThreadBehaviorN2()
        {
            for (int i = 0; i < TestCount; i++)
                lock (SyncRoot)
                {
                    var c1 = new ContextMock(1);

                    using (GlobalContext<ContextMock>.Push(c1))
                    {
                        var c2 = new ContextMock(2);

                        using (GlobalContext<ContextMock>.Push(c2))
                        {
                            Assert.Equal(c2, GlobalContext<ContextMock>.CurrentContext);
                        }

                        Assert.Equal(c1, GlobalContext<ContextMock>.CurrentContext);
                    }

                    Assert.Null(GlobalContext<ContextMock>.GetCurrentContext(false));
                }
        }

        [Fact]
        public void TestUninitializableCrossThreadDependencyBehaviorN1x0()
        {
            for (int i = 0; i < TestCount; i++)
                lock (SyncRoot)
                {
                    var contextMock = new ContextMock(1);

                    var localSyncObject = new object();

                    using (GlobalContext<ContextMock>.Push(contextMock))
                    {
                        lock (localSyncObject)
                        {
                            new Thread(() =>
                            {
                                lock (localSyncObject)
                                {
                                    Assert.Equal(contextMock, GlobalContext<ContextMock>.CurrentContext);

                                    Monitor.PulseAll(localSyncObject);
                                }
                            }).Start();

                            Monitor.Wait(localSyncObject);
                        }
                    }
                }
        }

        [Fact]
        public void TestUninitializableCrossThreadBehaviorN1x1()
        {
            for (int i = 0; i < TestCount; i++)
                lock (SyncRoot)
                {
                    var c1 = new ContextMock(1);
                    var c2 = new ContextMock(2);

                    var localSyncObject = new object();

                    using (GlobalContext<ContextMock>.Push(c1))
                    {
                        lock (localSyncObject)
                        {
                            new Thread(() =>
                            {
                                lock (localSyncObject)
                                {
                                    using (GlobalContext<ContextMock>.Push(c2))
                                    {
                                        Assert.Equal(c2, GlobalContext<ContextMock>.CurrentContext);
                                    }

                                    Assert.Equal(c1, GlobalContext<ContextMock>.CurrentContext);

                                    Monitor.PulseAll(localSyncObject);
                                }
                            }).Start();

                            Monitor.Wait(localSyncObject);
                        }
                    }
                }
        }

        [Fact]
        public void TestUninitializableInitializationFailure()
        {
            for (int i = 0; i < TestCount; i++)
                lock (SyncRoot)
                {
                    //Fails due to ContextMock not having a ContextInitializerAttribute and lacking a default constructor
                    Assert.Throws<ArgumentException>(() => ThreadContextual<ContextMock>.GetCurrentContext(true));
                }
        }

        [Fact]
        public void TestInitialization()
        {
            Assert.IsType<InitializableContextMock>(GlobalContext<InitializableContextMock>.CurrentContext);
        }

        [Fact]
        public void TestRoutedInitialization()
        {
            Assert.IsType<RoutedInitializerContextMock.First>(GlobalContext<RoutedInitializerContextMock>.CurrentContext);
        }

        [Fact]
        public void TestTwiceRoutedInitialization()
        {
            Assert.IsType<TwiceRoutedInitializerContextMock.Second>(GlobalContext<TwiceRoutedInitializerContextMock>.CurrentContext);
        }
    }
}
