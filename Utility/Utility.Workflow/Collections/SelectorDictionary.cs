using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility.Collections.Tools;

namespace Utility.Collections
{
    public static class SelectorDictionary
    {
        public static SelectorDictionary<TKey, TValue, TKey, TUnderlyingValue> SelectDictionary<TKey, TValue, TUnderlyingValue>(
            this IDictionary<TKey, TUnderlyingValue> underlyingDictionary,
            Func<TUnderlyingValue, TValue> valueSelector)
        {
            return SelectDictionary(underlyingDictionary, valueSelector, null);
        }

        public static SelectorDictionary<TKey, TValue, TKey, TUnderlyingValue> SelectDictionary<TKey, TValue, TUnderlyingValue>(
            this IDictionary<TKey, TUnderlyingValue> underlyingDictionary,
            Func<TUnderlyingValue, TValue> valueSelector,
            Func<TValue, TUnderlyingValue> valueConverter)
        {
            Func<TKey, TKey> identity = (k) => k;
            return SelectDictionary(underlyingDictionary, identity, identity, valueSelector, valueConverter);
        }

        public static SelectorDictionary<TKey, TValue, TUnderlyingKey, TValue> SelectDictionary<TKey, TValue, TUnderlyingKey>(
            this IDictionary<TUnderlyingKey, TValue> underlyingDictionary,
            Func<TKey, TUnderlyingKey> keySelector,
            Func<TUnderlyingKey, TKey> keyConverter)
        {
            Func<TValue, TValue> identity = (v) => v;
            return SelectDictionary(underlyingDictionary, keySelector, keyConverter, identity, identity);
        }

        public static SelectorDictionary<TKey, TValue, TUnderlyingKey, TUnderlyingValue> SelectDictionary<TKey, TValue, TUnderlyingKey, TUnderlyingValue>(
            this IDictionary<TUnderlyingKey, TUnderlyingValue> underlyingDictionary,
            Func<TKey, TUnderlyingKey> keySelector,
            Func<TUnderlyingKey, TKey> keyConverter,
            Func<TUnderlyingValue, TValue> valueSelector)
        {
            return SelectDictionary(underlyingDictionary, keySelector, keyConverter, valueSelector);
        }

        public static SelectorDictionary<TKey, TValue, TUnderlyingKey, TUnderlyingValue> SelectDictionary<TKey, TValue, TUnderlyingKey, TUnderlyingValue>(
            this IDictionary<TUnderlyingKey, TUnderlyingValue> underlyingDictionary,
            Func<TKey, TUnderlyingKey> keySelector,
            Func<TUnderlyingKey, TKey> keyConverter,
            Func<TUnderlyingValue, TValue> valueSelector,
            Func<TValue, TUnderlyingValue> valueConverter)
        {
            return new SelectorDictionary<TKey, TValue, TUnderlyingKey, TUnderlyingValue>(underlyingDictionary, keySelector, keyConverter, valueSelector, valueConverter);
        }
    }

    public struct SelectorDictionary<TKey, TValue, TUnderlyingKey, TUnderlyingValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IDictionary<TUnderlyingKey, TUnderlyingValue> _underlyingDictionary;
        private readonly Func<TKey, TUnderlyingKey> _keySelector;
        private readonly Func<TUnderlyingKey, TKey> _keyConverter;
        private readonly Func<TUnderlyingValue, TValue> _valueSelector;
        private readonly Func<TValue, TUnderlyingValue> _valueConverter;

        public SelectorDictionary(IDictionary<TUnderlyingKey, TUnderlyingValue> underlyingDictionary,
            Func<TKey, TUnderlyingKey> keySelector,
            Func<TUnderlyingKey, TKey> keyConverter,
            Func<TUnderlyingValue, TValue> valueSelector,
            Func<TValue, TUnderlyingValue> valueConverter)
        {
            if (underlyingDictionary == null)
                throw new ArgumentNullException(nameof(underlyingDictionary));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (keyConverter == null)
                throw new ArgumentNullException(nameof(keyConverter));
            if (valueSelector == null)
                throw new ArgumentNullException(nameof(valueSelector));

            _underlyingDictionary = underlyingDictionary;

            _keySelector = keySelector;
            _keyConverter = RetrieveConverter(keyConverter);

            _valueSelector = valueSelector;
            _valueConverter = RetrieveConverter(valueConverter);
        }

        private static Func<TIn, TOut> RetrieveConverter<TIn, TOut>(Func<TIn, TOut> valueConverter)
        {
            if (valueConverter == null)
            {
                return null;
            }
            else
            {
                return (v) =>
                {
                    try
                    {
                        return valueConverter(v);
                    }
                    catch (FormatException ex)
                    {
                        throw new ArgumentException("Bad format.", ex);
                    }
                };
            }
        }

        public TValue this[TKey key]
        {
            get => _valueSelector(_underlyingDictionary[_keySelector(key)]);
            set
            {
                CheckReadonly();

                _underlyingDictionary[_keySelector(key)] = _valueConverter(value);
            }
        }

        private void CheckReadonly()
        {
            if (IsReadOnly)
                throw CollectionExceptions.ReadOnlyException();
        }

        public SelectorCollection<TKey, TUnderlyingKey> Keys => SelectorCollection.SelectCollection(_underlyingDictionary.Keys, _keyConverter);

        public SelectorCollection<TValue, TUnderlyingValue> Values => SelectorCollection.SelectCollection(_underlyingDictionary.Values, _valueSelector);

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        public int Count => _underlyingDictionary.Count;

        public bool IsReadOnly => _valueConverter != null;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public void Add(TKey key, TValue value)
        {
            CheckReadonly();

            _underlyingDictionary.Add(_keySelector(key), _valueConverter(value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            CheckReadonly();

            _underlyingDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_valueConverter != null)
                return _underlyingDictionary.Contains(new KeyValuePair<TUnderlyingKey, TUnderlyingValue>(_keySelector(item.Key), _valueConverter(item.Value)));

            TUnderlyingValue v;

            if (!_underlyingDictionary.TryGetValue(_keySelector(item.Key), out v))
                return false;

            return EqualityComparer<TValue>.Default.Equals(item.Value, _valueSelector(v));
        }

        private bool SelectKey(TKey key, out TUnderlyingKey underlyingKey)
        {
            try
            {
                underlyingKey = _keySelector(key);
                return true;
            }
            catch (FormatException)
            { }
            catch (ArgumentException)
            { }

            underlyingKey = default;
            return false;
        }

        //private bool SelectValue(TUnderlyingValue value, out TValue underlyingValue)
        //{
        //    try
        //    {
        //        underlyingValue = _valueSelector(value);
        //        return true;
        //    }
        //    catch (FormatException)
        //    { }
        //    catch (ArgumentException)
        //    { }

        //    underlyingValue = default;
        //    return false;
        //}

        //private bool ConvertKey(TUnderlyingKey key, out TKey underlyingKey)
        //{
        //    try
        //    {
        //        underlyingKey = _keyConverter(key);
        //        return true;
        //    }
        //    catch (FormatException)
        //    { }
        //    catch (ArgumentException)
        //    { }

        //    underlyingKey = default;
        //    return false;
        //}

        private bool ConvertValue(TValue value, out TUnderlyingValue underlyingValue)
        {
            try
            {
                underlyingValue = _valueConverter(value);
                return true;
            }
            catch (FormatException)
            { }
            catch (ArgumentException)
            { }

            underlyingValue = default;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            TUnderlyingKey k;
            return SelectKey(key, out k) && _underlyingDictionary.ContainsKey(k);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Util.ValidateNamedRange(array, arrayIndex, _underlyingDictionary.Count, indexName: nameof(arrayIndex));

            foreach (KeyValuePair<TUnderlyingKey, TUnderlyingValue> v in _underlyingDictionary)
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(_keyConverter(v.Key), _valueSelector(v.Value));
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TUnderlyingKey, TUnderlyingValue> v in _underlyingDictionary)
            {
                yield return new KeyValuePair<TKey, TValue>(_keyConverter(v.Key), _valueSelector(v.Value));
            }
        }

        public bool Remove(TKey key)
        {
            CheckReadonly();

            TUnderlyingKey k;
            return SelectKey(key, out k) && _underlyingDictionary.Remove(k);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            CheckReadonly();

            TUnderlyingKey k;
            TUnderlyingValue v;
            return SelectKey(item.Key, out k) && ConvertValue(item.Value, out v) && _underlyingDictionary.Remove(new KeyValuePair<TUnderlyingKey, TUnderlyingValue>(k, v));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            TUnderlyingValue v;
            TUnderlyingKey k;

            if (!SelectKey(key, out k))
            {
                value = default;
                return false;
            }

            bool success = _underlyingDictionary.TryGetValue(k, out v);

            value = _valueSelector(v);

            return success;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
