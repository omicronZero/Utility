using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections;
using Utility.Collections;

namespace Utility.ObjectDescription
{
    public sealed class PropertyIDTable : PropertyAccessorTable
    {
        private static readonly ConcurrentDictionary<Type, PropertyIDTable> _table;

        private PropertyIDTable _baseTable;
        private Lazy<PropertyIDTable>[] _interfaceTables;
        private Dictionary<AccessorKey, PropertyID> _properties;

        new public IList<PropertyID> Properties { get; }

        public Type Type { get; }

        private PropertyIDTable(Type type, PropertyIDTable baseTable, Lazy<PropertyIDTable>[] interfaceTables)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            Type = type;
            _baseTable = baseTable;
            _interfaceTables = interfaceTables.Length == 0 ? Array.Empty<Lazy<PropertyIDTable>>() : interfaceTables; //ensure that the finalizer may clean up the empty array
            _properties = new Dictionary<AccessorKey, PropertyID>();

            int ind = 0;

            foreach (PropertyInfo p in properties)
            {
                if (p.GetIndexParameters()?.Length > 0 || !(p.GetMethod.IsPublic || p.SetMethod.IsPublic))
                    continue;

                var id = new PropertyID(this, p, ind);

                _properties.Add(new AccessorKey(p.Name), id);
                _properties.Add(new AccessorKey(ind), id);

                ind++;
            }

            Properties =  new PropertyList(this);
        }

        public override bool IsProperty(PropertyAccessor accessor)
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));

            var pid = accessor as PropertyID;

            if (pid == null)
                return false;

            return pid.Table == this;
        }

        public PropertyIDTable GetInterfaceTable(Type interfaceType)
        {
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            return _interfaceTables.FirstOrDefault((s) => s.Value.Type == interfaceType).Value;
        }

        private int EntryCount => _properties.Count / 2;

        new public PropertyID GetAccessor(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            PropertyID pid;

            if (!_properties.TryGetValue(new AccessorKey(name), out pid))
            {
                if (_baseTable != null)
                {
                    pid = _baseTable.GetAccessor(name);
                }
                else
                {
                    pid = null;
                }
            }

            return pid;
        }

        protected override PropertyAccessor GetAccessorCore(string name)
        {
            return GetAccessor(name);
        }

        protected override IList<PropertyAccessor> PropertiesCore => new ListAccessor<PropertyAccessor>((i) => Properties[i], () => Properties.Count, false);

        public IList<PropertyIDTable> GetInterfaceTables()
        {
            return SelectorList.SelectList(_interfaceTables, (s) => s.Value);
        }

        static PropertyIDTable()
        {
            _table = new ConcurrentDictionary<Type, PropertyIDTable>();
        }

        public static PropertyIDTable Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return _table.GetOrAdd(type, (tp) =>
            {
                return new PropertyIDTable(
                    tp,
                    tp == typeof(object) ? null : Get(tp.BaseType),
                    Array.ConvertAll(type.GetInterfaces(), (ifc) => new Lazy<PropertyIDTable>(() => Get(ifc))));
            });
        }

        private struct AccessorKey : IEquatable<AccessorKey>
        {
            public int Index { get; }
            public string Name { get; }

            public override int GetHashCode()
            {
                return Index;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                var k = obj as AccessorKey?;

                if (k == null)
                    return false;

                return Equals(k.Value);
            }

            public bool Equals(AccessorKey other)
            {
                return Index == other.Index && Name == other.Name;
            }

            public AccessorKey(string name)
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                Name = name;
                Index = name.GetHashCode();
            }

            public AccessorKey(int index)
            {
                Index = index;
                Name = null;
            }
        }

        private sealed class PropertyList : IList<PropertyID>
        {
            public PropertyIDTable Instance { get; }

            public PropertyList(PropertyIDTable instance)
            {
                if (instance == null)
                    throw new ArgumentNullException(nameof(instance));

                Instance = instance;
            }

            public PropertyID this[int index]
            {
                get => Instance._properties[new AccessorKey(index)];
                set => throw ReadonlyException();
            }

            public int Count => Instance._properties.Count;

            public bool IsReadOnly => true;

            public bool Contains(PropertyID item)
            {
                if (item == null)
                    return false;

                return item.Table == Instance && Instance._properties.Contains(new KeyValuePair<AccessorKey, PropertyID>(new AccessorKey(item.Index), item));
            }

            public void CopyTo(PropertyID[] array, int arrayIndex)
            {
                Instance._properties.Values.CopyTo(array, arrayIndex);
            }

            public IEnumerator<PropertyID> GetEnumerator()
            {
                return Instance._properties.Values.GetEnumerator();
            }

            public int IndexOf(PropertyID item)
            {
                if (item == null || item.Table != Instance)
                    return -1;

                return item.Index;
            }

            public void Add(PropertyID item)
            {
                throw ReadonlyException();
            }

            public void Clear()
            {
                throw ReadonlyException();
            }

            public void Insert(int index, PropertyID item)
            {
                throw ReadonlyException();
            }

            public bool Remove(PropertyID item)
            {
                throw ReadonlyException();
            }

            public void RemoveAt(int index)
            {
                throw ReadonlyException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static Exception ReadonlyException()
            {
                return new NotSupportedException("The current list is read-only.");
            }
        }
    }
}
