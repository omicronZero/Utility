
using System;
using Utility;


namespace Utility.Geometry.Buffers
{
	public struct AccessRangeView2D<T>
	{
		private AccessView2D<T> _view;

					public Interval<long> IntervalX { get; }
					public Interval<long> IntervalY { get; }
		
		public bool CanRead
		{
			get => _view.CanRead;
		}

		public bool CanWrite
		{
			get => _view.CanWrite;
		}

		public bool IsReadOnly
		{
			get => _view.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => _view.IsWriteOnly;
		}

		public AccessRangeView2D(Func<long, long, T> getter,
						  Action<long, long, T> setter,
						  Interval<long> intervalX, Interval<long> intervalY)
			: this(new AccessView2D<T>(getter, setter), intervalX, intervalY)
		{ }

		public AccessRangeView2D(AccessView2D<T> view, Interval<long> intervalX, Interval<long> intervalY)
		{
			_view = view;
						IntervalX = intervalX;
						IntervalY = intervalY;
					}

		public AccessView2D<T> AsView()
		{
			AccessRangeView2D<T> inst = this;
			return new AccessView2D<T>(
				CanRead ? new Func<long, long, T>((x, y) => inst[x, y]) : null,
				CanWrite ? new Action<long, long, T>((x, y, value) => inst[x, y] = value) : null);
		}

		public T this[long x, long y]
		{
			get
			{
				CheckBounds(x, y);

				return _view[x, y];
			}
			set 
			{
				CheckBounds(x, y);

				_view[x, y] = value;
			}
		}

		private void CheckBounds(long x, long y)
		{
				if (x < IntervalX.Start || x >= IntervalX.End)
			throw new ArgumentOutOfRangeException(nameof(x), "Parameter x does not fall into the valid index range specified by IntervalX.");
				if (y < IntervalY.Start || y >= IntervalY.End)
			throw new ArgumentOutOfRangeException(nameof(y), "Parameter y does not fall into the valid index range specified by IntervalY.");
				}

		public static implicit operator AccessRangeView2D<T, long>(AccessRangeView2D<T> value)
		{
			return new AccessRangeView2D<T, long>(value._view, value.IntervalX, value.IntervalY);
		}

		public static implicit operator AccessRangeView2D<T>(AccessRangeView2D<T, long> value)
		{
			return new AccessRangeView2D<T>(value.View, value.IntervalX, value.IntervalY);
		}
	}


	
	public struct AccessRangeView2D<T, TIndex>
		where TIndex : IComparable<TIndex>
	{
		internal AccessView2D<T, TIndex> View;

					public Interval<TIndex> IntervalX { get; }
					public Interval<TIndex> IntervalY { get; }
		
		public bool CanRead
		{
			get => View.CanRead;
		}

		public bool CanWrite
		{
			get => View.CanWrite;
		}

		public bool IsReadOnly
		{
			get => View.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => View.IsWriteOnly;
		}

		public AccessRangeView2D(Func<TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, T> setter,
						  Interval<TIndex> intervalX, Interval<TIndex> intervalY)
			: this(new AccessView2D<T, TIndex>(getter, setter), intervalX, intervalY)
		{ }

		public AccessRangeView2D(AccessView2D<T, TIndex> view, Interval<TIndex> intervalX, Interval<TIndex> intervalY)
		{
			View = view;
						IntervalX = intervalX;
						IntervalY = intervalY;
					}

		public AccessView2D<T, TIndex> AsView()
		{
			AccessRangeView2D<T, TIndex> inst = this;
			return new AccessView2D<T, TIndex>(
				CanRead ? new Func<TIndex, TIndex, T>((x, y) => inst[x, y]) : null,
				CanWrite ? new Action<TIndex, TIndex, T>((x, y, value) => inst[x, y] = value) : null);
		}

		public T this[TIndex x, TIndex y]
		{
			get
			{
				CheckBounds(x, y);

				return View[x, y];
			}
			set 
			{
				CheckBounds(x, y);

				View[x, y] = value;
			}
		}

		private void CheckBounds(TIndex x, TIndex y)
		{
				if (x < IntervalX.StartComparable || x >= IntervalX.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(x), "Parameter x does not fall into the valid index range specified by IntervalX.");
				if (y < IntervalY.StartComparable || y >= IntervalY.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(y), "Parameter y does not fall into the valid index range specified by IntervalY.");
				}
	}

	public struct AccessRangeView3D<T>
	{
		private AccessView3D<T> _view;

					public Interval<long> IntervalX { get; }
					public Interval<long> IntervalY { get; }
					public Interval<long> IntervalZ { get; }
		
		public bool CanRead
		{
			get => _view.CanRead;
		}

		public bool CanWrite
		{
			get => _view.CanWrite;
		}

		public bool IsReadOnly
		{
			get => _view.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => _view.IsWriteOnly;
		}

		public AccessRangeView3D(Func<long, long, long, T> getter,
						  Action<long, long, long, T> setter,
						  Interval<long> intervalX, Interval<long> intervalY, Interval<long> intervalZ)
			: this(new AccessView3D<T>(getter, setter), intervalX, intervalY, intervalZ)
		{ }

		public AccessRangeView3D(AccessView3D<T> view, Interval<long> intervalX, Interval<long> intervalY, Interval<long> intervalZ)
		{
			_view = view;
						IntervalX = intervalX;
						IntervalY = intervalY;
						IntervalZ = intervalZ;
					}

		public AccessView3D<T> AsView()
		{
			AccessRangeView3D<T> inst = this;
			return new AccessView3D<T>(
				CanRead ? new Func<long, long, long, T>((x, y, z) => inst[x, y, z]) : null,
				CanWrite ? new Action<long, long, long, T>((x, y, z, value) => inst[x, y, z] = value) : null);
		}

		public T this[long x, long y, long z]
		{
			get
			{
				CheckBounds(x, y, z);

				return _view[x, y, z];
			}
			set 
			{
				CheckBounds(x, y, z);

				_view[x, y, z] = value;
			}
		}

		private void CheckBounds(long x, long y, long z)
		{
				if (x < IntervalX.Start || x >= IntervalX.End)
			throw new ArgumentOutOfRangeException(nameof(x), "Parameter x does not fall into the valid index range specified by IntervalX.");
				if (y < IntervalY.Start || y >= IntervalY.End)
			throw new ArgumentOutOfRangeException(nameof(y), "Parameter y does not fall into the valid index range specified by IntervalY.");
				if (z < IntervalZ.Start || z >= IntervalZ.End)
			throw new ArgumentOutOfRangeException(nameof(z), "Parameter z does not fall into the valid index range specified by IntervalZ.");
				}

		public static implicit operator AccessRangeView3D<T, long>(AccessRangeView3D<T> value)
		{
			return new AccessRangeView3D<T, long>(value._view, value.IntervalX, value.IntervalY, value.IntervalZ);
		}

		public static implicit operator AccessRangeView3D<T>(AccessRangeView3D<T, long> value)
		{
			return new AccessRangeView3D<T>(value.View, value.IntervalX, value.IntervalY, value.IntervalZ);
		}
	}


	
	public struct AccessRangeView3D<T, TIndex>
		where TIndex : IComparable<TIndex>
	{
		internal AccessView3D<T, TIndex> View;

					public Interval<TIndex> IntervalX { get; }
					public Interval<TIndex> IntervalY { get; }
					public Interval<TIndex> IntervalZ { get; }
		
		public bool CanRead
		{
			get => View.CanRead;
		}

		public bool CanWrite
		{
			get => View.CanWrite;
		}

		public bool IsReadOnly
		{
			get => View.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => View.IsWriteOnly;
		}

		public AccessRangeView3D(Func<TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, T> setter,
						  Interval<TIndex> intervalX, Interval<TIndex> intervalY, Interval<TIndex> intervalZ)
			: this(new AccessView3D<T, TIndex>(getter, setter), intervalX, intervalY, intervalZ)
		{ }

		public AccessRangeView3D(AccessView3D<T, TIndex> view, Interval<TIndex> intervalX, Interval<TIndex> intervalY, Interval<TIndex> intervalZ)
		{
			View = view;
						IntervalX = intervalX;
						IntervalY = intervalY;
						IntervalZ = intervalZ;
					}

		public AccessView3D<T, TIndex> AsView()
		{
			AccessRangeView3D<T, TIndex> inst = this;
			return new AccessView3D<T, TIndex>(
				CanRead ? new Func<TIndex, TIndex, TIndex, T>((x, y, z) => inst[x, y, z]) : null,
				CanWrite ? new Action<TIndex, TIndex, TIndex, T>((x, y, z, value) => inst[x, y, z] = value) : null);
		}

		public T this[TIndex x, TIndex y, TIndex z]
		{
			get
			{
				CheckBounds(x, y, z);

				return View[x, y, z];
			}
			set 
			{
				CheckBounds(x, y, z);

				View[x, y, z] = value;
			}
		}

		private void CheckBounds(TIndex x, TIndex y, TIndex z)
		{
				if (x < IntervalX.StartComparable || x >= IntervalX.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(x), "Parameter x does not fall into the valid index range specified by IntervalX.");
				if (y < IntervalY.StartComparable || y >= IntervalY.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(y), "Parameter y does not fall into the valid index range specified by IntervalY.");
				if (z < IntervalZ.StartComparable || z >= IntervalZ.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(z), "Parameter z does not fall into the valid index range specified by IntervalZ.");
				}
	}

	public struct AccessRangeView4D<T>
	{
		private AccessView4D<T> _view;

					public Interval<long> Interval1 { get; }
					public Interval<long> Interval2 { get; }
					public Interval<long> Interval3 { get; }
					public Interval<long> Interval4 { get; }
		
		public bool CanRead
		{
			get => _view.CanRead;
		}

		public bool CanWrite
		{
			get => _view.CanWrite;
		}

		public bool IsReadOnly
		{
			get => _view.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => _view.IsWriteOnly;
		}

		public AccessRangeView4D(Func<long, long, long, long, T> getter,
						  Action<long, long, long, long, T> setter,
						  Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4)
			: this(new AccessView4D<T>(getter, setter), interval1, interval2, interval3, interval4)
		{ }

		public AccessRangeView4D(AccessView4D<T> view, Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4)
		{
			_view = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
					}

		public AccessView4D<T> AsView()
		{
			AccessRangeView4D<T> inst = this;
			return new AccessView4D<T>(
				CanRead ? new Func<long, long, long, long, T>((index1, index2, index3, index4) => inst[index1, index2, index3, index4]) : null,
				CanWrite ? new Action<long, long, long, long, T>((index1, index2, index3, index4, value) => inst[index1, index2, index3, index4] = value) : null);
		}

		public T this[long index1, long index2, long index3, long index4]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4);

				return _view[index1, index2, index3, index4];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4);

				_view[index1, index2, index3, index4] = value;
			}
		}

		private void CheckBounds(long index1, long index2, long index3, long index4)
		{
				if (index1 < Interval1.Start || index1 >= Interval1.End)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.Start || index2 >= Interval2.End)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.Start || index3 >= Interval3.End)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.Start || index4 >= Interval4.End)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				}

		public static implicit operator AccessRangeView4D<T, long>(AccessRangeView4D<T> value)
		{
			return new AccessRangeView4D<T, long>(value._view, value.Interval1, value.Interval2, value.Interval3, value.Interval4);
		}

		public static implicit operator AccessRangeView4D<T>(AccessRangeView4D<T, long> value)
		{
			return new AccessRangeView4D<T>(value.View, value.Interval1, value.Interval2, value.Interval3, value.Interval4);
		}
	}


	
	public struct AccessRangeView4D<T, TIndex>
		where TIndex : IComparable<TIndex>
	{
		internal AccessView4D<T, TIndex> View;

					public Interval<TIndex> Interval1 { get; }
					public Interval<TIndex> Interval2 { get; }
					public Interval<TIndex> Interval3 { get; }
					public Interval<TIndex> Interval4 { get; }
		
		public bool CanRead
		{
			get => View.CanRead;
		}

		public bool CanWrite
		{
			get => View.CanWrite;
		}

		public bool IsReadOnly
		{
			get => View.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => View.IsWriteOnly;
		}

		public AccessRangeView4D(Func<TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, T> setter,
						  Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4)
			: this(new AccessView4D<T, TIndex>(getter, setter), interval1, interval2, interval3, interval4)
		{ }

		public AccessRangeView4D(AccessView4D<T, TIndex> view, Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4)
		{
			View = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
					}

		public AccessView4D<T, TIndex> AsView()
		{
			AccessRangeView4D<T, TIndex> inst = this;
			return new AccessView4D<T, TIndex>(
				CanRead ? new Func<TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4) => inst[index1, index2, index3, index4]) : null,
				CanWrite ? new Action<TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4, value) => inst[index1, index2, index3, index4] = value) : null);
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4);

				return View[index1, index2, index3, index4];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4);

				View[index1, index2, index3, index4] = value;
			}
		}

		private void CheckBounds(TIndex index1, TIndex index2, TIndex index3, TIndex index4)
		{
				if (index1 < Interval1.StartComparable || index1 >= Interval1.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.StartComparable || index2 >= Interval2.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.StartComparable || index3 >= Interval3.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.StartComparable || index4 >= Interval4.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				}
	}

	public struct AccessRangeView5D<T>
	{
		private AccessView5D<T> _view;

					public Interval<long> Interval1 { get; }
					public Interval<long> Interval2 { get; }
					public Interval<long> Interval3 { get; }
					public Interval<long> Interval4 { get; }
					public Interval<long> Interval5 { get; }
		
		public bool CanRead
		{
			get => _view.CanRead;
		}

		public bool CanWrite
		{
			get => _view.CanWrite;
		}

		public bool IsReadOnly
		{
			get => _view.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => _view.IsWriteOnly;
		}

		public AccessRangeView5D(Func<long, long, long, long, long, T> getter,
						  Action<long, long, long, long, long, T> setter,
						  Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4, Interval<long> interval5)
			: this(new AccessView5D<T>(getter, setter), interval1, interval2, interval3, interval4, interval5)
		{ }

		public AccessRangeView5D(AccessView5D<T> view, Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4, Interval<long> interval5)
		{
			_view = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
						Interval5 = interval5;
					}

		public AccessView5D<T> AsView()
		{
			AccessRangeView5D<T> inst = this;
			return new AccessView5D<T>(
				CanRead ? new Func<long, long, long, long, long, T>((index1, index2, index3, index4, index5) => inst[index1, index2, index3, index4, index5]) : null,
				CanWrite ? new Action<long, long, long, long, long, T>((index1, index2, index3, index4, index5, value) => inst[index1, index2, index3, index4, index5] = value) : null);
		}

		public T this[long index1, long index2, long index3, long index4, long index5]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4, index5);

				return _view[index1, index2, index3, index4, index5];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4, index5);

				_view[index1, index2, index3, index4, index5] = value;
			}
		}

		private void CheckBounds(long index1, long index2, long index3, long index4, long index5)
		{
				if (index1 < Interval1.Start || index1 >= Interval1.End)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.Start || index2 >= Interval2.End)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.Start || index3 >= Interval3.End)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.Start || index4 >= Interval4.End)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				if (index5 < Interval5.Start || index5 >= Interval5.End)
			throw new ArgumentOutOfRangeException(nameof(index5), "Parameter index5 does not fall into the valid index range specified by Interval5.");
				}

		public static implicit operator AccessRangeView5D<T, long>(AccessRangeView5D<T> value)
		{
			return new AccessRangeView5D<T, long>(value._view, value.Interval1, value.Interval2, value.Interval3, value.Interval4, value.Interval5);
		}

		public static implicit operator AccessRangeView5D<T>(AccessRangeView5D<T, long> value)
		{
			return new AccessRangeView5D<T>(value.View, value.Interval1, value.Interval2, value.Interval3, value.Interval4, value.Interval5);
		}
	}


	
	public struct AccessRangeView5D<T, TIndex>
		where TIndex : IComparable<TIndex>
	{
		internal AccessView5D<T, TIndex> View;

					public Interval<TIndex> Interval1 { get; }
					public Interval<TIndex> Interval2 { get; }
					public Interval<TIndex> Interval3 { get; }
					public Interval<TIndex> Interval4 { get; }
					public Interval<TIndex> Interval5 { get; }
		
		public bool CanRead
		{
			get => View.CanRead;
		}

		public bool CanWrite
		{
			get => View.CanWrite;
		}

		public bool IsReadOnly
		{
			get => View.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => View.IsWriteOnly;
		}

		public AccessRangeView5D(Func<TIndex, TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, TIndex, T> setter,
						  Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4, Interval<TIndex> interval5)
			: this(new AccessView5D<T, TIndex>(getter, setter), interval1, interval2, interval3, interval4, interval5)
		{ }

		public AccessRangeView5D(AccessView5D<T, TIndex> view, Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4, Interval<TIndex> interval5)
		{
			View = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
						Interval5 = interval5;
					}

		public AccessView5D<T, TIndex> AsView()
		{
			AccessRangeView5D<T, TIndex> inst = this;
			return new AccessView5D<T, TIndex>(
				CanRead ? new Func<TIndex, TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4, index5) => inst[index1, index2, index3, index4, index5]) : null,
				CanWrite ? new Action<TIndex, TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4, index5, value) => inst[index1, index2, index3, index4, index5] = value) : null);
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4, index5);

				return View[index1, index2, index3, index4, index5];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4, index5);

				View[index1, index2, index3, index4, index5] = value;
			}
		}

		private void CheckBounds(TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5)
		{
				if (index1 < Interval1.StartComparable || index1 >= Interval1.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.StartComparable || index2 >= Interval2.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.StartComparable || index3 >= Interval3.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.StartComparable || index4 >= Interval4.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				if (index5 < Interval5.StartComparable || index5 >= Interval5.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index5), "Parameter index5 does not fall into the valid index range specified by Interval5.");
				}
	}

	public struct AccessRangeView6D<T>
	{
		private AccessView6D<T> _view;

					public Interval<long> Interval1 { get; }
					public Interval<long> Interval2 { get; }
					public Interval<long> Interval3 { get; }
					public Interval<long> Interval4 { get; }
					public Interval<long> Interval5 { get; }
					public Interval<long> Interval6 { get; }
		
		public bool CanRead
		{
			get => _view.CanRead;
		}

		public bool CanWrite
		{
			get => _view.CanWrite;
		}

		public bool IsReadOnly
		{
			get => _view.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => _view.IsWriteOnly;
		}

		public AccessRangeView6D(Func<long, long, long, long, long, long, T> getter,
						  Action<long, long, long, long, long, long, T> setter,
						  Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4, Interval<long> interval5, Interval<long> interval6)
			: this(new AccessView6D<T>(getter, setter), interval1, interval2, interval3, interval4, interval5, interval6)
		{ }

		public AccessRangeView6D(AccessView6D<T> view, Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4, Interval<long> interval5, Interval<long> interval6)
		{
			_view = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
						Interval5 = interval5;
						Interval6 = interval6;
					}

		public AccessView6D<T> AsView()
		{
			AccessRangeView6D<T> inst = this;
			return new AccessView6D<T>(
				CanRead ? new Func<long, long, long, long, long, long, T>((index1, index2, index3, index4, index5, index6) => inst[index1, index2, index3, index4, index5, index6]) : null,
				CanWrite ? new Action<long, long, long, long, long, long, T>((index1, index2, index3, index4, index5, index6, value) => inst[index1, index2, index3, index4, index5, index6] = value) : null);
		}

		public T this[long index1, long index2, long index3, long index4, long index5, long index6]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4, index5, index6);

				return _view[index1, index2, index3, index4, index5, index6];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4, index5, index6);

				_view[index1, index2, index3, index4, index5, index6] = value;
			}
		}

		private void CheckBounds(long index1, long index2, long index3, long index4, long index5, long index6)
		{
				if (index1 < Interval1.Start || index1 >= Interval1.End)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.Start || index2 >= Interval2.End)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.Start || index3 >= Interval3.End)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.Start || index4 >= Interval4.End)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				if (index5 < Interval5.Start || index5 >= Interval5.End)
			throw new ArgumentOutOfRangeException(nameof(index5), "Parameter index5 does not fall into the valid index range specified by Interval5.");
				if (index6 < Interval6.Start || index6 >= Interval6.End)
			throw new ArgumentOutOfRangeException(nameof(index6), "Parameter index6 does not fall into the valid index range specified by Interval6.");
				}

		public static implicit operator AccessRangeView6D<T, long>(AccessRangeView6D<T> value)
		{
			return new AccessRangeView6D<T, long>(value._view, value.Interval1, value.Interval2, value.Interval3, value.Interval4, value.Interval5, value.Interval6);
		}

		public static implicit operator AccessRangeView6D<T>(AccessRangeView6D<T, long> value)
		{
			return new AccessRangeView6D<T>(value.View, value.Interval1, value.Interval2, value.Interval3, value.Interval4, value.Interval5, value.Interval6);
		}
	}


	
	public struct AccessRangeView6D<T, TIndex>
		where TIndex : IComparable<TIndex>
	{
		internal AccessView6D<T, TIndex> View;

					public Interval<TIndex> Interval1 { get; }
					public Interval<TIndex> Interval2 { get; }
					public Interval<TIndex> Interval3 { get; }
					public Interval<TIndex> Interval4 { get; }
					public Interval<TIndex> Interval5 { get; }
					public Interval<TIndex> Interval6 { get; }
		
		public bool CanRead
		{
			get => View.CanRead;
		}

		public bool CanWrite
		{
			get => View.CanWrite;
		}

		public bool IsReadOnly
		{
			get => View.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => View.IsWriteOnly;
		}

		public AccessRangeView6D(Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> setter,
						  Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4, Interval<TIndex> interval5, Interval<TIndex> interval6)
			: this(new AccessView6D<T, TIndex>(getter, setter), interval1, interval2, interval3, interval4, interval5, interval6)
		{ }

		public AccessRangeView6D(AccessView6D<T, TIndex> view, Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4, Interval<TIndex> interval5, Interval<TIndex> interval6)
		{
			View = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
						Interval5 = interval5;
						Interval6 = interval6;
					}

		public AccessView6D<T, TIndex> AsView()
		{
			AccessRangeView6D<T, TIndex> inst = this;
			return new AccessView6D<T, TIndex>(
				CanRead ? new Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4, index5, index6) => inst[index1, index2, index3, index4, index5, index6]) : null,
				CanWrite ? new Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4, index5, index6, value) => inst[index1, index2, index3, index4, index5, index6] = value) : null);
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5, TIndex index6]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4, index5, index6);

				return View[index1, index2, index3, index4, index5, index6];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4, index5, index6);

				View[index1, index2, index3, index4, index5, index6] = value;
			}
		}

		private void CheckBounds(TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5, TIndex index6)
		{
				if (index1 < Interval1.StartComparable || index1 >= Interval1.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.StartComparable || index2 >= Interval2.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.StartComparable || index3 >= Interval3.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.StartComparable || index4 >= Interval4.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				if (index5 < Interval5.StartComparable || index5 >= Interval5.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index5), "Parameter index5 does not fall into the valid index range specified by Interval5.");
				if (index6 < Interval6.StartComparable || index6 >= Interval6.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index6), "Parameter index6 does not fall into the valid index range specified by Interval6.");
				}
	}

	public struct AccessRangeView7D<T>
	{
		private AccessView7D<T> _view;

					public Interval<long> Interval1 { get; }
					public Interval<long> Interval2 { get; }
					public Interval<long> Interval3 { get; }
					public Interval<long> Interval4 { get; }
					public Interval<long> Interval5 { get; }
					public Interval<long> Interval6 { get; }
					public Interval<long> Interval7 { get; }
		
		public bool CanRead
		{
			get => _view.CanRead;
		}

		public bool CanWrite
		{
			get => _view.CanWrite;
		}

		public bool IsReadOnly
		{
			get => _view.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => _view.IsWriteOnly;
		}

		public AccessRangeView7D(Func<long, long, long, long, long, long, long, T> getter,
						  Action<long, long, long, long, long, long, long, T> setter,
						  Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4, Interval<long> interval5, Interval<long> interval6, Interval<long> interval7)
			: this(new AccessView7D<T>(getter, setter), interval1, interval2, interval3, interval4, interval5, interval6, interval7)
		{ }

		public AccessRangeView7D(AccessView7D<T> view, Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4, Interval<long> interval5, Interval<long> interval6, Interval<long> interval7)
		{
			_view = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
						Interval5 = interval5;
						Interval6 = interval6;
						Interval7 = interval7;
					}

		public AccessView7D<T> AsView()
		{
			AccessRangeView7D<T> inst = this;
			return new AccessView7D<T>(
				CanRead ? new Func<long, long, long, long, long, long, long, T>((index1, index2, index3, index4, index5, index6, index7) => inst[index1, index2, index3, index4, index5, index6, index7]) : null,
				CanWrite ? new Action<long, long, long, long, long, long, long, T>((index1, index2, index3, index4, index5, index6, index7, value) => inst[index1, index2, index3, index4, index5, index6, index7] = value) : null);
		}

		public T this[long index1, long index2, long index3, long index4, long index5, long index6, long index7]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4, index5, index6, index7);

				return _view[index1, index2, index3, index4, index5, index6, index7];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4, index5, index6, index7);

				_view[index1, index2, index3, index4, index5, index6, index7] = value;
			}
		}

		private void CheckBounds(long index1, long index2, long index3, long index4, long index5, long index6, long index7)
		{
				if (index1 < Interval1.Start || index1 >= Interval1.End)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.Start || index2 >= Interval2.End)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.Start || index3 >= Interval3.End)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.Start || index4 >= Interval4.End)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				if (index5 < Interval5.Start || index5 >= Interval5.End)
			throw new ArgumentOutOfRangeException(nameof(index5), "Parameter index5 does not fall into the valid index range specified by Interval5.");
				if (index6 < Interval6.Start || index6 >= Interval6.End)
			throw new ArgumentOutOfRangeException(nameof(index6), "Parameter index6 does not fall into the valid index range specified by Interval6.");
				if (index7 < Interval7.Start || index7 >= Interval7.End)
			throw new ArgumentOutOfRangeException(nameof(index7), "Parameter index7 does not fall into the valid index range specified by Interval7.");
				}

		public static implicit operator AccessRangeView7D<T, long>(AccessRangeView7D<T> value)
		{
			return new AccessRangeView7D<T, long>(value._view, value.Interval1, value.Interval2, value.Interval3, value.Interval4, value.Interval5, value.Interval6, value.Interval7);
		}

		public static implicit operator AccessRangeView7D<T>(AccessRangeView7D<T, long> value)
		{
			return new AccessRangeView7D<T>(value.View, value.Interval1, value.Interval2, value.Interval3, value.Interval4, value.Interval5, value.Interval6, value.Interval7);
		}
	}


	
	public struct AccessRangeView7D<T, TIndex>
		where TIndex : IComparable<TIndex>
	{
		internal AccessView7D<T, TIndex> View;

					public Interval<TIndex> Interval1 { get; }
					public Interval<TIndex> Interval2 { get; }
					public Interval<TIndex> Interval3 { get; }
					public Interval<TIndex> Interval4 { get; }
					public Interval<TIndex> Interval5 { get; }
					public Interval<TIndex> Interval6 { get; }
					public Interval<TIndex> Interval7 { get; }
		
		public bool CanRead
		{
			get => View.CanRead;
		}

		public bool CanWrite
		{
			get => View.CanWrite;
		}

		public bool IsReadOnly
		{
			get => View.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => View.IsWriteOnly;
		}

		public AccessRangeView7D(Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> setter,
						  Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4, Interval<TIndex> interval5, Interval<TIndex> interval6, Interval<TIndex> interval7)
			: this(new AccessView7D<T, TIndex>(getter, setter), interval1, interval2, interval3, interval4, interval5, interval6, interval7)
		{ }

		public AccessRangeView7D(AccessView7D<T, TIndex> view, Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4, Interval<TIndex> interval5, Interval<TIndex> interval6, Interval<TIndex> interval7)
		{
			View = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
						Interval5 = interval5;
						Interval6 = interval6;
						Interval7 = interval7;
					}

		public AccessView7D<T, TIndex> AsView()
		{
			AccessRangeView7D<T, TIndex> inst = this;
			return new AccessView7D<T, TIndex>(
				CanRead ? new Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4, index5, index6, index7) => inst[index1, index2, index3, index4, index5, index6, index7]) : null,
				CanWrite ? new Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4, index5, index6, index7, value) => inst[index1, index2, index3, index4, index5, index6, index7] = value) : null);
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5, TIndex index6, TIndex index7]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4, index5, index6, index7);

				return View[index1, index2, index3, index4, index5, index6, index7];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4, index5, index6, index7);

				View[index1, index2, index3, index4, index5, index6, index7] = value;
			}
		}

		private void CheckBounds(TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5, TIndex index6, TIndex index7)
		{
				if (index1 < Interval1.StartComparable || index1 >= Interval1.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.StartComparable || index2 >= Interval2.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.StartComparable || index3 >= Interval3.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.StartComparable || index4 >= Interval4.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				if (index5 < Interval5.StartComparable || index5 >= Interval5.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index5), "Parameter index5 does not fall into the valid index range specified by Interval5.");
				if (index6 < Interval6.StartComparable || index6 >= Interval6.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index6), "Parameter index6 does not fall into the valid index range specified by Interval6.");
				if (index7 < Interval7.StartComparable || index7 >= Interval7.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index7), "Parameter index7 does not fall into the valid index range specified by Interval7.");
				}
	}

	public struct AccessRangeView8D<T>
	{
		private AccessView8D<T> _view;

					public Interval<long> Interval1 { get; }
					public Interval<long> Interval2 { get; }
					public Interval<long> Interval3 { get; }
					public Interval<long> Interval4 { get; }
					public Interval<long> Interval5 { get; }
					public Interval<long> Interval6 { get; }
					public Interval<long> Interval7 { get; }
					public Interval<long> Interval8 { get; }
		
		public bool CanRead
		{
			get => _view.CanRead;
		}

		public bool CanWrite
		{
			get => _view.CanWrite;
		}

		public bool IsReadOnly
		{
			get => _view.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => _view.IsWriteOnly;
		}

		public AccessRangeView8D(Func<long, long, long, long, long, long, long, long, T> getter,
						  Action<long, long, long, long, long, long, long, long, T> setter,
						  Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4, Interval<long> interval5, Interval<long> interval6, Interval<long> interval7, Interval<long> interval8)
			: this(new AccessView8D<T>(getter, setter), interval1, interval2, interval3, interval4, interval5, interval6, interval7, interval8)
		{ }

		public AccessRangeView8D(AccessView8D<T> view, Interval<long> interval1, Interval<long> interval2, Interval<long> interval3, Interval<long> interval4, Interval<long> interval5, Interval<long> interval6, Interval<long> interval7, Interval<long> interval8)
		{
			_view = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
						Interval5 = interval5;
						Interval6 = interval6;
						Interval7 = interval7;
						Interval8 = interval8;
					}

		public AccessView8D<T> AsView()
		{
			AccessRangeView8D<T> inst = this;
			return new AccessView8D<T>(
				CanRead ? new Func<long, long, long, long, long, long, long, long, T>((index1, index2, index3, index4, index5, index6, index7, index8) => inst[index1, index2, index3, index4, index5, index6, index7, index8]) : null,
				CanWrite ? new Action<long, long, long, long, long, long, long, long, T>((index1, index2, index3, index4, index5, index6, index7, index8, value) => inst[index1, index2, index3, index4, index5, index6, index7, index8] = value) : null);
		}

		public T this[long index1, long index2, long index3, long index4, long index5, long index6, long index7, long index8]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4, index5, index6, index7, index8);

				return _view[index1, index2, index3, index4, index5, index6, index7, index8];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4, index5, index6, index7, index8);

				_view[index1, index2, index3, index4, index5, index6, index7, index8] = value;
			}
		}

		private void CheckBounds(long index1, long index2, long index3, long index4, long index5, long index6, long index7, long index8)
		{
				if (index1 < Interval1.Start || index1 >= Interval1.End)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.Start || index2 >= Interval2.End)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.Start || index3 >= Interval3.End)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.Start || index4 >= Interval4.End)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				if (index5 < Interval5.Start || index5 >= Interval5.End)
			throw new ArgumentOutOfRangeException(nameof(index5), "Parameter index5 does not fall into the valid index range specified by Interval5.");
				if (index6 < Interval6.Start || index6 >= Interval6.End)
			throw new ArgumentOutOfRangeException(nameof(index6), "Parameter index6 does not fall into the valid index range specified by Interval6.");
				if (index7 < Interval7.Start || index7 >= Interval7.End)
			throw new ArgumentOutOfRangeException(nameof(index7), "Parameter index7 does not fall into the valid index range specified by Interval7.");
				if (index8 < Interval8.Start || index8 >= Interval8.End)
			throw new ArgumentOutOfRangeException(nameof(index8), "Parameter index8 does not fall into the valid index range specified by Interval8.");
				}

		public static implicit operator AccessRangeView8D<T, long>(AccessRangeView8D<T> value)
		{
			return new AccessRangeView8D<T, long>(value._view, value.Interval1, value.Interval2, value.Interval3, value.Interval4, value.Interval5, value.Interval6, value.Interval7, value.Interval8);
		}

		public static implicit operator AccessRangeView8D<T>(AccessRangeView8D<T, long> value)
		{
			return new AccessRangeView8D<T>(value.View, value.Interval1, value.Interval2, value.Interval3, value.Interval4, value.Interval5, value.Interval6, value.Interval7, value.Interval8);
		}
	}


	
	public struct AccessRangeView8D<T, TIndex>
		where TIndex : IComparable<TIndex>
	{
		internal AccessView8D<T, TIndex> View;

					public Interval<TIndex> Interval1 { get; }
					public Interval<TIndex> Interval2 { get; }
					public Interval<TIndex> Interval3 { get; }
					public Interval<TIndex> Interval4 { get; }
					public Interval<TIndex> Interval5 { get; }
					public Interval<TIndex> Interval6 { get; }
					public Interval<TIndex> Interval7 { get; }
					public Interval<TIndex> Interval8 { get; }
		
		public bool CanRead
		{
			get => View.CanRead;
		}

		public bool CanWrite
		{
			get => View.CanWrite;
		}

		public bool IsReadOnly
		{
			get => View.IsReadOnly;
		}

		public bool IsWriteOnly
		{
			get => View.IsWriteOnly;
		}

		public AccessRangeView8D(Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> setter,
						  Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4, Interval<TIndex> interval5, Interval<TIndex> interval6, Interval<TIndex> interval7, Interval<TIndex> interval8)
			: this(new AccessView8D<T, TIndex>(getter, setter), interval1, interval2, interval3, interval4, interval5, interval6, interval7, interval8)
		{ }

		public AccessRangeView8D(AccessView8D<T, TIndex> view, Interval<TIndex> interval1, Interval<TIndex> interval2, Interval<TIndex> interval3, Interval<TIndex> interval4, Interval<TIndex> interval5, Interval<TIndex> interval6, Interval<TIndex> interval7, Interval<TIndex> interval8)
		{
			View = view;
						Interval1 = interval1;
						Interval2 = interval2;
						Interval3 = interval3;
						Interval4 = interval4;
						Interval5 = interval5;
						Interval6 = interval6;
						Interval7 = interval7;
						Interval8 = interval8;
					}

		public AccessView8D<T, TIndex> AsView()
		{
			AccessRangeView8D<T, TIndex> inst = this;
			return new AccessView8D<T, TIndex>(
				CanRead ? new Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4, index5, index6, index7, index8) => inst[index1, index2, index3, index4, index5, index6, index7, index8]) : null,
				CanWrite ? new Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T>((index1, index2, index3, index4, index5, index6, index7, index8, value) => inst[index1, index2, index3, index4, index5, index6, index7, index8] = value) : null);
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5, TIndex index6, TIndex index7, TIndex index8]
		{
			get
			{
				CheckBounds(index1, index2, index3, index4, index5, index6, index7, index8);

				return View[index1, index2, index3, index4, index5, index6, index7, index8];
			}
			set 
			{
				CheckBounds(index1, index2, index3, index4, index5, index6, index7, index8);

				View[index1, index2, index3, index4, index5, index6, index7, index8] = value;
			}
		}

		private void CheckBounds(TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5, TIndex index6, TIndex index7, TIndex index8)
		{
				if (index1 < Interval1.StartComparable || index1 >= Interval1.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index1), "Parameter index1 does not fall into the valid index range specified by Interval1.");
				if (index2 < Interval2.StartComparable || index2 >= Interval2.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index2), "Parameter index2 does not fall into the valid index range specified by Interval2.");
				if (index3 < Interval3.StartComparable || index3 >= Interval3.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index3), "Parameter index3 does not fall into the valid index range specified by Interval3.");
				if (index4 < Interval4.StartComparable || index4 >= Interval4.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index4), "Parameter index4 does not fall into the valid index range specified by Interval4.");
				if (index5 < Interval5.StartComparable || index5 >= Interval5.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index5), "Parameter index5 does not fall into the valid index range specified by Interval5.");
				if (index6 < Interval6.StartComparable || index6 >= Interval6.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index6), "Parameter index6 does not fall into the valid index range specified by Interval6.");
				if (index7 < Interval7.StartComparable || index7 >= Interval7.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index7), "Parameter index7 does not fall into the valid index range specified by Interval7.");
				if (index8 < Interval8.StartComparable || index8 >= Interval8.EndComparable)
			throw new ArgumentOutOfRangeException(nameof(index8), "Parameter index8 does not fall into the valid index range specified by Interval8.");
				}
	}

}