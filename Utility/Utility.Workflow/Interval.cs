using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility
{
    [Serializable]
    public struct Interval<T> : ISerializable
        where T : IComparable<T>
    {
        public static Interval<T> Empty { get => new Interval<T>(default, default, false, false); }

        public T Start { get; set; }
        public T End { get; set; }
        public IntervalBoundaryProperties BoundaryProperties { get; set; }

        public Interval(T start, T end, IntervalBoundaryProperties boundaryProperties)
        {
            Start = start;
            End = end;
            BoundaryProperties = boundaryProperties;
        }

        public Interval(T start, T end, bool includeStart, bool includeEnd)
            : this(start, end, (includeStart ? IntervalBoundaryProperties.IncludeLowerBound : 0) | (includeEnd ? IntervalBoundaryProperties.IncludeUpperBound : 0))
        { }

        public Interval(T startInclusive, T endInclusive)
            : this(startInclusive, endInclusive, IntervalBoundaryProperties.IncludeLowerBound | IntervalBoundaryProperties.IncludeUpperBound)
        { }

        public Interval(Range<T> range, IntervalBoundaryProperties boundaryProperties)
            : this(range.Start, range.End, boundaryProperties)
        { }

        public Interval(Range<T> range, bool includeStart, bool includeEnd)
            : this(range.Start, range.End, includeStart, includeEnd)
        { }

        public Interval(Range<T> range)
            : this(range.Start, range.End, IntervalBoundaryProperties.IncludeLowerBound | IntervalBoundaryProperties.IncludeUpperBound)
        { }

        private Interval(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            Start = info.GetValue<T>(nameof(Start));
            End = info.GetValue<T>(nameof(End));
            BoundaryProperties = info.GetValue<IntervalBoundaryProperties>(nameof(BoundaryProperties));
        }

        private void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(Start), Start);
            info.AddValue(nameof(End), End);
            info.AddValue(nameof(BoundaryProperties), BoundaryProperties);
        }

        public Range<T> Range => new Range<T>(Start, End);
        public Comparable<T> StartComparable => Start;
        public Comparable<T> EndComparable => End;

        public bool IncludeStart
        {
            get => (BoundaryProperties & IntervalBoundaryProperties.IncludeLowerBound) == IntervalBoundaryProperties.IncludeLowerBound;
            set
            {
                BoundaryProperties = (BoundaryProperties & ~IntervalBoundaryProperties.IncludeLowerBound) | (value ? IntervalBoundaryProperties.IncludeLowerBound : 0);
            }
        }
        public bool IncludeEnd
        {
            get => (BoundaryProperties & IntervalBoundaryProperties.IncludeUpperBound) == IntervalBoundaryProperties.IncludeUpperBound;
            set
            {
                BoundaryProperties = (BoundaryProperties & ~IntervalBoundaryProperties.IncludeUpperBound) | (value ? IntervalBoundaryProperties.IncludeUpperBound : 0);
            }
        }

        public bool IsEmpty
        {
            get
            {
                int c = StartComparable.CompareTo(EndComparable);

                //if start and end refer to a single value and is excluded by one of the boundaries, the interval is empty
                if (c == 0)
                    return (BoundaryProperties & (IntervalBoundaryProperties.IncludeLowerBound | IntervalBoundaryProperties.IncludeUpperBound)) != (IntervalBoundaryProperties.IncludeLowerBound | IntervalBoundaryProperties.IncludeUpperBound);
                else
                    return false;
            }
        }

        public bool Contains(T value)
        {
            if (IncludeStart)
            {
                if (StartComparable > value)
                    return false;
            }
            else
            {
                if (StartComparable >= value)
                    return false;
            }

            if (IncludeEnd)
            {
                if (EndComparable < value)
                    return false;
            }
            else
            {
                if (EndComparable <= value)
                    return false;
            }
            return true;
        }

        public void Normalize()
        {
            if (EndComparable < StartComparable)
            {
                this = new Interval<T>(End, Start, IncludeEnd, IncludeStart);
            }
        }

        public bool Contains(Interval<T> other)
        {
            if (!IncludeStart && other.IncludeStart)
            {
                if (StartComparable >= other.StartComparable)
                    return false;
            }
            else if (StartComparable > other.StartComparable)
                return false;

            if (!IncludeEnd && other.IncludeEnd)
            {
                if (EndComparable <= other.EndComparable)
                    return false;
            }
            else if (EndComparable < other.EndComparable)
                return false;

            return true;
        }

        public bool Intersects(Interval<T> other)
        {
            //<, > denote any bracket
            //<a, b> & (b, c> is empty
            //<a, b) & <b, c> is empty
            //<a, b] & [b, c> is b

            if (!IncludeEnd || !other.IncludeStart)
            {
                if (EndComparable <= other.StartComparable)
                    return false;
            }
            else if (EndComparable < other.StartComparable)
                return false;

            if (!IncludeStart || !other.IncludeEnd)
            {
                if (StartComparable >= other.EndComparable)
                    return false;
            }
            else if (StartComparable > other.EndComparable)
                return false;

            return true;
        }

        public void Union(Interval<T> other)
        {
            if (IsEmpty)
            {
                this = other;
                return;
            }
            else if (other.IsEmpty)
                return;

            int c = other.Start.CompareTo(Start);

            if (c < 0)
            {
                Start = other.Start;
                IncludeStart = other.IncludeStart;
            }
            else if (c == 0)
            {
                Start = other.Start;
                IncludeStart |= other.IncludeStart;
            }

            c = other.End.CompareTo(End);
            if (c > 0)
            {
                End = other.End;
                IncludeEnd = other.IncludeEnd;
            }
            else if (c == 0)
            {
                End = other.End;
                IncludeEnd |= other.IncludeEnd;
            }
        }

        public void Union(T value)
        {
            if (IsEmpty)
            {
                Start = value;
                End = value;
                IncludeStart = true;
                IncludeEnd = true;
                return;
            }

            int c = Start.CompareTo(value);

            if (c > 0)
            {
                Start = value;
                IncludeStart = true;
            }
            else if (c == 0)
            {
                Start = value;
                IncludeStart = true;
            }

            c = End.CompareTo(value);
            if (c < 0)
            {
                End = value;
                IncludeEnd = true;
            }
            else if (c == 0)
            {
                End = value;
                IncludeEnd = true;
            }
        }

        public void Intersect(Interval<T> other)
        {
            if (IsEmpty)
                return;
            else if (other.IsEmpty)
            {
                this = Empty;
                return;
            }

            int c = Start.CompareTo(other.Start);

            if (c > 0)
            {
                Start = other.Start;
                IncludeStart = other.IncludeStart;
            }
            else if (c >= 0)
            {
                Start = other.Start;
                IncludeStart &= other.IncludeStart;
            }

            c = End.CompareTo(other.End);
            if (c < 0)
            {
                End = other.End;
                IncludeEnd = other.IncludeEnd;
            }
            else if (c <= 0)
            {
                End = other.End;
                IncludeEnd &= other.IncludeEnd;
            }
        }

        public override int GetHashCode()
        {
            if (IsEmpty)
                return 0;

            return Start.GetHashCode() ^ ~End.GetHashCode() ^ BoundaryProperties.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Interval<T> interval && Equals(interval);
        }

        public override string ToString()
        {
            if (IsEmpty)
                return "{ }";
            else if (StartComparable == EndComparable)
                return "{" + Start?.ToString() ?? string.Empty + "}";

            return (IncludeStart ? "[" : "(") + Start + ", ..., " + End + (IncludeEnd ? "]" : ")");
        }

        public bool Equals(Interval<T> other)
        {
            if (IsEmpty)
                return other.IsEmpty;
            else if (other.IsEmpty)
                return false;

            return StartComparable == other.StartComparable && EndComparable == other.EndComparable && BoundaryProperties == other.BoundaryProperties;
        }

        public static explicit operator Interval<T>(Range<T> range)
        {
            return new Interval<T>(range);
        }

        public static bool operator ==(Interval<T> left, Interval<T> right)
        {
            if (left.IsEmpty)
                return right.IsEmpty;
            else if (right.IsEmpty)
                return false;

            return left.StartComparable == right.StartComparable && left.EndComparable == right.EndComparable && left.BoundaryProperties == right.BoundaryProperties;
        }

        public static bool operator !=(Interval<T> left, Interval<T> right)
        {
            if (left.IsEmpty)
                return !right.IsEmpty;
            else if (right.IsEmpty)
                return true;

            return left.StartComparable != right.StartComparable || left.EndComparable != right.EndComparable || left.BoundaryProperties != right.BoundaryProperties;
        }

        public static Interval<T> Value(T value)
        {
            return new Interval<T>(value, value, IntervalBoundaryProperties.IncludeLowerBound | IntervalBoundaryProperties.IncludeUpperBound);
        }

        public static Interval<T> CreateNormalized(T first, T second, bool includeFirst, bool includeSecond)
        {
            if (new Comparable<T>(first) <= second)
            {
                return new Interval<T>(first, second, includeFirst, includeSecond);
            }
            else
            {
                return new Interval<T>(second, first, includeSecond, includeFirst);
            }
        }

        public static Interval<T> CreateNormalized(T first, T second) => CreateNormalized(first, second, true, true);

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }
    }
}
