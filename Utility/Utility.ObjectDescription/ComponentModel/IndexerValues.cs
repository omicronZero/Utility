using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription.ComponentModel
{
    public struct IndexerValues
    {
        public Indexer Indexer { get; }
        private readonly object _underlyingInstance;

        public object this[params object[] index]
        {
            get
            {
                if (index == null)
                    throw new ArgumentNullException(nameof(index));

                return Indexer.GetMethod.Invoke(_underlyingInstance, index);
            }
            set
            {
                if (index == null)
                    throw new ArgumentNullException(nameof(index));

                if (Indexer.SetMethod == null)
                    throw new NotSupportedException("The indexer does not support writing.");

                Indexer.SetMethod.Invoke(_underlyingInstance, index, value);
            }
        }

        public IndexerValues(Indexer indexer, object underlyingInstance)
        {
            if (indexer == null)
                throw new ArgumentNullException(nameof(indexer));
            if (underlyingInstance == null)
                throw new ArgumentNullException(nameof(underlyingInstance));

            Indexer = indexer;
            _underlyingInstance = underlyingInstance;
        }
    }
}
