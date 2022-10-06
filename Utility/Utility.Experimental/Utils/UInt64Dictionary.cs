using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class UInt64Dictionary<TEntry> : IdDictionary<TEntry, ulong>
    {
        public UInt64Dictionary()
            : this(NextLongGenerator())
        { }

        public UInt64Dictionary(Func<ulong> nextId)
            : base(nextId)
        { }

        public UInt64Dictionary(Func<ulong> nextId, IEqualityComparer<TEntry> comparer)
            : base(nextId, comparer)
        { }

        private static Func<ulong> NextLongGenerator()
        {
            ulong v = 0;

            return () =>
            {
                if (v == ulong.MaxValue)
                    throw new InvalidOperationException("End of sequence reached.");

                return v++;
            };
        }

        public static UInt64Dictionary<TEntry> CreateReferenceDictionary()
        {
            return new UInt64Dictionary<TEntry>(NextLongGenerator(), ReferenceComparer<TEntry>.Default);
        }

        new public static UInt64Dictionary<TEntry> CreateReferenceDictionary(Func<ulong> nextId)
        {
            return new UInt64Dictionary<TEntry>(nextId, ReferenceComparer<TEntry>.Default);
        }
    }
}
