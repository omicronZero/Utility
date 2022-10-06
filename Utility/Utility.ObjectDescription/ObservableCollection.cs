using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Utility.Collections;

namespace Utility.ObjectDescription
{
    public class ObservableCollection<T> : Collection<T>, IObservableCollection<T>
    {
        private EventHandler<ObservableCollectionChangedEventArgs<T>> _collectionChangedHandler;

        private NotifyCollectionChangedEventHandler _notifyCollectionChangedHandler;
        private readonly CompositeObserver<T> _itemObserver;
        private readonly Func<T, IObservableObject> _observationProvider;
        private readonly List<IDisposable> _itemObservationHandles;
        private int _observationCounter;

        public ObservableCollection(bool observeItems)
            : this(new List<T>(), observeItems)
        { }

        public ObservableCollection(IList<T> list, bool observeItems)
            : this(list, observeItems ? (v) => v as IObservableObject : (Func<T, IObservableObject>)null)
        { }

        public ObservableCollection(Func<T, IObservableObject> observationProvider)
            : this(new List<T>(), observationProvider)
        { }

        public ObservableCollection(IList<T> list, Func<T, IObservableObject> observationProvider)
            : base(list)
        {
            _collectionChangedHandler += (s, e) => _notifyCollectionChangedHandler(s, e);

            if (observationProvider != null)
            {
                _itemObserver = new CompositeObserver<T>();
                _observationProvider = observationProvider;
                _itemObservationHandles = new List<IDisposable>();
            }
        }

        public event EventHandler<ObservableCollectionChangedEventArgs<T>> CollectionChanged
        {
            add
            {
                if (_observationCounter++ == 0)
                    RegisterItemObservers();

                _collectionChangedHandler += value;
            }
            remove
            {
                if (--_observationCounter == 0)
                    UnregisterItemObservers();

                _collectionChangedHandler -= value;
            }
        }

        public bool ObserveItems => _observationProvider != null;

        private void RegisterItemObservers()
        {
            if (ObserveItems)
            {
                foreach (T v in InternalList)
                {
                    IObservableObject o = _observationProvider(v);

                    if (o == null)
                        _itemObservationHandles.Add(null);
                    else
                        _itemObservationHandles.Add(o.RegisterObserver(_itemObserver));
                }
            }
        }

        private void UnregisterItemObservers()
        {
            if (ObserveItems)
            {
                foreach (IDisposable d in _itemObservationHandles)
                    d?.Dispose();

                _itemObservationHandles.Clear();
            }
        }

        private void OnCollectionChanged(ObservableCollectionChangedEventArgs<T> e)
        {
            _collectionChangedHandler?.Invoke(this, e);
        }

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                _notifyCollectionChangedHandler += value;
            }

            remove
            {
                _notifyCollectionChangedHandler -= value;
            }
        }

        protected override void OnInternalListChanged()
        {
            base.OnInternalListChanged();

            Reset();
        }

        private void Reset()
        {
            OnCollectionChanged(ObservableCollectionChangedEventArgs<T>.Reset());
            if (ObserveItems && _observationCounter > 0)
            {
                UnregisterItemObservers();
                RegisterItemObservers();
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            Reset();
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(ObservableCollectionChangedEventArgs<T>.Inserted(item, index));

            if (ObserveItems && _observationCounter > 0)
                _itemObservationHandles.Insert(index, _observationProvider(item)?.RegisterObserver(_itemObserver));
        }

        protected override void RemoveItem(int index)
        {
            T item = InternalList[index];

            base.RemoveItem(index);

            OnCollectionChanged(ObservableCollectionChangedEventArgs<T>.Removed(item, index));

            if (ObserveItems && _observationCounter > 0)
            {
                _itemObservationHandles[index]?.Dispose();
                _itemObservationHandles.RemoveAt(index);
            }
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = InternalList[index];
            base.SetItem(index, item);

            OnCollectionChanged(ObservableCollectionChangedEventArgs<T>.Replaced(item, oldItem, index));

            if (ObserveItems && _observationCounter > 0)
            {
                _itemObservationHandles[index]?.Dispose();
                _itemObservationHandles[index] = _observationProvider(item)?.RegisterObserver(_itemObserver);
            }
        }
    }
}
