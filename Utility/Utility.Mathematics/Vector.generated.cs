

using System;

namespace Utility.Mathematics
{
	[System.Serializable()]
	public partial struct Vector2f : System.IEquatable<Vector2f>
	{
		public static readonly Vector2f Zero = new Vector2f();
				public float X;
					public float Y;
					public Vector2f(
			float x
			, float y
						)
			{
					this.X = x;
						this.Y = y;
					}

				public static Vector2f AxisX => new Vector2f(1, 0);
				public static Vector2f AxisY => new Vector2f(0, 1);
		
		public override string ToString()
		{
			return $"({X}, {Y})";
		}

		public float Dot(Vector2f other)
		{
			return this.X * other.X  + this.Y * other.Y ;
		}

		public float Dot(ref Vector2f other)
		{
			return this.X * other.X  + this.Y * other.Y ;
		}

		public float LengthSq()
		{
			return this.X * this.X  + this.Y * this.Y ;
		}

		public  float Length()
		{
			return (float)System.Math.Sqrt(LengthSq());
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			Vector2f? other = obj as Vector2f?;
			if (other == null)
				return false;
			return Equals(other.Value);
		}

		public bool Equals(Vector2f other)
		{
			return this == other;
		}

		public void Normalize()
		{
			this = this / Length();
		}

		#region tuple
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void Deconstruct(out float x, out float y)
		{
						x = X;
						y = Y;
					}
		
		public static implicit operator Vector2f((float, float) value)
		{
			return new Vector2f(value.Item1, value.Item2);
		}
		#endregion

		#region static
		public static Vector2f Lerp(Vector2f left, Vector2f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Vector2f Lerp(ref Vector2f left, ref Vector2f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Vector2f left, ref Vector2f right, float offset,  ref Vector2f result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public float Dot(Vector2f left, Vector2f right)
		{
			return left.X * right.X  + left.Y * right.Y ;
		}

		public float Dot(ref Vector2f left, ref Vector2f right)
		{
			return left.X * right.X  + left.Y * right.Y ;
		}

		public static Vector2f operator+(Vector2f left, Vector2f right)
		{
			return new Vector2f(
				left.X + right.X
								, left.Y + right.Y
							);
		}

		public static Vector2f operator-(Vector2f left, Vector2f right)
		{
			return new Vector2f(
				left.X - right.X
								, left.Y - right.Y
							);
		}

		public static Vector2f operator*(Vector2f left, float right)
		{
			return new Vector2f(
				left.X * right
								, left.Y * right
							);
		}

		public static Vector2f operator/(Vector2f left, float right)
		{
			return new Vector2f(
				left.X * right
								, left.Y / right
							);
		}

		public static Vector2f operator*(float left, Vector2f right)
		{
			return new Vector2f(
				left * right.X
								, left * right.Y
							);
		}
		
		public static Vector2f operator+(Vector2f value)
		{
			return value;
		}

		public static Vector2f operator-(Vector2f value)
		{
			return new Vector2f(
				-value.X
									, -value.Y
							);
		}

		public static bool operator==(Vector2f left, Vector2f right)
		{
			return left.X == right.X
								&& left.Y == right.Y
				;
		}

		public static bool operator!=(Vector2f left, Vector2f right)
		{
			return left.X != right.X
								|| left.Y != right.Y
				;
		}
				public static void Transform(Matrix2x2f transform, Vector2f[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix2x2f transform, ref Vector2f value)
		{
			value = new Vector2f(value.X * transform.M11 + value.X * transform.M12, value.Y * transform.M21 + value.Y * transform.M22);
		}
					public static Vector3f[] Transform(Matrix3x2f transform, Vector2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector3f[] buffer = new Vector3f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector3f Transform(Matrix3x2f transform, Vector2f value)
			{
				return new Vector3f(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32);
			}


			public static Vector3f[] Transform(ref Matrix3x2f transform, Vector2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector3f[] buffer = new Vector3f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector3f Transform(ref Matrix3x2f transform, Vector2f value)
			{
				return new Vector3f(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32);
			}
						public static Vector4f[] Transform(Matrix4x2f transform, Vector2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector4f[] buffer = new Vector4f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector4f Transform(Matrix4x2f transform, Vector2f value)
			{
				return new Vector4f(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32, value.X * transform.M41 + value.Y * transform.M42);
			}


			public static Vector4f[] Transform(ref Matrix4x2f transform, Vector2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector4f[] buffer = new Vector4f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector4f Transform(ref Matrix4x2f transform, Vector2f value)
			{
				return new Vector4f(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32, value.X * transform.M41 + value.Y * transform.M42);
			}
								public static Vector2f Transform(Matrix2x3f transform, Vector2f value)
			{
			return new Vector2f(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static Vector2f Transform(Matrix2x3f transform, ref Vector2f value)
			{
			return new Vector2f(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static Vector2f Transform(ref Matrix2x3f transform, ref Vector2f value)
			{
			return new Vector2f(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static void Transform(Matrix2x3f transform, Vector2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				for(int i = 0; i < values.Length; i++)
					Transform(ref transform, ref values[i]);
			}
						#endregion
	}
	[System.Serializable()]
	public partial struct Vector3f : System.IEquatable<Vector3f>
	{
		public static readonly Vector3f Zero = new Vector3f();
				public float X;
					public float Y;
					public float Z;
					public Vector3f(
			float x
			, float y
			, float z
						)
			{
					this.X = x;
						this.Y = y;
						this.Z = z;
					}

				public static Vector3f AxisX => new Vector3f(1, 0, 0);
				public static Vector3f AxisY => new Vector3f(0, 1, 0);
				public static Vector3f AxisZ => new Vector3f(0, 0, 1);
		
		public override string ToString()
		{
			return $"({X}, {Y}, {Z})";
		}

		public float Dot(Vector3f other)
		{
			return this.X * other.X  + this.Y * other.Y  + this.Z * other.Z ;
		}

		public float Dot(ref Vector3f other)
		{
			return this.X * other.X  + this.Y * other.Y  + this.Z * other.Z ;
		}

		public float LengthSq()
		{
			return this.X * this.X  + this.Y * this.Y  + this.Z * this.Z ;
		}

		public  float Length()
		{
			return (float)System.Math.Sqrt(LengthSq());
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode()  ^ this.Z.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			Vector3f? other = obj as Vector3f?;
			if (other == null)
				return false;
			return Equals(other.Value);
		}

		public bool Equals(Vector3f other)
		{
			return this == other;
		}

		public void Normalize()
		{
			this = this / Length();
		}

		#region tuple
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void Deconstruct(out float x, out float y, out float z)
		{
						x = X;
						y = Y;
						z = Z;
					}
		
		public static implicit operator Vector3f((float, float, float) value)
		{
			return new Vector3f(value.Item1, value.Item2, value.Item3);
		}
		#endregion

		#region static
		public static Vector3f Lerp(Vector3f left, Vector3f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Vector3f Lerp(ref Vector3f left, ref Vector3f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Vector3f left, ref Vector3f right, float offset,  ref Vector3f result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public float Dot(Vector3f left, Vector3f right)
		{
			return left.X * right.X  + left.Y * right.Y  + left.Z * right.Z ;
		}

		public float Dot(ref Vector3f left, ref Vector3f right)
		{
			return left.X * right.X  + left.Y * right.Y  + left.Z * right.Z ;
		}

		public static Vector3f operator+(Vector3f left, Vector3f right)
		{
			return new Vector3f(
				left.X + right.X
								, left.Y + right.Y
								, left.Z + right.Z
							);
		}

		public static Vector3f operator-(Vector3f left, Vector3f right)
		{
			return new Vector3f(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
							);
		}

		public static Vector3f operator*(Vector3f left, float right)
		{
			return new Vector3f(
				left.X * right
								, left.Y * right
								, left.Z * right
							);
		}

		public static Vector3f operator/(Vector3f left, float right)
		{
			return new Vector3f(
				left.X * right
								, left.Y / right
								, left.Z / right
							);
		}

		public static Vector3f operator*(float left, Vector3f right)
		{
			return new Vector3f(
				left * right.X
								, left * right.Y
								, left * right.Z
							);
		}
		
		public static Vector3f operator+(Vector3f value)
		{
			return value;
		}

		public static Vector3f operator-(Vector3f value)
		{
			return new Vector3f(
				-value.X
									, -value.Y
									, -value.Z
							);
		}

		public static bool operator==(Vector3f left, Vector3f right)
		{
			return left.X == right.X
								&& left.Y == right.Y
								&& left.Z == right.Z
				;
		}

		public static bool operator!=(Vector3f left, Vector3f right)
		{
			return left.X != right.X
								|| left.Y != right.Y
								|| left.Z != right.Z
				;
		}
				public static void Transform(Matrix3x3f transform, Vector3f[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix3x3f transform, ref Vector3f value)
		{
			value = new Vector3f(value.X * transform.M11 + value.X * transform.M12 + value.X * transform.M13, value.Y * transform.M21 + value.Y * transform.M22 + value.Y * transform.M23, value.Z * transform.M31 + value.Z * transform.M32 + value.Z * transform.M33);
		}
					public static Vector2f[] Transform(Matrix2x3f transform, Vector3f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector2f[] buffer = new Vector2f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector2f Transform(Matrix2x3f transform, Vector3f value)
			{
				return new Vector2f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23);
			}


			public static Vector2f[] Transform(ref Matrix2x3f transform, Vector3f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector2f[] buffer = new Vector2f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector2f Transform(ref Matrix2x3f transform, Vector3f value)
			{
				return new Vector2f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23);
			}
						public static Vector4f[] Transform(Matrix4x3f transform, Vector3f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector4f[] buffer = new Vector4f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector4f Transform(Matrix4x3f transform, Vector3f value)
			{
				return new Vector4f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33, value.X * transform.M41 + value.Y * transform.M42 + value.Z * transform.M43);
			}


			public static Vector4f[] Transform(ref Matrix4x3f transform, Vector3f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector4f[] buffer = new Vector4f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector4f Transform(ref Matrix4x3f transform, Vector3f value)
			{
				return new Vector4f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33, value.X * transform.M41 + value.Y * transform.M42 + value.Z * transform.M43);
			}
									#endregion
	}
	[System.Serializable()]
	public partial struct Vector4f : System.IEquatable<Vector4f>
	{
		public static readonly Vector4f Zero = new Vector4f();
				public float X;
					public float Y;
					public float Z;
					public float W;
					public Vector4f(
			float x
			, float y
			, float z
			, float w
						)
			{
					this.X = x;
						this.Y = y;
						this.Z = z;
						this.W = w;
					}

				public static Vector4f AxisX => new Vector4f(1, 0, 0, 0);
				public static Vector4f AxisY => new Vector4f(0, 1, 0, 0);
				public static Vector4f AxisZ => new Vector4f(0, 0, 1, 0);
				public static Vector4f AxisW => new Vector4f(0, 0, 0, 1);
		
		public override string ToString()
		{
			return $"({X}, {Y}, {Z}, {W})";
		}

		public float Dot(Vector4f other)
		{
			return this.X * other.X  + this.Y * other.Y  + this.Z * other.Z  + this.W * other.W ;
		}

		public float Dot(ref Vector4f other)
		{
			return this.X * other.X  + this.Y * other.Y  + this.Z * other.Z  + this.W * other.W ;
		}

		public float LengthSq()
		{
			return this.X * this.X  + this.Y * this.Y  + this.Z * this.Z  + this.W * this.W ;
		}

		public  float Length()
		{
			return (float)System.Math.Sqrt(LengthSq());
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode()  ^ this.Z.GetHashCode()  ^ this.W.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			Vector4f? other = obj as Vector4f?;
			if (other == null)
				return false;
			return Equals(other.Value);
		}

		public bool Equals(Vector4f other)
		{
			return this == other;
		}

		public void Normalize()
		{
			this = this / Length();
		}

		#region tuple
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void Deconstruct(out float x, out float y, out float z, out float w)
		{
						x = X;
						y = Y;
						z = Z;
						w = W;
					}
		
		public static implicit operator Vector4f((float, float, float, float) value)
		{
			return new Vector4f(value.Item1, value.Item2, value.Item3, value.Item4);
		}
		#endregion

		#region static
		public static Vector4f Lerp(Vector4f left, Vector4f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Vector4f Lerp(ref Vector4f left, ref Vector4f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Vector4f left, ref Vector4f right, float offset,  ref Vector4f result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public float Dot(Vector4f left, Vector4f right)
		{
			return left.X * right.X  + left.Y * right.Y  + left.Z * right.Z  + left.W * right.W ;
		}

		public float Dot(ref Vector4f left, ref Vector4f right)
		{
			return left.X * right.X  + left.Y * right.Y  + left.Z * right.Z  + left.W * right.W ;
		}

		public static Vector4f operator+(Vector4f left, Vector4f right)
		{
			return new Vector4f(
				left.X + right.X
								, left.Y + right.Y
								, left.Z + right.Z
								, left.W + right.W
							);
		}

		public static Vector4f operator-(Vector4f left, Vector4f right)
		{
			return new Vector4f(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
								, left.W - right.W
							);
		}

		public static Vector4f operator*(Vector4f left, float right)
		{
			return new Vector4f(
				left.X * right
								, left.Y * right
								, left.Z * right
								, left.W * right
							);
		}

		public static Vector4f operator/(Vector4f left, float right)
		{
			return new Vector4f(
				left.X * right
								, left.Y / right
								, left.Z / right
								, left.W / right
							);
		}

		public static Vector4f operator*(float left, Vector4f right)
		{
			return new Vector4f(
				left * right.X
								, left * right.Y
								, left * right.Z
								, left * right.W
							);
		}
		
		public static Vector4f operator+(Vector4f value)
		{
			return value;
		}

		public static Vector4f operator-(Vector4f value)
		{
			return new Vector4f(
				-value.X
									, -value.Y
									, -value.Z
									, -value.W
							);
		}

		public static bool operator==(Vector4f left, Vector4f right)
		{
			return left.X == right.X
								&& left.Y == right.Y
								&& left.Z == right.Z
								&& left.W == right.W
				;
		}

		public static bool operator!=(Vector4f left, Vector4f right)
		{
			return left.X != right.X
								|| left.Y != right.Y
								|| left.Z != right.Z
								|| left.W != right.W
				;
		}
				public static void Transform(Matrix4x4f transform, Vector4f[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix4x4f transform, ref Vector4f value)
		{
			value = new Vector4f(value.X * transform.M11 + value.X * transform.M12 + value.X * transform.M13 + value.X * transform.M14, value.Y * transform.M21 + value.Y * transform.M22 + value.Y * transform.M23 + value.Y * transform.M24, value.Z * transform.M31 + value.Z * transform.M32 + value.Z * transform.M33 + value.Z * transform.M34, value.W * transform.M41 + value.W * transform.M42 + value.W * transform.M43 + value.W * transform.M44);
		}
					public static Vector2f[] Transform(Matrix2x4f transform, Vector4f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector2f[] buffer = new Vector2f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector2f Transform(Matrix2x4f transform, Vector4f value)
			{
				return new Vector2f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24);
			}


			public static Vector2f[] Transform(ref Matrix2x4f transform, Vector4f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector2f[] buffer = new Vector2f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector2f Transform(ref Matrix2x4f transform, Vector4f value)
			{
				return new Vector2f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24);
			}
						public static Vector3f[] Transform(Matrix3x4f transform, Vector4f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector3f[] buffer = new Vector3f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector3f Transform(Matrix3x4f transform, Vector4f value)
			{
				return new Vector3f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33 + value.W * transform.M34);
			}


			public static Vector3f[] Transform(ref Matrix3x4f transform, Vector4f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector3f[] buffer = new Vector3f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector3f Transform(ref Matrix3x4f transform, Vector4f value)
			{
				return new Vector3f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33 + value.W * transform.M34);
			}
									#endregion
	}
	[System.Serializable()]
	public partial struct Vector2r : System.IEquatable<Vector2r>
	{
		public static readonly Vector2r Zero = new Vector2r();
				public double X;
					public double Y;
					public Vector2r(
			double x
			, double y
						)
			{
					this.X = x;
						this.Y = y;
					}

				public static Vector2r AxisX => new Vector2r(1, 0);
				public static Vector2r AxisY => new Vector2r(0, 1);
		
		public override string ToString()
		{
			return $"({X}, {Y})";
		}

		public double Dot(Vector2r other)
		{
			return this.X * other.X  + this.Y * other.Y ;
		}

		public double Dot(ref Vector2r other)
		{
			return this.X * other.X  + this.Y * other.Y ;
		}

		public double LengthSq()
		{
			return this.X * this.X  + this.Y * this.Y ;
		}

		public  double Length()
		{
			return (double)System.Math.Sqrt(LengthSq());
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			Vector2r? other = obj as Vector2r?;
			if (other == null)
				return false;
			return Equals(other.Value);
		}

		public bool Equals(Vector2r other)
		{
			return this == other;
		}

		public void Normalize()
		{
			this = this / Length();
		}

		#region tuple
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void Deconstruct(out double x, out double y)
		{
						x = X;
						y = Y;
					}
		
		public static implicit operator Vector2r((double, double) value)
		{
			return new Vector2r(value.Item1, value.Item2);
		}
		#endregion

		#region static
		public static Vector2r Lerp(Vector2r left, Vector2r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Vector2r Lerp(ref Vector2r left, ref Vector2r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Vector2r left, ref Vector2r right, double offset,  ref Vector2r result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public double Dot(Vector2r left, Vector2r right)
		{
			return left.X * right.X  + left.Y * right.Y ;
		}

		public double Dot(ref Vector2r left, ref Vector2r right)
		{
			return left.X * right.X  + left.Y * right.Y ;
		}

		public static Vector2r operator+(Vector2r left, Vector2r right)
		{
			return new Vector2r(
				left.X + right.X
								, left.Y + right.Y
							);
		}

		public static Vector2r operator-(Vector2r left, Vector2r right)
		{
			return new Vector2r(
				left.X - right.X
								, left.Y - right.Y
							);
		}

		public static Vector2r operator*(Vector2r left, double right)
		{
			return new Vector2r(
				left.X * right
								, left.Y * right
							);
		}

		public static Vector2r operator/(Vector2r left, double right)
		{
			return new Vector2r(
				left.X * right
								, left.Y / right
							);
		}

		public static Vector2r operator*(double left, Vector2r right)
		{
			return new Vector2r(
				left * right.X
								, left * right.Y
							);
		}
		
		public static Vector2r operator+(Vector2r value)
		{
			return value;
		}

		public static Vector2r operator-(Vector2r value)
		{
			return new Vector2r(
				-value.X
									, -value.Y
							);
		}

		public static bool operator==(Vector2r left, Vector2r right)
		{
			return left.X == right.X
								&& left.Y == right.Y
				;
		}

		public static bool operator!=(Vector2r left, Vector2r right)
		{
			return left.X != right.X
								|| left.Y != right.Y
				;
		}
				public static void Transform(Matrix2x2r transform, Vector2r[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix2x2r transform, ref Vector2r value)
		{
			value = new Vector2r(value.X * transform.M11 + value.X * transform.M12, value.Y * transform.M21 + value.Y * transform.M22);
		}
					public static Vector3r[] Transform(Matrix3x2r transform, Vector2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector3r[] buffer = new Vector3r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector3r Transform(Matrix3x2r transform, Vector2r value)
			{
				return new Vector3r(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32);
			}


			public static Vector3r[] Transform(ref Matrix3x2r transform, Vector2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector3r[] buffer = new Vector3r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector3r Transform(ref Matrix3x2r transform, Vector2r value)
			{
				return new Vector3r(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32);
			}
						public static Vector4r[] Transform(Matrix4x2r transform, Vector2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector4r[] buffer = new Vector4r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector4r Transform(Matrix4x2r transform, Vector2r value)
			{
				return new Vector4r(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32, value.X * transform.M41 + value.Y * transform.M42);
			}


			public static Vector4r[] Transform(ref Matrix4x2r transform, Vector2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector4r[] buffer = new Vector4r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector4r Transform(ref Matrix4x2r transform, Vector2r value)
			{
				return new Vector4r(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32, value.X * transform.M41 + value.Y * transform.M42);
			}
								public static Vector2r Transform(Matrix2x3r transform, Vector2r value)
			{
			return new Vector2r(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static Vector2r Transform(Matrix2x3r transform, ref Vector2r value)
			{
			return new Vector2r(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static Vector2r Transform(ref Matrix2x3r transform, ref Vector2r value)
			{
			return new Vector2r(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static void Transform(Matrix2x3r transform, Vector2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				for(int i = 0; i < values.Length; i++)
					Transform(ref transform, ref values[i]);
			}
						#endregion
	}
	[System.Serializable()]
	public partial struct Vector3r : System.IEquatable<Vector3r>
	{
		public static readonly Vector3r Zero = new Vector3r();
				public double X;
					public double Y;
					public double Z;
					public Vector3r(
			double x
			, double y
			, double z
						)
			{
					this.X = x;
						this.Y = y;
						this.Z = z;
					}

				public static Vector3r AxisX => new Vector3r(1, 0, 0);
				public static Vector3r AxisY => new Vector3r(0, 1, 0);
				public static Vector3r AxisZ => new Vector3r(0, 0, 1);
		
		public override string ToString()
		{
			return $"({X}, {Y}, {Z})";
		}

		public double Dot(Vector3r other)
		{
			return this.X * other.X  + this.Y * other.Y  + this.Z * other.Z ;
		}

		public double Dot(ref Vector3r other)
		{
			return this.X * other.X  + this.Y * other.Y  + this.Z * other.Z ;
		}

		public double LengthSq()
		{
			return this.X * this.X  + this.Y * this.Y  + this.Z * this.Z ;
		}

		public  double Length()
		{
			return (double)System.Math.Sqrt(LengthSq());
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode()  ^ this.Z.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			Vector3r? other = obj as Vector3r?;
			if (other == null)
				return false;
			return Equals(other.Value);
		}

		public bool Equals(Vector3r other)
		{
			return this == other;
		}

		public void Normalize()
		{
			this = this / Length();
		}

		#region tuple
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void Deconstruct(out double x, out double y, out double z)
		{
						x = X;
						y = Y;
						z = Z;
					}
		
		public static implicit operator Vector3r((double, double, double) value)
		{
			return new Vector3r(value.Item1, value.Item2, value.Item3);
		}
		#endregion

		#region static
		public static Vector3r Lerp(Vector3r left, Vector3r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Vector3r Lerp(ref Vector3r left, ref Vector3r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Vector3r left, ref Vector3r right, double offset,  ref Vector3r result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public double Dot(Vector3r left, Vector3r right)
		{
			return left.X * right.X  + left.Y * right.Y  + left.Z * right.Z ;
		}

		public double Dot(ref Vector3r left, ref Vector3r right)
		{
			return left.X * right.X  + left.Y * right.Y  + left.Z * right.Z ;
		}

		public static Vector3r operator+(Vector3r left, Vector3r right)
		{
			return new Vector3r(
				left.X + right.X
								, left.Y + right.Y
								, left.Z + right.Z
							);
		}

		public static Vector3r operator-(Vector3r left, Vector3r right)
		{
			return new Vector3r(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
							);
		}

		public static Vector3r operator*(Vector3r left, double right)
		{
			return new Vector3r(
				left.X * right
								, left.Y * right
								, left.Z * right
							);
		}

		public static Vector3r operator/(Vector3r left, double right)
		{
			return new Vector3r(
				left.X * right
								, left.Y / right
								, left.Z / right
							);
		}

		public static Vector3r operator*(double left, Vector3r right)
		{
			return new Vector3r(
				left * right.X
								, left * right.Y
								, left * right.Z
							);
		}
		
		public static Vector3r operator+(Vector3r value)
		{
			return value;
		}

		public static Vector3r operator-(Vector3r value)
		{
			return new Vector3r(
				-value.X
									, -value.Y
									, -value.Z
							);
		}

		public static bool operator==(Vector3r left, Vector3r right)
		{
			return left.X == right.X
								&& left.Y == right.Y
								&& left.Z == right.Z
				;
		}

		public static bool operator!=(Vector3r left, Vector3r right)
		{
			return left.X != right.X
								|| left.Y != right.Y
								|| left.Z != right.Z
				;
		}
				public static void Transform(Matrix3x3r transform, Vector3r[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix3x3r transform, ref Vector3r value)
		{
			value = new Vector3r(value.X * transform.M11 + value.X * transform.M12 + value.X * transform.M13, value.Y * transform.M21 + value.Y * transform.M22 + value.Y * transform.M23, value.Z * transform.M31 + value.Z * transform.M32 + value.Z * transform.M33);
		}
					public static Vector2r[] Transform(Matrix2x3r transform, Vector3r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector2r[] buffer = new Vector2r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector2r Transform(Matrix2x3r transform, Vector3r value)
			{
				return new Vector2r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23);
			}


			public static Vector2r[] Transform(ref Matrix2x3r transform, Vector3r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector2r[] buffer = new Vector2r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector2r Transform(ref Matrix2x3r transform, Vector3r value)
			{
				return new Vector2r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23);
			}
						public static Vector4r[] Transform(Matrix4x3r transform, Vector3r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector4r[] buffer = new Vector4r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector4r Transform(Matrix4x3r transform, Vector3r value)
			{
				return new Vector4r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33, value.X * transform.M41 + value.Y * transform.M42 + value.Z * transform.M43);
			}


			public static Vector4r[] Transform(ref Matrix4x3r transform, Vector3r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector4r[] buffer = new Vector4r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector4r Transform(ref Matrix4x3r transform, Vector3r value)
			{
				return new Vector4r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33, value.X * transform.M41 + value.Y * transform.M42 + value.Z * transform.M43);
			}
									#endregion
	}
	[System.Serializable()]
	public partial struct Vector4r : System.IEquatable<Vector4r>
	{
		public static readonly Vector4r Zero = new Vector4r();
				public double X;
					public double Y;
					public double Z;
					public double W;
					public Vector4r(
			double x
			, double y
			, double z
			, double w
						)
			{
					this.X = x;
						this.Y = y;
						this.Z = z;
						this.W = w;
					}

				public static Vector4r AxisX => new Vector4r(1, 0, 0, 0);
				public static Vector4r AxisY => new Vector4r(0, 1, 0, 0);
				public static Vector4r AxisZ => new Vector4r(0, 0, 1, 0);
				public static Vector4r AxisW => new Vector4r(0, 0, 0, 1);
		
		public override string ToString()
		{
			return $"({X}, {Y}, {Z}, {W})";
		}

		public double Dot(Vector4r other)
		{
			return this.X * other.X  + this.Y * other.Y  + this.Z * other.Z  + this.W * other.W ;
		}

		public double Dot(ref Vector4r other)
		{
			return this.X * other.X  + this.Y * other.Y  + this.Z * other.Z  + this.W * other.W ;
		}

		public double LengthSq()
		{
			return this.X * this.X  + this.Y * this.Y  + this.Z * this.Z  + this.W * this.W ;
		}

		public  double Length()
		{
			return (double)System.Math.Sqrt(LengthSq());
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode()  ^ this.Z.GetHashCode()  ^ this.W.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			Vector4r? other = obj as Vector4r?;
			if (other == null)
				return false;
			return Equals(other.Value);
		}

		public bool Equals(Vector4r other)
		{
			return this == other;
		}

		public void Normalize()
		{
			this = this / Length();
		}

		#region tuple
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void Deconstruct(out double x, out double y, out double z, out double w)
		{
						x = X;
						y = Y;
						z = Z;
						w = W;
					}
		
		public static implicit operator Vector4r((double, double, double, double) value)
		{
			return new Vector4r(value.Item1, value.Item2, value.Item3, value.Item4);
		}
		#endregion

		#region static
		public static Vector4r Lerp(Vector4r left, Vector4r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Vector4r Lerp(ref Vector4r left, ref Vector4r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Vector4r left, ref Vector4r right, double offset,  ref Vector4r result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public double Dot(Vector4r left, Vector4r right)
		{
			return left.X * right.X  + left.Y * right.Y  + left.Z * right.Z  + left.W * right.W ;
		}

		public double Dot(ref Vector4r left, ref Vector4r right)
		{
			return left.X * right.X  + left.Y * right.Y  + left.Z * right.Z  + left.W * right.W ;
		}

		public static Vector4r operator+(Vector4r left, Vector4r right)
		{
			return new Vector4r(
				left.X + right.X
								, left.Y + right.Y
								, left.Z + right.Z
								, left.W + right.W
							);
		}

		public static Vector4r operator-(Vector4r left, Vector4r right)
		{
			return new Vector4r(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
								, left.W - right.W
							);
		}

		public static Vector4r operator*(Vector4r left, double right)
		{
			return new Vector4r(
				left.X * right
								, left.Y * right
								, left.Z * right
								, left.W * right
							);
		}

		public static Vector4r operator/(Vector4r left, double right)
		{
			return new Vector4r(
				left.X * right
								, left.Y / right
								, left.Z / right
								, left.W / right
							);
		}

		public static Vector4r operator*(double left, Vector4r right)
		{
			return new Vector4r(
				left * right.X
								, left * right.Y
								, left * right.Z
								, left * right.W
							);
		}
		
		public static Vector4r operator+(Vector4r value)
		{
			return value;
		}

		public static Vector4r operator-(Vector4r value)
		{
			return new Vector4r(
				-value.X
									, -value.Y
									, -value.Z
									, -value.W
							);
		}

		public static bool operator==(Vector4r left, Vector4r right)
		{
			return left.X == right.X
								&& left.Y == right.Y
								&& left.Z == right.Z
								&& left.W == right.W
				;
		}

		public static bool operator!=(Vector4r left, Vector4r right)
		{
			return left.X != right.X
								|| left.Y != right.Y
								|| left.Z != right.Z
								|| left.W != right.W
				;
		}
				public static void Transform(Matrix4x4r transform, Vector4r[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix4x4r transform, ref Vector4r value)
		{
			value = new Vector4r(value.X * transform.M11 + value.X * transform.M12 + value.X * transform.M13 + value.X * transform.M14, value.Y * transform.M21 + value.Y * transform.M22 + value.Y * transform.M23 + value.Y * transform.M24, value.Z * transform.M31 + value.Z * transform.M32 + value.Z * transform.M33 + value.Z * transform.M34, value.W * transform.M41 + value.W * transform.M42 + value.W * transform.M43 + value.W * transform.M44);
		}
					public static Vector2r[] Transform(Matrix2x4r transform, Vector4r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector2r[] buffer = new Vector2r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector2r Transform(Matrix2x4r transform, Vector4r value)
			{
				return new Vector2r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24);
			}


			public static Vector2r[] Transform(ref Matrix2x4r transform, Vector4r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector2r[] buffer = new Vector2r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector2r Transform(ref Matrix2x4r transform, Vector4r value)
			{
				return new Vector2r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24);
			}
						public static Vector3r[] Transform(Matrix3x4r transform, Vector4r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector3r[] buffer = new Vector3r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Vector3r Transform(Matrix3x4r transform, Vector4r value)
			{
				return new Vector3r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33 + value.W * transform.M34);
			}


			public static Vector3r[] Transform(ref Matrix3x4r transform, Vector4r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Vector3r[] buffer = new Vector3r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Vector3r Transform(ref Matrix3x4r transform, Vector4r value)
			{
				return new Vector3r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33 + value.W * transform.M34);
			}
									#endregion
	}
}