using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Services
{
    public class GenericServiceProvider : IServiceProvider
    {
        private readonly ConcurrentDictionary<Type, object> _services;

        public GenericServiceProvider()
        {
            _services = new ConcurrentDictionary<Type, object>();
        }

        public void SetService(Type serviceType, object instance)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (instance == null)
                _services.TryRemove(serviceType, out _);
            else
            {
                if (!serviceType.IsInstanceOfType(instance))
                    throw new ArgumentException("The specified instance is not an instance of the service type.");

                _services[serviceType] = instance;
            }
        }

        public void SetService<T>(T instance) => SetService(typeof(T), instance);

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            object service;

            if (!_services.TryGetValue(serviceType, out service))
                service = null;

            return service;
        }

        public bool ContainsService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            return _services.ContainsKey(serviceType);
        }
    }
}
