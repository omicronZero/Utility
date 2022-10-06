using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Utility
{
    public static partial class Util
    {
        public static void ValidateIndex<T>(T[] array, long index)
        {
            ValidateNamedIndex(array, index);
        }

        public static void ValidateNamedIndex<T>(T[] array, long index, string arrayName = "array", string indexName = "index")
        {
            if (array == null)
                throw new ArgumentNullException(arrayName);

            if (index < 0)
                throw new ArgumentOutOfRangeException(indexName, index, "Non-negative index expected.");
            if (index >= array.Length)
                throw new ArgumentOutOfRangeException(indexName, "Index is outside the bounds of the array.");
        }

        public static void ValidateIndex(long index, long length)
        {
            ValidateNamedIndex(index, length);
        }

        public static void ValidateNamedIndex(long index, long length, string indexName = "index", string lengthName = "length")
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(indexName, index, "Non-negative index expected.");
            if (length < 0)
                throw new ArgumentOutOfRangeException(lengthName, length, "Non-negative length expected.");
            if (index >= length)
                throw new ArgumentOutOfRangeException(indexName, "Index is outside the bounds of the array.");
        }

        public static void ValidateRange<T>(T[] array, long index, long count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative index expected.");
            else if (index >= array.LongLength)
                throw new ArgumentOutOfRangeException(nameof(index), "Index does not fall into the buffer's boundaries.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative count expected.");

            if (index + count > array.Length)
                throw new ArgumentException("Indicated range exceeds the buffer's boundaries.");
        }

        public static void ValidateRange(long index, long count, long length)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative index expected.");
            else if (index >= length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index does not fall into the buffer's boundaries.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative count expected.");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Non-negative length expected.");

            if (index + count > length)
                throw new ArgumentException("Indicated range exceeeds the buffer's boundaries.");
        }

        public static void ValidateNamedRange<T>(T[] array,
            long index,
            long count,
            string arrayName = "array",
            string indexName = "index",
            string countName = "count")
        {
            if (array == null)
                throw new ArgumentNullException(arrayName);

            if (index < 0)
                throw new ArgumentOutOfRangeException(indexName, "Non-negative index expected.");
            else if (index >= array.LongLength)
                throw new ArgumentOutOfRangeException(indexName, "Index does not fall into the buffer's boundaries.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(countName, "Non-negative count expected.");

            if (index + count > array.LongLength)
                throw new ArgumentException("Indicated range exceeds the buffer's boundaries.");
        }

        public static void ValidateNamedRange(
            long index,
            long count,
            long length,
            string indexName = "index",
            string countName = "count",
            string lengthName = "length")
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(indexName, "Non-negative index expected.");
            else if (index >= length)
                throw new ArgumentOutOfRangeException(indexName, "Index does not fall into the buffer's boundaries.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(countName, "Non-negative count expected.");

            if (length < 0)
                throw new ArgumentOutOfRangeException(lengthName, "Non-negative length expected.");

            if (index + count > length)
                throw new ArgumentException("Indicated range exceeeds the buffer's boundaries.");
        }

        internal static T[] SelectNotNull<T>(T[] values, Predicate<T> predicate, bool enforceCopy = false)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (values == null)
                return null;

            List<T> l = null;

            for (int i = 0; i < values.Length; i++)
            {
                T v = values[i];

                if (predicate(v))
                {
                    if (l != null)
                    {
                        l.Add(values[i]);
                    }
                }
                else
                {
                    if (l == null)
                    {
                        l = new List<T>();

                        for (int j = 0; j < i; j++)
                            l.Add(values[i]);
                    }
                }
            }

            return l?.ToArray() ?? (enforceCopy ? (T[])values.Clone() : values);
        }
    }
}
