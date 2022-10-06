using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Utility.Collections;

namespace Utility.ObjectDescription.ObservableQuery
{
    public class ObservableQueryList<T> : ReadOnlyListBase<T>, IObservableList<T>, IDisposable, IQueryResult
    {
        private readonly IObservationListQuery<T> _query;

        private readonly List<T> _innerList;

        private bool _disposed;

        private EventHandler<ObservableCollectionChangedEventArgs<T>> _collectionChangedHandler;
        private NotifyCollectionChangedEventHandler _notifyCollectionChangedHandler;

        public event EventHandler<ObservableCollectionChangedEventArgs<T>> CollectionChanged
        {
            add
            {
                if (_disposed)
                    throw new ObjectDisposedException(this.GetType().Name);

                _collectionChangedHandler += value;
            }
            remove
            {
                if (_disposed)
                    throw new ObjectDisposedException(this.GetType().Name);

                _collectionChangedHandler -= value;
            }
        }

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                if (_disposed)
                    throw new ObjectDisposedException(this.GetType().Name);

                NotifyCollectionChangedEventHandler handler = _notifyCollectionChangedHandler;

                _notifyCollectionChangedHandler += value;

                if (handler == null && _notifyCollectionChangedHandler != null)
                    CollectionChanged += ObservableQueryList_CollectionChanged;
            }

            remove
            {
                if (_disposed)
                    throw new ObjectDisposedException(this.GetType().Name);

                NotifyCollectionChangedEventHandler handler = _notifyCollectionChangedHandler;

                _notifyCollectionChangedHandler -= value;

                if (handler != null && _notifyCollectionChangedHandler == null)
                    CollectionChanged -= ObservableQueryList_CollectionChanged;
            }
        }

        public ObservableQueryList(IObservationListQuery<T> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            _query = query;
            _innerList = new List<T>();
            _query.CollectionChanged += _query_CollectionChanged;

            Reset();
        }

        private void _query_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                InsertItems(e.NewStartingIndex, e.NewItems.CastList<T>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                MoveItems(e.NewStartingIndex, e.NewItems.CastList<T>(), e.OldStartingIndex);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                RemoveItems(e.NewStartingIndex, e.OldItems.CastList<T>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                ReplaceItems(e.NewStartingIndex, e.NewItems.CastList<T>(), e.OldItems.CastList<T>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ResetItems();
            }
            _collectionChangedHandler?.Invoke(this, ObservableCollectionChangedEventArgs<T>.From(e));
        }

        private void ResetItems()
        {
            _innerList.Clear();
            _innerList.AddRange(_query.GetItems());
        }

        private void ReplaceItems(int newStartingIndex, IList<T> newItems, IList<T> oldItems)
        {
            int nc = newItems.Count;
            int oc = oldItems.Count;

            int c = Math.Min(nc, oc);

            for (int i = 0; i < c; i++)
            {
                _innerList[newStartingIndex + i] = newItems[i];
            }

            //should not occur but let's treat it anyways
            if (nc < oc) //items have been removed
            {
                _innerList.RemoveRange(newStartingIndex + c, oc - nc);
            }
            else if (oc < nc) //additional items have been inserted
            {
                _innerList.InsertRange(newStartingIndex + c, System.Linq.Enumerable.Skip(newItems, c));
            }
        }

        private void RemoveItems(int newStartingIndex, IList<T> oldItems)
        {
            _innerList.RemoveRange(newStartingIndex, oldItems.Count);
        }

        private void MoveItems(int newStartingIndex, IList<T> items, int oldStartingIndex)
        {
            throw new NotImplementedException();
        }

        private void InsertItems(int newStartingIndex, IList<T> items)
        {
            throw new NotImplementedException();
        }

        protected override T GetItem(int index)
        {
            return _innerList[index];
        }

        public override int Count
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(this.GetType().Name);

                return _innerList.Count;
            }
        }

        private void ObservableQueryList_CollectionChanged(object sender, ObservableCollectionChangedEventArgs<T> e)
        {
            _notifyCollectionChangedHandler?.Invoke(sender, e);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        public override int IndexOf(T item)
        {
            if (_disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            return _innerList.IndexOf(item);
        }

        public override bool Contains(T item)
        {
            if (_disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            return _innerList.Contains(item);
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            if (_disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            base.CopyTo(array, arrayIndex);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                _collectionChangedHandler = null;
                _notifyCollectionChangedHandler = null;

                _innerList.Clear();
                _query.CollectionChanged -= _query_CollectionChanged;
            }
        }

        public void Reset()
        {
            ResetItems();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
