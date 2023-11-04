//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Text;

//namespace Utility.Collections.ObjectModel
//{
//    public class ObservableCollectionToList<T> : INotifyCollectionChanged, IDisposable
//    {
//        private readonly IEnumerable<T> _underlyingEnumerable;

//        private readonly Dictionary<Id, T> _idMapping;

//        private bool _disposed;

//        private NotifyCollectionChangedEventHandler _collectionChangedEvent;

//        private Node _root;

//        private Id _currentId;

//        //TODO: use GUID?

//        public ObservableCollectionToList(IEnumerable<T> underlyingEnumerable)
//        {
//            _underlyingEnumerable = underlyingEnumerable ?? throw new ArgumentNullException(nameof(underlyingEnumerable));

//            if (!(underlyingEnumerable is INotifyCollectionChanged notify))
//                throw new ArgumentException($"Notifying collection of type {typeof(INotifyCollectionChanged).FullName} expected.", nameof(underlyingEnumerable));

//            notify.CollectionChanged += Notify_CollectionChanged;
//            _idMapping = new Dictionary<T, Id>();

//            Reset();
//        }

//        public event NotifyCollectionChangedEventHandler CollectionChanged
//        {
//            add
//            {
//                if (_disposed)
//                    throw new ObjectDisposedException(this.GetType().Name);

//                _collectionChangedEvent += value;
//            }
//            remove
//            {
//                if (_disposed)
//                    throw new ObjectDisposedException(this.GetType().Name);

//                _collectionChangedEvent -= value;
//            }
//        }

//        private Id NextId(T item) {
//            var id = _currentId++;

//            _idMapping.Add(id, item);

//            return id;
//        }

//        protected void Reset()
//        {
//            _root = CreateRootNode();

//            _idMapping.Clear();
//            _currentId = 0;

//            foreach (var item in _underlyingEnumerable)
//            {
//                _root.Insert(NextId(item), item);
//            }
//        }

//        protected Node CreateRootNode()
//        {
//            return new HashCodeComparableNode(32);
//        }

//        private void Notify_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            throw new NotImplementedException();
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (!_disposed)
//                {
//                    _collectionChangedEvent -= Notify_CollectionChanged;
//                    _root = null;
//                    _disposed = true;
//                }
//            }
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        protected abstract class Node
//        {
//            public abstract int Count { get; }
//            public abstract int Insert(Id value);
//            public abstract int GetIndex(Id value);
//            public abstract int Remove(Id value);
//            public abstract int Replace(Id oldValue, Id newValue);
//        }

//        protected sealed class Leaf : Node
//        {
//            public Id Value { get; private set; }

//            public Leaf(Id value)
//            {
//                Value = value;
//            }

//            public override int Count => 1;
//        }

//        protected class ComparableNode
//        {
//        }
//    }
//}
