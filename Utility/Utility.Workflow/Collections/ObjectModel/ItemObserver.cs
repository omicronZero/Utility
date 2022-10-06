using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Collections.ObjectModel
{
    //TODO: move to CollectionExtensions or NotifyCollectionChangedExtensions
    public sealed class ItemObserver<T> : IDisposable
    {
        private readonly Action<T> _itemInserted;
        private readonly Action<T> _itemRemoved;
        private readonly Action _itemsReset;

        private readonly INotifyCollectionChanged _collection;

        private bool _disposed;

        private ItemObserver(
            INotifyCollectionChanged collection,
            Action<T> itemInserted,
            Action<T> itemRemoved,
            Action itemsReset)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (!(collection is IEnumerable))
                throw new ArgumentException("The collection must be enumerable.", nameof(collection));

            _itemInserted = itemInserted;
            _itemRemoved = itemRemoved;
            _itemsReset = itemsReset;

            _collection = collection;

            collection.CollectionChanged += Collection_CollectionChanged;
        }

        public static ItemObserver<T> Register(
            INotifyCollectionChanged collection,
            Action<T> itemInserted,
            Action<T> itemRemoved,
            Action itemsReset)
        {
            return new ItemObserver<T>(collection, itemInserted, itemRemoved, itemsReset);
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<T> oldItems = null;
            IEnumerable<T> newItems = null;

            if (e.Action == NotifyCollectionChangedAction.Reset)
                _itemsReset?.Invoke();

            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                newItems = e.NewItems?.OfType<T>();
            }

            if (e.Action == NotifyCollectionChangedAction.Remove
                 || e.Action == NotifyCollectionChangedAction.Replace)
            {
                oldItems = e.OldItems?.OfType<T>();
            }

            if (_itemRemoved != null && oldItems != null)
            {
                foreach (var item in oldItems)
                    _itemRemoved(item);
            }

            if (_itemInserted != null && newItems != null)
            {
                foreach (var item in newItems)
                    _itemInserted(item);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                _collection.CollectionChanged -= Collection_CollectionChanged;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
