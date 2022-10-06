using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Utility.ObjectDescription.ComponentModel;
using Utility;

namespace Utility.ObjectDescription
{
    public class NotifyPropertyChangedObservable<T> : IObservableObject
        where T : class, INotifyPropertyChanged
    {
        private static readonly PropertyData[] ObjectProperties = GetProperties();

        private readonly T _instance;
        private readonly CompositeObserver _observer;
        private readonly PropertyDescription[] _properties;

        public NotifyPropertyChangedObservable(T instance)
        {
            _instance = instance;
            _observer = new CompositeObserver();
            _properties = new PropertyDescription[ObjectProperties.Length];
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HashedImmuting<string> name = e.PropertyName;
            PropertyData changedProperty = default;
            bool foundChanged = false;
            int index = 0;

            foreach (PropertyData pd in ObjectProperties)
            {
                if (name == pd.Name)
                {
                    changedProperty = pd;
                    foundChanged = true;
                    break;
                }
                index++;
            }

            PropertyDescription description = _properties[index];

            if (description == null)
            {
                description = new PropertyDescription(this, changedProperty);
                _properties[index] = description;
            }

            if (foundChanged)
            {
                if (changedProperty.Property == null)
                {
                    _observer.OnPropertyChanged(description, changedProperty.Property.GetValue(_instance));
                }
                else
                {
                }
            }
        }

        public PropertyCollection GetObjectProperties()
        {
            return new PropertyCollection(this, ObjectProperties);
        }

        public IDisposable RegisterObserver(IObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            bool wasEmpty = _observer.Count == 0;

            IDisposable h = _observer.AddObserver(observer);

            observer.OnRegistered(this);

            if (wasEmpty)
                _instance.PropertyChanged += Instance_PropertyChanged;

            return Disposable.Create(() =>
            {
                h.Dispose();
                observer.OnUnregistered(this);

                if (_observer.Count == 0)
                {
                    _instance.PropertyChanged -= Instance_PropertyChanged;
                }
            }, true);
        }

        private static PropertyData[] GetProperties()
        {
            PropertyInfo[] p = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyData[] pdata = new PropertyData[p.Length];

            int dst = 0;

            for (int src = 0; src < p.Length; src++)
            {
                //only keep destination data
                if (p[src].CanRead)
                {
                    pdata[dst] = new PropertyData(p[src]);
                    dst++;
                }
            }

            if (dst < pdata.Length)
                Array.Resize(ref pdata, dst);

            return pdata;
        }

        IReadOnlyList<IObjectProperty> IObservableObject.GetObjectProperties()
        {
            return GetObjectProperties().ToObjectPropertyCollection();
        }

        internal struct PropertyData
        {
            public PropertyInfo Property { get; }
            public Indexer Indexer { get; }
            public HashedImmuting<string> Name { get; }

            public PropertyData(PropertyInfo property)
            {
                Property = property;

                ParameterInfo[] p = property.GetIndexParameters();

                if (p != null && p.Length == 0)
                    p = null;

                if (p == null)
                    Indexer = null;
                else
                    Indexer = new ReflectionIndexer(property);

                if (p == null)
                {
                    Name = property.Name;
                }
                else
                {
                    Name = property.Name + "[]";
                }
            }
        }

        public struct PropertyCollection : IReadOnlyList<PropertyDescription>
        {
            private readonly NotifyPropertyChangedObservable<T> _instance;
            private readonly PropertyData[] _properties;

            internal PropertyCollection(NotifyPropertyChangedObservable<T> instance, PropertyData[] properties)
            {
                if (instance == null)
                    throw new ArgumentNullException(nameof(instance));
                if (properties == null)
                    throw new ArgumentNullException(nameof(properties));

                _instance = instance;
                _properties = properties;
            }

            public PropertyDescription this[int index] => new PropertyDescription(_instance, _properties[index]);

            public int Count => _properties.Length;

            public IEnumerator<PropertyDescription> GetEnumerator()
            {
                PropertyCollection _this = this;
                return Enumerable.Range(0, _properties.Length).Select((i) => _this[i]).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IReadOnlyList<IObjectProperty> ToObjectPropertyCollection()
            {
                return new PropertyCastCollection(this);
            }
        }

        private sealed class PropertyCastCollection : IReadOnlyList<IObjectProperty>
        {
            private readonly PropertyCollection _collection;

            public PropertyCastCollection(PropertyCollection collection)
            {
                _collection = collection;
            }

            public IObjectProperty this[int index] => _collection[index];

            public int Count => _collection.Count;

            public IEnumerator<IObjectProperty> GetEnumerator()
            {
                return _collection.Cast<IObjectProperty>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class PropertyDescription : IObservableProperty
        {
            private readonly PropertyData _data;
            private readonly NotifyPropertyChangedObservable<T> _instance;

            internal PropertyDescription(NotifyPropertyChangedObservable<T> instance, PropertyData data)
            {
                _instance = instance;
                _data = data;
            }

            public object Value
            {
                get => _data.Indexer == null ? _data.Property.GetValue(_instance._instance) : new IndexerValues(_data.Indexer, _instance._instance);
                set
                {
                    if (!_data.Property.CanWrite || _data.Indexer != null)
                        throw new InvalidOperationException("The specified property is read-only.");

                    if (_data.Indexer == null)
                    {
                        _data.Property.SetValue(_instance._instance, value);
                    }
                }
            }

            public Func<TProperty> GetGetter<TProperty>()
            {
                if (Indexed)
                    throw new InvalidOperationException("The property is indexed.");

                return (Func<TProperty>)_data.Property.GetGetMethod().CreateDelegate(typeof(Func<TProperty>), _instance._instance);
            }

            public Action<TProperty> GetSetter<TProperty>()
            {
                if (IsReadonly)
                    throw new NotSupportedException("The property is read-only.");
                if (Indexed)
                    throw new InvalidOperationException("The property is indexed.");

                return (Action<TProperty>)_data.Property.GetSetMethod().CreateDelegate(typeof(Action<TProperty>), _instance._instance);
            }

            public bool Indexed => _data.Indexer != null;

            public bool IsReadonly => !_data.Property.CanWrite;

            public Type PropertyType => _data.Indexer == null ? _data.Property.PropertyType : typeof(IndexerValues);

            public IDisposable RegisterObserver(IObserver observer)
            {
                if (observer == null)
                    throw new ArgumentNullException(nameof(observer));

                return _instance._observer.AddObserver(Observer.FromDelegate((property, v) =>
                {
                    if (property == this)
                    {
                        observer.OnPropertyChanged(this, v);
                    }
                }));
            }
        }
    }
}
