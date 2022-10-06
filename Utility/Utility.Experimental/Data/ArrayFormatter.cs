using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utility.Data
{
    //TODO: add implementation with EmptyType
    //TODO: add primitive array

    public sealed class ArrayFormatter<TInParameters, TOutParameters>
        : TypeFormatter<ArraySizeParameter<TInParameters>, TOutParameters>
    {
        private readonly TypeFormatter<TInParameters, TOutParameters> _entryFormatter;

        public ArrayFormatter(TypeFormatter<TInParameters, TOutParameters> entryFormatter)
        {
            if (entryFormatter == null)
                throw new ArgumentNullException(nameof(entryFormatter));

            _entryFormatter = entryFormatter;
        }

        public override object Read(Stream stream, Type valueType, ArraySizeParameter<TInParameters> parameters)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));

            if (parameters.ArraySizes.Any((l) => l < 0))
                throw new ArgumentException("Non-negative array sizes expected.", nameof(parameters));

            return typeof(Impl<>).MakeGenericType(valueType).InvokeMember(
                 "ReadCore",
                 BindingFlags.Public | BindingFlags.Static,
                 null,
                 null,
                 new object[] { _entryFormatter, stream, parameters });
        }

        public override bool SupportsType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsArray && type.GetArrayRank() == 1 && _entryFormatter.SupportsType(type.GetElementType());
        }

        public override void Write(Stream stream, object value, Type valueType, TOutParameters parameters)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));

            if (!(valueType.IsArray))
                throw new ArgumentException("One-dimensional array type expected.");

            if (!valueType.IsInstanceOfType(value))
                throw new ArgumentException("Value must be an instance of the indicated value type.", nameof(valueType));

            typeof(Impl<>).MakeGenericType(valueType).InvokeMember(
                "WriteCore",
                BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                new object[] { _entryFormatter, stream, value, parameters });
        }

        private sealed class Impl<T>
        {
            public static void WriteCore(TypeFormatter<TInParameters, TOutParameters> entryFormatter, Stream stream, IEnumerable<T> value, TOutParameters parameters)
            {
                Type tp = typeof(T);
                T[] arr = value as T[];

                if (arr != null)
                {
                    for (long l = 0; l < arr.LongLength; l++)
                        entryFormatter.Write(stream, arr[l], tp, parameters);
                }
                else
                {
                    foreach (T obj in value)
                        entryFormatter.Write(stream, obj, tp, parameters);
                }
            }

            public static Array ReadCore(TypeFormatter<TInParameters, TOutParameters> entryFormatter, Stream stream, ArraySizeParameter<TInParameters> parameters)
            {
                Type tp = typeof(T);

                long instanceCount = 0;

                for (int i = 0; i < parameters.ArraySizes.Length; i++)
                    instanceCount *= parameters.ArraySizes[i];

                //TODO: test all cases
                if (parameters.ArraySizes.Length == 1)
                {
                    T[] arr = new T[parameters.ArraySizes[0]];

                    for (int i = 0; i < instanceCount; i++)
                        arr[i] = entryFormatter.Read<T>(stream, parameters.InParameters);

                    return arr;
                }
                else
                {
                    Array arr = Array.CreateInstance(typeof(T), parameters.ArraySizes);

                    if (instanceCount > 0)
                    {
                        long[] index = new long[parameters.ArraySizes.Length];

                        bool terminated;
                        do
                        {
                            arr.SetValue(entryFormatter.Read(stream, tp, parameters.InParameters), index);

                            //try to increment index to select next index

                            //TODO: test for right order in multi-dimensional arrays

                            terminated = false;

                            for (int i = 0; !terminated && i < index.Length; i++)
                            {
                                if (index[i] == parameters.ArraySizes[i] - 1) //size reached ==> increase next index
                                    index[i] = 0;
                                else
                                {
                                    index[i] += 1;
                                    terminated = true;
                                }
                            }

                        } while (terminated);
                    }

                    return arr;
                }
            }
        }
    }
}
