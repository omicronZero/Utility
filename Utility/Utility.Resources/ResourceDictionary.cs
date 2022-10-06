using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Utility.Data.Resources
{
    public class ResourceDictionary<TID, TResourceType> : IResourceDictionary<TID, TResourceType>
        where TResourceType : IResource
    {
        private readonly ConcurrentDictionary<TID, Result<TResourceType>> _dictionary;
        private readonly Func<TID, Result<TResourceType>> _factory;

        public ResourceDictionary(Func<TID, Result<TResourceType>> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _dictionary = new ConcurrentDictionary<TID, Result<TResourceType>>();
            _factory = factory;
        }

        public bool GetResource(TID id, out TResourceType resource)
        {
            return _dictionary.GetOrAdd(id, _factory).TryGetResult(out resource);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public static IResourceDictionary<TID, TResourceType> FromDelegate(Func<TID, Result<TResourceType>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return new DelegateDictionary(handler);
        }

        public static IResourceDictionary<TID, TResourceType> FromDictionary(IDictionary<TID, TResourceType> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return FromDelegate((id) =>
            {
                TResourceType r;

                if (!dictionary.TryGetValue(id, out r))
                    return Result<TResourceType>.Failed;

                return r;
            });
        }

        public static IResourceDictionary<TID, TResourceType> Group(IEnumerable<IResourceDictionary<TID, TResourceType>> dictionaries)
        {
            if (dictionaries == null)
                throw new ArgumentNullException(nameof(dictionaries));

            return FromDelegate((id) =>
            {
                foreach (IResourceDictionary<TID, TResourceType> dictionary in dictionaries)
                {
                    if (dictionary == null)
                        continue;

                    TResourceType resource;

                    if (dictionary.GetResource(id, out resource))
                        return resource;
                }
                return Result<TResourceType>.Failed;
            });
        }

        private sealed class DelegateDictionary : IResourceDictionary<TID, TResourceType>
        {
            private readonly Func<TID, Result<TResourceType>> _handler;

            public DelegateDictionary(Func<TID, Result<TResourceType>> handler)
            {
                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                _handler = handler;
            }

            public bool GetResource(TID id, out TResourceType resource)
            {
                return _handler(id).TryGetResult(out resource);
            }
        }
    }
}
