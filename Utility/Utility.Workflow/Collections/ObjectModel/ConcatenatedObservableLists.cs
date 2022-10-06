using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utility.Collections.ObjectModel
{
    public class ConcatenatedObservableLists<T> : IList<T>, INotifyCollectionChanged
    {
        //TODO: add generation

        private readonly List<IList<T>> _innerLists;
        private readonly List<int> _innerListCounts;
        private readonly List<Guid> _listIdentifiers;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ConcatenatedObservableLists()
        {
            _innerLists = new List<IList<T>>();
            _innerListCounts = new List<int>();
            _listIdentifiers = new List<Guid>();
        }

        public IDisposable AddList(IList<T> list) => InsertList(_innerListCounts.Count, list);

        public IDisposable InsertList(int index, IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (index < 0 || index > _innerListCounts.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index does not fall into the range of the current list of concatenated lists.");

            if (!(list is INotifyCollectionChanged ntfy))
                throw new ArgumentException($"The supplied list must implement the {typeof(INotifyCollectionChanged).FullName}-interface.");

            Guid identifier = Guid.NewGuid();

            void handler(object s, NotifyCollectionChangedEventArgs e)
            {
                int listIndex = _listIdentifiers.IndexOf(identifier);

                if (listIndex == -1)
                    throw new InvalidOperationException("Received a CollectionChanged-event from a list that is no longer part of the current collection.");

                InnerList_CollectionChanged(identifier, list, listIndex, e);
            }


            ntfy.CollectionChanged += handler;

            int absoluteIndex = 0;

            for (int i = 0; i < index; i++)
                absoluteIndex += _innerListCounts[i];

            _innerLists.Insert(index, list);
            _innerListCounts.Insert(index, list.Count);
            _listIdentifiers.Insert(index, identifier);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, absoluteIndex));

            return Disposable.Create(() =>
            {
                int listIndex = _listIdentifiers.IndexOf(identifier);

                if (listIndex == -1) //shouldn't even happen
                    return;

                ntfy.CollectionChanged -= handler;

                int absoluteIndex = 0;

                for (int i = 0; i < listIndex; i++)
                    absoluteIndex += _innerListCounts[i];

                _innerListCounts.RemoveAt(listIndex);
                _listIdentifiers.RemoveAt(listIndex);
                _innerLists.RemoveAt(listIndex);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list, absoluteIndex));
            });
        }

        private void InnerList_CollectionChanged(Guid id, IList<T> list, int listIndex, NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            int baseItemIndex = 0;

            for (int i = 0; i < listIndex; i++)
                baseItemIndex += _innerListCounts[i];

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //may be expensive for the callee as most of the items may not be actually reset but we can't change the
                //design decision nor do we want to store all items

                _innerListCounts[listIndex] = list.Count;

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, e.NewStartingIndex + baseItemIndex));
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, e.OldStartingIndex + baseItemIndex));
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.NewItems, e.NewStartingIndex + baseItemIndex, e.OldStartingIndex + baseItemIndex));
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems!, e.OldItems!, e.NewStartingIndex + baseItemIndex));
            }
            //else: we don't care.
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        //TODO: store that value
        public int Count
        {
            get => _innerListCounts.Sum();
        }

        bool ICollection<T>.IsReadOnly => true;

        public T this[int index]
        {
            get
            {
                if (index >= 0)
                {
                    for (int i = 0; i < _innerListCounts.Count; i++)
                    {
                        if (index - _innerListCounts[i] < 0)
                            return _innerLists[i][index];

                        index -= _innerListCounts[i];
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(index), "Index does not fall into the current collection.");
            }
        }

        public bool Contains(T item)
        {
            foreach (var l in _innerLists)
            {
                if (l.Contains(item))
                    return true;
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index does not fall into the range of the array.");
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("The indicated array does not have sufficient capacity to store all of the current instance's items.");

            for (int i = 0; i < _innerListCounts.Count; i++)
            {
                _innerLists[i].CopyTo(array, arrayIndex);
                arrayIndex += _innerListCounts[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _innerLists.SelectMany((s) => s).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            int baseIndex = 0;

            for (int i = 0; i < _innerListCounts.Count; i++)
            {
                int index = _innerLists[i].IndexOf(item);

                if (index >= 0)
                    return baseIndex + index;

                baseIndex += _innerListCounts[i];
            }

            return -1;
        }

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
