using System;
using System.Collections.Generic;
using System.Text;
using Utility.Contexts;
using Xunit;

namespace Utility.Contexts.Tests
{
    public class ContextInitializerTests
    {

        [Fact]
        public void TestGetInitializerInitialization()
        {
            Type propagate;
            ContextInitializer initializer;

            initializer = ContextInitializer.GetInitializer(typeof(InitializableContextMock), out propagate);

            Assert.NotNull(initializer);
            Assert.Null(propagate);
        }

        [Fact]
        public void TestGetInitializerRouting()
        {
            Type propagate;
            ContextInitializer initializer;

            initializer = ContextInitializer.GetInitializer(typeof(RoutedInitializerContextMock), out propagate);

            Assert.Null(initializer);
            Assert.NotNull(propagate);
        }

        [Fact]
        public void TestGetInitializerNullReference()
        {
            Assert.Throws<ArgumentNullException>(() => ContextInitializer.GetInitializer(null, out _));
        }

        [Fact]
        public void TestCreateDefaultNullReference()
        {
            Assert.Throws<ArgumentNullException>(() => ContextInitializer.CreateDefault(null));
        }

        [Fact]
        public void TestCreateDefaultDirect()
        {
            Assert.NotNull(ContextInitializer.CreateDefault<InitializableContextMock>());
        }

        [Fact]
        public void TestCreateDefaultRouted()
        {
            Assert.NotNull(ContextInitializer.CreateDefault<RoutedInitializerContextMock>());
        }
    }
}
