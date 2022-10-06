using System;
using System.Collections.Generic;
using System.Text;
using Utility.Services;
using Xunit;

namespace Utility.Tests.Services
{
    public class GenericServiceProviderTests
    {
        [Fact]
        public void TestSetServiceGetService()
        {
            GenericServiceProvider serviceProvider = new GenericServiceProvider();

            Assert.Null(serviceProvider.GetService(typeof(string)));
            Assert.Null(serviceProvider.GetService(typeof(int)));

            serviceProvider.SetService(typeof(string), "test");

            Assert.Equal("test", serviceProvider.GetService(typeof(string)));
            Assert.Null(serviceProvider.GetService(typeof(int)));

            serviceProvider.SetService(typeof(string), "a");

            Assert.Equal("a", serviceProvider.GetService(typeof(string)));
            Assert.Null(serviceProvider.GetService(typeof(int)));

            serviceProvider.SetService(typeof(int), 123);

            Assert.Equal("a", serviceProvider.GetService(typeof(string)));
            Assert.NotNull(serviceProvider.GetService(typeof(int)));
        }

        [Fact]
        public void TestSetServiceExceptions()
        {
            var serviceProvider = new GenericServiceProvider();

            serviceProvider.SetService(typeof(string), "bla");
            serviceProvider.SetService(typeof(int), 1);

            Assert.Throws<ArgumentException>(() => serviceProvider.SetService(typeof(int), "notAnInteger")); //string is not an int

            serviceProvider.SetService(typeof(int), null); //int is not nullable but setting it to null deletes the corresponding service

            Assert.Throws<ArgumentException>(() => serviceProvider.SetService(typeof(string), 1)); //int is not a string

            Assert.Throws<ArgumentNullException>("serviceType", () => serviceProvider.SetService(null, "test"));

            serviceProvider.SetService(typeof(object), "anObject");
            serviceProvider.SetService(typeof(ValueType), 1);
        }
        [Fact]
        public void TestSetServiceGenericGetService()
        {
            GenericServiceProvider serviceProvider = new GenericServiceProvider();

            Assert.Null(serviceProvider.GetService(typeof(string)));
            Assert.Null(serviceProvider.GetService(typeof(int)));

            serviceProvider.SetService("test");

            Assert.Equal("test", serviceProvider.GetService(typeof(string)));
            Assert.Null(serviceProvider.GetService(typeof(int)));

            serviceProvider.SetService("a");

            Assert.Equal("a", serviceProvider.GetService(typeof(string)));
            Assert.Null(serviceProvider.GetService(typeof(int)));

            serviceProvider.SetService(123);

            Assert.Equal("a", serviceProvider.GetService(typeof(string)));
            Assert.NotNull(serviceProvider.GetService(typeof(int)));
        }

        [Fact]
        public void TestSetServiceGenericExceptions()
        {
            var serviceProvider = new GenericServiceProvider();

            serviceProvider.SetService("bla");
            serviceProvider.SetService(1);

            Assert.Throws<ArgumentException>(() => serviceProvider.SetService(typeof(int), "notAnInteger")); //string is not an int

            Assert.Throws<ArgumentException>(() => serviceProvider.SetService(typeof(string), 1)); //int is not a string

            serviceProvider.SetService((object)"anObject");
            serviceProvider.SetService((ValueType)1);
        }

        [Fact]
        public void TestHasService()
        {
            var serviceProvider = new GenericServiceProvider();

            serviceProvider.SetService(typeof(string), "bla");

            Assert.True(serviceProvider.ContainsService(typeof(string)));
            Assert.False(serviceProvider.ContainsService(typeof(int)));

            serviceProvider.SetService(typeof(int), 1);
        }

        [Fact]
        public void TestDeleteService()
        {
            var serviceProvider = new GenericServiceProvider();

            serviceProvider.SetService(typeof(string), "bla");
            serviceProvider.SetService(typeof(int), 1);

            Assert.True(serviceProvider.ContainsService(typeof(int)));
            Assert.True(serviceProvider.ContainsService(typeof(string)));

            serviceProvider.SetService(typeof(string), null);

            Assert.True(serviceProvider.ContainsService(typeof(int)));
            Assert.False(serviceProvider.ContainsService(typeof(string)));

            serviceProvider.SetService(typeof(int), null);
            serviceProvider.SetService(typeof(string), "bla");

            Assert.False(serviceProvider.ContainsService(typeof(int)));
            Assert.True(serviceProvider.ContainsService(typeof(string)));
        }

        [Fact]
        public void TestGetServiceExceptions()
        {
            var serviceProvider = new GenericServiceProvider();

            Assert.Throws<ArgumentNullException>("serviceType", () => serviceProvider.GetService(null));
        }
    }
}
