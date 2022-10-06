using System;
using System.Collections.Generic;
using System.Text;
using Utility.Services;
using Xunit;

namespace Utility.Tests.Services
{
    public class ServiceProviderTests
    {
        [Fact]
        public void TestCreate()
        {
            Dictionary<Type, object> services = new Dictionary<Type, object>();

            services.Add(typeof(string), string.Empty);
            services.Add(typeof(int), 1);

            object getService(Type type)
            {
                object result;
                if (!services.TryGetValue(type, out result))
                    result = null;

                return result;
            }

            Type[] queries = new Type[] { typeof(string), typeof(int), typeof(object) };

            var serviceProvider = ServiceProvider.Create(getService);

            foreach (Type query in queries)
                Assert.Equal(getService(query), serviceProvider.GetService(query));
        }

        [Fact]
        public void TestGetService()
        {
            var provider = new GenericServiceProvider();

            provider.SetService(1);
            provider.SetService("test");

            Assert.Equal(1, provider.GetService<int>());
        }

        [Fact]
        public void TestCombine()
        {
            List<IServiceProvider> providers = new List<IServiceProvider>();

            for (int i = 0; i < 10; i++)
            {
                var p = new GenericServiceProvider();

                p.SetService(i);

                if ((i + 2) % 3 == 0) //only adds it to 1, 4, 7...
                    p.SetService(i.ToString());

                providers.Add(p);
            }

            IServiceProvider combined = ServiceProvider.Combine(providers);

            Assert.Equal(0, combined.GetService(typeof(int)));
            Assert.Equal("1", combined.GetService(typeof(string)));
            Assert.Null(combined.GetService(typeof(object)));
        }

        [Fact]
        public void TestTryGetService()
        {
            var provider = new GenericServiceProvider();

            provider.SetService(1);
            provider.SetService("test");

            int serviceInt;
            bool hasService = provider.TryGetService(out serviceInt);

            Assert.True(hasService);
            Assert.Equal(1, serviceInt);

            string serviceString;
            hasService = provider.TryGetService(out serviceString);

            Assert.True(hasService);
            Assert.Equal("test", serviceString);

            object serviceObject = "will be set to null";

            hasService = provider.TryGetService(out serviceObject);

            Assert.False(hasService);
            Assert.Null(serviceObject);
        }

        [Fact]
        public void TestReadOnly()
        {
            var provider = new GenericServiceProvider();

            provider.SetService(1);
            provider.SetService("test");

            var readOnly = provider.AsReadOnly();

            Assert.Equal(1, readOnly.GetService<int>());
            Assert.Equal("test", readOnly.GetService<string>());

            Assert.Null(readOnly.GetService<object>());

            Assert.True(object.ReferenceEquals(readOnly, readOnly.AsReadOnly()));
        }

        [Fact]
        public void TestInherit()
        {
            var baseProvider = new GenericServiceProvider();

            baseProvider.SetService(0);
            baseProvider.SetService("base");

            var child = new GenericServiceProvider();

            child.SetService("child");

            Assert.True(object.ReferenceEquals(child, ServiceProvider.Inherit(null, child)));

            Assert.Throws<ArgumentNullException>("instanceProvider", () => ServiceProvider.Inherit(baseProvider, null));

            var inherited = ServiceProvider.Inherit(baseProvider, child);

            Assert.Equal(0, inherited.GetService<int>());
            Assert.Equal("child", inherited.GetService<string>());
            Assert.Null(inherited.GetService<object>());
        }
    }
}
