using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Utility.Collections;

namespace Utility
{
    public class ObserverCollectionAdapter<T> : IList<T>, INotifyCollectionChanged, IBindingList, IDisposable
    {
        private readonly IList<T> _list;
        private readonly bool _forcedReadonly;
        private List<T> _copy;
        private WeakReference<List<T>> _copyWeakReference;

        private bool _disposed;

        private ListChangedEventHandler _listChanged;
        private NotifyCollectionChangedEventHandler _collectionChanged;

        public static bool Supports(Type collectionType)
        {
            if (collectionType == null)
                throw new ArgumentNullException(nameof(collectionType));

            return typeof(INotifyCollectionChanged).IsAssignableFrom(collectionType)
                || typeof(IBindingList).IsAssignableFrom(collectionType);
        }

        private List<T> Copy
        {
            get
            {
                if (_copy != null)
                    return _copy;

                if (_copyWeakReference == null)
                    return null;

                List<T> c;
                return _copyWeakReference.TryGetTarget(out c) ? c : null;
            }
        }

        private IList<T> List
        {
            get
            {
                IList<T> l = _list;

                if (_disposed)
                    throw new ObjectDisposedException(this.GetType().Name);

                return l;
            }
        }

        event ListChangedEventHandler IBindingList.ListChanged
        {
            add
            {
                if (_listChanged == null && _collectionChanged == null && value != null)
                    BeginObservation();

                if (value != null && _copy == null)
                {
                    if (!_copyWeakReference.TryGetTarget(out _copy))
                    {
                        _copy = new List<T>(_list);
                        _copyWeakReference = new WeakReference<List<T>>(_copy);
                    }
                }

                _listChanged += value;
            }

            remove
            {
                _listChanged -= value;

                if (_listChanged == null && _collectionChanged == null)
                    EndObservation();

                if (_listChanged == null)
                {
                    _copy = null;
                }
            }
        }

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                if (_listChanged == null && _collectionChanged == null && value != null)
                    BeginObservation();

                _collectionChanged += value;
            }

            remove
            {
                _collectionChanged -= value;

                if (_listChanged == null && _collectionChanged == null)
                    EndObservation();
            }
        }

        public ObserverCollectionAdapter(IList<T> list, bool forcedReadonly)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (!(list is INotifyCollectionChanged) && !(list is IBindingList))
                throw new ArgumentException("The specified list does not offer a supported collection change.", nameof(list));

            _list = list;
            _forcedReadonly = forcedReadonly;
        }

        private void BeginObservation()
        {
            if (List is INotifyCollectionChanged ncc)
            {
                ncc.CollectionChanged += INotifyCollectionChanged_CollectionChanged;
            }
            else if (List is IBindingList bl)
            {
                bl.ListChanged += BindingList_ListChanged;
            }
        }

        private void EndObservation()
        {
            if (List is INotifyCollectionChanged ncc)
            {
                ncc.CollectionChanged -= INotifyCollectionChanged_CollectionChanged;
            }
            else if (List is IBindingList bl)
            {
                bl.ListChanged -= BindingList_ListChanged;
            }
        }

        private void BindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            _listChanged?.Invoke(this, e);

            NotifyCollectionChangedEventHandler notifyCollectionEvent = _collectionChanged;

            if (notifyCollectionEvent != null)
            {
                NotifyCollectionChangedEventArgs nccargs;

                var listCopy = Copy;

                if (e.ListChangedType == ListChangedType.ItemAdded)
                {
                    T v = List[e.NewIndex];
                    listCopy?.Add(v);
                    nccargs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewIndex, v);
                }
                else if (e.ListChangedType == ListChangedType.ItemChanged)
                {
                    T ov = listCopy[e.NewIndex];
                    T v = List[e.NewIndex];

                    if (listCopy != null)
                        listCopy[e.NewIndex] = v;

                    nccargs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, v, ov, e.NewIndex);
                }
                else if (e.ListChangedType == ListChangedType.ItemDeleted)
                {
                    T v = List[e.NewIndex];
                    listCopy?.RemoveAt(e.NewIndex);
                    nccargs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, v, e.NewIndex);
                }
                else if (e.ListChangedType == ListChangedType.ItemMoved)
                {
                    T v = listCopy[e.OldIndex];

                    if (listCopy != null)
                    {
                        listCopy.RemoveAt(e.OldIndex);
                        listCopy.Insert(e.NewIndex, v);
                    }

                    nccargs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.NewIndex, e.OldIndex);
                }
                else if (e.ListChangedType == ListChangedType.Reset)
                {
                    if (listCopy != null)
                    {
                        _copy = new List<T>();
                        _copyWeakReference = new WeakReference<List<T>>(_copy);
                    }
                    nccargs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, listCopy == null ? List.ToArray() : (IList)listCopy);
                }
                else
                    return;

                notifyCollectionEvent(this, nccargs);
            }
        }

        private void INotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _collectionChanged?.Invoke(this, e);

            ListChangedEventHandler bindingListEvent = _listChanged;

            if (bindingListEvent != null)
            {
                var copy = Copy;

                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    int c = e.NewStartingIndex + e.NewItems.Count;

                    copy?.InsertRange(e.NewStartingIndex, new DelegateList<T>((i) => (T)e.NewItems[i], () => e.NewItems.Count, false));

                    for (int i = e.NewStartingIndex; i < c; i++)
                        bindingListEvent(this, new ListChangedEventArgs(ListChangedType.ItemAdded, i));
                }
                else if (e.Action == NotifyCollectionChangedAction.Move)
                {
                    copy?.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
                    copy?.InsertRange(e.NewStartingIndex, new DelegateList<T>((i) => (T)e.NewItems[i], () => e.NewItems.Count, false));

                    int c = e.OldItems.Count;
                    int oind = e.OldStartingIndex;
                    int nind = e.NewStartingIndex;

                    for (int i = 0; i < c; i++)
                        bindingListEvent(this, new ListChangedEventArgs(ListChangedType.ItemMoved, nind + i, oind + i));
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    copy?.RemoveRange(e.NewStartingIndex, e.NewItems.Count);

                    int c = e.NewStartingIndex + e.NewItems.Count;

                    for (int i = e.NewStartingIndex; i < c; i++)
                        bindingListEvent(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, i));
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    copy?.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
                    copy?.InsertRange(e.NewStartingIndex, new DelegateList<T>((i) => (T)e.NewItems[i], () => e.NewItems.Count, false));

                    int c = e.OldItems.Count;
                    int oind = e.OldStartingIndex;
                    int nind = e.NewStartingIndex;

                    for (int i = 0; i < c; i++)
                        bindingListEvent(this, new ListChangedEventArgs(ListChangedType.ItemMoved, nind + i, oind + i));
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    copy.Clear();
                    copy.AddRange(e.NewItems.Cast<T>());

                    bindingListEvent(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
                else
                    return;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_listChanged != null || _collectionChanged != null)
                    EndObservation();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void ThrowReadonly()
        {
            if (IsReadOnly)
                throw new NotSupportedException("The collection is read-only.");
        }

        #region IList<T>
        public T this[int index]
        {
            get => List[index];
            set
            {
                ThrowReadonly();

                List[index] = value;
            }
        }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => _forcedReadonly || List.IsReadOnly;

        public void Add(T item)
        {
            ThrowReadonly();

            List.Add(item);
        }

        public void Clear()
        {
            ThrowReadonly();

            List.Clear();
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ThrowReadonly();

            List.Insert(index, item);
        }

        public bool Remove(T item)
        {
            ThrowReadonly();

            return List.Remove(item);
        }

        public void RemoveAt(int index)
        {
            ThrowReadonly();

            List.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IBindingList
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = ValidateCast(value);
        }

        private static T ValidateCast(object value)
        {
            return value is T v ? v : throw new ArgumentException($"Expected value of type{typeof(T).Name}.");
        }

        bool IBindingList.AllowEdit => !IsReadOnly;

        bool IBindingList.AllowNew => false;

        bool IBindingList.AllowRemove => !IsReadOnly;

        bool IBindingList.IsSorted => false;

        ListSortDirection IBindingList.SortDirection => throw new NotSupportedException();

        PropertyDescriptor IBindingList.SortProperty => null;

        bool IBindingList.SupportsChangeNotification => false;

        bool IBindingList.SupportsSearching => false;

        bool IBindingList.SupportsSorting => false;

        bool IList.IsFixedSize => List.IsFixedSize();

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        int IList.Add(object value)
        {
            T v = ValidateCast(value);
            Add(v);
            //implemented as in System.Collections.ObjectModel.Collection<T>
            return List.Count - 1;
        }

        void IBindingList.AddIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        object IBindingList.AddNew()
        {
            throw new NotSupportedException();
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value)
        {
            return value is T v && Contains(v);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (!(array is T[] arr))
                throw new ArgumentException($"Expected one-dimensional array of type {typeof(T).FullName}.");

            CopyTo(arr, index);
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        int IList.IndexOf(object value)
        {
            return value is T v ? IndexOf(v) : -1;
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, ValidateCast(value));
        }

        void IList.Remove(object value)
        {
            Remove(ValidateCast(value));
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        void IBindingList.RemoveSort()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
