

using System;
using System.Collections.Generic;

namespace Utility
{

	public struct AnyOf<T1, T2> : IEquatable<AnyOf<T1, T2>>, ICastable
	{
		public int TypeIndex { get; }

		
		private readonly T1 _value1;
			
		private readonly T2 _value2;
			
		
		public AnyOf(T1 value1)
			: this()
		{
			TypeIndex = 1;
			_value1 = value1;
		}

		public static implicit operator AnyOf<T1, T2>(T1 value1)
		{
			return new AnyOf<T1, T2>(value1);
		}

		public static explicit operator T1(AnyOf<T1, T2> value)
		{
			return value.Cast<T1>();
		}
			
		public AnyOf(T2 value2)
			: this()
		{
			TypeIndex = 2;
			_value2 = value2;
		}

		public static implicit operator AnyOf<T1, T2>(T2 value2)
		{
			return new AnyOf<T1, T2>(value2);
		}

		public static explicit operator T2(AnyOf<T1, T2> value)
		{
			return value.Cast<T2>();
		}
			
		bool ICastable.HasInstanceType => true;

		public Type InstanceType
		{
			get
			{
			
				if (TypeIndex == 1)
					return typeof(T1);
				
				if (TypeIndex == 2)
					return typeof(T2);
				
				throw new InvalidOperationException("The current instance has not been initialized to a value.");
			}
		}

		public bool IsDefault => TypeIndex == 0;

		public override bool Equals(object obj) => obj is AnyOf<T1, T2> other && Equals(other);

		public bool Equals(AnyOf<T1, T2> other)
		{
			return other == this;
		}

		public override int GetHashCode()
		{
			
			if (TypeIndex == 1)
				return _value1?.GetHashCode() ?? 0;
				
			if (TypeIndex == 2)
				return _value2?.GetHashCode() ?? 0;
				
			return 0;
		}

		public static bool operator ==(AnyOf<T1, T2> left, AnyOf<T1, T2> right)
		{
			
			if (left.TypeIndex == 1)
			{

				
				if (right.TypeIndex == 1)
					return (T1)left is T1 v && EqualityComparer<T1>.Default.Equals(v, (T1)right);
					
				if (right.TypeIndex == 2)
					return (T1)left is T2 v && EqualityComparer<T2>.Default.Equals(v, (T2)right);
					
			}
				
			if (left.TypeIndex == 2)
			{

				
				if (right.TypeIndex == 1)
					return (T2)left is T1 v && EqualityComparer<T1>.Default.Equals(v, (T1)right);
					
				if (right.TypeIndex == 2)
					return (T2)left is T2 v && EqualityComparer<T2>.Default.Equals(v, (T2)right);
					
			}
				
			return false;
		}

		public static bool operator !=(AnyOf<T1, T2> left, AnyOf<T1, T2> right)
		{
			return !(left == right);
		}

		public bool Is<T>()
		{
			
			if (TypeIndex == 1)
				return _value1 is T;
				
			if (TypeIndex == 2)
				return _value2 is T;
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		public bool Is<T>(out T value)
		{
			
			if (TypeIndex == 1)
			{
				if (_value1 is T v)
				{
					value = v;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
					
				
			if (TypeIndex == 2)
			{
				if (_value2 is T v)
				{
					value = v;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		private static void ThrowTypeIndex(int requested, int actual)
		{
			throw new InvalidOperationException($"The requested value was { requested } but the actual value is { actual }.");
		}

		
		public bool HasValue1 => TypeIndex == 1;
		
		public T1 Value1
		{
			get
			{
				if (TypeIndex != 1)
					ThrowTypeIndex(1, TypeIndex);

				return _value1;
			}
		}
		
		public bool HasValue2 => TypeIndex == 2;
		
		public T2 Value2
		{
			get
			{
				if (TypeIndex != 2)
					ThrowTypeIndex(2, TypeIndex);

				return _value2;
			}
		}
		
		public T Cast<T>()
		{
			
			if (TypeIndex == 1)
			{
				if (_value1 is T v)
					return v;
				else
					throw new InvalidCastException();
			}
					
				
			if (TypeIndex == 2)
			{
				if (_value2 is T v)
					return v;
				else
					throw new InvalidCastException();
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		public object GetInstance()
		{
			
			if (TypeIndex == 1)
			{
				return _value1;
			}
					
				
			if (TypeIndex == 2)
			{
				return _value2;
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		public override string ToString() => IsDefault ? "" : (GetInstance()?.ToString() ?? "null");

		public T As<T>()
			where T: class
		{
			
			if (TypeIndex == 1)
			{
				if (_value1 is T v)
					return v;
				else
					return null;
			}
					
				
			if (TypeIndex == 2)
			{
				if (_value2 is T v)
					return v;
				else
					return null;
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

						
		public static implicit operator AnyOf<T2, T1>(AnyOf<T1, T2> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
			}
	
	public struct AnyOf<T1, T2, T3> : IEquatable<AnyOf<T1, T2, T3>>, ICastable
	{
		public int TypeIndex { get; }

		
		private readonly T1 _value1;
			
		private readonly T2 _value2;
			
		private readonly T3 _value3;
			
		
		public AnyOf(T1 value1)
			: this()
		{
			TypeIndex = 1;
			_value1 = value1;
		}

		public static implicit operator AnyOf<T1, T2, T3>(T1 value1)
		{
			return new AnyOf<T1, T2, T3>(value1);
		}

		public static explicit operator T1(AnyOf<T1, T2, T3> value)
		{
			return value.Cast<T1>();
		}
			
		public AnyOf(T2 value2)
			: this()
		{
			TypeIndex = 2;
			_value2 = value2;
		}

		public static implicit operator AnyOf<T1, T2, T3>(T2 value2)
		{
			return new AnyOf<T1, T2, T3>(value2);
		}

		public static explicit operator T2(AnyOf<T1, T2, T3> value)
		{
			return value.Cast<T2>();
		}
			
		public AnyOf(T3 value3)
			: this()
		{
			TypeIndex = 3;
			_value3 = value3;
		}

		public static implicit operator AnyOf<T1, T2, T3>(T3 value3)
		{
			return new AnyOf<T1, T2, T3>(value3);
		}

		public static explicit operator T3(AnyOf<T1, T2, T3> value)
		{
			return value.Cast<T3>();
		}
			
		bool ICastable.HasInstanceType => true;

		public Type InstanceType
		{
			get
			{
			
				if (TypeIndex == 1)
					return typeof(T1);
				
				if (TypeIndex == 2)
					return typeof(T2);
				
				if (TypeIndex == 3)
					return typeof(T3);
				
				throw new InvalidOperationException("The current instance has not been initialized to a value.");
			}
		}

		public bool IsDefault => TypeIndex == 0;

		public override bool Equals(object obj) => obj is AnyOf<T1, T2, T3> other && Equals(other);

		public bool Equals(AnyOf<T1, T2, T3> other)
		{
			return other == this;
		}

		public override int GetHashCode()
		{
			
			if (TypeIndex == 1)
				return _value1?.GetHashCode() ?? 0;
				
			if (TypeIndex == 2)
				return _value2?.GetHashCode() ?? 0;
				
			if (TypeIndex == 3)
				return _value3?.GetHashCode() ?? 0;
				
			return 0;
		}

		public static bool operator ==(AnyOf<T1, T2, T3> left, AnyOf<T1, T2, T3> right)
		{
			
			if (left.TypeIndex == 1)
			{

				
				if (right.TypeIndex == 1)
					return (T1)left is T1 v && EqualityComparer<T1>.Default.Equals(v, (T1)right);
					
				if (right.TypeIndex == 2)
					return (T1)left is T2 v && EqualityComparer<T2>.Default.Equals(v, (T2)right);
					
				if (right.TypeIndex == 3)
					return (T1)left is T3 v && EqualityComparer<T3>.Default.Equals(v, (T3)right);
					
			}
				
			if (left.TypeIndex == 2)
			{

				
				if (right.TypeIndex == 1)
					return (T2)left is T1 v && EqualityComparer<T1>.Default.Equals(v, (T1)right);
					
				if (right.TypeIndex == 2)
					return (T2)left is T2 v && EqualityComparer<T2>.Default.Equals(v, (T2)right);
					
				if (right.TypeIndex == 3)
					return (T2)left is T3 v && EqualityComparer<T3>.Default.Equals(v, (T3)right);
					
			}
				
			if (left.TypeIndex == 3)
			{

				
				if (right.TypeIndex == 1)
					return (T3)left is T1 v && EqualityComparer<T1>.Default.Equals(v, (T1)right);
					
				if (right.TypeIndex == 2)
					return (T3)left is T2 v && EqualityComparer<T2>.Default.Equals(v, (T2)right);
					
				if (right.TypeIndex == 3)
					return (T3)left is T3 v && EqualityComparer<T3>.Default.Equals(v, (T3)right);
					
			}
				
			return false;
		}

		public static bool operator !=(AnyOf<T1, T2, T3> left, AnyOf<T1, T2, T3> right)
		{
			return !(left == right);
		}

		public bool Is<T>()
		{
			
			if (TypeIndex == 1)
				return _value1 is T;
				
			if (TypeIndex == 2)
				return _value2 is T;
				
			if (TypeIndex == 3)
				return _value3 is T;
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		public bool Is<T>(out T value)
		{
			
			if (TypeIndex == 1)
			{
				if (_value1 is T v)
				{
					value = v;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
					
				
			if (TypeIndex == 2)
			{
				if (_value2 is T v)
				{
					value = v;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
					
				
			if (TypeIndex == 3)
			{
				if (_value3 is T v)
				{
					value = v;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		private static void ThrowTypeIndex(int requested, int actual)
		{
			throw new InvalidOperationException($"The requested value was { requested } but the actual value is { actual }.");
		}

		
		public bool HasValue1 => TypeIndex == 1;
		
		public T1 Value1
		{
			get
			{
				if (TypeIndex != 1)
					ThrowTypeIndex(1, TypeIndex);

				return _value1;
			}
		}
		
		public bool HasValue2 => TypeIndex == 2;
		
		public T2 Value2
		{
			get
			{
				if (TypeIndex != 2)
					ThrowTypeIndex(2, TypeIndex);

				return _value2;
			}
		}
		
		public bool HasValue3 => TypeIndex == 3;
		
		public T3 Value3
		{
			get
			{
				if (TypeIndex != 3)
					ThrowTypeIndex(3, TypeIndex);

				return _value3;
			}
		}
		
		public T Cast<T>()
		{
			
			if (TypeIndex == 1)
			{
				if (_value1 is T v)
					return v;
				else
					throw new InvalidCastException();
			}
					
				
			if (TypeIndex == 2)
			{
				if (_value2 is T v)
					return v;
				else
					throw new InvalidCastException();
			}
					
				
			if (TypeIndex == 3)
			{
				if (_value3 is T v)
					return v;
				else
					throw new InvalidCastException();
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		public object GetInstance()
		{
			
			if (TypeIndex == 1)
			{
				return _value1;
			}
					
				
			if (TypeIndex == 2)
			{
				return _value2;
			}
					
				
			if (TypeIndex == 3)
			{
				return _value3;
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		public override string ToString() => IsDefault ? "" : (GetInstance()?.ToString() ?? "null");

		public T As<T>()
			where T: class
		{
			
			if (TypeIndex == 1)
			{
				if (_value1 is T v)
					return v;
				else
					return null;
			}
					
				
			if (TypeIndex == 2)
			{
				if (_value2 is T v)
					return v;
				else
					return null;
			}
					
				
			if (TypeIndex == 3)
			{
				if (_value3 is T v)
					return v;
				else
					return null;
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

						
		public static implicit operator AnyOf<T1, T2>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3>(AnyOf<T1, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T1>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3>(AnyOf<T2, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T1, T3>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3>(AnyOf<T1, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T1>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3>(AnyOf<T3, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T3>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3>(AnyOf<T2, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T2>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3>(AnyOf<T3, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T1, T3>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T3, T1, T2>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T1, T3, T2>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T2, T3, T1>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T3, T2, T1>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
			}
	
	public struct AnyOf<T1, T2, T3, T4> : IEquatable<AnyOf<T1, T2, T3, T4>>, ICastable
	{
		public int TypeIndex { get; }

		
		private readonly T1 _value1;
			
		private readonly T2 _value2;
			
		private readonly T3 _value3;
			
		private readonly T4 _value4;
			
		
		public AnyOf(T1 value1)
			: this()
		{
			TypeIndex = 1;
			_value1 = value1;
		}

		public static implicit operator AnyOf<T1, T2, T3, T4>(T1 value1)
		{
			return new AnyOf<T1, T2, T3, T4>(value1);
		}

		public static explicit operator T1(AnyOf<T1, T2, T3, T4> value)
		{
			return value.Cast<T1>();
		}
			
		public AnyOf(T2 value2)
			: this()
		{
			TypeIndex = 2;
			_value2 = value2;
		}

		public static implicit operator AnyOf<T1, T2, T3, T4>(T2 value2)
		{
			return new AnyOf<T1, T2, T3, T4>(value2);
		}

		public static explicit operator T2(AnyOf<T1, T2, T3, T4> value)
		{
			return value.Cast<T2>();
		}
			
		public AnyOf(T3 value3)
			: this()
		{
			TypeIndex = 3;
			_value3 = value3;
		}

		public static implicit operator AnyOf<T1, T2, T3, T4>(T3 value3)
		{
			return new AnyOf<T1, T2, T3, T4>(value3);
		}

		public static explicit operator T3(AnyOf<T1, T2, T3, T4> value)
		{
			return value.Cast<T3>();
		}
			
		public AnyOf(T4 value4)
			: this()
		{
			TypeIndex = 4;
			_value4 = value4;
		}

		public static implicit operator AnyOf<T1, T2, T3, T4>(T4 value4)
		{
			return new AnyOf<T1, T2, T3, T4>(value4);
		}

		public static explicit operator T4(AnyOf<T1, T2, T3, T4> value)
		{
			return value.Cast<T4>();
		}
			
		bool ICastable.HasInstanceType => true;

		public Type InstanceType
		{
			get
			{
			
				if (TypeIndex == 1)
					return typeof(T1);
				
				if (TypeIndex == 2)
					return typeof(T2);
				
				if (TypeIndex == 3)
					return typeof(T3);
				
				if (TypeIndex == 4)
					return typeof(T4);
				
				throw new InvalidOperationException("The current instance has not been initialized to a value.");
			}
		}

		public bool IsDefault => TypeIndex == 0;

		public override bool Equals(object obj) => obj is AnyOf<T1, T2, T3, T4> other && Equals(other);

		public bool Equals(AnyOf<T1, T2, T3, T4> other)
		{
			return other == this;
		}

		public override int GetHashCode()
		{
			
			if (TypeIndex == 1)
				return _value1?.GetHashCode() ?? 0;
				
			if (TypeIndex == 2)
				return _value2?.GetHashCode() ?? 0;
				
			if (TypeIndex == 3)
				return _value3?.GetHashCode() ?? 0;
				
			if (TypeIndex == 4)
				return _value4?.GetHashCode() ?? 0;
				
			return 0;
		}

		public static bool operator ==(AnyOf<T1, T2, T3, T4> left, AnyOf<T1, T2, T3, T4> right)
		{
			
			if (left.TypeIndex == 1)
			{

				
				if (right.TypeIndex == 1)
					return (T1)left is T1 v && EqualityComparer<T1>.Default.Equals(v, (T1)right);
					
				if (right.TypeIndex == 2)
					return (T1)left is T2 v && EqualityComparer<T2>.Default.Equals(v, (T2)right);
					
				if (right.TypeIndex == 3)
					return (T1)left is T3 v && EqualityComparer<T3>.Default.Equals(v, (T3)right);
					
				if (right.TypeIndex == 4)
					return (T1)left is T4 v && EqualityComparer<T4>.Default.Equals(v, (T4)right);
					
			}
				
			if (left.TypeIndex == 2)
			{

				
				if (right.TypeIndex == 1)
					return (T2)left is T1 v && EqualityComparer<T1>.Default.Equals(v, (T1)right);
					
				if (right.TypeIndex == 2)
					return (T2)left is T2 v && EqualityComparer<T2>.Default.Equals(v, (T2)right);
					
				if (right.TypeIndex == 3)
					return (T2)left is T3 v && EqualityComparer<T3>.Default.Equals(v, (T3)right);
					
				if (right.TypeIndex == 4)
					return (T2)left is T4 v && EqualityComparer<T4>.Default.Equals(v, (T4)right);
					
			}
				
			if (left.TypeIndex == 3)
			{

				
				if (right.TypeIndex == 1)
					return (T3)left is T1 v && EqualityComparer<T1>.Default.Equals(v, (T1)right);
					
				if (right.TypeIndex == 2)
					return (T3)left is T2 v && EqualityComparer<T2>.Default.Equals(v, (T2)right);
					
				if (right.TypeIndex == 3)
					return (T3)left is T3 v && EqualityComparer<T3>.Default.Equals(v, (T3)right);
					
				if (right.TypeIndex == 4)
					return (T3)left is T4 v && EqualityComparer<T4>.Default.Equals(v, (T4)right);
					
			}
				
			if (left.TypeIndex == 4)
			{

				
				if (right.TypeIndex == 1)
					return (T4)left is T1 v && EqualityComparer<T1>.Default.Equals(v, (T1)right);
					
				if (right.TypeIndex == 2)
					return (T4)left is T2 v && EqualityComparer<T2>.Default.Equals(v, (T2)right);
					
				if (right.TypeIndex == 3)
					return (T4)left is T3 v && EqualityComparer<T3>.Default.Equals(v, (T3)right);
					
				if (right.TypeIndex == 4)
					return (T4)left is T4 v && EqualityComparer<T4>.Default.Equals(v, (T4)right);
					
			}
				
			return false;
		}

		public static bool operator !=(AnyOf<T1, T2, T3, T4> left, AnyOf<T1, T2, T3, T4> right)
		{
			return !(left == right);
		}

		public bool Is<T>()
		{
			
			if (TypeIndex == 1)
				return _value1 is T;
				
			if (TypeIndex == 2)
				return _value2 is T;
				
			if (TypeIndex == 3)
				return _value3 is T;
				
			if (TypeIndex == 4)
				return _value4 is T;
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		public bool Is<T>(out T value)
		{
			
			if (TypeIndex == 1)
			{
				if (_value1 is T v)
				{
					value = v;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
					
				
			if (TypeIndex == 2)
			{
				if (_value2 is T v)
				{
					value = v;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
					
				
			if (TypeIndex == 3)
			{
				if (_value3 is T v)
				{
					value = v;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
					
				
			if (TypeIndex == 4)
			{
				if (_value4 is T v)
				{
					value = v;
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		private static void ThrowTypeIndex(int requested, int actual)
		{
			throw new InvalidOperationException($"The requested value was { requested } but the actual value is { actual }.");
		}

		
		public bool HasValue1 => TypeIndex == 1;
		
		public T1 Value1
		{
			get
			{
				if (TypeIndex != 1)
					ThrowTypeIndex(1, TypeIndex);

				return _value1;
			}
		}
		
		public bool HasValue2 => TypeIndex == 2;
		
		public T2 Value2
		{
			get
			{
				if (TypeIndex != 2)
					ThrowTypeIndex(2, TypeIndex);

				return _value2;
			}
		}
		
		public bool HasValue3 => TypeIndex == 3;
		
		public T3 Value3
		{
			get
			{
				if (TypeIndex != 3)
					ThrowTypeIndex(3, TypeIndex);

				return _value3;
			}
		}
		
		public bool HasValue4 => TypeIndex == 4;
		
		public T4 Value4
		{
			get
			{
				if (TypeIndex != 4)
					ThrowTypeIndex(4, TypeIndex);

				return _value4;
			}
		}
		
		public T Cast<T>()
		{
			
			if (TypeIndex == 1)
			{
				if (_value1 is T v)
					return v;
				else
					throw new InvalidCastException();
			}
					
				
			if (TypeIndex == 2)
			{
				if (_value2 is T v)
					return v;
				else
					throw new InvalidCastException();
			}
					
				
			if (TypeIndex == 3)
			{
				if (_value3 is T v)
					return v;
				else
					throw new InvalidCastException();
			}
					
				
			if (TypeIndex == 4)
			{
				if (_value4 is T v)
					return v;
				else
					throw new InvalidCastException();
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		public object GetInstance()
		{
			
			if (TypeIndex == 1)
			{
				return _value1;
			}
					
				
			if (TypeIndex == 2)
			{
				return _value2;
			}
					
				
			if (TypeIndex == 3)
			{
				return _value3;
			}
					
				
			if (TypeIndex == 4)
			{
				return _value4;
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

		public override string ToString() => IsDefault ? "" : (GetInstance()?.ToString() ?? "null");

		public T As<T>()
			where T: class
		{
			
			if (TypeIndex == 1)
			{
				if (_value1 is T v)
					return v;
				else
					return null;
			}
					
				
			if (TypeIndex == 2)
			{
				if (_value2 is T v)
					return v;
				else
					return null;
			}
					
				
			if (TypeIndex == 3)
			{
				if (_value3 is T v)
					return v;
				else
					return null;
			}
					
				
			if (TypeIndex == 4)
			{
				if (_value4 is T v)
					return v;
				else
					return null;
			}
					
				
			throw new InvalidOperationException("The current instance has not been initialized to a value.");
		}

						
		public static implicit operator AnyOf<T1, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T1, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T2, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T1, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T1, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T3, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T2, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T3, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T1, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T1, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T4, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T4, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T2, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T4, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T4, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T4, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T4, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T1, T2, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T1, T2, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T1, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T2, T1, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T1, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T3, T1, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T1, T3, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T1, T3, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T3, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T2, T3, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T2, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T3, T2, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T1, T2, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T1, T2, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T1, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T2, T1, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T4, T1, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T4, T1, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T1, T4, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T1, T4, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T4, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T2, T4, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T4, T2, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T4, T2, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T1, T3, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T1, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T1, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T3, T1, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T4, T1, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T4, T1, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T1, T4, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T1, T4, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T4, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T3, T4, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T4, T3, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T4, T3, T1> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T3, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T2, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T3, T2, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T4, T2, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T4, T2, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T4, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T2, T4, T3> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T3, T4, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T3, T4, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T4, T3, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
		
		public static implicit operator AnyOf<T1, T2, T3, T4>(AnyOf<T4, T3, T2> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}

							
		public static implicit operator AnyOf<T2, T1, T3, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T3, T1, T2, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T1, T3, T2, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T2, T3, T1, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T3, T2, T1, T4>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T4, T2, T1, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T2, T4, T1, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T1, T4, T2, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T4, T1, T2, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T2, T1, T4, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T1, T2, T4, T3>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T1, T3, T4, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T3, T1, T4, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T4, T1, T3, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T1, T4, T3, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T3, T4, T1, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T4, T3, T1, T2>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T4, T3, T2, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T3, T4, T2, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T2, T4, T3, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T4, T2, T3, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T3, T2, T4, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
						
		public static implicit operator AnyOf<T2, T3, T4, T1>(AnyOf<T1, T2, T3, T4> value)
		{
			
			if (value.TypeIndex == 2)
				return value.Cast<T2>();
					
				
			if (value.TypeIndex == 3)
				return value.Cast<T3>();
					
				
			if (value.TypeIndex == 4)
				return value.Cast<T4>();
					
				
			if (value.TypeIndex == 1)
				return value.Cast<T1>();
					
				
			throw new InvalidOperationException("The instance has not been initialized to a value.");
		}
		
			}
	}