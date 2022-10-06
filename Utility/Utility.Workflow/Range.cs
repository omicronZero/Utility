using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    [Serializable]
    public struct Range<T> where T : IComparable<T>
    {
        public T Start { get; set; }
        public T End { get; set; }

        public Comparable<T> StartComparable
        {
            get => Start;
            set => Start = value;
        }

        public Comparable<T> EndComparable
        {
            get => End;
            set => End = value;
        }

        public Range(T start, T end)
        {
            Start = start;
            End = end;
        }

        public bool IsEmpty
        {
            get { return StartComparable == EndComparable; }
        }

        public bool Contains(T value)
        {
            return StartComparable <= value && value <= EndComparable;
        }

        public void Normalize()
        {
            this = new Range<T>(StartComparable < EndComparable ? StartComparable : EndComparable, StartComparable < EndComparable ? EndComparable : StartComparable);
        }

        public bool Contains(Range<T> other)
        {
            return StartComparable <= other.StartComparable && other.EndComparable <= EndComparable;
        }

        public bool Contains(T start, T end) => Contains(new Range<T>(start, end));

        public bool Intersects(Range<T> other)
        {
            return StartComparable <= other.EndComparable && other.StartComparable <= EndComparable;
        }

        public void Intersect(T start, T end) => Intersect(new Range<T>(start, end));

        public bool Intersects(T start, T end) => Intersects(new Range<T>(start, end));

        public void Union(Range<T> other)
        {
            this = new Range<T>(StartComparable < other.StartComparable ? StartComparable : other.StartComparable, EndComparable > other.EndComparable ? EndComparable : other.EndComparable);
        }

        public void Union(T start, T end) => Union(new Range<T>(start, end));

        public void Union(T value)
        {
            if (value < StartComparable)
                StartComparable = value;

            if (value > EndComparable)
                EndComparable = value;
        }

        public override int GetHashCode()
        {
            return StartComparable.GetHashCode() ^ ~EndComparable.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var o = obj as Range<T>?;

            if (o == null)
                return false;

            return Equals(o.Value);
        }

        public void Intersect(Range<T> other)
        {
            this = new Range<T>(StartComparable < other.StartComparable ? other.StartComparable : StartComparable, EndComparable > other.EndComparable ? other.EndComparable : EndComparable);
        }

        public bool Equals(Range<T> other)
        {
            return StartComparable == other.StartComparable && EndComparable == other.EndComparable;
        }

        public static bool operator ==(Range<T> left, Range<T> right)
        {
            return left.StartComparable == right.StartComparable && left.EndComparable == right.EndComparable;
        }

        public static bool operator !=(Range<T> left, Range<T> right)
        {
            return left.StartComparable != right.StartComparable || left.EndComparable != right.EndComparable;
        }

        public static Range<T> CreateNormalized(T first, T second)
        {
            return new Range<T>(Comparable<T>.Min(first, second), Comparable<T>.Max(first, second));
        }

        public static Range<T> FromEnumerable(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            Comparable<T> start;
            Comparable<T> end;

            using (var enr = enumerable.GetEnumerator())
            {
                if (!enr.MoveNext())
                {
                    throw new ArgumentException("The specified enumeration was empty.", nameof(enumerable));
                }

                start = enr.Current;
                end = enr.Current;

                while (enr.MoveNext())
                {
                    if (enr.Current < start)
                        start = enr.Current;

                    if (enr.Current > end)
                        end = enr.Current;
                }
            }

            return new Range<T>(start, end);
        }
    }
}
