
using System;

namespace Utility.Geometry.Buffers
{
	public struct AccessView2D<T>
	{
		private readonly Func<long, long, T> _getter;
		private readonly Action<long, long, T> _setter;

		public bool CanRead
		{
			get => _getter != null;
		}

		public bool CanWrite
		{
			get => _setter != null;
		}

		public bool IsReadOnly
		{
			get => _getter != null && _setter == null;
		}

		public bool IsWriteOnly
		{
			get => _getter == null && _setter != null;
		}

		public AccessView2D(Func<long, long, T> getter,
						  Action<long, long, T> setter)
		{
			_getter = getter;
			_setter = setter;
		}

		public T this[long x, long y]
		{
			get
			{
				if (_getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return _getter(x, y);
			}
			set 
			{
				if (_setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				_setter(x, y, value);
			}
		}

		public static implicit operator AccessView2D<T, long>(AccessView2D<T> value)
		{
			return new AccessView2D<T, long>(value._getter, value._setter);
		}

		public static implicit operator AccessView2D<T>(AccessView2D<T, long> value)
		{
			return new AccessView2D<T>(value.Getter, value.Setter);
		}
	}

	public struct AccessView2D<T, TIndex>
	{
		internal Func<TIndex, TIndex, T> Getter { get; }
		internal Action<TIndex, TIndex, T> Setter { get; }

		public bool CanRead
		{
			get => Getter != null;
		}

		public bool CanWrite
		{
			get => Setter != null;
		}

		public bool IsReadOnly
		{
			get => Getter != null && Setter == null;
		}

		public bool IsWriteOnly
		{
			get => Getter == null && Setter != null;
		}

		public AccessView2D(Func<TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, T> setter)
		{
			Getter = getter;
			Setter = setter;
		}

		public T this[TIndex x, TIndex y]
		{
			get
			{
				if (Getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return Getter(x, y);
			}
			set 
			{
				if (Setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				Setter(x, y, value);
			}
		}
	}

	public struct AccessView3D<T>
	{
		private readonly Func<long, long, long, T> _getter;
		private readonly Action<long, long, long, T> _setter;

		public bool CanRead
		{
			get => _getter != null;
		}

		public bool CanWrite
		{
			get => _setter != null;
		}

		public bool IsReadOnly
		{
			get => _getter != null && _setter == null;
		}

		public bool IsWriteOnly
		{
			get => _getter == null && _setter != null;
		}

		public AccessView3D(Func<long, long, long, T> getter,
						  Action<long, long, long, T> setter)
		{
			_getter = getter;
			_setter = setter;
		}

		public T this[long x, long y, long z]
		{
			get
			{
				if (_getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return _getter(x, y, z);
			}
			set 
			{
				if (_setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				_setter(x, y, z, value);
			}
		}

		public static implicit operator AccessView3D<T, long>(AccessView3D<T> value)
		{
			return new AccessView3D<T, long>(value._getter, value._setter);
		}

		public static implicit operator AccessView3D<T>(AccessView3D<T, long> value)
		{
			return new AccessView3D<T>(value.Getter, value.Setter);
		}
	}

	public struct AccessView3D<T, TIndex>
	{
		internal Func<TIndex, TIndex, TIndex, T> Getter { get; }
		internal Action<TIndex, TIndex, TIndex, T> Setter { get; }

		public bool CanRead
		{
			get => Getter != null;
		}

		public bool CanWrite
		{
			get => Setter != null;
		}

		public bool IsReadOnly
		{
			get => Getter != null && Setter == null;
		}

		public bool IsWriteOnly
		{
			get => Getter == null && Setter != null;
		}

		public AccessView3D(Func<TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, T> setter)
		{
			Getter = getter;
			Setter = setter;
		}

		public T this[TIndex x, TIndex y, TIndex z]
		{
			get
			{
				if (Getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return Getter(x, y, z);
			}
			set 
			{
				if (Setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				Setter(x, y, z, value);
			}
		}
	}

	public struct AccessView4D<T>
	{
		private readonly Func<long, long, long, long, T> _getter;
		private readonly Action<long, long, long, long, T> _setter;

		public bool CanRead
		{
			get => _getter != null;
		}

		public bool CanWrite
		{
			get => _setter != null;
		}

		public bool IsReadOnly
		{
			get => _getter != null && _setter == null;
		}

		public bool IsWriteOnly
		{
			get => _getter == null && _setter != null;
		}

		public AccessView4D(Func<long, long, long, long, T> getter,
						  Action<long, long, long, long, T> setter)
		{
			_getter = getter;
			_setter = setter;
		}

		public T this[long index1, long index2, long index3, long index4]
		{
			get
			{
				if (_getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return _getter(index1, index2, index3, index4);
			}
			set 
			{
				if (_setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				_setter(index1, index2, index3, index4, value);
			}
		}

		public static implicit operator AccessView4D<T, long>(AccessView4D<T> value)
		{
			return new AccessView4D<T, long>(value._getter, value._setter);
		}

		public static implicit operator AccessView4D<T>(AccessView4D<T, long> value)
		{
			return new AccessView4D<T>(value.Getter, value.Setter);
		}
	}

	public struct AccessView4D<T, TIndex>
	{
		internal Func<TIndex, TIndex, TIndex, TIndex, T> Getter { get; }
		internal Action<TIndex, TIndex, TIndex, TIndex, T> Setter { get; }

		public bool CanRead
		{
			get => Getter != null;
		}

		public bool CanWrite
		{
			get => Setter != null;
		}

		public bool IsReadOnly
		{
			get => Getter != null && Setter == null;
		}

		public bool IsWriteOnly
		{
			get => Getter == null && Setter != null;
		}

		public AccessView4D(Func<TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, T> setter)
		{
			Getter = getter;
			Setter = setter;
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4]
		{
			get
			{
				if (Getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return Getter(index1, index2, index3, index4);
			}
			set 
			{
				if (Setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				Setter(index1, index2, index3, index4, value);
			}
		}
	}

	public struct AccessView5D<T>
	{
		private readonly Func<long, long, long, long, long, T> _getter;
		private readonly Action<long, long, long, long, long, T> _setter;

		public bool CanRead
		{
			get => _getter != null;
		}

		public bool CanWrite
		{
			get => _setter != null;
		}

		public bool IsReadOnly
		{
			get => _getter != null && _setter == null;
		}

		public bool IsWriteOnly
		{
			get => _getter == null && _setter != null;
		}

		public AccessView5D(Func<long, long, long, long, long, T> getter,
						  Action<long, long, long, long, long, T> setter)
		{
			_getter = getter;
			_setter = setter;
		}

		public T this[long index1, long index2, long index3, long index4, long index5]
		{
			get
			{
				if (_getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return _getter(index1, index2, index3, index4, index5);
			}
			set 
			{
				if (_setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				_setter(index1, index2, index3, index4, index5, value);
			}
		}

		public static implicit operator AccessView5D<T, long>(AccessView5D<T> value)
		{
			return new AccessView5D<T, long>(value._getter, value._setter);
		}

		public static implicit operator AccessView5D<T>(AccessView5D<T, long> value)
		{
			return new AccessView5D<T>(value.Getter, value.Setter);
		}
	}

	public struct AccessView5D<T, TIndex>
	{
		internal Func<TIndex, TIndex, TIndex, TIndex, TIndex, T> Getter { get; }
		internal Action<TIndex, TIndex, TIndex, TIndex, TIndex, T> Setter { get; }

		public bool CanRead
		{
			get => Getter != null;
		}

		public bool CanWrite
		{
			get => Setter != null;
		}

		public bool IsReadOnly
		{
			get => Getter != null && Setter == null;
		}

		public bool IsWriteOnly
		{
			get => Getter == null && Setter != null;
		}

		public AccessView5D(Func<TIndex, TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, TIndex, T> setter)
		{
			Getter = getter;
			Setter = setter;
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5]
		{
			get
			{
				if (Getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return Getter(index1, index2, index3, index4, index5);
			}
			set 
			{
				if (Setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				Setter(index1, index2, index3, index4, index5, value);
			}
		}
	}

	public struct AccessView6D<T>
	{
		private readonly Func<long, long, long, long, long, long, T> _getter;
		private readonly Action<long, long, long, long, long, long, T> _setter;

		public bool CanRead
		{
			get => _getter != null;
		}

		public bool CanWrite
		{
			get => _setter != null;
		}

		public bool IsReadOnly
		{
			get => _getter != null && _setter == null;
		}

		public bool IsWriteOnly
		{
			get => _getter == null && _setter != null;
		}

		public AccessView6D(Func<long, long, long, long, long, long, T> getter,
						  Action<long, long, long, long, long, long, T> setter)
		{
			_getter = getter;
			_setter = setter;
		}

		public T this[long index1, long index2, long index3, long index4, long index5, long index6]
		{
			get
			{
				if (_getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return _getter(index1, index2, index3, index4, index5, index6);
			}
			set 
			{
				if (_setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				_setter(index1, index2, index3, index4, index5, index6, value);
			}
		}

		public static implicit operator AccessView6D<T, long>(AccessView6D<T> value)
		{
			return new AccessView6D<T, long>(value._getter, value._setter);
		}

		public static implicit operator AccessView6D<T>(AccessView6D<T, long> value)
		{
			return new AccessView6D<T>(value.Getter, value.Setter);
		}
	}

	public struct AccessView6D<T, TIndex>
	{
		internal Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> Getter { get; }
		internal Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> Setter { get; }

		public bool CanRead
		{
			get => Getter != null;
		}

		public bool CanWrite
		{
			get => Setter != null;
		}

		public bool IsReadOnly
		{
			get => Getter != null && Setter == null;
		}

		public bool IsWriteOnly
		{
			get => Getter == null && Setter != null;
		}

		public AccessView6D(Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> setter)
		{
			Getter = getter;
			Setter = setter;
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5, TIndex index6]
		{
			get
			{
				if (Getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return Getter(index1, index2, index3, index4, index5, index6);
			}
			set 
			{
				if (Setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				Setter(index1, index2, index3, index4, index5, index6, value);
			}
		}
	}

	public struct AccessView7D<T>
	{
		private readonly Func<long, long, long, long, long, long, long, T> _getter;
		private readonly Action<long, long, long, long, long, long, long, T> _setter;

		public bool CanRead
		{
			get => _getter != null;
		}

		public bool CanWrite
		{
			get => _setter != null;
		}

		public bool IsReadOnly
		{
			get => _getter != null && _setter == null;
		}

		public bool IsWriteOnly
		{
			get => _getter == null && _setter != null;
		}

		public AccessView7D(Func<long, long, long, long, long, long, long, T> getter,
						  Action<long, long, long, long, long, long, long, T> setter)
		{
			_getter = getter;
			_setter = setter;
		}

		public T this[long index1, long index2, long index3, long index4, long index5, long index6, long index7]
		{
			get
			{
				if (_getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return _getter(index1, index2, index3, index4, index5, index6, index7);
			}
			set 
			{
				if (_setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				_setter(index1, index2, index3, index4, index5, index6, index7, value);
			}
		}

		public static implicit operator AccessView7D<T, long>(AccessView7D<T> value)
		{
			return new AccessView7D<T, long>(value._getter, value._setter);
		}

		public static implicit operator AccessView7D<T>(AccessView7D<T, long> value)
		{
			return new AccessView7D<T>(value.Getter, value.Setter);
		}
	}

	public struct AccessView7D<T, TIndex>
	{
		internal Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> Getter { get; }
		internal Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> Setter { get; }

		public bool CanRead
		{
			get => Getter != null;
		}

		public bool CanWrite
		{
			get => Setter != null;
		}

		public bool IsReadOnly
		{
			get => Getter != null && Setter == null;
		}

		public bool IsWriteOnly
		{
			get => Getter == null && Setter != null;
		}

		public AccessView7D(Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> setter)
		{
			Getter = getter;
			Setter = setter;
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5, TIndex index6, TIndex index7]
		{
			get
			{
				if (Getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return Getter(index1, index2, index3, index4, index5, index6, index7);
			}
			set 
			{
				if (Setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				Setter(index1, index2, index3, index4, index5, index6, index7, value);
			}
		}
	}

	public struct AccessView8D<T>
	{
		private readonly Func<long, long, long, long, long, long, long, long, T> _getter;
		private readonly Action<long, long, long, long, long, long, long, long, T> _setter;

		public bool CanRead
		{
			get => _getter != null;
		}

		public bool CanWrite
		{
			get => _setter != null;
		}

		public bool IsReadOnly
		{
			get => _getter != null && _setter == null;
		}

		public bool IsWriteOnly
		{
			get => _getter == null && _setter != null;
		}

		public AccessView8D(Func<long, long, long, long, long, long, long, long, T> getter,
						  Action<long, long, long, long, long, long, long, long, T> setter)
		{
			_getter = getter;
			_setter = setter;
		}

		public T this[long index1, long index2, long index3, long index4, long index5, long index6, long index7, long index8]
		{
			get
			{
				if (_getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return _getter(index1, index2, index3, index4, index5, index6, index7, index8);
			}
			set 
			{
				if (_setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				_setter(index1, index2, index3, index4, index5, index6, index7, index8, value);
			}
		}

		public static implicit operator AccessView8D<T, long>(AccessView8D<T> value)
		{
			return new AccessView8D<T, long>(value._getter, value._setter);
		}

		public static implicit operator AccessView8D<T>(AccessView8D<T, long> value)
		{
			return new AccessView8D<T>(value.Getter, value.Setter);
		}
	}

	public struct AccessView8D<T, TIndex>
	{
		internal Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> Getter { get; }
		internal Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> Setter { get; }

		public bool CanRead
		{
			get => Getter != null;
		}

		public bool CanWrite
		{
			get => Setter != null;
		}

		public bool IsReadOnly
		{
			get => Getter != null && Setter == null;
		}

		public bool IsWriteOnly
		{
			get => Getter == null && Setter != null;
		}

		public AccessView8D(Func<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> getter,
						  Action<TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, TIndex, T> setter)
		{
			Getter = getter;
			Setter = setter;
		}

		public T this[TIndex index1, TIndex index2, TIndex index3, TIndex index4, TIndex index5, TIndex index6, TIndex index7, TIndex index8]
		{
			get
			{
				if (Getter == null)
				{
					throw new NotSupportedException("Reading is not supported by this instance.");
				}

				return Getter(index1, index2, index3, index4, index5, index6, index7, index8);
			}
			set 
			{
				if (Setter == null)
				{
					throw new NotSupportedException("Writing is not supported by this instance.");
				}

				Setter(index1, index2, index3, index4, index5, index6, index7, index8, value);
			}
		}
	}


}