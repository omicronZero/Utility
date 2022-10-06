using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Utility
{
    /// <summary>
    /// Provides an implementation of the <see cref="IEqualityComparer{T}"/> type that allows for a comparison of references of instances of types.
    /// For value types, the default implementation provided by <see cref="EqualityComparer{T}.Default"/> is used.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ReferenceComparer<T> : IEqualityComparer<T>
    {
        private static readonly bool IsValueType = typeof(T).IsValueType;

        public static ReferenceComparer<T> Default { get; } = new ReferenceComparer<T>();

        public bool Equals(T x, T y)
        {
            return IsValueType ? EqualityComparer<T>.Default.Equals(x, y) : object.ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return IsValueType ? EqualityComparer<T>.Default.GetHashCode(obj) : RuntimeHelpers.GetHashCode(obj);
        }
    }
}
