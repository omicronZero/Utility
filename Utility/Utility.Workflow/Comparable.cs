using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Utility
{
    public struct Comparable<T> : IComparable<Comparable<T>>, IEquatable<Comparable<T>>
        where T : IComparable<T>
    {
        public T Instance { get; }

        public Comparable(T instance)
        {
            Instance = instance;
        }

        public int CompareTo(Comparable<T> other)
        {
            if (Instance == null)
            {
                if (other.Instance != null)
                {
                    throw new ArgumentException("Comparison between null and a non-null value not possible.");
                }
                else
                {
                    return 0;
                }
            }
            else if (other.Instance == null)
                throw new ArgumentException("Comparison between null and a non-null value not possible.");

            return Instance.CompareTo(other.Instance);
        }

        public override int GetHashCode()
        {
            return Instance?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var o = obj as Comparable<T>?;

            if (o == null)
                return false;

            return Equals(o.Value);
        }

        public bool Equals(Comparable<T> other)
        {
            return Instance?.Equals(other.Instance) ?? other.Instance == null;
        }

        public override string ToString()
        {
            return Instance?.ToString() ?? base.ToString();
        }

        public static bool operator <(Comparable<T> left, Comparable<T> right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Comparable<T> left, Comparable<T> right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator ==(Comparable<T> left, Comparable<T> right)
        {
            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(Comparable<T> left, Comparable<T> right)
        {
            return left.CompareTo(right) != 0;
        }

        public static bool operator >=(Comparable<T> left, Comparable<T> right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator >(Comparable<T> left, Comparable<T> right)
        {
            return left.CompareTo(right) > 0;
        }

        public static implicit operator Comparable<T>(T value)
        {
            return new Comparable<T>(value);
        }

        public static implicit operator T(Comparable<T> value)
        {
            return value.Instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Comparable<T> Min(Comparable<T> first, Comparable<T> second)
        {
            return first <= second ? first : second;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Comparable<T> Max(Comparable<T> first, Comparable<T> second)
        {
            return first >= second ? first : second;
        }
    }
}
