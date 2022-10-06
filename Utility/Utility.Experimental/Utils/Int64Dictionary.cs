using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class Int64Dictionary<TEntry> : IdDictionary<TEntry, long>
    {
        public Int64Dictionary()
            : this(NextLongGenerator())
        { }

        public Int64Dictionary(Func<long> nextId)
            : base(nextId)
        { }

        public Int64Dictionary(Func<long> nextId, IEqualityComparer<TEntry> comparer)
            : base(nextId, comparer)
        { }

        private static Func<long> NextLongGenerator()
        {
            ulong v = 0;

            return () =>
            {
                if (v == ulong.MaxValue)
                    throw new InvalidOperationException("End of sequence reached.");

                return unchecked((long)v++);
            };
        }

        public static Int64Dictionary<TEntry> CreateReferenceDictionary()
        {
            return new Int64Dictionary<TEntry>(NextLongGenerator(), ReferenceComparer<TEntry>.Default);
        }

        new public static Int64Dictionary<TEntry> CreateReferenceDictionary(Func<long> nextId)
        {
            return new Int64Dictionary<TEntry>(nextId, ReferenceComparer<TEntry>.Default);
        }
    }
}
