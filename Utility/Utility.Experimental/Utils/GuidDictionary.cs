using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class GuidDictionary<TEntry> : IdDictionary<TEntry, Guid>
    {
        public GuidDictionary()
            : this(Guid.NewGuid)
        { }

        public GuidDictionary(Func<Guid> nextGuid)
            : base(nextGuid)
        { }

        public GuidDictionary(Func<Guid> nextGuid, IEqualityComparer<TEntry> comparer)
            : base(nextGuid, comparer)
        { }

        public static GuidDictionary<TEntry> CreateReferenceDictionary()
        {
            return new GuidDictionary<TEntry>(Guid.NewGuid, ReferenceComparer<TEntry>.Default);
        }

        new public static GuidDictionary<TEntry> CreateReferenceDictionary(Func<Guid> nextGuid)
        {
            return new GuidDictionary<TEntry>(nextGuid, ReferenceComparer<TEntry>.Default);
        }
    }
}
