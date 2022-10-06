

using System;

namespace Utility.Mathematics
{
	[System.Serializable()]
	public partial struct Point2f : System.IEquatable<Point2f>
	{
		public static readonly Point2f Zero = new Point2f();
				public float X;
					public float Y;
					public Point2f(
			float x
			, float y
						)
			{
					this.X = x;
						this.Y = y;
					}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}
		
		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var other = obj as Point2f?;

			if (other == null)
				return false;

			return Equals(other.Value);
		}

		public bool Equals(Point2f other)
		{
			return this == other;
		}

		#region tuple
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void Deconstruct(out float x, out float y)
		{
						x = X;
						y = Y;
					}
		
		public static implicit operator Point2f((float, float) value)
		{
			return new Point2f(value.Item1, value.Item2);
		}
		#endregion

		#region static
		public static Point2f Lerp(Point2f left, Point2f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Point2f Lerp(ref Point2f left, ref Point2f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Point2f left, ref Point2f right, float offset, ref Point2f result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public static Point2f operator+(Point2f left, Vector2f right)
		{
			return new Point2f(
				left.X + right.X
								, left.Y + right.Y
							);
		}

		public static Vector2f operator-(Point2f left, Point2f right)
		{
			return new Vector2f(
				left.X - right.X
								, left.Y - right.Y
							);
		}

		public static Point2f operator-(Point2f left, Vector2f right)
		{
			return new Point2f(
				left.X - right.X
								, left.Y - right.Y
							);
		}
		
		public static Point2f operator+(Point2f value)
		{
			return value;
		}

		public static Point2f operator-(Point2f value)
		{
			return new Point2f(
				-value.X
									, -value.Y
							);
		}

		public static bool operator==(Point2f left, Point2f right)
		{
			return left.X == right.X
								&& left.Y == right.Y
				;
		}

		public static bool operator!=(Point2f left, Point2f right)
		{
			return left.X != right.X
								|| left.Y != right.Y
				;
		}

		public static explicit operator Point2f(Vector2f value)
		{
			return new Point2f(
				value.X
									, value.Y
							);
		}

		public static implicit operator Vector2f(Point2f value)
		{
			return new Point2f(
				value.X
									, value.Y
							);
		}

				public static void Transform(Matrix2x2f transform, Point2f[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix2x2f transform, ref Point2f value)
		{
			value = new Point2f(value.X * transform.M11 + value.X * transform.M12, value.Y * transform.M21 + value.Y * transform.M22);
		}
					public static Point3f[] Transform(Matrix3x2f transform, Point2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point3f[] buffer = new Point3f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point3f Transform(Matrix3x2f transform, Point2f value)
			{
				return new Point3f(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32);
			}


			public static Point3f[] Transform(ref Matrix3x2f transform, Point2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point3f[] buffer = new Point3f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point3f Transform(ref Matrix3x2f transform, Point2f value)
			{
				return new Point3f(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32);
			}
						public static Point4f[] Transform(Matrix4x2f transform, Point2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point4f[] buffer = new Point4f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point4f Transform(Matrix4x2f transform, Point2f value)
			{
				return new Point4f(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32, value.X * transform.M41 + value.Y * transform.M42);
			}


			public static Point4f[] Transform(ref Matrix4x2f transform, Point2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point4f[] buffer = new Point4f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point4f Transform(ref Matrix4x2f transform, Point2f value)
			{
				return new Point4f(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32, value.X * transform.M41 + value.Y * transform.M42);
			}
								public static Point2f Transform(Matrix2x3f transform, Point2f value)
			{
			return new Point2f(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static Point2f Transform(Matrix2x3f transform, ref Point2f value)
			{
			return new Point2f(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static Point2f Transform(ref Matrix2x3f transform, ref Point2f value)
			{
			return new Point2f(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static void Transform(Matrix2x3f transform, Point2f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				for(int i = 0; i < values.Length; i++)
					Transform(ref transform, ref values[i]);
			}
						#endregion
	}
	[System.Serializable()]
	public partial struct Point3f : System.IEquatable<Point3f>
	{
		public static readonly Point3f Zero = new Point3f();
				public float X;
					public float Y;
					public float Z;
					public Point3f(
			float x
			, float y
			, float z
						)
			{
					this.X = x;
						this.Y = y;
						this.Z = z;
					}

		public override string ToString()
		{
			return $"({X}, {Y}, {Z})";
		}
		
		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode()  ^ this.Z.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var other = obj as Point3f?;

			if (other == null)
				return false;

			return Equals(other.Value);
		}

		public bool Equals(Point3f other)
		{
			return this == other;
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
		
		public static implicit operator Point3f((float, float, float) value)
		{
			return new Point3f(value.Item1, value.Item2, value.Item3);
		}
		#endregion

		#region static
		public static Point3f Lerp(Point3f left, Point3f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Point3f Lerp(ref Point3f left, ref Point3f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Point3f left, ref Point3f right, float offset, ref Point3f result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public static Point3f operator+(Point3f left, Vector3f right)
		{
			return new Point3f(
				left.X + right.X
								, left.Y + right.Y
								, left.Z + right.Z
							);
		}

		public static Vector3f operator-(Point3f left, Point3f right)
		{
			return new Vector3f(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
							);
		}

		public static Point3f operator-(Point3f left, Vector3f right)
		{
			return new Point3f(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
							);
		}
		
		public static Point3f operator+(Point3f value)
		{
			return value;
		}

		public static Point3f operator-(Point3f value)
		{
			return new Point3f(
				-value.X
									, -value.Y
									, -value.Z
							);
		}

		public static bool operator==(Point3f left, Point3f right)
		{
			return left.X == right.X
								&& left.Y == right.Y
								&& left.Z == right.Z
				;
		}

		public static bool operator!=(Point3f left, Point3f right)
		{
			return left.X != right.X
								|| left.Y != right.Y
								|| left.Z != right.Z
				;
		}

		public static explicit operator Point3f(Vector3f value)
		{
			return new Point3f(
				value.X
									, value.Y
									, value.Z
							);
		}

		public static implicit operator Vector3f(Point3f value)
		{
			return new Point3f(
				value.X
									, value.Y
									, value.Z
							);
		}

				public static void Transform(Matrix3x3f transform, Point3f[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix3x3f transform, ref Point3f value)
		{
			value = new Point3f(value.X * transform.M11 + value.X * transform.M12 + value.X * transform.M13, value.Y * transform.M21 + value.Y * transform.M22 + value.Y * transform.M23, value.Z * transform.M31 + value.Z * transform.M32 + value.Z * transform.M33);
		}
					public static Point2f[] Transform(Matrix2x3f transform, Point3f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point2f[] buffer = new Point2f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point2f Transform(Matrix2x3f transform, Point3f value)
			{
				return new Point2f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23);
			}


			public static Point2f[] Transform(ref Matrix2x3f transform, Point3f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point2f[] buffer = new Point2f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point2f Transform(ref Matrix2x3f transform, Point3f value)
			{
				return new Point2f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23);
			}
						public static Point4f[] Transform(Matrix4x3f transform, Point3f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point4f[] buffer = new Point4f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point4f Transform(Matrix4x3f transform, Point3f value)
			{
				return new Point4f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33, value.X * transform.M41 + value.Y * transform.M42 + value.Z * transform.M43);
			}


			public static Point4f[] Transform(ref Matrix4x3f transform, Point3f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point4f[] buffer = new Point4f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point4f Transform(ref Matrix4x3f transform, Point3f value)
			{
				return new Point4f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33, value.X * transform.M41 + value.Y * transform.M42 + value.Z * transform.M43);
			}
									#endregion
	}
	[System.Serializable()]
	public partial struct Point4f : System.IEquatable<Point4f>
	{
		public static readonly Point4f Zero = new Point4f();
				public float X;
					public float Y;
					public float Z;
					public float W;
					public Point4f(
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

		public override string ToString()
		{
			return $"({X}, {Y}, {Z}, {W})";
		}
		
		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode()  ^ this.Z.GetHashCode()  ^ this.W.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var other = obj as Point4f?;

			if (other == null)
				return false;

			return Equals(other.Value);
		}

		public bool Equals(Point4f other)
		{
			return this == other;
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
		
		public static implicit operator Point4f((float, float, float, float) value)
		{
			return new Point4f(value.Item1, value.Item2, value.Item3, value.Item4);
		}
		#endregion

		#region static
		public static Point4f Lerp(Point4f left, Point4f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Point4f Lerp(ref Point4f left, ref Point4f right, float offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Point4f left, ref Point4f right, float offset, ref Point4f result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public static Point4f operator+(Point4f left, Vector4f right)
		{
			return new Point4f(
				left.X + right.X
								, left.Y + right.Y
								, left.Z + right.Z
								, left.W + right.W
							);
		}

		public static Vector4f operator-(Point4f left, Point4f right)
		{
			return new Vector4f(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
								, left.W - right.W
							);
		}

		public static Point4f operator-(Point4f left, Vector4f right)
		{
			return new Point4f(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
								, left.W - right.W
							);
		}
		
		public static Point4f operator+(Point4f value)
		{
			return value;
		}

		public static Point4f operator-(Point4f value)
		{
			return new Point4f(
				-value.X
									, -value.Y
									, -value.Z
									, -value.W
							);
		}

		public static bool operator==(Point4f left, Point4f right)
		{
			return left.X == right.X
								&& left.Y == right.Y
								&& left.Z == right.Z
								&& left.W == right.W
				;
		}

		public static bool operator!=(Point4f left, Point4f right)
		{
			return left.X != right.X
								|| left.Y != right.Y
								|| left.Z != right.Z
								|| left.W != right.W
				;
		}

		public static explicit operator Point4f(Vector4f value)
		{
			return new Point4f(
				value.X
									, value.Y
									, value.Z
									, value.W
							);
		}

		public static implicit operator Vector4f(Point4f value)
		{
			return new Point4f(
				value.X
									, value.Y
									, value.Z
									, value.W
							);
		}

				public static void Transform(Matrix4x4f transform, Point4f[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix4x4f transform, ref Point4f value)
		{
			value = new Point4f(value.X * transform.M11 + value.X * transform.M12 + value.X * transform.M13 + value.X * transform.M14, value.Y * transform.M21 + value.Y * transform.M22 + value.Y * transform.M23 + value.Y * transform.M24, value.Z * transform.M31 + value.Z * transform.M32 + value.Z * transform.M33 + value.Z * transform.M34, value.W * transform.M41 + value.W * transform.M42 + value.W * transform.M43 + value.W * transform.M44);
		}
					public static Point2f[] Transform(Matrix2x4f transform, Point4f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point2f[] buffer = new Point2f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point2f Transform(Matrix2x4f transform, Point4f value)
			{
				return new Point2f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24);
			}


			public static Point2f[] Transform(ref Matrix2x4f transform, Point4f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point2f[] buffer = new Point2f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point2f Transform(ref Matrix2x4f transform, Point4f value)
			{
				return new Point2f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24);
			}
						public static Point3f[] Transform(Matrix3x4f transform, Point4f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point3f[] buffer = new Point3f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point3f Transform(Matrix3x4f transform, Point4f value)
			{
				return new Point3f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33 + value.W * transform.M34);
			}


			public static Point3f[] Transform(ref Matrix3x4f transform, Point4f[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point3f[] buffer = new Point3f[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point3f Transform(ref Matrix3x4f transform, Point4f value)
			{
				return new Point3f(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33 + value.W * transform.M34);
			}
									#endregion
	}
	[System.Serializable()]
	public partial struct Point2r : System.IEquatable<Point2r>
	{
		public static readonly Point2r Zero = new Point2r();
				public double X;
					public double Y;
					public Point2r(
			double x
			, double y
						)
			{
					this.X = x;
						this.Y = y;
					}

		public override string ToString()
		{
			return $"({X}, {Y})";
		}
		
		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var other = obj as Point2r?;

			if (other == null)
				return false;

			return Equals(other.Value);
		}

		public bool Equals(Point2r other)
		{
			return this == other;
		}

		#region tuple
		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void Deconstruct(out double x, out double y)
		{
						x = X;
						y = Y;
					}
		
		public static implicit operator Point2r((double, double) value)
		{
			return new Point2r(value.Item1, value.Item2);
		}
		#endregion

		#region static
		public static Point2r Lerp(Point2r left, Point2r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Point2r Lerp(ref Point2r left, ref Point2r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Point2r left, ref Point2r right, double offset, ref Point2r result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public static Point2r operator+(Point2r left, Vector2r right)
		{
			return new Point2r(
				left.X + right.X
								, left.Y + right.Y
							);
		}

		public static Vector2r operator-(Point2r left, Point2r right)
		{
			return new Vector2r(
				left.X - right.X
								, left.Y - right.Y
							);
		}

		public static Point2r operator-(Point2r left, Vector2r right)
		{
			return new Point2r(
				left.X - right.X
								, left.Y - right.Y
							);
		}
		
		public static Point2r operator+(Point2r value)
		{
			return value;
		}

		public static Point2r operator-(Point2r value)
		{
			return new Point2r(
				-value.X
									, -value.Y
							);
		}

		public static bool operator==(Point2r left, Point2r right)
		{
			return left.X == right.X
								&& left.Y == right.Y
				;
		}

		public static bool operator!=(Point2r left, Point2r right)
		{
			return left.X != right.X
								|| left.Y != right.Y
				;
		}

		public static explicit operator Point2r(Vector2r value)
		{
			return new Point2r(
				value.X
									, value.Y
							);
		}

		public static implicit operator Vector2r(Point2r value)
		{
			return new Point2r(
				value.X
									, value.Y
							);
		}

				public static void Transform(Matrix2x2r transform, Point2r[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix2x2r transform, ref Point2r value)
		{
			value = new Point2r(value.X * transform.M11 + value.X * transform.M12, value.Y * transform.M21 + value.Y * transform.M22);
		}
					public static Point3r[] Transform(Matrix3x2r transform, Point2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point3r[] buffer = new Point3r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point3r Transform(Matrix3x2r transform, Point2r value)
			{
				return new Point3r(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32);
			}


			public static Point3r[] Transform(ref Matrix3x2r transform, Point2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point3r[] buffer = new Point3r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point3r Transform(ref Matrix3x2r transform, Point2r value)
			{
				return new Point3r(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32);
			}
						public static Point4r[] Transform(Matrix4x2r transform, Point2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point4r[] buffer = new Point4r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point4r Transform(Matrix4x2r transform, Point2r value)
			{
				return new Point4r(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32, value.X * transform.M41 + value.Y * transform.M42);
			}


			public static Point4r[] Transform(ref Matrix4x2r transform, Point2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point4r[] buffer = new Point4r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point4r Transform(ref Matrix4x2r transform, Point2r value)
			{
				return new Point4r(value.X * transform.M11 + value.Y * transform.M12, value.X * transform.M21 + value.Y * transform.M22, value.X * transform.M31 + value.Y * transform.M32, value.X * transform.M41 + value.Y * transform.M42);
			}
								public static Point2r Transform(Matrix2x3r transform, Point2r value)
			{
			return new Point2r(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static Point2r Transform(Matrix2x3r transform, ref Point2r value)
			{
			return new Point2r(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static Point2r Transform(ref Matrix2x3r transform, ref Point2r value)
			{
			return new Point2r(transform.M11 * value.X + transform.M12 * value.Y + transform.M13, transform.M21 * value.X + transform.M22 * value.Y + transform.M23);			}

			public static void Transform(Matrix2x3r transform, Point2r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				for(int i = 0; i < values.Length; i++)
					Transform(ref transform, ref values[i]);
			}
						#endregion
	}
	[System.Serializable()]
	public partial struct Point3r : System.IEquatable<Point3r>
	{
		public static readonly Point3r Zero = new Point3r();
				public double X;
					public double Y;
					public double Z;
					public Point3r(
			double x
			, double y
			, double z
						)
			{
					this.X = x;
						this.Y = y;
						this.Z = z;
					}

		public override string ToString()
		{
			return $"({X}, {Y}, {Z})";
		}
		
		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode()  ^ this.Z.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var other = obj as Point3r?;

			if (other == null)
				return false;

			return Equals(other.Value);
		}

		public bool Equals(Point3r other)
		{
			return this == other;
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
		
		public static implicit operator Point3r((double, double, double) value)
		{
			return new Point3r(value.Item1, value.Item2, value.Item3);
		}
		#endregion

		#region static
		public static Point3r Lerp(Point3r left, Point3r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Point3r Lerp(ref Point3r left, ref Point3r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Point3r left, ref Point3r right, double offset, ref Point3r result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public static Point3r operator+(Point3r left, Vector3r right)
		{
			return new Point3r(
				left.X + right.X
								, left.Y + right.Y
								, left.Z + right.Z
							);
		}

		public static Vector3r operator-(Point3r left, Point3r right)
		{
			return new Vector3r(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
							);
		}

		public static Point3r operator-(Point3r left, Vector3r right)
		{
			return new Point3r(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
							);
		}
		
		public static Point3r operator+(Point3r value)
		{
			return value;
		}

		public static Point3r operator-(Point3r value)
		{
			return new Point3r(
				-value.X
									, -value.Y
									, -value.Z
							);
		}

		public static bool operator==(Point3r left, Point3r right)
		{
			return left.X == right.X
								&& left.Y == right.Y
								&& left.Z == right.Z
				;
		}

		public static bool operator!=(Point3r left, Point3r right)
		{
			return left.X != right.X
								|| left.Y != right.Y
								|| left.Z != right.Z
				;
		}

		public static explicit operator Point3r(Vector3r value)
		{
			return new Point3r(
				value.X
									, value.Y
									, value.Z
							);
		}

		public static implicit operator Vector3r(Point3r value)
		{
			return new Point3r(
				value.X
									, value.Y
									, value.Z
							);
		}

				public static void Transform(Matrix3x3r transform, Point3r[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix3x3r transform, ref Point3r value)
		{
			value = new Point3r(value.X * transform.M11 + value.X * transform.M12 + value.X * transform.M13, value.Y * transform.M21 + value.Y * transform.M22 + value.Y * transform.M23, value.Z * transform.M31 + value.Z * transform.M32 + value.Z * transform.M33);
		}
					public static Point2r[] Transform(Matrix2x3r transform, Point3r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point2r[] buffer = new Point2r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point2r Transform(Matrix2x3r transform, Point3r value)
			{
				return new Point2r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23);
			}


			public static Point2r[] Transform(ref Matrix2x3r transform, Point3r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point2r[] buffer = new Point2r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point2r Transform(ref Matrix2x3r transform, Point3r value)
			{
				return new Point2r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23);
			}
						public static Point4r[] Transform(Matrix4x3r transform, Point3r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point4r[] buffer = new Point4r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point4r Transform(Matrix4x3r transform, Point3r value)
			{
				return new Point4r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33, value.X * transform.M41 + value.Y * transform.M42 + value.Z * transform.M43);
			}


			public static Point4r[] Transform(ref Matrix4x3r transform, Point3r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point4r[] buffer = new Point4r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point4r Transform(ref Matrix4x3r transform, Point3r value)
			{
				return new Point4r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33, value.X * transform.M41 + value.Y * transform.M42 + value.Z * transform.M43);
			}
									#endregion
	}
	[System.Serializable()]
	public partial struct Point4r : System.IEquatable<Point4r>
	{
		public static readonly Point4r Zero = new Point4r();
				public double X;
					public double Y;
					public double Z;
					public double W;
					public Point4r(
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

		public override string ToString()
		{
			return $"({X}, {Y}, {Z}, {W})";
		}
		
		public override int GetHashCode()
		{
			return this.X.GetHashCode()  ^ this.Y.GetHashCode()  ^ this.Z.GetHashCode()  ^ this.W.GetHashCode() ;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var other = obj as Point4r?;

			if (other == null)
				return false;

			return Equals(other.Value);
		}

		public bool Equals(Point4r other)
		{
			return this == other;
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
		
		public static implicit operator Point4r((double, double, double, double) value)
		{
			return new Point4r(value.Item1, value.Item2, value.Item3, value.Item4);
		}
		#endregion

		#region static
		public static Point4r Lerp(Point4r left, Point4r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static Point4r Lerp(ref Point4r left, ref Point4r right, double offset)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			return left + (right - left) * offset;
		}

		public static void Lerp(ref Point4r left, ref Point4r right, double offset, ref Point4r result)
		{
			if (offset < 0)
				offset = 0;
			if (offset > 1)
				offset = 1;
			result = left + (right - left) * offset;
		}

		public static Point4r operator+(Point4r left, Vector4r right)
		{
			return new Point4r(
				left.X + right.X
								, left.Y + right.Y
								, left.Z + right.Z
								, left.W + right.W
							);
		}

		public static Vector4r operator-(Point4r left, Point4r right)
		{
			return new Vector4r(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
								, left.W - right.W
							);
		}

		public static Point4r operator-(Point4r left, Vector4r right)
		{
			return new Point4r(
				left.X - right.X
								, left.Y - right.Y
								, left.Z - right.Z
								, left.W - right.W
							);
		}
		
		public static Point4r operator+(Point4r value)
		{
			return value;
		}

		public static Point4r operator-(Point4r value)
		{
			return new Point4r(
				-value.X
									, -value.Y
									, -value.Z
									, -value.W
							);
		}

		public static bool operator==(Point4r left, Point4r right)
		{
			return left.X == right.X
								&& left.Y == right.Y
								&& left.Z == right.Z
								&& left.W == right.W
				;
		}

		public static bool operator!=(Point4r left, Point4r right)
		{
			return left.X != right.X
								|| left.Y != right.Y
								|| left.Z != right.Z
								|| left.W != right.W
				;
		}

		public static explicit operator Point4r(Vector4r value)
		{
			return new Point4r(
				value.X
									, value.Y
									, value.Z
									, value.W
							);
		}

		public static implicit operator Vector4r(Point4r value)
		{
			return new Point4r(
				value.X
									, value.Y
									, value.Z
									, value.W
							);
		}

				public static void Transform(Matrix4x4r transform, Point4r[] values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			for(int i = 0; i < values.Length; i++)
				Transform(transform, ref values[i]);
		}

		public static void Transform(Matrix4x4r transform, ref Point4r value)
		{
			value = new Point4r(value.X * transform.M11 + value.X * transform.M12 + value.X * transform.M13 + value.X * transform.M14, value.Y * transform.M21 + value.Y * transform.M22 + value.Y * transform.M23 + value.Y * transform.M24, value.Z * transform.M31 + value.Z * transform.M32 + value.Z * transform.M33 + value.Z * transform.M34, value.W * transform.M41 + value.W * transform.M42 + value.W * transform.M43 + value.W * transform.M44);
		}
					public static Point2r[] Transform(Matrix2x4r transform, Point4r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point2r[] buffer = new Point2r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point2r Transform(Matrix2x4r transform, Point4r value)
			{
				return new Point2r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24);
			}


			public static Point2r[] Transform(ref Matrix2x4r transform, Point4r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point2r[] buffer = new Point2r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point2r Transform(ref Matrix2x4r transform, Point4r value)
			{
				return new Point2r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24);
			}
						public static Point3r[] Transform(Matrix3x4r transform, Point4r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point3r[] buffer = new Point3r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(transform, values[i]);
				return buffer;
			}

			public static Point3r Transform(Matrix3x4r transform, Point4r value)
			{
				return new Point3r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33 + value.W * transform.M34);
			}


			public static Point3r[] Transform(ref Matrix3x4r transform, Point4r[] values)
			{
				if (values == null)
					throw new ArgumentNullException("values");
				Point3r[] buffer = new Point3r[values.Length];
				for(int i = 0; i < values.Length; i++)
					buffer[i] = Transform(ref transform, values[i]);
				return buffer;
			}

			public static Point3r Transform(ref Matrix3x4r transform, Point4r value)
			{
				return new Point3r(value.X * transform.M11 + value.Y * transform.M12 + value.Z * transform.M13 + value.W * transform.M14, value.X * transform.M21 + value.Y * transform.M22 + value.Z * transform.M23 + value.W * transform.M24, value.X * transform.M31 + value.Y * transform.M32 + value.Z * transform.M33 + value.W * transform.M34);
			}
									#endregion
	}
}