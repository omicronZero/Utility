using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections.Tools;

namespace Utility.Collections
{
    public static class Set
    {
        public static Set<T> Empty<T>() => default;
        public static Set<T> Create<T>(params T[] items) => new Set<T>(items);
        public static Set<T> Create<T>(T item) => new Set<T>(item);
        public static Set<T> Create<T>(IEnumerable<T> items) => new Set<T>(items);

        public static Set<T> Union<T>(Set<T> set1, Set<T> set2) => Set<T>.Union(set1, set2);
        public static Set<T> Union<T>(params Set<T>[] sets) => Set<T>.Union(sets);
        public static Set<T> Union<T>(IEnumerable<Set<T>> sets) => Set<T>.Union(sets);

        public static Set<T> Intersect<T>(Set<T> set1, Set<T> set2) => Set<T>.Intersect(set1, set2);
        public static Set<T> Intersect<T>(Set<T> set1, Set<T> set2, Set<T> set3) => Set<T>.Intersect(set1, set2, set3);
        public static Set<T> Intersect<T>(params Set<T>[] sets) => Set<T>.Intersect(sets);
        public static Set<T> Intersect<T>(IEnumerable<Set<T>> sets, bool gatherFirst = false) => Set<T>.Intersect(sets, gatherFirst);
    }

    //TODO: make default an empty set (e.g., by making _hashCode an int?)
    public struct Set<T> : IEquatable<Set<T>>, IReadOnlyCollection<T>, ICollection<T>
    {
        private readonly T _value;
        private readonly HashSet<T> _values;
        private readonly int? _hashCode;

        public static Set<T> Empty { get; } = new Set<T>();

        private static readonly HashSet<T> emptyHashSet = new HashSet<T>();

        public Set(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _values = null;
            _value = value;
            _hashCode = value.GetHashCode();
        }

        public Set(params T[] values)
            : this()
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (var value in values)
            {
                if (value == null)
                    throw new ArgumentException("Values must not be null.", nameof(values));
            }

            if (values.Length == 0)
            {
                this = Empty;
            }
            else if (values.Length == 1)
            {
                _value = values[0];
                _hashCode = values[0].GetHashCode();
            }
            else
            {
                _values = new HashSet<T>(values);

                int hc = 0;

                foreach (var v in _values)
                    hc ^= v.GetHashCode();

                _hashCode = hc;
            }
        }

        public Set(IEnumerable<T> valueEnumerable)
        {
            if (valueEnumerable == null)
                throw new ArgumentNullException(nameof(valueEnumerable));

            if (valueEnumerable is Set<T> s)
            {
                this = s;
                return;
            }

            var hs = new HashSet<T>(valueEnumerable);

            if (hs.Count == 0)
            {
                this = Empty;
            }
            else if (hs.Count == 1)
            {
                _value = hs.Single();
                _values = null;
                _hashCode = _value.GetHashCode();
            }
            else
            {
                _value = default;
                _values = hs;

                int hc = 0;

                foreach (var v in _values)
                    hc ^= v.GetHashCode();

                _hashCode = hc;
            }
        }

        private Set(HashSet<T> hashset)
        {
            _values = hashset ?? throw new ArgumentNullException(nameof(hashset));
            _value = default;

            int hc = 0;

            foreach (var v in hashset)
                hc ^= v.GetHashCode();

            _hashCode = hc;
        }

        private Set(HashSet<T> hashset, int hashCode)
        {
            _values = hashset ?? throw new ArgumentNullException(nameof(hashset));
            _value = default;
            _hashCode = hashCode;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (IsEmpty)
                return Enumerable.Empty<T>().GetEnumerator();

            return IsSingleton ? EnumerableExtensions.Single(_value).GetEnumerator() : _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            return $"{{{string.Join(", ", this)}}}";
        }

        public StringBuilder ToString(StringBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.AppendJoin(", ", this);
        }

        public bool IsEmpty => _hashCode == null;

        public bool IsSingleton => !IsEmpty && _values == null;

        public bool IsEmptyOrSingleton => _values == null;

        public int Count => IsEmpty ? 0 : (IsSingleton ? 1 : _values.Count);

        bool ICollection<T>.IsReadOnly => true;

        public T GetSingleton()
        {
            CheckSingleton();

            return _value;
        }

        private void CheckSingleton()
        {
            if (!IsSingleton)
                throw new InvalidOperationException("The current instance is not a singleton.");
        }

        public bool Contains(T point)
        {
            if (IsEmpty)
                return false;
            if (IsSingleton)
                return EqualityComparer<T>.Default.Equals(_value, point);

            return _values.Contains(point);
        }

        public bool IsSupersetOf(Set<T> set)
        {
            if (set.IsEmpty)
                return true;
            if (IsEmpty)
                return false;

            if (IsSingleton)
                return set.Count == 1 && Contains(set._value);
            else if (set.IsSingleton)
                return _values.Contains(set._value);
            else
                return _values.IsSupersetOf(set._values);
        }

        public bool IsSupersetOf(IEnumerable<T> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            if (points is Set<T> s)
                return IsSupersetOf(s);

            if (IsEmpty)
                return !points.Any();

            if (IsSingleton)
            {
                var eq = EqualityComparer<T>.Default;

                foreach (var point in points)
                {
                    if (!eq.Equals(_value, point))
                        return false;
                }

                return true;
            }
            else
                return _values.IsSupersetOf(points);
        }

        public bool IsSubsetOf(Set<T> set)
        {
            return set.IsSupersetOf(this);
        }

        public bool IsSubsetOf(IEnumerable<T> set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            if (IsEmpty)
                return true;
            else if (IsSingleton)
                return set.Contains(_value);
            else
                return _values.IsSubsetOf(set);
        }

        public bool Equals(Set<T> other)
        {
            if (IsEmpty)
                return other.IsEmpty;

            if (IsSingleton)
            {
                if (!other.IsSingleton)
                    return false;

                return _hashCode == other._hashCode && EqualityComparer<T>.Default.Equals(_value, other._value);
            }
            else if (other.IsSingleton)
                return false;

            //check whether both are the same hashset (i.e., the struct is compared with itself
            if (object.ReferenceEquals(_values, other._values))
                return true;

            if (_hashCode != other._hashCode)
                return false;

            if (_values.Count != other._values.Count)
                return false;

            return _values.SetEquals(other._values);
        }

        public override bool Equals(object obj) => obj is Set<T> set && Equals(set);

        public override int GetHashCode()
        {
            return _hashCode ?? 0;
        }

        public static bool operator ==(Set<T> left, Set<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Set<T> left, Set<T> right) => !(left == right);

        public static Set<TOut> Map<TOut>(Set<T> set, Func<T, TOut> converter)
        {
            if (set.IsEmpty)
                return Set<TOut>.Empty;
            if (set.IsSingleton)
                return new Set<TOut>(converter(set.GetSingleton()));

            return new Set<TOut>(set.Select(converter));
        }

        public static Set<T> Union(Set<T> set1, Set<T> set2)
        {
            if (set1.IsEmpty)
                return set2;
            else if (set2.IsEmpty)
                return set1;

            var hs = set1._values;

            if (hs == null)
            {
                if (set2.IsSingleton)
                {
                    hs = new HashSet<T>();
                    hs.Add(set1._value);
                    hs.Add(set2._value);
                }
                else
                {
                    hs = new HashSet<T>(set2._values);
                    hs.Add(set1._value);
                }
            }
            else
            {
                hs = new HashSet<T>(hs);
                if (set2.IsSingleton)
                    hs.Add(set2._value);
                else
                    hs.UnionWith(set2._values);
            }

            return new Set<T>(hs);
        }

        public static Set<T> Union(Set<T> set1, Set<T> set2, Set<T> set3)
        {
            if (set1.IsEmpty)
                return Union(set2, set3);
            else if (set2.IsEmpty)
                return Union(set1, set3);
            else if (set3.IsEmpty)
                return Union(set1, set2);

            var hs = new HashSet<T>();

            if (set1.IsSingleton)
                hs.Add(set1._value);
            else
                hs.UnionWith(set1._values);

            if (set2.IsSingleton)
                hs.Add(set2._value);
            else
                hs.UnionWith(set2._values);

            if (set3.IsSingleton)
                hs.Add(set3._value);
            else
                hs.UnionWith(set3._values);

            return new Set<T>(hs);
        }

        public static Set<T> Union(params Set<T>[] sets)
        {
            if (sets == null)
                throw new ArgumentNullException(nameof(sets));

            if (sets.Length == 0)
                return Empty;
            else if (sets.Length == 1)
                return sets[0];
            else if (sets.Length == 2)
                return Union(sets[0], sets[1]);
            else if (sets.Length == 3)
                return Union(sets[0], sets[1], sets[2]);

            return Union((IEnumerable<Set<T>>)sets);
        }

        public static Set<T> Union(IEnumerable<Set<T>> setEnumerable)
        {
            if (setEnumerable == null)
                throw new ArgumentNullException(nameof(setEnumerable));

            HashSet<T> hs = null;

            Set<T> unmodifiedSet = Empty;

            foreach (var s in setEnumerable)
            {
                if (s.IsEmpty)
                    continue;

                if (hs == null)
                {
                    //we haven't assigned unmodifiedSet to a set, yet --> let's assign it to
                    //s (if it is empty, we'll run into this condition once more)
                    if (unmodifiedSet.IsEmpty)
                    {
                        unmodifiedSet = s;
                        continue;
                    }

                    //both singleton sets are equal --> ignore
                    if (unmodifiedSet.IsSingleton && s.IsSingleton
                        && EqualityComparer<T>.Default.Equals(unmodifiedSet._value, s._value))
                        continue;

                    //we'll get a set with at least two entries, so let's create a set to which
                    //we can add
                    hs = new HashSet<T>();

                    if (unmodifiedSet.IsSingleton)
                        hs.Add(unmodifiedSet._value);
                    else
                        hs.UnionWith(unmodifiedSet._values);
                }

                //add s to the hash-set. We only reach this if we cannot reuse s without
                //modifying unmodifiedSet or s
                if (s.IsSingleton)
                    hs.Add(s._value);
                else
                    hs.UnionWith(s._values);
            }

            //if hs was not set, we haven't done anything in our union and we can just return
            //the empty set or the unmodified set we assigned above
            if (hs == null)
                return unmodifiedSet;
            else
                return new Set<T>(hs);
        }

        public static Set<T> Intersect(Set<T> set1, Set<T> set2)
        {
            if (set1.IsEmpty || set2.IsEmpty)
                return Empty;

            if (set1.IsSingleton)
            {
                if (set2.IsSingleton)
                    return EqualityComparer<T>.Default.Equals(set1._value, set2._value) ? set1 : Empty;
                else
                    return set2.Contains(set1._value) ? set1 : Empty;
            }
            else if (set2.IsSingleton)
                return Intersect(set2, set1);
            else
            {
                if (set1._values == set2._values) //both sets refer to the same instance
                    return set1;

                var hs = new HashSet<T>(set1._values);
                hs.IntersectWith(set2._values);

                if (hs.Count == 0)
                    return Empty;
                else if (hs.Count == 1)
                    return new Set<T>(hs.Single());
                else
                    return new Set<T>(hs);
            }
        }

        public static Set<T> Intersect(Set<T> set1, Set<T> set2, Set<T> set3)
        {
            if (set1.IsEmpty || set2.IsEmpty || set3.IsEmpty)
                return Empty;

            //sort by count
            if (set2.Count > set3.Count) // |S1| ? |S2| <= |S3|
                (set2, set3) = (set3, set2);
            if (set1.Count > set2.Count) // |S1| <= |S2| ? |S3|
                (set1, set2) = (set2, set1);
            if (set2.Count > set3.Count) // |S1| <= |S2| <= |S3|
                (set2, set3) = (set3, set2);

            if (set1.IsSingleton)
            {
                if (set2.IsSingleton)
                {
                    //set1 and set2 are both singletons --> if the singletons are not equal, we have an empty set
                    if (!EqualityComparer<T>.Default.Equals(set1._value, set2._value))
                        return Empty;

                    //set3 must contain the singleton of set1 (and thus set2), otherwise, we'll return the empty set
                    if (set3.IsSingleton)
                    {
                        if (!EqualityComparer<T>.Default.Equals(set1._value, set3._value))
                            return Empty;

                        return set1;
                    }
                    else if (set3.Contains(set1._value))
                        return set1;
                    else
                        return Empty;
                }
                else //set2 is not a singleton, set3 cannot be a singleton
                {
                    var singleton = set1._value;

                    //set1.singleton must be in both sets, otherwise, we'll get an empty set
                    if (!set2.Contains(singleton) || !set3.Contains(singleton))
                        return Empty;

                    //now we've reduced it to a 2-set-intersection problem
                    return Intersect(set2, set3);
                }
            }
            else //set1 is not a singleton, set2 and set3 cannot be singletons
            {
                //check whether some refer to the same instance (Intersection(a, b) will check this condition further)
                if (set1._values == set2._values)
                    return Intersect(set1, set3);
                else if (set1._values == set3._values)
                    return Intersect(set1, set2);
                else if (set2._values == set3._values)
                    return Intersect(set2, set1);

                //now we perform the expensive intersections

                var hs = new HashSet<T>(set1._values);

                hs.IntersectWith(set2._values);
                hs.IntersectWith(set3._values);

                if (hs.Count == 1)
                    return new Set<T>(hs.Single());
                else
                    return new Set<T>(hs);
            }
        }

        public static Set<T> Intersect(params Set<T>[] sets)
        {
            if (sets is null)
                throw new ArgumentNullException(nameof(sets));

            if (sets.Length == 0)
                throw new ArgumentException("Non-empty number of sets expected.", nameof(sets));
            else if (sets.Length == 1)
                return sets[0];
            else if (sets.Length == 2)
                return Intersect(sets[0], sets[1]);
            else if (sets.Length == 3)
                return Intersect(sets[0], sets[1], sets[2]);

            //O(n)
            foreach (var set in sets)
            {
                if (set.IsEmpty)
                    return Empty;
            }

            //we start intersection at the smallest sets, so let's sort first
            //O(n log n)
            Array.Sort(sets, (x, y) =>
            {
                //compare by count, then by hash-code (used by pruning)
                //this is a heuristic ordering to bring equal-by-reference hashsets closer together
                int c = x.Count.CompareTo(y.Count);
                if (c == 0)
                    c = (x._hashCode ?? 0).CompareTo(y._hashCode ?? 0);
                return c;
            });

            int count = 1;

            //prune unnecessary entries via a heuristic
            for (int i = 1; i < sets.Length - count; i++)
            {
                //we only prune for non-singletons since they are the expensive ones
                if (!sets[count - 1].IsSingleton)
                {
                    //sets[i] can't be a singleton since we're sorted by count
                    if (sets[i]._values != sets[count]._values)
                        sets[count++] = sets[i];
                }
            }

            HashSet<T> hs = null; //the hashset
            bool requiresHashsetCopy = true; // denotes whether the hashset must be copied before being modified
            T singleton = default;
            Set<T> lastOriginal = sets[0];

            if (lastOriginal.IsSingleton)
                singleton = lastOriginal._value;
            else
                hs = lastOriginal._values;

            for(int i = 0; i < count;i++)
            {
                var set = sets[i];

                if (hs is null) //the smallest set is a singleton
                {
                    if (set.IsSingleton)
                    {
                        if (!EqualityComparer<T>.Default.Equals(singleton, set._value))
                            return Empty;
                    }
                    else if (!set.Contains(singleton))
                    {
                        return Empty;
                    }
                }
                else //the current smallest set is a hashset
                {
                    if (set.IsSingleton)
                    {
                        if (!hs.Contains(set._value))
                            return Empty;
                        else
                        {
                            hs = null;
                            singleton = set._value;
                        }
                    }
                    else
                    {
                        //hashset-hashset intersection
                        if (hs != set._values)
                        {
                            //do we require a copy? (if we haven't created a hashset of the first hashset in the list)
                            if (requiresHashsetCopy)
                            {
                                hs = new HashSet<T>(hs);
                                requiresHashsetCopy = false;
                            }

                            hs.IntersectWith(set._values);

                            //re-interpret result
                            if (hs.Count == 0)
                                return Empty;
                            else if (hs.Count == 1)
                            {
                                singleton = hs.Single();
                                hs = null;
                            }
                        }
                    }
                }
            }

            if (hs != null)
                if (requiresHashsetCopy)
                    return lastOriginal; //we didn't make any changes
                else
                    return new Set<T>(hs);
            else
                return new Set<T>(singleton);
        }

        /// <summary>
        /// Performs intersection over the indicated enumeration of sets.
        /// </summary>
        /// <param name="setEnumerable">The non-empty enumeration of sets.</param>
        /// <param name="gatherFirst">If set to <see langword="true"/>, this will first create an array and then perform intersection.
        /// Set to <see langword="true"/> if the enumerables are not lazy. This should outperform the iterable-based approach.</param>
        /// <returns>The intersection over the indicated sets.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="setEnumerable"/> was set to <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="setEnumerable"/> did not contain any value.</exception>
        public static Set<T> Intersect(IEnumerable<Set<T>> setEnumerable, bool gatherFirst = false)
        {
            if (setEnumerable is null)
                throw new ArgumentNullException(nameof(setEnumerable));

            using var enr = setEnumerable.GetEnumerator();

            if (!enr.MoveNext())
                throw new ArgumentException("Non-empty number of sets expected.", nameof(setEnumerable));

            if (setEnumerable is Set<T>[] sets)
                return Intersect(sets);
            else if (gatherFirst)
                return Intersect(setEnumerable.ToArray());

            var first = enr.Current;
            var resultHashset = first._values;
            var resultValue = first._value;
            var resultRequiresCopy = true;

            while (enr.MoveNext())
            {
                var current = enr.Current;

                if (current.IsEmpty)
                    return Empty;

                if (current.IsSingleton)
                {
                    if (resultHashset is null) //current is a singleton, the current result is also a singleton
                    {
                        if (!EqualityComparer<T>.Default.Equals(resultValue, current._value))
                            return Empty;
                    }
                    else if (!resultHashset.Contains(current._value)) //current is a singleton, the current result is a hash-set
                        return Empty;
                }
                else if (resultHashset is null) //current is not a singleton, but the result is a singleton
                {
                    if (!current._values.Contains(resultValue))
                        return Empty;
                }
                else //current is not a singleton and the result is also not a singleton
                {
                    if (resultRequiresCopy)
                    {
                        resultHashset = new HashSet<T>(resultHashset);
                        resultRequiresCopy = false;
                    }

                    resultHashset.IntersectWith(current._values);
                }
            }

            if (resultHashset == null)
                return new Set<T>(resultValue);
            else if (resultRequiresCopy)
                return first;
            else
                return new Set<T>(resultHashset);
        }

        void ICollection<T>.Add(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        void ICollection<T>.Clear()
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

            foreach(var item in this)
                array[arrayIndex++] = item;
        }

        bool ICollection<T>.Remove(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }
    }
}
