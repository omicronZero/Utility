
using System;
using System.Collections.Generic;

namespace Utility
{

	[Serializable]
	public partial struct RangeChar : IEquatable<RangeChar>
	{
		public char Start { get; set; }
		public char End { get; set; }
		public int Span => (int)(End - Start);

		public RangeChar(char start, char end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(char value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeChar(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeChar other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(char start, char end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeChar other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(char start, char end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeChar other)
		{
			this = new RangeChar(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(char start, char end)
		{
			this = new RangeChar(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(char value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeChar?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"{{{Start}, ..., {End}}}";
		}

		public void Intersect(RangeChar other)
		{
			this = new RangeChar(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(char start, char end)
		{
			this = new RangeChar(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeChar other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<char>(RangeChar range)
		{
			return new Range<char>(range.Start, range.End);
		}

		public static implicit operator RangeChar(Range<char> range)
		{
			return new RangeChar(range.Start, range.End);
		}

		public static bool operator==(RangeChar left, RangeChar right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeChar left, RangeChar right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeChar CreateNormalized(char first, char second)
        {
            return new RangeChar(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeChar CreateSpanning(char start, int width)
		{
					if (width < 0)
				return new RangeChar((char)(start + width), (char)(start - 2 * width));
			else
							return new RangeChar(start, (char)(start + width));
		}

        public static RangeChar FromEnumerable(IEnumerable<char> enumerable)
        {
            char start;
            char end;

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

            return new RangeChar(start, end);
        }
	}
		[Serializable]
	public partial struct RangeInt16 : IEquatable<RangeInt16>
	{
		public short Start { get; set; }
		public short End { get; set; }
		public short Span => (short)(End - Start);

		public RangeInt16(short start, short end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(short value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeInt16(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeInt16 other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(short start, short end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeInt16 other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(short start, short end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeInt16 other)
		{
			this = new RangeInt16(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(short start, short end)
		{
			this = new RangeInt16(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(short value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeInt16?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"{{{Start}, ..., {End}}}";
		}

		public void Intersect(RangeInt16 other)
		{
			this = new RangeInt16(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(short start, short end)
		{
			this = new RangeInt16(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeInt16 other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<short>(RangeInt16 range)
		{
			return new Range<short>(range.Start, range.End);
		}

		public static implicit operator RangeInt16(Range<short> range)
		{
			return new RangeInt16(range.Start, range.End);
		}

		public static bool operator==(RangeInt16 left, RangeInt16 right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeInt16 left, RangeInt16 right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeInt16 CreateNormalized(short first, short second)
        {
            return new RangeInt16(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeInt16 CreateSpanning(short start, short width)
		{
					if (width < 0)
				return new RangeInt16((short)(start + width), (short)(start - 2 * width));
			else
							return new RangeInt16(start, (short)(start + width));
		}

        public static RangeInt16 FromEnumerable(IEnumerable<short> enumerable)
        {
            short start;
            short end;

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

            return new RangeInt16(start, end);
        }
	}
		[Serializable]
	public partial struct RangeUInt16 : IEquatable<RangeUInt16>
	{
		public ushort Start { get; set; }
		public ushort End { get; set; }
		public ushort Span => (ushort)(End - Start);

		public RangeUInt16(ushort start, ushort end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(ushort value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeUInt16(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeUInt16 other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(ushort start, ushort end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeUInt16 other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(ushort start, ushort end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeUInt16 other)
		{
			this = new RangeUInt16(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(ushort start, ushort end)
		{
			this = new RangeUInt16(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(ushort value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeUInt16?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"{{{Start}, ..., {End}}}";
		}

		public void Intersect(RangeUInt16 other)
		{
			this = new RangeUInt16(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(ushort start, ushort end)
		{
			this = new RangeUInt16(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeUInt16 other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<ushort>(RangeUInt16 range)
		{
			return new Range<ushort>(range.Start, range.End);
		}

		public static implicit operator RangeUInt16(Range<ushort> range)
		{
			return new RangeUInt16(range.Start, range.End);
		}

		public static bool operator==(RangeUInt16 left, RangeUInt16 right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeUInt16 left, RangeUInt16 right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeUInt16 CreateNormalized(ushort first, ushort second)
        {
            return new RangeUInt16(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeUInt16 CreateSpanning(ushort start, ushort width)
		{
						return new RangeUInt16(start, (ushort)(start + width));
		}

        public static RangeUInt16 FromEnumerable(IEnumerable<ushort> enumerable)
        {
            ushort start;
            ushort end;

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

            return new RangeUInt16(start, end);
        }
	}
		[Serializable]
	public partial struct RangeInt32 : IEquatable<RangeInt32>
	{
		public int Start { get; set; }
		public int End { get; set; }
		public int Span => (int)(End - Start);

		public RangeInt32(int start, int end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(int value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeInt32(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeInt32 other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(int start, int end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeInt32 other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(int start, int end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeInt32 other)
		{
			this = new RangeInt32(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(int start, int end)
		{
			this = new RangeInt32(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(int value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeInt32?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"{{{Start}, ..., {End}}}";
		}

		public void Intersect(RangeInt32 other)
		{
			this = new RangeInt32(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(int start, int end)
		{
			this = new RangeInt32(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeInt32 other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<int>(RangeInt32 range)
		{
			return new Range<int>(range.Start, range.End);
		}

		public static implicit operator RangeInt32(Range<int> range)
		{
			return new RangeInt32(range.Start, range.End);
		}

		public static bool operator==(RangeInt32 left, RangeInt32 right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeInt32 left, RangeInt32 right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeInt32 CreateNormalized(int first, int second)
        {
            return new RangeInt32(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeInt32 CreateSpanning(int start, int width)
		{
					if (width < 0)
				return new RangeInt32((int)(start + width), (int)(start - 2 * width));
			else
							return new RangeInt32(start, (int)(start + width));
		}

        public static RangeInt32 FromEnumerable(IEnumerable<int> enumerable)
        {
            int start;
            int end;

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

            return new RangeInt32(start, end);
        }
	}
		[Serializable]
	public partial struct RangeUInt32 : IEquatable<RangeUInt32>
	{
		public uint Start { get; set; }
		public uint End { get; set; }
		public uint Span => (uint)(End - Start);

		public RangeUInt32(uint start, uint end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(uint value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeUInt32(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeUInt32 other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(uint start, uint end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeUInt32 other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(uint start, uint end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeUInt32 other)
		{
			this = new RangeUInt32(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(uint start, uint end)
		{
			this = new RangeUInt32(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(uint value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeUInt32?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"{{{Start}, ..., {End}}}";
		}

		public void Intersect(RangeUInt32 other)
		{
			this = new RangeUInt32(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(uint start, uint end)
		{
			this = new RangeUInt32(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeUInt32 other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<uint>(RangeUInt32 range)
		{
			return new Range<uint>(range.Start, range.End);
		}

		public static implicit operator RangeUInt32(Range<uint> range)
		{
			return new RangeUInt32(range.Start, range.End);
		}

		public static bool operator==(RangeUInt32 left, RangeUInt32 right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeUInt32 left, RangeUInt32 right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeUInt32 CreateNormalized(uint first, uint second)
        {
            return new RangeUInt32(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeUInt32 CreateSpanning(uint start, uint width)
		{
						return new RangeUInt32(start, (uint)(start + width));
		}

        public static RangeUInt32 FromEnumerable(IEnumerable<uint> enumerable)
        {
            uint start;
            uint end;

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

            return new RangeUInt32(start, end);
        }
	}
		[Serializable]
	public partial struct RangeInt64 : IEquatable<RangeInt64>
	{
		public long Start { get; set; }
		public long End { get; set; }
		public long Span => (long)(End - Start);

		public RangeInt64(long start, long end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(long value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeInt64(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeInt64 other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(long start, long end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeInt64 other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(long start, long end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeInt64 other)
		{
			this = new RangeInt64(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(long start, long end)
		{
			this = new RangeInt64(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(long value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeInt64?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"{{{Start}, ..., {End}}}";
		}

		public void Intersect(RangeInt64 other)
		{
			this = new RangeInt64(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(long start, long end)
		{
			this = new RangeInt64(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeInt64 other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<long>(RangeInt64 range)
		{
			return new Range<long>(range.Start, range.End);
		}

		public static implicit operator RangeInt64(Range<long> range)
		{
			return new RangeInt64(range.Start, range.End);
		}

		public static bool operator==(RangeInt64 left, RangeInt64 right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeInt64 left, RangeInt64 right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeInt64 CreateNormalized(long first, long second)
        {
            return new RangeInt64(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeInt64 CreateSpanning(long start, long width)
		{
					if (width < 0)
				return new RangeInt64((long)(start + width), (long)(start - 2 * width));
			else
							return new RangeInt64(start, (long)(start + width));
		}

        public static RangeInt64 FromEnumerable(IEnumerable<long> enumerable)
        {
            long start;
            long end;

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

            return new RangeInt64(start, end);
        }
	}
		[Serializable]
	public partial struct RangeUInt64 : IEquatable<RangeUInt64>
	{
		public ulong Start { get; set; }
		public ulong End { get; set; }
		public ulong Span => (ulong)(End - Start);

		public RangeUInt64(ulong start, ulong end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(ulong value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeUInt64(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeUInt64 other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(ulong start, ulong end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeUInt64 other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(ulong start, ulong end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeUInt64 other)
		{
			this = new RangeUInt64(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(ulong start, ulong end)
		{
			this = new RangeUInt64(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(ulong value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeUInt64?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"{{{Start}, ..., {End}}}";
		}

		public void Intersect(RangeUInt64 other)
		{
			this = new RangeUInt64(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(ulong start, ulong end)
		{
			this = new RangeUInt64(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeUInt64 other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<ulong>(RangeUInt64 range)
		{
			return new Range<ulong>(range.Start, range.End);
		}

		public static implicit operator RangeUInt64(Range<ulong> range)
		{
			return new RangeUInt64(range.Start, range.End);
		}

		public static bool operator==(RangeUInt64 left, RangeUInt64 right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeUInt64 left, RangeUInt64 right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeUInt64 CreateNormalized(ulong first, ulong second)
        {
            return new RangeUInt64(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeUInt64 CreateSpanning(ulong start, ulong width)
		{
						return new RangeUInt64(start, (ulong)(start + width));
		}

        public static RangeUInt64 FromEnumerable(IEnumerable<ulong> enumerable)
        {
            ulong start;
            ulong end;

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

            return new RangeUInt64(start, end);
        }
	}
		[Serializable]
	public partial struct RangeSingle : IEquatable<RangeSingle>
	{
		public float Start { get; set; }
		public float End { get; set; }
		public float Span => (float)(End - Start);

		public RangeSingle(float start, float end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(float value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeSingle(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeSingle other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(float start, float end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeSingle other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(float start, float end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeSingle other)
		{
			this = new RangeSingle(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(float start, float end)
		{
			this = new RangeSingle(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(float value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeSingle?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"[{Start}, ..., {End}]";
		}

		public void Intersect(RangeSingle other)
		{
			this = new RangeSingle(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(float start, float end)
		{
			this = new RangeSingle(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeSingle other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<float>(RangeSingle range)
		{
			return new Range<float>(range.Start, range.End);
		}

		public static implicit operator RangeSingle(Range<float> range)
		{
			return new RangeSingle(range.Start, range.End);
		}

		public static bool operator==(RangeSingle left, RangeSingle right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeSingle left, RangeSingle right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeSingle CreateNormalized(float first, float second)
        {
            return new RangeSingle(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeSingle CreateSpanning(float start, float width)
		{
					if (width < 0)
				return new RangeSingle((float)(start + width), (float)(start - 2 * width));
			else
							return new RangeSingle(start, (float)(start + width));
		}

        public static RangeSingle FromEnumerable(IEnumerable<float> enumerable)
        {
            float start;
            float end;

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

            return new RangeSingle(start, end);
        }
	}
		[Serializable]
	public partial struct RangeDouble : IEquatable<RangeDouble>
	{
		public double Start { get; set; }
		public double End { get; set; }
		public double Span => (double)(End - Start);

		public RangeDouble(double start, double end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(double value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeDouble(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeDouble other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(double start, double end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeDouble other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(double start, double end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeDouble other)
		{
			this = new RangeDouble(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(double start, double end)
		{
			this = new RangeDouble(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(double value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeDouble?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"[{Start}, ..., {End}]";
		}

		public void Intersect(RangeDouble other)
		{
			this = new RangeDouble(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(double start, double end)
		{
			this = new RangeDouble(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeDouble other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<double>(RangeDouble range)
		{
			return new Range<double>(range.Start, range.End);
		}

		public static implicit operator RangeDouble(Range<double> range)
		{
			return new RangeDouble(range.Start, range.End);
		}

		public static bool operator==(RangeDouble left, RangeDouble right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeDouble left, RangeDouble right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeDouble CreateNormalized(double first, double second)
        {
            return new RangeDouble(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeDouble CreateSpanning(double start, double width)
		{
					if (width < 0)
				return new RangeDouble((double)(start + width), (double)(start - 2 * width));
			else
							return new RangeDouble(start, (double)(start + width));
		}

        public static RangeDouble FromEnumerable(IEnumerable<double> enumerable)
        {
            double start;
            double end;

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

            return new RangeDouble(start, end);
        }
	}
		[Serializable]
	public partial struct RangeDecimal : IEquatable<RangeDecimal>
	{
		public decimal Start { get; set; }
		public decimal End { get; set; }
		public decimal Span => (decimal)(End - Start);

		public RangeDecimal(decimal start, decimal end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(decimal value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeDecimal(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeDecimal other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(decimal start, decimal end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeDecimal other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(decimal start, decimal end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeDecimal other)
		{
			this = new RangeDecimal(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(decimal start, decimal end)
		{
			this = new RangeDecimal(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(decimal value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeDecimal?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"[{Start}, ..., {End}]";
		}

		public void Intersect(RangeDecimal other)
		{
			this = new RangeDecimal(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(decimal start, decimal end)
		{
			this = new RangeDecimal(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeDecimal other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<decimal>(RangeDecimal range)
		{
			return new Range<decimal>(range.Start, range.End);
		}

		public static implicit operator RangeDecimal(Range<decimal> range)
		{
			return new RangeDecimal(range.Start, range.End);
		}

		public static bool operator==(RangeDecimal left, RangeDecimal right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeDecimal left, RangeDecimal right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeDecimal CreateNormalized(decimal first, decimal second)
        {
            return new RangeDecimal(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeDecimal CreateSpanning(decimal start, decimal width)
		{
					if (width < 0)
				return new RangeDecimal((decimal)(start + width), (decimal)(start - 2 * width));
			else
							return new RangeDecimal(start, (decimal)(start + width));
		}

        public static RangeDecimal FromEnumerable(IEnumerable<decimal> enumerable)
        {
            decimal start;
            decimal end;

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

            return new RangeDecimal(start, end);
        }
	}
		[Serializable]
	public partial struct RangeDateTime : IEquatable<RangeDateTime>
	{
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public TimeSpan Span => (TimeSpan)(End - Start);

		public RangeDateTime(DateTime start, DateTime end)
		{
			Start = start;
			End = end;
		}

		public bool IsEmpty
		{
			get { return Start == End; }
		}

		public bool Contains(DateTime value)
		{
			return Start <= value && value <= End;
		}

		public void Normalize()
		{
			this = new RangeDateTime(Start < End ? Start : End, Start < End ? End : Start);
		}

		public bool Contains(RangeDateTime other)
		{
			return Start <= other.Start && other.End <= End;
		}

		public bool Contains(DateTime start, DateTime end)
		{
			return Start <= start && end <= End;
		}

		public bool Intersects(RangeDateTime other)
		{
			return Start <= other.End && other.Start <= End;
		}

		public bool Intersects(DateTime start, DateTime end)
		{
			return Start <= end && start <= End;
		}

		public void Union(RangeDateTime other)
		{
			this = new RangeDateTime(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
		}

		public void Union(DateTime start, DateTime end)
		{
			this = new RangeDateTime(Start < start ? Start : start, End > end ? End : end);
		}

		public void Union(DateTime value)
		{
			if (value < Start)
				Start = value;

			if (value > End)
				End = value;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ ~End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			
			var o = obj as RangeDateTime?;

			if (o == null)
				return false;

			return Equals(o.Value);
		}

		public override string ToString()
		{
			if (End < Start)
				return "{}";
			else if (Start == End)
				return $"{{{Start}}}";
			else
				return $"{{{Start}, ..., {End}}}";
		}

		public void Intersect(RangeDateTime other)
		{
			this = new RangeDateTime(Start < other.Start ? other.Start : Start, End > other.End ? other.End : End);
		}

		public void Intersect(DateTime start, DateTime end)
		{
			this = new RangeDateTime(Start < start ? start : Start, End > end ? end : End);
		}

		public bool Equals(RangeDateTime other)
		{
			return Start == other.Start && End == other.End;
		}

		public static implicit operator Range<DateTime>(RangeDateTime range)
		{
			return new Range<DateTime>(range.Start, range.End);
		}

		public static implicit operator RangeDateTime(Range<DateTime> range)
		{
			return new RangeDateTime(range.Start, range.End);
		}

		public static bool operator==(RangeDateTime left, RangeDateTime right)
		{
			return left.Start == right.Start && left.End == right.End;
		}

		public static bool operator!=(RangeDateTime left, RangeDateTime right)
		{
			return left.Start != right.Start || left.End != right.End;
		}

        public static RangeDateTime CreateNormalized(DateTime first, DateTime second)
        {
            return new RangeDateTime(first <= second ? first : second, first >= second ? first : second);
        }

		public static RangeDateTime CreateSpanning(DateTime start, TimeSpan width)
		{
						return new RangeDateTime(start, (DateTime)(start + width));
		}

        public static RangeDateTime FromEnumerable(IEnumerable<DateTime> enumerable)
        {
            DateTime start;
            DateTime end;

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

            return new RangeDateTime(start, end);
        }
	}
	}