using System.Collections.Generic;
using System.Text;
using Utility.Collections;
using Utility.Collections.Tools;

namespace Utility.Collections
{
    public abstract class ReadOnlyCollectionBase<T> : CollectionBase<T>
    {
        public sealed override bool IsReadOnly => true;

        public sealed override void Add(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public sealed override void Clear()
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public sealed override bool Remove(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }
    }
}
