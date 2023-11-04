using System;
using System.Collections.Generic;
using System.Text;
using Utility.Collections.Adapters;
using Utility.Collections.Tools;

namespace Utility.Collections
{
    public abstract class ReadOnlyListBase<T> : ListBase<T>
    {
        protected abstract T GetItem(int index);

        public sealed override T this[int index]
        {
            get => GetItem(index);
            set => throw CollectionExceptions.ReadOnlyException();
        }

        public sealed override bool IsReadOnly => true;

        public sealed override void Clear()
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public sealed override void Insert(int index, T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public sealed override void RemoveAt(int index)
        {
            throw CollectionExceptions.ReadOnlyException();
        }
    }
}
