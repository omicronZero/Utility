using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Utility.Services
{
    public static class ServiceProvider
    {
        public static IServiceProvider AsReadOnly(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (serviceProvider is ReadOnlyServiceProvider)
                return serviceProvider;

            return new ReadOnlyServiceProvider(serviceProvider);
        }

        /// <summary>
        /// Returns a service of the specified type or, if unavailable, null if <typeparamref name="T"/> is a reference type.
        /// For unavailable value types, the method throws an exception. Rather use <see cref="TryGetService{T}(IServiceProvider, out T)"/>
        /// for value types.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <param name="serviceProvider">The service provider on which to retrieve the type.</param>
        /// <returns>The retrieved service.</returns>
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            T service;

            if (!TryGetService(serviceProvider, out service))
            {
                service = default; //set to null or default, if no service could be retrieved

                if (service != null) //for value types, default will not be null.
                    throw new ArgumentException("A service could not be retrieved for the specified value type.", nameof(T));
            }

            return service;
        }

        public static bool TryGetService<T>(this IServiceProvider serviceProvider, out T service)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            object serviceObj = serviceProvider.GetService(typeof(T));

            //we do not punish bad service provider behavior and simply return null in case
            //serviceProvider.GetService(typeof(B)) returns an object of type A that is not
            //assignable to B.
            if (!(serviceObj is T s))
            {
                service = default;
                return false;
            }

            service = s;
            return true;
        }

        public static Result<T> GetServiceResult<T>(this IServiceProvider serviceProvider)
        {
            T service;

            if (TryGetService(serviceProvider, out service))
                return service;
            else
                return Result<T>.Failed;
        }

        public static IServiceProvider Create(Func<Type, object> serviceSelector)
        {
            if (serviceSelector == null)
                throw new ArgumentNullException(nameof(serviceSelector));

            return new DelegateServiceProvider(serviceSelector);
        }

        public static IServiceProvider Combine(IEnumerable<IServiceProvider> serviceProviders)
        {
            if (serviceProviders == null)
                throw new ArgumentNullException(nameof(serviceProviders));

            return new DelegateServiceProvider((type) =>
            {
                foreach (var serviceProvider in serviceProviders)
                {
                    object service = serviceProvider.GetService(type);

                    if (service != null)
                        return service;
                }

                return null;
            });
        }

        public static IServiceProvider Inherit(IServiceProvider baseProvider, IServiceProvider instanceProvider)
        {
            if (instanceProvider == null)
                throw new ArgumentNullException(nameof(instanceProvider));

            if (baseProvider == null)
                return instanceProvider;

            return new DelegateServiceProvider((t) =>
            {
                object s = instanceProvider.GetService(t);

                if (s == null)
                    s = baseProvider.GetService(t);

                return s;
            });
        }

        private sealed class DelegateServiceProvider : IServiceProvider
        {
            private readonly ConcurrentDictionary<Type, object> _services;
            private readonly Func<Type, object> _serviceProvider;

            public DelegateServiceProvider(Func<Type, object> serviceProvider)
            {
                if (serviceProvider == null)
                    throw new ArgumentNullException(nameof(serviceProvider));

                _services = new ConcurrentDictionary<Type, object>(ReferenceComparer<Type>.Default);
                _serviceProvider = serviceProvider;
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == null)
                    throw new ArgumentNullException(nameof(serviceType));

                return _services.GetOrAdd(serviceType, _serviceProvider);
            }
        }

        private sealed class ReadOnlyServiceProvider : IServiceProvider
        {
            private readonly IServiceProvider _underlyingServiceProvider;

            public ReadOnlyServiceProvider(IServiceProvider underlyingServiceProvider)
            {
                _underlyingServiceProvider = underlyingServiceProvider ?? throw new ArgumentNullException(nameof(underlyingServiceProvider));
            }

            public object GetService(Type serviceType)
            {
                return _underlyingServiceProvider.GetService(serviceType);
            }
        }
    }
}
