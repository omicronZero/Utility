using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Utility.Collections;
using Utility.Collections.Tools;
using Utility.Reflection;
using Utility.Workflow.Collections.Adapters;

namespace Utility
{
    public static class EnumerableExtensions
    {
        public static IList<T> AsReadonlyList<T>(this IList<T> instance)
        {
            if (instance == null)
                return null;

            if (instance is ReadOnlyList<T>) //captures also derived types
                return instance;

            if (instance is ISetCollection<T>)
                return new ReadOnlySetList<T>(instance);

            return new ReadOnlyList<T>(instance);
        }

        public static ICollection<T> AsReadonlyCollection<T>(this ICollection<T> instance)
        {
            if (instance == null)
                return null;

            if (instance is ReadOnlyCollection<T>) //captures also derived types
                return instance;

            if (instance is ISetCollection<T> setCollection)
                return new ReadOnlySetCollection<T>(setCollection);

            return new ReadOnlyCollection<T>(instance);
        }

        public static IDictionary<TKey, TValue> AsReadonlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> instance)
        {
            if (instance == null)
                return null;

            if (instance is ReadOnlyDictionary<TKey, TValue>) //captures also derived types
                return instance;

            if (instance is ISetCollection<KeyValuePair<TKey, TValue>>)
                return new ReadOnlySetDictionary<TKey, TValue>(instance);

            return new ReadOnlyDictionary<TKey, TValue>(instance);
        }

        public static IDictionary<TKey, TValue> ViewAsDictionary<TKey, TValue>(
            this ICollection<TKey> keySet,
            Func<TKey, TValue> valueGetter,
            Action<TKey, TValue> valueSetter = null,
            bool validateQueriedKeys = true,
            IEqualityComparer<TValue> valueComparer = null)
        {
            return new ViewCollectionAsDictionary<TKey, TValue>(keySet, valueGetter, valueSetter, validateQueriedKeys, valueComparer);
        }

        public static IEnumerable<T> Lazy<T>(Func<IEnumerator<T>> enumeratorFactory)
        {
            if (enumeratorFactory == null)
                throw new ArgumentNullException(nameof(enumeratorFactory));

            return new LazyEnumerable<T>(enumeratorFactory);
        }

        public static IEnumerable<T> ConcatOrInvariant<T>(this IEnumerable<T> first, IEnumerable<T> second, bool returnEmptyUponNull = false)
        {
            if (first == null)
                return second ?? (returnEmptyUponNull ? Enumerable.Empty<T>() : null);
            else if (second == null)
                return first;
            else
                return first.Concat(second);
        }

        public static IEnumerable<T> Lazy<T>(Func<IEnumerable<T>> enumerableFactory, bool synchronizeInstantiation = true)
        {
            if (enumerableFactory == null)
                throw new ArgumentNullException(nameof(enumerableFactory));

            object syncObj = synchronizeInstantiation ? new object() : null;

            IEnumerable<T> instance = null;

            return Lazy(() =>
            {
                if (syncObj == null)
                {
                    if (instance == null)
                        instance = enumerableFactory() ?? throw new InvalidOperationException("Factory function returned a null value upon lazy instantiation.");

                    return instance.GetEnumerator();
                }
                else
                {
                    //LazyInitializer has a bit of an overhead
                    if (instance == null)
                    {
                        lock (syncObj)
                        {
                            if (instance == null)
                                instance = enumerableFactory();
                        }
                    }
                    return instance.GetEnumerator();
                }
            });
        }

        public static IEnumerable<T> AsReadonlyAuto<T>(this IEnumerable<T> instance)
        {
            //dictionary is not converted due to inefficiency
            if (instance == null)
                return null;


            if (instance is IList<T> l)
                return l.AsReadonlyList();


            if (instance is ICollection<T> c)
                return c.AsReadonlyCollection();

            return instance;
        }

        public static ICollection<T> ConcatCollections<T>(IEnumerable<ICollection<T>> collectionsEnumerable)
        {
            if (collectionsEnumerable == null)
                throw new ArgumentNullException(nameof(collectionsEnumerable));

            return new ConcatenatedCollection<T>(collectionsEnumerable);
        }

        public static IList<T> ConcatLists<T>(IEnumerable<IList<T>> listsEnumerable)
        {
            if (listsEnumerable == null)
                throw new ArgumentNullException(nameof(listsEnumerable));

            return new ConcatenatedList<T>(listsEnumerable);
        }

        public static IDictionary<TKey, TValue> ConcatDictionaries<TKey, TValue>(params IDictionary<TKey, TValue>[] dictionaries)
        {
            if (dictionaries == null)
                throw new ArgumentNullException(nameof(dictionaries));

            return ConcatDictionaries((IEnumerable<IDictionary<TKey, TValue>>)dictionaries);
        }

        public static IDictionary<TKey, TValue> ConcatDictionaries<TKey, TValue>(IEnumerable<IDictionary<TKey, TValue>> dictionariesEnumerable)
        {
            if (dictionariesEnumerable == null)
                throw new ArgumentNullException(nameof(dictionariesEnumerable));

            return new ConcatenatedDictionary<TKey, TValue>(dictionariesEnumerable);
        }

        public static IEnumerable<TLeaf> SelectTreeLeafs<TNode, TLeaf>(this IEnumerable<TNode> enumerable, Func<TNode, TLeaf> leafSelector)
            where TNode : IEnumerable<TNode>
        {
            return SelectTreeLeafs(enumerable, (node) => node, leafSelector);
        }

        public static IEnumerable<TLeaf> SelectTreeLeafs<TNode, TLeaf>(this IEnumerable<TNode> enumerable, Func<TNode, IEnumerable<TNode>> nodesSelector, Func<TNode, TLeaf> leafSelector)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (nodesSelector == null)
                throw new ArgumentNullException(nameof(nodesSelector));
            if (leafSelector == null)
                throw new ArgumentNullException(nameof(leafSelector));

            var nodes = new Stack<IEnumerable<TNode>>();

            nodes.Push(enumerable);

            do
            {
                IEnumerable<TNode> items = nodes.Pop();

                foreach (TNode it in items)
                {
                    yield return leafSelector(it);

                    var children = nodesSelector(it);

                    if (children != null)
                        nodes.Push(children);
                }
            } while (nodes.Count > 0);
        }

        public static IEnumerable<TLeaf> SelectManyTreeLeafs<TNode, TLeaf>(this IEnumerable<TNode> enumerable, Func<TNode, IEnumerable<TLeaf>> leafSelector)
            where TNode : IEnumerable<TNode>
        {
            return SelectManyTreeLeafs(enumerable, (node) => node, leafSelector);
        }

        public static IEnumerable<TLeaf> SelectManyTreeLeafs<TNode, TLeaf>(this IEnumerable<TNode> enumerable, Func<TNode, IEnumerable<TNode>> nodesSelector, Func<TNode, IEnumerable<TLeaf>> leafSelector)
        {
            return SelectTreeLeafs(enumerable, nodesSelector, leafSelector).Where((s) => s != null).SelectMany((s) => s);
        }

        public static bool AddSet<T>(this ISetCollection<T> collection, IEnumerable<T> items)
        {
            bool wereEdgesAdded = false;

            foreach (var e in items)
                wereEdgesAdded |= collection.Add(e);

            return wereEdgesAdded;
        }

        public static bool AddSet<T>(this ISetCollection<T> collection, params T[] items)
        {
            return AddSet(collection, (IEnumerable<T>)items);
        }

        public static void AddSequence<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var e in items)
                collection.Add(e);
        }

        public static void AddSequence<T>(this ICollection<T> collection, params T[] items)
        {
            AddSequence(collection, (IEnumerable<T>)items);
        }
        public static bool IsSetterReadonly<T>(this IList<T> instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (!(instance is IList c))
                return instance.IsReadOnly;

            return c.IsReadOnly;
        }

        public static bool IsFixedSize<T>(this IList<T> instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return (instance is IList c) && c.IsFixedSize;
        }

        public static IList<T> Single<T>(T item)
        {
            return new SingleItemList<T>(item);
        }

        public static IList<TResult> CastList<TSource, TResult>(this IList<TSource> list)
            where TResult : TSource
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return list as IList<TResult> ?? new CastListWrapper<TSource, TResult>(list);
        }

        public static IList<TResult> CastList<TResult>(this IList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            //The creation of the cast list via CastList<TResult> is more inefficient than directly calling CastList<TSource, TResult> but the case that IList is an IList<T> is caught by this call
            Type[] sourceTypes = TypeReflectionExtensions.GetGenericImplementations(list.GetType(), typeof(IList<>)).Select((s) => s.GetGenericArguments()[0]).ToArray();

            if (sourceTypes.Length == 1)
            {
                if (sourceTypes[0].IsAssignableFrom(typeof(TResult)))
                    return (IList<TResult>)Activator.CreateInstance(typeof(CastListWrapper<,>).MakeGenericType(sourceTypes[0], typeof(TResult)), list);
            }

            return list as IList<TResult> ?? new CastListWrapper<TResult>(list);
        }

        private sealed class SingleItemList<T> : ListBase<T>
        {
            private readonly T _item;

            internal SingleItemList(T item)
            {
                _item = item;
            }

            public override int Count => 1;

            public override bool IsReadOnly => true;

            public override bool Contains(T item)
            {
                return EqualityComparer<T>.Default.Equals(_item, item);
            }

            public override void CopyTo(T[] array, int arrayIndex)
            {
                Util.ValidateRange(array, arrayIndex, 1);
                array[arrayIndex] = _item;
            }

            public override IEnumerator<T> GetEnumerator()
            {
                yield return _item;
            }

            public override int IndexOf(T item)
            {
                return Contains(item) ? 0 : -1;
            }

            public override T this[int index]
            {
                get
                {
                    Util.ValidateNamedIndex(index, 1, lengthName: nameof(Count));
                    return _item;
                }
                set => throw CollectionExceptions.ReadOnlyException();
            }

            public override void Insert(int index, T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public override bool Remove(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public override void RemoveAt(int index)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public override void Add(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public override void Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }
        }

        public static ISetCollection<T> AsSetCollection<T>(this ISet<T> instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new SetCollection<T>(instance);
        }

        private sealed class CastListWrapper<TResult> : ListBase<TResult>
        {
            private readonly IList _innerList;

            public CastListWrapper(IList innerList)
            {
                if (innerList == null)
                    throw new ArgumentNullException(nameof(innerList));

                _innerList = innerList;
            }

            public override TResult this[int index]
            {
                get => (TResult)_innerList[index];
                set => _innerList[index] = value;
            }

            public override int Count => _innerList.Count;

            public override bool IsReadOnly => _innerList.IsReadOnly || _innerList.IsFixedSize;

            public override void Clear()
            {
                _innerList.Clear();
            }

            public override IEnumerator<TResult> GetEnumerator()
            {
                return _innerList.Cast<TResult>().GetEnumerator();
            }

            public override int IndexOf(TResult item)
            {
                return _innerList.IndexOf(item);
            }

            public override void Insert(int index, TResult item)
            {
                _innerList.Insert(index, item);
            }

            public override void RemoveAt(int index)
            {
                _innerList.RemoveAt(index);
            }

            public override bool Remove(TResult item)
            {
                int c = _innerList.Count;
                _innerList.Remove(item);

                return c != _innerList.Count;
            }

            protected override bool IsFixedSize => _innerList.IsFixedSize;
            protected override object SyncRoot => _innerList.IsSynchronized ? _innerList.SyncRoot : null;

            public override bool Contains(TResult item)
            {
                return _innerList.Contains(item);
            }

            public override void Add(TResult item)
            {
                _innerList.Add(item);
            }

            public override void CopyTo(TResult[] array, int arrayIndex)
            {
                _innerList.CopyTo(array, arrayIndex);
            }
        }

        private sealed class CastListWrapper<TSource, TResult> : ListBase<TResult>
            where TResult : TSource
        {
            private readonly IList<TSource> _innerList;

            public CastListWrapper(IList<TSource> innerList)
            {
                if (innerList == null)
                    throw new ArgumentNullException(nameof(innerList));

                _innerList = innerList;
            }

            public override TResult this[int index]
            {
                get => (TResult)_innerList[index];
                set => _innerList[index] = value;
            }

            public override int Count => _innerList.Count;

            public override bool IsReadOnly => _innerList.IsReadOnly;

            public override void Clear()
            {
                _innerList.Clear();
            }

            public override IEnumerator<TResult> GetEnumerator()
            {
                return _innerList.Cast<TResult>().GetEnumerator();
            }

            public override int IndexOf(TResult item)
            {
                return _innerList.IndexOf(item);
            }

            public override void Insert(int index, TResult item)
            {
                _innerList.Insert(index, item);
            }

            public override void RemoveAt(int index)
            {
                _innerList.RemoveAt(index);
            }

            public override bool Remove(TResult item)
            {
                int c = _innerList.Count;
                _innerList.Remove(item);

                return c != _innerList.Count;
            }

            protected override bool IsFixedSize => _innerList.IsFixedSize();
            protected override object SyncRoot => _innerList is IList l && l.IsSynchronized ? l.SyncRoot : null;

            public override bool Contains(TResult item)
            {
                return _innerList.Contains(item);
            }

            public override void Add(TResult item)
            {
                _innerList.Add(item);
            }

            public override void CopyTo(TResult[] array, int arrayIndex)
            {
                TSource[] src = array as TSource[];

                if (src != null)
                    _innerList.CopyTo(src, arrayIndex);
                else
                {
                    base.CopyTo(array, arrayIndex);
                }
            }
        }

        private sealed class SetCollection<T> : ISetCollection<T>, ISet<T>
        {
            private readonly ISet<T> _set;

            public SetCollection(ISet<T> set)
            {
                _set = set ?? throw new ArgumentNullException(nameof(set));
            }

            public int Count => _set.Count;

            public bool IsReadOnly => _set.IsReadOnly;

            private static IEnumerable<T> GetInnerSet(IEnumerable<T> other)
            {
                if (other is SetCollection<T> setCollection)
                    return setCollection._set;

                return other;
            }

            void ICollection<T>.Add(T item)
            {
                ((ICollection<T>)_set).Add(item);
            }

            public bool Add(T item)
            {
                return _set.Add(item);
            }

            public void Clear()
            {
                _set.Clear();
            }

            public bool Contains(T item)
            {
                return _set.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _set.CopyTo(array, arrayIndex);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _set.GetEnumerator();
            }

            public bool Remove(T item)
            {
                return _set.Remove(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_set).GetEnumerator();
            }

            public void ExceptWith(IEnumerable<T> other)
            {
                _set.ExceptWith(GetInnerSet(other));
            }

            public void IntersectWith(IEnumerable<T> other)
            {
                _set.IntersectWith(GetInnerSet(other));
            }

            public bool IsProperSubsetOf(IEnumerable<T> other)
            {
                return _set.IsProperSubsetOf(GetInnerSet(other));
            }

            public bool IsProperSupersetOf(IEnumerable<T> other)
            {
                return _set.IsProperSupersetOf(GetInnerSet(other));
            }

            public bool IsSubsetOf(IEnumerable<T> other)
            {
                return _set.IsSubsetOf(GetInnerSet(other));
            }

            public bool IsSupersetOf(IEnumerable<T> other)
            {
                return _set.IsSupersetOf(GetInnerSet(other));
            }

            public bool Overlaps(IEnumerable<T> other)
            {
                return _set.Overlaps(GetInnerSet(other));
            }

            public bool SetEquals(IEnumerable<T> other)
            {
                return _set.SetEquals(GetInnerSet(other));
            }

            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                _set.SymmetricExceptWith(GetInnerSet(other));
            }

            public void UnionWith(IEnumerable<T> other)
            {
                _set.UnionWith(GetInnerSet(other));
            }
        }

        private class ReadOnlyCollection<T> : ICollection<T>
        {
            protected ICollection<T> InnerCollection { get; }

            public ReadOnlyCollection(ICollection<T> innerCollection)
            {
                if (innerCollection == null)
                    throw new ArgumentNullException(nameof(innerCollection));

                InnerCollection = innerCollection;
            }

            public int Count => InnerCollection.Count;

            public bool IsReadOnly => true;

            public void Add(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Remove(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Contains(T item)
            {
                return InnerCollection.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                InnerCollection.CopyTo(array, arrayIndex);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return InnerCollection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)InnerCollection).GetEnumerator();
            }
        }

        private class ReadOnlyList<T> : IList<T>
        {
            protected IList<T> InnerList { get; }

            public ReadOnlyList(IList<T> innerList)
            {
                if (innerList == null)
                    throw new ArgumentNullException(nameof(InnerList));

                InnerList = innerList;
            }

            public T this[int index]
            {
                get => InnerList[index];
                set => throw CollectionExceptions.ReadOnlyException();
            }

            public int Count => InnerList.Count;

            public bool IsReadOnly => true;

            public bool Contains(T item)
            {
                return InnerList.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                InnerList.CopyTo(array, arrayIndex);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return InnerList.GetEnumerator();
            }

            public int IndexOf(T item)
            {
                return InnerList.IndexOf(item);
            }

            public void Add(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void Insert(int index, T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Remove(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void RemoveAt(int index)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)InnerList).GetEnumerator();
            }
        }

        private sealed class ReadOnlySetCollection<T> : ReadOnlyCollection<T>, ISetCollection<T>
        {
            public ReadOnlySetCollection(ISetCollection<T> innerCollection) 
                : base(innerCollection)
            {
            }

            bool ISetCollection<T>.Add(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }
        }

        private sealed class ReadOnlySetList<T> : ReadOnlyList<T>, ISetCollection<T>
        {
            public ReadOnlySetList(IList<T> innerCollection)
                : base(innerCollection)
            {
                if (!(innerCollection is ISetCollection<T>))
                    throw new InvalidOperationException($"Expected an instance of type {typeof(ISetCollection<T>).FullName}.");
            }

            bool ISetCollection<T>.Add(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }
        }

        private class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            protected IDictionary<TKey, TValue> InnerDictionary { get; }
            private ICollection<TKey> _keys;
            private ICollection<TValue> _values;
            private ICollection<TKey> _readonlyKeys;
            private ICollection<TValue> _readonlyValues;

            public ReadOnlyDictionary(IDictionary<TKey, TValue> innerDictionary)
            {
                if (innerDictionary == null)
                    throw new ArgumentNullException(nameof(innerDictionary));

                InnerDictionary = innerDictionary;
            }

            public TValue this[TKey key]
            {
                get => InnerDictionary[key];
                set => throw CollectionExceptions.ReadOnlyException();
            }

            public ICollection<TKey> Keys
            {
                get
                {
                    ICollection<TKey> k = InnerDictionary.Keys;

                    if (k != _keys)
                    {
                        _keys = k;
                        _readonlyKeys = k.AsReadonlyCollection();
                    }

                    return _readonlyKeys;
                }
            }

            public ICollection<TValue> Values
            {
                get
                {

                    ICollection<TValue> v = InnerDictionary.Values;

                    if (v != _values)
                    {
                        _values = v;
                        _readonlyValues = v.AsReadonlyCollection();
                    }

                    return _readonlyValues;
                }
            }

            public int Count => InnerDictionary.Count;

            public bool IsReadOnly => true;

            public void Add(TKey key, TValue value)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return InnerDictionary.Contains(item);
            }

            public bool ContainsKey(TKey key)
            {
                return InnerDictionary.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                InnerDictionary.CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return InnerDictionary.GetEnumerator();
            }

            public bool Remove(TKey key)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return InnerDictionary.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)InnerDictionary).GetEnumerator();
            }
        }

        private sealed class ReadOnlySetDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>, ISetCollection<KeyValuePair<TKey, TValue>>
        {
            public ReadOnlySetDictionary(IDictionary<TKey, TValue> innerDictionary) 
                : base(innerDictionary)
            {
                if (!(innerDictionary is ISetCollection<KeyValuePair<TKey, TValue>>))
                    throw new InvalidOperationException($"Expected an instance of type {typeof(ISetCollection<KeyValuePair<TKey, TValue>>).FullName}.");
            }

            bool ISetCollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }
        }

        private sealed class ConcatenatedCollection<T> : ICollection<T>
        {
            private readonly IEnumerable<ICollection<T>> _collections;

            public ConcatenatedCollection(IEnumerable<ICollection<T>> collections)
            {
                if (collections == null)
                    throw new ArgumentNullException(nameof(collections));

                _collections = collections;
            }

            public int Count => _collections.Sum((curr) => curr.Count);

            public bool IsReadOnly => true;

            public void Add(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Contains(T item)
            {
                return _collections.Any((s) => s.Contains(item));
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                Util.ValidateNamedRange(array, arrayIndex, Count);

                long i = arrayIndex;

                foreach (T v in this)
                    array[i++] = v;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _collections.SelectMany((s) => s).GetEnumerator();
            }

            public bool Remove(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class ConcatenatedList<T> : IList<T>
        {
            private readonly IEnumerable<IList<T>> _lists;

            public ConcatenatedList(IEnumerable<IList<T>> lists)
            {
                if (lists == null)
                    throw new ArgumentNullException(nameof(lists));

                _lists = lists;
            }

            public T this[int index]
            {
                get
                {
                    foreach (IList<T> l in _lists)
                    {
                        int c = l.Count;

                        if (index < c)
                            return l[index];

                        index -= c;
                    }
                    throw new ArgumentException("Index exceeds list's boundaries.", nameof(index));
                }
                set => throw CollectionExceptions.ReadOnlyException();
            }

            public int Count => _lists.Sum((s) => s.Count);

            public bool IsReadOnly => true;

            public void Add(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Contains(T item)
            {
                return _lists.Any((s) => s.Contains(item));
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                Util.ValidateNamedRange(array, arrayIndex, Count);

                long i = arrayIndex;

                foreach (T v in this)
                    array[i++] = v;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _lists.SelectMany((s) => s).GetEnumerator();
            }

            public int IndexOf(T item)
            {
                int c = 0;
                foreach (IList<T> l in _lists)
                {
                    int ind = l.IndexOf(item);

                    if (ind >= 0)
                        return ind + c;

                    c += l.Count;
                }
                return -1;
            }

            public void Insert(int index, T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Remove(T item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void RemoveAt(int index)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class ConcatenatedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            private readonly IEnumerable<IDictionary<TKey, TValue>> _dictionaries;

            public ConcatenatedDictionary(IEnumerable<IDictionary<TKey, TValue>> dictionaries)
            {
                if (dictionaries == null)
                    throw new ArgumentNullException(nameof(dictionaries));

                _dictionaries = dictionaries;
            }

            public int Count => _dictionaries.Sum((s) => s.Count);

            public ICollection<TKey> Keys => new ConcatenatedCollection<TKey>(_dictionaries.Select((s) => s.Keys));

            public ICollection<TValue> Values => new ConcatenatedCollection<TValue>(_dictionaries.Select((s) => s.Values));

            public bool IsReadOnly => true;

            public TValue this[TKey key]
            {
                get
                {
                    TValue value;
                    foreach (IDictionary<TKey, TValue> v in _dictionaries)
                        if (v.TryGetValue(key, out value))
                            return value;

                    throw new KeyNotFoundException();
                }
                set => throw CollectionExceptions.ReadOnlyException();
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                Util.ValidateNamedRange(array, arrayIndex, Count);

                long i = arrayIndex;

                foreach (var v in this)
                    array[i++] = v;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return _dictionaries.SelectMany((s) => s).GetEnumerator();
            }

            public bool ContainsKey(TKey key)
            {
                return _dictionaries.Any((s) => s.ContainsKey(key));
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return _dictionaries.Any((s) => s.Contains(item));
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                foreach (IDictionary<TKey, TValue> v in _dictionaries)
                    if (v.TryGetValue(key, out value))
                        return true;

                value = default;
                return false;
            }

            public void Add(TKey key, TValue value)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Remove(TKey key)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public void Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class LazyEnumerable<T> : IEnumerable<T>
        {
            private readonly Func<IEnumerator<T>> _enumeratorFactory;

            public LazyEnumerable(Func<IEnumerator<T>> enumeratorFactory)
            {
                if (enumeratorFactory == null)
                    throw new ArgumentNullException(nameof(enumeratorFactory));

                _enumeratorFactory = enumeratorFactory;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _enumeratorFactory();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class ViewCollectionAsDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            private readonly ICollection<TKey> _keySet;
            private readonly Func<TKey, TValue> _valueGetter;
            private readonly Action<TKey, TValue> _valueSetter;
            private readonly bool _validateQueriedKeys;
            private readonly IEqualityComparer<TValue> _valueComparer;

            public ViewCollectionAsDictionary(
                ICollection<TKey> keySet,
                Func<TKey, TValue> valueGetter,
                Action<TKey, TValue> valueSetter,
                bool validateQueriedKeys,
                IEqualityComparer<TValue> valueComparer)
            {
                _keySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
                _valueGetter = valueGetter ?? throw new ArgumentNullException(nameof(valueGetter));
                _valueSetter = valueSetter;
                _validateQueriedKeys = validateQueriedKeys;
                _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
            }

            public TValue this[TKey key]
            {
                get
                {
                    if (_validateQueriedKeys && !_keySet.Contains(key))
                        throw new KeyNotFoundException();

                    return _valueGetter(key);
                }
                set
                {
                    if (IsReadOnly)
                        throw CollectionExceptions.ReadOnlyException();

                    _valueSetter(key, value);
                }
            }

            public ICollection<TKey> Keys => _keySet.AsReadonlyCollection();
            public ICollection<TValue> Values => _keySet.SelectCollection(_valueGetter);

            public int Count => _keySet.Count;

            public bool IsReadOnly => _valueSetter == null;

            public void Add(TKey key, TValue value)
            {
                if (IsReadOnly)
                    throw CollectionExceptions.ReadOnlyException();

                if (_validateQueriedKeys && _keySet.Contains(key))
                    throw new ArgumentException("The indicated key is already in the dictionary.", nameof(key));

                _keySet.Add(key);
                _valueSetter(key, value);
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                Add(item.Key, item.Value);
            }

            public void Clear()
            {
                if (IsReadOnly)
                    throw CollectionExceptions.ReadOnlyException();

                _keySet.Clear();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return TryGetValue(item.Key, out var value) && _valueComparer.Equals(value, item.Value);
            }

            public bool ContainsKey(TKey key)
            {
                return _keySet.Contains(key);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

                foreach (var k in _keySet)
                    array[arrayIndex++] = new KeyValuePair<TKey, TValue>(k, _valueGetter(k));
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                foreach (var k in _keySet)
                    yield return new KeyValuePair<TKey, TValue>(k, _valueGetter(k));
            }

            public bool Remove(TKey key)
            {
                if (IsReadOnly)
                    throw CollectionExceptions.ReadOnlyException();

                return _keySet.Remove(key);
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                if (IsReadOnly)
                    throw CollectionExceptions.ReadOnlyException();

                return Contains(item) && Remove(item.Key);
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                if (_keySet.Contains(key))
                {
                    value = _valueGetter(key);
                    return true;
                }

                value = default;
                return false;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        //private static class Helper<T>
        //{
        //    public static bool IsTKeyValuePair { get; }

        //    public static bool CreateDictionary(IEnumerable<T> instance, out object readonlyDictionary)
        //    {
        //        Type tp = null;

        //        foreach (Type ctp in Util.GetGenericImplementations(instance.GetType(), typeof(IDictionary<,>)))
        //        {
        //            if (tp != null)
        //            {
        //                readonlyDictionary = null;
        //                return false;
        //            }

        //            tp = ctp;
        //        }

        //        if (tp == null)
        //        {
        //            readonlyDictionary = null;
        //            return false;
        //        }

        //        Type readonlyDictionaryType = typeof(ReadonlyDictionary<,>).MakeGenericType(tp.GetGenericArguments());

        //        readonlyDictionary = Activator.CreateInstance(readonlyDictionaryType, instance);
        //        return true;
        //    }

        //    static Helper()
        //    {
        //        IsTKeyValuePair = typeof(KeyValuePair<,>).IsAssignableFrom(typeof(T));
        //    }
        //}
    }
}
