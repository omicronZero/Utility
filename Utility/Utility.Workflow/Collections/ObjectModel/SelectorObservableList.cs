using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Collections.ObjectModel
{
    public class SelectorObservableList<T, TInner> : IList<T>, INotifyCollectionChanged, IDisposable
    {

        private readonly IList<TInner> _innerList;
        private readonly Func<TInner, T> _selector;

        private List<T> _items;

        private bool _disposed;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SelectorObservableList(IList<TInner> innerList, Func<TInner, T> selector)
        {
            _innerList = innerList ?? throw new ArgumentNullException(nameof(innerList));
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));

            if (!(innerList is INotifyCollectionChanged ntfy))
                throw new ArgumentException($"Inner list must implement the {typeof(INotifyCollectionChanged).FullName}-interface for observation.");

            _items = new List<T>();

            foreach (var item in innerList)
                _items.Add(_selector(item));

            ntfy.CollectionChanged += InnerList_CollectionChanged;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ((INotifyCollectionChanged)_innerList).CollectionChanged -= InnerList_CollectionChanged;
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void InnerList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var oldItems = e.OldItems;
            var newItems = e.NewItems;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //we don't reset, we remove and add to keep track of the items

                //we swap the old item list with a new instance
                var outerOldItems = _items;

                _items = new List<T>();

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 0, outerOldItems.AsReadOnly()));

                foreach (var item in _innerList)
                {
                    _items.Add(_selector(item));
                }

                if (_items.Count > 0)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 0, _items.AsReadOnly()));
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (newItems == null)
                    throw new ArgumentException("NewItems-collection not set to an instance.");

                var items = newItems.Cast<TInner>().Select(_selector).ToArray();

                _items.InsertRange(e.NewStartingIndex, items);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Array.AsReadOnly(items), e.NewStartingIndex));
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (oldItems == null)
                    throw new ArgumentException("OldItems-collection not set to an instance.");

                T[] items = new T[oldItems.Count];

                _items.CopyTo(e.OldStartingIndex, items, 0, oldItems.Count);
                _items.RemoveRange(e.OldStartingIndex, oldItems.Count);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Array.AsReadOnly(items), e.OldStartingIndex));
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                if (newItems == null)
                    throw new ArgumentException("NewItems-collection not set to an instance.");

                T[] items = new T[newItems.Count];

                _items.CopyTo(e.NewStartingIndex, items, 0, newItems.Count);
                _items.RemoveRange(e.OldStartingIndex, newItems.Count);
                _items.InsertRange(e.NewStartingIndex, items);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, Array.AsReadOnly(items), e.NewStartingIndex, e.OldStartingIndex));
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (oldItems == null)
                    throw new ArgumentException("OldItems-collection not set to an instance.");
                if (newItems == null)
                    throw new ArgumentException("NewItems-collection not set to an instance.");

                var items = newItems.Cast<TInner>().Select(_selector).ToArray();
                var oldOuterItems = new T[items.Length];

                int baseIndex = e.NewStartingIndex;

                for (int i = 0; i < items.Length; i++)
                {
                    oldOuterItems[i] = items[baseIndex + i];
                    items[baseIndex + i] = items[i];
                }

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, Array.AsReadOnly(items), Array.AsReadOnly(oldOuterItems), e.NewStartingIndex));
            }
            //else: we don't care.
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public int Count => _items.Count;

        bool ICollection<T>.IsReadOnly => true;

        public T this[int index] => _items[index];

        public bool Contains(T item) => _items.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        public int IndexOf(T item) => _items.IndexOf(item);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw CollectionReadOnly();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw CollectionReadOnly();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw CollectionReadOnly();
        }

        void ICollection<T>.Add(T item)
        {
            throw CollectionReadOnly();
        }

        void ICollection<T>.Clear()
        {
            throw CollectionReadOnly();
        }

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw CollectionReadOnly();
        }

        private static Exception CollectionReadOnly() => new InvalidOperationException("The current instance is read-only.");
    }
}
