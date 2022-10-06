using System;
using System.Collections.Generic;

namespace Utility
{
    [Serializable]
    public struct HashedImmuting<T> : IEquatable<HashedImmuting<T>>
    {
        private readonly int _hashCode;

        public T Instance { get; }

        public HashedImmuting(T instance)
        {
            Instance = instance;
            _hashCode = instance?.GetHashCode() ?? 0;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(HashedImmuting<T> other)
        {
            return _hashCode == other._hashCode && EqualityComparer<T>.Default.Equals(Instance, other.Instance);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is HashedImmuting<T> immuting && Equals(immuting);
        }

        public static implicit operator HashedImmuting<T>(T instance)
        {
            return new HashedImmuting<T>(instance);
        }

        public static explicit operator T(HashedImmuting<T> instance)
        {
            return instance.Instance;
        }

        public static bool operator ==(HashedImmuting<T> left, HashedImmuting<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HashedImmuting<T> left, HashedImmuting<T> right)
        {
            return !left.Equals(right);
        }
    }
}
