
using System;

namespace Utility.Mathematics
{

	[System.Serializable()]
	public partial struct Matrix2x2f : System.IEquatable<Matrix2x2f>
	{
		public static readonly Matrix2x2f Zero = new Matrix2x2f();

				public static readonly Matrix2x2f Identity = new Matrix2x2f(1, 0, 0, 1);

		
		public float M11  , M12  , M21  , M22  ;

		public Matrix2x2f( float m11  ,float m12  ,float m21  ,float m22 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M21 = m21;
						this.M22 = m22;
					}

		public Matrix2x2f Transpose()
		{
			return new Matrix2x2f(
				M11 , M21 , M12 , M22 			);
		}
		
		public static Matrix2x2f operator*(Matrix2x2f left, float right)
		{
			return new Matrix2x2f(
		 left.M11 * right , left.M12 * right , left.M21 * right , left.M22 * right 			);
		}
		
		public static Matrix2x2f operator*(float left, Matrix2x2f right)
		{
			return new Matrix2x2f(
		 left * right.M11 , left * right.M12 , left * right.M21 , left * right.M22 			);
		}
		
		public static Matrix2x2f operator/(Matrix2x2f left, float right)
		{
			return new Matrix2x2f(
		 left.M11 / right , left.M12 / right , left.M21 / right , left.M22 / right 			);
		}
		
		public static Matrix2x2f operator+(Matrix2x2f left, Matrix2x2f right)
		{
			return new Matrix2x2f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 			);
		}
		
		public static Matrix2x2f operator-(Matrix2x2f left, Matrix2x2f right)
		{
			return new Matrix2x2f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 			);
		}
		
		public static Matrix2x2f operator+(Matrix2x2f value)
		{
			return value;
		}
		
		public static Matrix2x2f operator-(Matrix2x2f value)
		{
			return new Matrix2x2f(
		-value.M11 ,-value.M12 ,-value.M21 ,-value.M22 			);
		}
		
		public static bool operator==(Matrix2x2f left, Matrix2x2f right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M21 == right.M21 && left.M22 == right.M22 ;
		}
		
		public static bool operator!=(Matrix2x2f left, Matrix2x2f right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M21 != right.M21 || left.M22 != right.M22 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix2x2f m && Equals(m);
		}

		public bool Equals(Matrix2x2f other)
		{
			return this == other;
		}

		public unsafe float this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix2x2f m = this;

				return ((float*)&m)[row * 2 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix2x2f m = this;

				((float*)&m)[row * 2 + column] = value;
			}
		}

		public unsafe Vector2f GetRow(int row)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x2f m = this;

			return ((Vector2f*)&m)[row];
		}

		public unsafe Vector2f GetColumn(int column)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x2f m = this;
			float* p = (float*)&m;

			return new Vector2f(p[0 + column], p[2 + column]);
		}

		public unsafe void SetRow(int row, Vector2f value)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x2f m = this;

			((Vector2f*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector2f value)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x2f m = this;
			float* p = (float*)&m;

						p[0 + column] = value.X;
						p[2 + column] = value.Y;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

				public bool IsIdentity
			{
				get
				{
				return  this.M11 == 1 
				 &&  this.M12 == 0 
				 &&  this.M21 == 0 
				 &&  this.M22 == 1 
				;
				}
			}
		
		public static Vector2f Transform(Vector2f left, Matrix2x2f right)
		{
			return new Vector2f(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22
			);
		}

		public Vector2f Transform(Vector2f value)
		{
			return new Vector2f(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y
			);
		}

		public static Vector2f Transform(Matrix2x2f left, Vector2f right)
		{
			return new Vector2f(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y
			);
		}

		public static Point2f Transform(Point2f left, Matrix2x2f right)
		{
			return new Point2f(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22
			);
		}

		public Point2f Transform(Point2f value)
		{
			return new Point2f(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y
			);
		}

		public static Point2f Transform(Matrix2x2f left, Point2f right)
		{
			return new Point2f(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y
			);
		}
		
		public static Matrix2x2f FromRows(Vector2f row1, Vector2f row2)
		{
			return new Matrix2x2f(
			row1.X, row1.Y, row2.X, row2.Y
			);
		}
		
		public static Matrix2x2f FromRows(Vector2f[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 2)
				throw new ArgumentException("Expected a number of 2 rows.", nameof(rows));

			return FromRows(rows[0], rows[1]);
		}
		
		public static Matrix2x2f FromColumns(Vector2f column1, Vector2f column2)
		{
			return new Matrix2x2f(
			column1.X, column1.Y, column2.X, column2.Y
			);
		}
		
		public static Matrix2x2f FromColumns(Vector2f[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 2)
				throw new ArgumentException("Expected a number of 2 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1]);
		}

		public static Matrix2x2f operator*(Matrix2x2f left, Matrix2x2f right)
		{
			return new Matrix2x2f(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 			);
		}
		public static Matrix2x3f operator*(Matrix2x2f left, Matrix2x3f right)
		{
			return new Matrix2x3f(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 			);
		}
		public static Matrix2x4f operator*(Matrix2x2f left, Matrix2x4f right)
		{
			return new Matrix2x4f(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M11 * right.M14  +  left.M12 * right.M24 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M21 * right.M14  +  left.M22 * right.M24 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix2x3f : System.IEquatable<Matrix2x3f>
	{
		public static readonly Matrix2x3f Zero = new Matrix2x3f();

		
		public float M11  , M12  , M13  , M21  , M22  , M23  ;

		public Matrix2x3f( float m11  ,float m12  ,float m13  ,float m21  ,float m22  ,float m23 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
					}

		public Matrix3x2f Transpose()
		{
			return new Matrix3x2f(
				M11 , M21 , M12 , M22 , M13 , M23 			);
		}
		
		public static Matrix2x3f operator*(Matrix2x3f left, float right)
		{
			return new Matrix2x3f(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M21 * right , left.M22 * right , left.M23 * right 			);
		}
		
		public static Matrix2x3f operator*(float left, Matrix2x3f right)
		{
			return new Matrix2x3f(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M21 , left * right.M22 , left * right.M23 			);
		}
		
		public static Matrix2x3f operator/(Matrix2x3f left, float right)
		{
			return new Matrix2x3f(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M21 / right , left.M22 / right , left.M23 / right 			);
		}
		
		public static Matrix2x3f operator+(Matrix2x3f left, Matrix2x3f right)
		{
			return new Matrix2x3f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 			);
		}
		
		public static Matrix2x3f operator-(Matrix2x3f left, Matrix2x3f right)
		{
			return new Matrix2x3f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 			);
		}
		
		public static Matrix2x3f operator+(Matrix2x3f value)
		{
			return value;
		}
		
		public static Matrix2x3f operator-(Matrix2x3f value)
		{
			return new Matrix2x3f(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M21 ,-value.M22 ,-value.M23 			);
		}
		
		public static bool operator==(Matrix2x3f left, Matrix2x3f right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 ;
		}
		
		public static bool operator!=(Matrix2x3f left, Matrix2x3f right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix2x3f m && Equals(m);
		}

		public bool Equals(Matrix2x3f other)
		{
			return this == other;
		}

		public unsafe float this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix2x3f m = this;

				return ((float*)&m)[row * 2 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix2x3f m = this;

				((float*)&m)[row * 2 + column] = value;
			}
		}

		public unsafe Vector3f GetRow(int row)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x3f m = this;

			return ((Vector3f*)&m)[row];
		}

		public unsafe Vector2f GetColumn(int column)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x3f m = this;
			float* p = (float*)&m;

			return new Vector2f(p[0 + column], p[3 + column]);
		}

		public unsafe void SetRow(int row, Vector3f value)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x3f m = this;

			((Vector3f*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector2f value)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x3f m = this;
			float* p = (float*)&m;

						p[0 + column] = value.X;
						p[3 + column] = value.Y;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector3f Transform(Vector2f left, Matrix2x3f right)
		{
			return new Vector3f(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22, left.X * right.M13 + left.Y * right.M23
			);
		}

		public Vector2f Transform(Vector3f value)
		{
			return new Vector2f(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z
			);
		}

		public static Vector2f Transform(Matrix2x3f left, Vector3f right)
		{
			return new Vector2f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z
			);
		}

		public static Point3f Transform(Point2f left, Matrix2x3f right)
		{
			return new Point3f(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22, left.X * right.M13 + left.Y * right.M23
			);
		}

		public Point2f Transform(Point3f value)
		{
			return new Point2f(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z
			);
		}

		public static Point2f Transform(Matrix2x3f left, Point3f right)
		{
			return new Point2f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z
			);
		}
		
		public static Matrix2x3f FromRows(Vector3f row1, Vector3f row2)
		{
			return new Matrix2x3f(
			row1.X, row1.Y, row1.Z, row2.X, row2.Y, row2.Z
			);
		}
		
		public static Matrix2x3f FromRows(Vector3f[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 2)
				throw new ArgumentException("Expected a number of 2 rows.", nameof(rows));

			return FromRows(rows[0], rows[1]);
		}
		
		public static Matrix2x3f FromColumns(Vector2f column1, Vector2f column2, Vector2f column3)
		{
			return new Matrix2x3f(
			column1.X, column1.Y, column2.X, column2.Y, column3.X, column3.Y
			);
		}
		
		public static Matrix2x3f FromColumns(Vector2f[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 3)
				throw new ArgumentException("Expected a number of 3 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2]);
		}

		public static Matrix2x2f operator*(Matrix2x3f left, Matrix3x2f right)
		{
			return new Matrix2x2f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 			);
		}
		public static Matrix2x3f operator*(Matrix2x3f left, Matrix3x3f right)
		{
			return new Matrix2x3f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 			);
		}
		public static Matrix2x4f operator*(Matrix2x3f left, Matrix3x4f right)
		{
			return new Matrix2x4f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix2x4f : System.IEquatable<Matrix2x4f>
	{
		public static readonly Matrix2x4f Zero = new Matrix2x4f();

		
		public float M11  , M12  , M13  , M14  , M21  , M22  , M23  , M24  ;

		public Matrix2x4f( float m11  ,float m12  ,float m13  ,float m14  ,float m21  ,float m22  ,float m23  ,float m24 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M14 = m14;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M24 = m24;
					}

		public Matrix4x2f Transpose()
		{
			return new Matrix4x2f(
				M11 , M21 , M12 , M22 , M13 , M23 , M14 , M24 			);
		}
		
		public static Matrix2x4f operator*(Matrix2x4f left, float right)
		{
			return new Matrix2x4f(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M14 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M24 * right 			);
		}
		
		public static Matrix2x4f operator*(float left, Matrix2x4f right)
		{
			return new Matrix2x4f(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M14 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M24 			);
		}
		
		public static Matrix2x4f operator/(Matrix2x4f left, float right)
		{
			return new Matrix2x4f(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M14 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M24 / right 			);
		}
		
		public static Matrix2x4f operator+(Matrix2x4f left, Matrix2x4f right)
		{
			return new Matrix2x4f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 			);
		}
		
		public static Matrix2x4f operator-(Matrix2x4f left, Matrix2x4f right)
		{
			return new Matrix2x4f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 			);
		}
		
		public static Matrix2x4f operator+(Matrix2x4f value)
		{
			return value;
		}
		
		public static Matrix2x4f operator-(Matrix2x4f value)
		{
			return new Matrix2x4f(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M14 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M24 			);
		}
		
		public static bool operator==(Matrix2x4f left, Matrix2x4f right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24 ;
		}
		
		public static bool operator!=(Matrix2x4f left, Matrix2x4f right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M24 != right.M24 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M14.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M24.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix2x4f m && Equals(m);
		}

		public bool Equals(Matrix2x4f other)
		{
			return this == other;
		}

		public unsafe float this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix2x4f m = this;

				return ((float*)&m)[row * 2 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix2x4f m = this;

				((float*)&m)[row * 2 + column] = value;
			}
		}

		public unsafe Vector4f GetRow(int row)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x4f m = this;

			return ((Vector4f*)&m)[row];
		}

		public unsafe Vector2f GetColumn(int column)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x4f m = this;
			float* p = (float*)&m;

			return new Vector2f(p[0 + column], p[4 + column]);
		}

		public unsafe void SetRow(int row, Vector4f value)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x4f m = this;

			((Vector4f*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector2f value)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x4f m = this;
			float* p = (float*)&m;

						p[0 + column] = value.X;
						p[4 + column] = value.Y;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector4f Transform(Vector2f left, Matrix2x4f right)
		{
			return new Vector4f(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22, left.X * right.M13 + left.Y * right.M23, left.X * right.M14 + left.Y * right.M24
			);
		}

		public Vector2f Transform(Vector4f value)
		{
			return new Vector2f(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W
			);
		}

		public static Vector2f Transform(Matrix2x4f left, Vector4f right)
		{
			return new Vector2f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W
			);
		}

		public static Point4f Transform(Point2f left, Matrix2x4f right)
		{
			return new Point4f(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22, left.X * right.M13 + left.Y * right.M23, left.X * right.M14 + left.Y * right.M24
			);
		}

		public Point2f Transform(Point4f value)
		{
			return new Point2f(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W
			);
		}

		public static Point2f Transform(Matrix2x4f left, Point4f right)
		{
			return new Point2f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W
			);
		}
		
		public static Matrix2x4f FromRows(Vector4f row1, Vector4f row2)
		{
			return new Matrix2x4f(
			row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W
			);
		}
		
		public static Matrix2x4f FromRows(Vector4f[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 2)
				throw new ArgumentException("Expected a number of 2 rows.", nameof(rows));

			return FromRows(rows[0], rows[1]);
		}
		
		public static Matrix2x4f FromColumns(Vector2f column1, Vector2f column2, Vector2f column3, Vector2f column4)
		{
			return new Matrix2x4f(
			column1.X, column1.Y, column2.X, column2.Y, column3.X, column3.Y, column4.X, column4.Y
			);
		}
		
		public static Matrix2x4f FromColumns(Vector2f[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 4)
				throw new ArgumentException("Expected a number of 4 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2], columns[3]);
		}

		public static Matrix2x2f operator*(Matrix2x4f left, Matrix4x2f right)
		{
			return new Matrix2x2f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 			);
		}
		public static Matrix2x3f operator*(Matrix2x4f left, Matrix4x3f right)
		{
			return new Matrix2x3f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 			);
		}
		public static Matrix2x4f operator*(Matrix2x4f left, Matrix4x4f right)
		{
			return new Matrix2x4f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34  +  left.M14 * right.M44 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34  +  left.M24 * right.M44 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix3x2f : System.IEquatable<Matrix3x2f>
	{
		public static readonly Matrix3x2f Zero = new Matrix3x2f();

		
		public float M11  , M12  , M21  , M22  , M31  , M32  ;

		public Matrix3x2f( float m11  ,float m12  ,float m21  ,float m22  ,float m31  ,float m32 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M21 = m21;
						this.M22 = m22;
						this.M31 = m31;
						this.M32 = m32;
					}

		public Matrix2x3f Transpose()
		{
			return new Matrix2x3f(
				M11 , M21 , M31 , M12 , M22 , M32 			);
		}
		
		public static Matrix3x2f operator*(Matrix3x2f left, float right)
		{
			return new Matrix3x2f(
		 left.M11 * right , left.M12 * right , left.M21 * right , left.M22 * right , left.M31 * right , left.M32 * right 			);
		}
		
		public static Matrix3x2f operator*(float left, Matrix3x2f right)
		{
			return new Matrix3x2f(
		 left * right.M11 , left * right.M12 , left * right.M21 , left * right.M22 , left * right.M31 , left * right.M32 			);
		}
		
		public static Matrix3x2f operator/(Matrix3x2f left, float right)
		{
			return new Matrix3x2f(
		 left.M11 / right , left.M12 / right , left.M21 / right , left.M22 / right , left.M31 / right , left.M32 / right 			);
		}
		
		public static Matrix3x2f operator+(Matrix3x2f left, Matrix3x2f right)
		{
			return new Matrix3x2f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 , left.M31 - right.M31 , left.M32 - right.M32 			);
		}
		
		public static Matrix3x2f operator-(Matrix3x2f left, Matrix3x2f right)
		{
			return new Matrix3x2f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 , left.M31 - right.M31 , left.M32 - right.M32 			);
		}
		
		public static Matrix3x2f operator+(Matrix3x2f value)
		{
			return value;
		}
		
		public static Matrix3x2f operator-(Matrix3x2f value)
		{
			return new Matrix3x2f(
		-value.M11 ,-value.M12 ,-value.M21 ,-value.M22 ,-value.M31 ,-value.M32 			);
		}
		
		public static bool operator==(Matrix3x2f left, Matrix3x2f right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M21 == right.M21 && left.M22 == right.M22 && left.M31 == right.M31 && left.M32 == right.M32 ;
		}
		
		public static bool operator!=(Matrix3x2f left, Matrix3x2f right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M21 != right.M21 || left.M22 != right.M22 || left.M31 != right.M31 || left.M32 != right.M32 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix3x2f m && Equals(m);
		}

		public bool Equals(Matrix3x2f other)
		{
			return this == other;
		}

		public unsafe float this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix3x2f m = this;

				return ((float*)&m)[row * 3 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix3x2f m = this;

				((float*)&m)[row * 3 + column] = value;
			}
		}

		public unsafe Vector2f GetRow(int row)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x2f m = this;

			return ((Vector2f*)&m)[row];
		}

		public unsafe Vector3f GetColumn(int column)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x2f m = this;
			float* p = (float*)&m;

			return new Vector3f(p[0 + column], p[2 + column], p[4 + column]);
		}

		public unsafe void SetRow(int row, Vector2f value)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x2f m = this;

			((Vector2f*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector3f value)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x2f m = this;
			float* p = (float*)&m;

						p[0 + column] = value.X;
						p[2 + column] = value.Y;
						p[4 + column] = value.Z;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector2f Transform(Vector3f left, Matrix3x2f right)
		{
			return new Vector2f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32
			);
		}

		public Vector3f Transform(Vector2f value)
		{
			return new Vector3f(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y, M31 * value.X + M32 * value.Y
			);
		}

		public static Vector3f Transform(Matrix3x2f left, Vector2f right)
		{
			return new Vector3f(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y, left.M31 * right.X + left.M32 * right.Y
			);
		}

		public static Point2f Transform(Point3f left, Matrix3x2f right)
		{
			return new Point2f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32
			);
		}

		public Point3f Transform(Point2f value)
		{
			return new Point3f(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y, M31 * value.X + M32 * value.Y
			);
		}

		public static Point3f Transform(Matrix3x2f left, Point2f right)
		{
			return new Point3f(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y, left.M31 * right.X + left.M32 * right.Y
			);
		}
		
		public static Matrix3x2f FromRows(Vector2f row1, Vector2f row2, Vector2f row3)
		{
			return new Matrix3x2f(
			row1.X, row1.Y, row2.X, row2.Y, row3.X, row3.Y
			);
		}
		
		public static Matrix3x2f FromRows(Vector2f[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 3)
				throw new ArgumentException("Expected a number of 3 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2]);
		}
		
		public static Matrix3x2f FromColumns(Vector3f column1, Vector3f column2)
		{
			return new Matrix3x2f(
			column1.X, column1.Y, column1.Z, column2.X, column2.Y, column2.Z
			);
		}
		
		public static Matrix3x2f FromColumns(Vector3f[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 2)
				throw new ArgumentException("Expected a number of 2 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1]);
		}

		public static Matrix3x2f operator*(Matrix3x2f left, Matrix2x2f right)
		{
			return new Matrix3x2f(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 			);
		}
		public static Matrix3x3f operator*(Matrix3x2f left, Matrix2x3f right)
		{
			return new Matrix3x3f(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M31 * right.M13  +  left.M32 * right.M23 			);
		}
		public static Matrix3x4f operator*(Matrix3x2f left, Matrix2x4f right)
		{
			return new Matrix3x4f(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M11 * right.M14  +  left.M12 * right.M24 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M21 * right.M14  +  left.M22 * right.M24 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M31 * right.M13  +  left.M32 * right.M23 ,  left.M31 * right.M14  +  left.M32 * right.M24 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix3x3f : System.IEquatable<Matrix3x3f>
	{
		public static readonly Matrix3x3f Zero = new Matrix3x3f();

				public static readonly Matrix3x3f Identity = new Matrix3x3f(1, 0, 0, 0, 1, 0, 0, 0, 1);

		
		public float M11  , M12  , M13  , M21  , M22  , M23  , M31  , M32  , M33  ;

		public Matrix3x3f( float m11  ,float m12  ,float m13  ,float m21  ,float m22  ,float m23  ,float m31  ,float m32  ,float m33 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M31 = m31;
						this.M32 = m32;
						this.M33 = m33;
					}

		public Matrix3x3f Transpose()
		{
			return new Matrix3x3f(
				M11 , M21 , M31 , M12 , M22 , M32 , M13 , M23 , M33 			);
		}
		
		public static Matrix3x3f operator*(Matrix3x3f left, float right)
		{
			return new Matrix3x3f(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M31 * right , left.M32 * right , left.M33 * right 			);
		}
		
		public static Matrix3x3f operator*(float left, Matrix3x3f right)
		{
			return new Matrix3x3f(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M31 , left * right.M32 , left * right.M33 			);
		}
		
		public static Matrix3x3f operator/(Matrix3x3f left, float right)
		{
			return new Matrix3x3f(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M31 / right , left.M32 / right , left.M33 / right 			);
		}
		
		public static Matrix3x3f operator+(Matrix3x3f left, Matrix3x3f right)
		{
			return new Matrix3x3f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 			);
		}
		
		public static Matrix3x3f operator-(Matrix3x3f left, Matrix3x3f right)
		{
			return new Matrix3x3f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 			);
		}
		
		public static Matrix3x3f operator+(Matrix3x3f value)
		{
			return value;
		}
		
		public static Matrix3x3f operator-(Matrix3x3f value)
		{
			return new Matrix3x3f(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M31 ,-value.M32 ,-value.M33 			);
		}
		
		public static bool operator==(Matrix3x3f left, Matrix3x3f right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 ;
		}
		
		public static bool operator!=(Matrix3x3f left, Matrix3x3f right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix3x3f m && Equals(m);
		}

		public bool Equals(Matrix3x3f other)
		{
			return this == other;
		}

		public unsafe float this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix3x3f m = this;

				return ((float*)&m)[row * 3 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix3x3f m = this;

				((float*)&m)[row * 3 + column] = value;
			}
		}

		public unsafe Vector3f GetRow(int row)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x3f m = this;

			return ((Vector3f*)&m)[row];
		}

		public unsafe Vector3f GetColumn(int column)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x3f m = this;
			float* p = (float*)&m;

			return new Vector3f(p[0 + column], p[3 + column], p[6 + column]);
		}

		public unsafe void SetRow(int row, Vector3f value)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x3f m = this;

			((Vector3f*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector3f value)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x3f m = this;
			float* p = (float*)&m;

						p[0 + column] = value.X;
						p[3 + column] = value.Y;
						p[6 + column] = value.Z;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

				public bool IsIdentity
			{
				get
				{
				return  this.M11 == 1 
				 &&  this.M12 == 0 
				 &&  this.M13 == 0 
				 &&  this.M21 == 0 
				 &&  this.M22 == 1 
				 &&  this.M23 == 0 
				 &&  this.M31 == 0 
				 &&  this.M32 == 0 
				 &&  this.M33 == 1 
				;
				}
			}
		
		public static Vector3f Transform(Vector3f left, Matrix3x3f right)
		{
			return new Vector3f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33
			);
		}

		public Vector3f Transform(Vector3f value)
		{
			return new Vector3f(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z, M31 * value.X + M32 * value.Y + M33 * value.Z
			);
		}

		public static Vector3f Transform(Matrix3x3f left, Vector3f right)
		{
			return new Vector3f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z
			);
		}

		public static Point3f Transform(Point3f left, Matrix3x3f right)
		{
			return new Point3f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33
			);
		}

		public Point3f Transform(Point3f value)
		{
			return new Point3f(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z, M31 * value.X + M32 * value.Y + M33 * value.Z
			);
		}

		public static Point3f Transform(Matrix3x3f left, Point3f right)
		{
			return new Point3f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z
			);
		}
		
		public static Matrix3x3f FromRows(Vector3f row1, Vector3f row2, Vector3f row3)
		{
			return new Matrix3x3f(
			row1.X, row1.Y, row1.Z, row2.X, row2.Y, row2.Z, row3.X, row3.Y, row3.Z
			);
		}
		
		public static Matrix3x3f FromRows(Vector3f[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 3)
				throw new ArgumentException("Expected a number of 3 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2]);
		}
		
		public static Matrix3x3f FromColumns(Vector3f column1, Vector3f column2, Vector3f column3)
		{
			return new Matrix3x3f(
			column1.X, column1.Y, column1.Z, column2.X, column2.Y, column2.Z, column3.X, column3.Y, column3.Z
			);
		}
		
		public static Matrix3x3f FromColumns(Vector3f[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 3)
				throw new ArgumentException("Expected a number of 3 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2]);
		}

		public static Matrix3x2f operator*(Matrix3x3f left, Matrix3x2f right)
		{
			return new Matrix3x2f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 			);
		}
		public static Matrix3x3f operator*(Matrix3x3f left, Matrix3x3f right)
		{
			return new Matrix3x3f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33 			);
		}
		public static Matrix3x4f operator*(Matrix3x3f left, Matrix3x4f right)
		{
			return new Matrix3x4f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33 ,  left.M31 * right.M14  +  left.M32 * right.M24  +  left.M33 * right.M34 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix3x4f : System.IEquatable<Matrix3x4f>
	{
		public static readonly Matrix3x4f Zero = new Matrix3x4f();

		
		public float M11  , M12  , M13  , M14  , M21  , M22  , M23  , M24  , M31  , M32  , M33  , M34  ;

		public Matrix3x4f( float m11  ,float m12  ,float m13  ,float m14  ,float m21  ,float m22  ,float m23  ,float m24  ,float m31  ,float m32  ,float m33  ,float m34 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M14 = m14;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M24 = m24;
						this.M31 = m31;
						this.M32 = m32;
						this.M33 = m33;
						this.M34 = m34;
					}

		public Matrix4x3f Transpose()
		{
			return new Matrix4x3f(
				M11 , M21 , M31 , M12 , M22 , M32 , M13 , M23 , M33 , M14 , M24 , M34 			);
		}
		
		public static Matrix3x4f operator*(Matrix3x4f left, float right)
		{
			return new Matrix3x4f(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M14 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M24 * right , left.M31 * right , left.M32 * right , left.M33 * right , left.M34 * right 			);
		}
		
		public static Matrix3x4f operator*(float left, Matrix3x4f right)
		{
			return new Matrix3x4f(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M14 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M24 , left * right.M31 , left * right.M32 , left * right.M33 , left * right.M34 			);
		}
		
		public static Matrix3x4f operator/(Matrix3x4f left, float right)
		{
			return new Matrix3x4f(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M14 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M24 / right , left.M31 / right , left.M32 / right , left.M33 / right , left.M34 / right 			);
		}
		
		public static Matrix3x4f operator+(Matrix3x4f left, Matrix3x4f right)
		{
			return new Matrix3x4f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M34 - right.M34 			);
		}
		
		public static Matrix3x4f operator-(Matrix3x4f left, Matrix3x4f right)
		{
			return new Matrix3x4f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M34 - right.M34 			);
		}
		
		public static Matrix3x4f operator+(Matrix3x4f value)
		{
			return value;
		}
		
		public static Matrix3x4f operator-(Matrix3x4f value)
		{
			return new Matrix3x4f(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M14 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M24 ,-value.M31 ,-value.M32 ,-value.M33 ,-value.M34 			);
		}
		
		public static bool operator==(Matrix3x4f left, Matrix3x4f right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M34 == right.M34 ;
		}
		
		public static bool operator!=(Matrix3x4f left, Matrix3x4f right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M24 != right.M24 || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 || left.M34 != right.M34 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M14.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M24.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode() ^ M34.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix3x4f m && Equals(m);
		}

		public bool Equals(Matrix3x4f other)
		{
			return this == other;
		}

		public unsafe float this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix3x4f m = this;

				return ((float*)&m)[row * 3 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix3x4f m = this;

				((float*)&m)[row * 3 + column] = value;
			}
		}

		public unsafe Vector4f GetRow(int row)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x4f m = this;

			return ((Vector4f*)&m)[row];
		}

		public unsafe Vector3f GetColumn(int column)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x4f m = this;
			float* p = (float*)&m;

			return new Vector3f(p[0 + column], p[4 + column], p[8 + column]);
		}

		public unsafe void SetRow(int row, Vector4f value)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x4f m = this;

			((Vector4f*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector3f value)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x4f m = this;
			float* p = (float*)&m;

						p[0 + column] = value.X;
						p[4 + column] = value.Y;
						p[8 + column] = value.Z;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector4f Transform(Vector3f left, Matrix3x4f right)
		{
			return new Vector4f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33, left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34
			);
		}

		public Vector3f Transform(Vector4f value)
		{
			return new Vector3f(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W, M31 * value.X + M32 * value.Y + M33 * value.Z + M34 * value.W
			);
		}

		public static Vector3f Transform(Matrix3x4f left, Vector4f right)
		{
			return new Vector3f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34 * right.W
			);
		}

		public static Point4f Transform(Point3f left, Matrix3x4f right)
		{
			return new Point4f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33, left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34
			);
		}

		public Point3f Transform(Point4f value)
		{
			return new Point3f(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W, M31 * value.X + M32 * value.Y + M33 * value.Z + M34 * value.W
			);
		}

		public static Point3f Transform(Matrix3x4f left, Point4f right)
		{
			return new Point3f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34 * right.W
			);
		}
		
		public static Matrix3x4f FromRows(Vector4f row1, Vector4f row2, Vector4f row3)
		{
			return new Matrix3x4f(
			row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W, row3.X, row3.Y, row3.Z, row3.W
			);
		}
		
		public static Matrix3x4f FromRows(Vector4f[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 3)
				throw new ArgumentException("Expected a number of 3 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2]);
		}
		
		public static Matrix3x4f FromColumns(Vector3f column1, Vector3f column2, Vector3f column3, Vector3f column4)
		{
			return new Matrix3x4f(
			column1.X, column1.Y, column1.Z, column2.X, column2.Y, column2.Z, column3.X, column3.Y, column3.Z, column4.X, column4.Y, column4.Z
			);
		}
		
		public static Matrix3x4f FromColumns(Vector3f[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 4)
				throw new ArgumentException("Expected a number of 4 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2], columns[3]);
		}

		public static Matrix3x2f operator*(Matrix3x4f left, Matrix4x2f right)
		{
			return new Matrix3x2f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 			);
		}
		public static Matrix3x3f operator*(Matrix3x4f left, Matrix4x3f right)
		{
			return new Matrix3x3f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33  +  left.M34 * right.M43 			);
		}
		public static Matrix3x4f operator*(Matrix3x4f left, Matrix4x4f right)
		{
			return new Matrix3x4f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34  +  left.M14 * right.M44 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34  +  left.M24 * right.M44 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33  +  left.M34 * right.M43 ,  left.M31 * right.M14  +  left.M32 * right.M24  +  left.M33 * right.M34  +  left.M34 * right.M44 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix4x2f : System.IEquatable<Matrix4x2f>
	{
		public static readonly Matrix4x2f Zero = new Matrix4x2f();

		
		public float M11  , M12  , M21  , M22  , M31  , M32  , M41  , M42  ;

		public Matrix4x2f( float m11  ,float m12  ,float m21  ,float m22  ,float m31  ,float m32  ,float m41  ,float m42 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M21 = m21;
						this.M22 = m22;
						this.M31 = m31;
						this.M32 = m32;
						this.M41 = m41;
						this.M42 = m42;
					}

		public Matrix2x4f Transpose()
		{
			return new Matrix2x4f(
				M11 , M21 , M31 , M41 , M12 , M22 , M32 , M42 			);
		}
		
		public static Matrix4x2f operator*(Matrix4x2f left, float right)
		{
			return new Matrix4x2f(
		 left.M11 * right , left.M12 * right , left.M21 * right , left.M22 * right , left.M31 * right , left.M32 * right , left.M41 * right , left.M42 * right 			);
		}
		
		public static Matrix4x2f operator*(float left, Matrix4x2f right)
		{
			return new Matrix4x2f(
		 left * right.M11 , left * right.M12 , left * right.M21 , left * right.M22 , left * right.M31 , left * right.M32 , left * right.M41 , left * right.M42 			);
		}
		
		public static Matrix4x2f operator/(Matrix4x2f left, float right)
		{
			return new Matrix4x2f(
		 left.M11 / right , left.M12 / right , left.M21 / right , left.M22 / right , left.M31 / right , left.M32 / right , left.M41 / right , left.M42 / right 			);
		}
		
		public static Matrix4x2f operator+(Matrix4x2f left, Matrix4x2f right)
		{
			return new Matrix4x2f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 , left.M31 - right.M31 , left.M32 - right.M32 , left.M41 - right.M41 , left.M42 - right.M42 			);
		}
		
		public static Matrix4x2f operator-(Matrix4x2f left, Matrix4x2f right)
		{
			return new Matrix4x2f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 , left.M31 - right.M31 , left.M32 - right.M32 , left.M41 - right.M41 , left.M42 - right.M42 			);
		}
		
		public static Matrix4x2f operator+(Matrix4x2f value)
		{
			return value;
		}
		
		public static Matrix4x2f operator-(Matrix4x2f value)
		{
			return new Matrix4x2f(
		-value.M11 ,-value.M12 ,-value.M21 ,-value.M22 ,-value.M31 ,-value.M32 ,-value.M41 ,-value.M42 			);
		}
		
		public static bool operator==(Matrix4x2f left, Matrix4x2f right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M21 == right.M21 && left.M22 == right.M22 && left.M31 == right.M31 && left.M32 == right.M32 && left.M41 == right.M41 && left.M42 == right.M42 ;
		}
		
		public static bool operator!=(Matrix4x2f left, Matrix4x2f right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M21 != right.M21 || left.M22 != right.M22 || left.M31 != right.M31 || left.M32 != right.M32 || left.M41 != right.M41 || left.M42 != right.M42 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M41.GetHashCode() ^ M42.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix4x2f m && Equals(m);
		}

		public bool Equals(Matrix4x2f other)
		{
			return this == other;
		}

		public unsafe float this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix4x2f m = this;

				return ((float*)&m)[row * 4 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix4x2f m = this;

				((float*)&m)[row * 4 + column] = value;
			}
		}

		public unsafe Vector2f GetRow(int row)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x2f m = this;

			return ((Vector2f*)&m)[row];
		}

		public unsafe Vector4f GetColumn(int column)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x2f m = this;
			float* p = (float*)&m;

			return new Vector4f(p[0 + column], p[2 + column], p[4 + column], p[6 + column]);
		}

		public unsafe void SetRow(int row, Vector2f value)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x2f m = this;

			((Vector2f*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector4f value)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x2f m = this;
			float* p = (float*)&m;

						p[0 + column] = value.X;
						p[2 + column] = value.Y;
						p[4 + column] = value.Z;
						p[6 + column] = value.W;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector2f Transform(Vector4f left, Matrix4x2f right)
		{
			return new Vector2f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42
			);
		}

		public Vector4f Transform(Vector2f value)
		{
			return new Vector4f(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y, M31 * value.X + M32 * value.Y, M41 * value.X + M42 * value.Y
			);
		}

		public static Vector4f Transform(Matrix4x2f left, Vector2f right)
		{
			return new Vector4f(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y, left.M31 * right.X + left.M32 * right.Y, left.M41 * right.X + left.M42 * right.Y
			);
		}

		public static Point2f Transform(Point4f left, Matrix4x2f right)
		{
			return new Point2f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42
			);
		}

		public Point4f Transform(Point2f value)
		{
			return new Point4f(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y, M31 * value.X + M32 * value.Y, M41 * value.X + M42 * value.Y
			);
		}

		public static Point4f Transform(Matrix4x2f left, Point2f right)
		{
			return new Point4f(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y, left.M31 * right.X + left.M32 * right.Y, left.M41 * right.X + left.M42 * right.Y
			);
		}
		
		public static Matrix4x2f FromRows(Vector2f row1, Vector2f row2, Vector2f row3, Vector2f row4)
		{
			return new Matrix4x2f(
			row1.X, row1.Y, row2.X, row2.Y, row3.X, row3.Y, row4.X, row4.Y
			);
		}
		
		public static Matrix4x2f FromRows(Vector2f[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 4)
				throw new ArgumentException("Expected a number of 4 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2], rows[3]);
		}
		
		public static Matrix4x2f FromColumns(Vector4f column1, Vector4f column2)
		{
			return new Matrix4x2f(
			column1.X, column1.Y, column1.Z, column1.W, column2.X, column2.Y, column2.Z, column2.W
			);
		}
		
		public static Matrix4x2f FromColumns(Vector4f[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 2)
				throw new ArgumentException("Expected a number of 2 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1]);
		}

		public static Matrix4x2f operator*(Matrix4x2f left, Matrix2x2f right)
		{
			return new Matrix4x2f(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M41 * right.M11  +  left.M42 * right.M21 ,  left.M41 * right.M12  +  left.M42 * right.M22 			);
		}
		public static Matrix4x3f operator*(Matrix4x2f left, Matrix2x3f right)
		{
			return new Matrix4x3f(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M31 * right.M13  +  left.M32 * right.M23 ,  left.M41 * right.M11  +  left.M42 * right.M21 ,  left.M41 * right.M12  +  left.M42 * right.M22 ,  left.M41 * right.M13  +  left.M42 * right.M23 			);
		}
		public static Matrix4x4f operator*(Matrix4x2f left, Matrix2x4f right)
		{
			return new Matrix4x4f(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M11 * right.M14  +  left.M12 * right.M24 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M21 * right.M14  +  left.M22 * right.M24 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M31 * right.M13  +  left.M32 * right.M23 ,  left.M31 * right.M14  +  left.M32 * right.M24 ,  left.M41 * right.M11  +  left.M42 * right.M21 ,  left.M41 * right.M12  +  left.M42 * right.M22 ,  left.M41 * right.M13  +  left.M42 * right.M23 ,  left.M41 * right.M14  +  left.M42 * right.M24 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix4x3f : System.IEquatable<Matrix4x3f>
	{
		public static readonly Matrix4x3f Zero = new Matrix4x3f();

		
		public float M11  , M12  , M13  , M21  , M22  , M23  , M31  , M32  , M33  , M41  , M42  , M43  ;

		public Matrix4x3f( float m11  ,float m12  ,float m13  ,float m21  ,float m22  ,float m23  ,float m31  ,float m32  ,float m33  ,float m41  ,float m42  ,float m43 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M31 = m31;
						this.M32 = m32;
						this.M33 = m33;
						this.M41 = m41;
						this.M42 = m42;
						this.M43 = m43;
					}

		public Matrix3x4f Transpose()
		{
			return new Matrix3x4f(
				M11 , M21 , M31 , M41 , M12 , M22 , M32 , M42 , M13 , M23 , M33 , M43 			);
		}
		
		public static Matrix4x3f operator*(Matrix4x3f left, float right)
		{
			return new Matrix4x3f(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M31 * right , left.M32 * right , left.M33 * right , left.M41 * right , left.M42 * right , left.M43 * right 			);
		}
		
		public static Matrix4x3f operator*(float left, Matrix4x3f right)
		{
			return new Matrix4x3f(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M31 , left * right.M32 , left * right.M33 , left * right.M41 , left * right.M42 , left * right.M43 			);
		}
		
		public static Matrix4x3f operator/(Matrix4x3f left, float right)
		{
			return new Matrix4x3f(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M31 / right , left.M32 / right , left.M33 / right , left.M41 / right , left.M42 / right , left.M43 / right 			);
		}
		
		public static Matrix4x3f operator+(Matrix4x3f left, Matrix4x3f right)
		{
			return new Matrix4x3f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M41 - right.M41 , left.M42 - right.M42 , left.M43 - right.M43 			);
		}
		
		public static Matrix4x3f operator-(Matrix4x3f left, Matrix4x3f right)
		{
			return new Matrix4x3f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M41 - right.M41 , left.M42 - right.M42 , left.M43 - right.M43 			);
		}
		
		public static Matrix4x3f operator+(Matrix4x3f value)
		{
			return value;
		}
		
		public static Matrix4x3f operator-(Matrix4x3f value)
		{
			return new Matrix4x3f(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M31 ,-value.M32 ,-value.M33 ,-value.M41 ,-value.M42 ,-value.M43 			);
		}
		
		public static bool operator==(Matrix4x3f left, Matrix4x3f right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43 ;
		}
		
		public static bool operator!=(Matrix4x3f left, Matrix4x3f right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 || left.M41 != right.M41 || left.M42 != right.M42 || left.M43 != right.M43 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode() ^ M41.GetHashCode() ^ M42.GetHashCode() ^ M43.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix4x3f m && Equals(m);
		}

		public bool Equals(Matrix4x3f other)
		{
			return this == other;
		}

		public unsafe float this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix4x3f m = this;

				return ((float*)&m)[row * 4 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix4x3f m = this;

				((float*)&m)[row * 4 + column] = value;
			}
		}

		public unsafe Vector3f GetRow(int row)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x3f m = this;

			return ((Vector3f*)&m)[row];
		}

		public unsafe Vector4f GetColumn(int column)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x3f m = this;
			float* p = (float*)&m;

			return new Vector4f(p[0 + column], p[3 + column], p[6 + column], p[9 + column]);
		}

		public unsafe void SetRow(int row, Vector3f value)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x3f m = this;

			((Vector3f*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector4f value)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x3f m = this;
			float* p = (float*)&m;

						p[0 + column] = value.X;
						p[3 + column] = value.Y;
						p[6 + column] = value.Z;
						p[9 + column] = value.W;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector3f Transform(Vector4f left, Matrix4x3f right)
		{
			return new Vector3f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43
			);
		}

		public Vector4f Transform(Vector3f value)
		{
			return new Vector4f(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z, M31 * value.X + M32 * value.Y + M33 * value.Z, M41 * value.X + M42 * value.Y + M43 * value.Z
			);
		}

		public static Vector4f Transform(Matrix4x3f left, Vector3f right)
		{
			return new Vector4f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z, left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z
			);
		}

		public static Point3f Transform(Point4f left, Matrix4x3f right)
		{
			return new Point3f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43
			);
		}

		public Point4f Transform(Point3f value)
		{
			return new Point4f(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z, M31 * value.X + M32 * value.Y + M33 * value.Z, M41 * value.X + M42 * value.Y + M43 * value.Z
			);
		}

		public static Point4f Transform(Matrix4x3f left, Point3f right)
		{
			return new Point4f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z, left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z
			);
		}
		
		public static Matrix4x3f FromRows(Vector3f row1, Vector3f row2, Vector3f row3, Vector3f row4)
		{
			return new Matrix4x3f(
			row1.X, row1.Y, row1.Z, row2.X, row2.Y, row2.Z, row3.X, row3.Y, row3.Z, row4.X, row4.Y, row4.Z
			);
		}
		
		public static Matrix4x3f FromRows(Vector3f[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 4)
				throw new ArgumentException("Expected a number of 4 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2], rows[3]);
		}
		
		public static Matrix4x3f FromColumns(Vector4f column1, Vector4f column2, Vector4f column3)
		{
			return new Matrix4x3f(
			column1.X, column1.Y, column1.Z, column1.W, column2.X, column2.Y, column2.Z, column2.W, column3.X, column3.Y, column3.Z, column3.W
			);
		}
		
		public static Matrix4x3f FromColumns(Vector4f[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 3)
				throw new ArgumentException("Expected a number of 3 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2]);
		}

		public static Matrix4x2f operator*(Matrix4x3f left, Matrix3x2f right)
		{
			return new Matrix4x2f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32 			);
		}
		public static Matrix4x3f operator*(Matrix4x3f left, Matrix3x3f right)
		{
			return new Matrix4x3f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32 ,  left.M41 * right.M13  +  left.M42 * right.M23  +  left.M43 * right.M33 			);
		}
		public static Matrix4x4f operator*(Matrix4x3f left, Matrix3x4f right)
		{
			return new Matrix4x4f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33 ,  left.M31 * right.M14  +  left.M32 * right.M24  +  left.M33 * right.M34 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32 ,  left.M41 * right.M13  +  left.M42 * right.M23  +  left.M43 * right.M33 ,  left.M41 * right.M14  +  left.M42 * right.M24  +  left.M43 * right.M34 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix4x4f : System.IEquatable<Matrix4x4f>
	{
		public static readonly Matrix4x4f Zero = new Matrix4x4f();

				public static readonly Matrix4x4f Identity = new Matrix4x4f(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

		
		public float M11  , M12  , M13  , M14  , M21  , M22  , M23  , M24  , M31  , M32  , M33  , M34  , M41  , M42  , M43  , M44  ;

		public Matrix4x4f( float m11  ,float m12  ,float m13  ,float m14  ,float m21  ,float m22  ,float m23  ,float m24  ,float m31  ,float m32  ,float m33  ,float m34  ,float m41  ,float m42  ,float m43  ,float m44 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M14 = m14;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M24 = m24;
						this.M31 = m31;
						this.M32 = m32;
						this.M33 = m33;
						this.M34 = m34;
						this.M41 = m41;
						this.M42 = m42;
						this.M43 = m43;
						this.M44 = m44;
					}

		public Matrix4x4f Transpose()
		{
			return new Matrix4x4f(
				M11 , M21 , M31 , M41 , M12 , M22 , M32 , M42 , M13 , M23 , M33 , M43 , M14 , M24 , M34 , M44 			);
		}
		
		public static Matrix4x4f operator*(Matrix4x4f left, float right)
		{
			return new Matrix4x4f(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M14 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M24 * right , left.M31 * right , left.M32 * right , left.M33 * right , left.M34 * right , left.M41 * right , left.M42 * right , left.M43 * right , left.M44 * right 			);
		}
		
		public static Matrix4x4f operator*(float left, Matrix4x4f right)
		{
			return new Matrix4x4f(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M14 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M24 , left * right.M31 , left * right.M32 , left * right.M33 , left * right.M34 , left * right.M41 , left * right.M42 , left * right.M43 , left * right.M44 			);
		}
		
		public static Matrix4x4f operator/(Matrix4x4f left, float right)
		{
			return new Matrix4x4f(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M14 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M24 / right , left.M31 / right , left.M32 / right , left.M33 / right , left.M34 / right , left.M41 / right , left.M42 / right , left.M43 / right , left.M44 / right 			);
		}
		
		public static Matrix4x4f operator+(Matrix4x4f left, Matrix4x4f right)
		{
			return new Matrix4x4f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M34 - right.M34 , left.M41 - right.M41 , left.M42 - right.M42 , left.M43 - right.M43 , left.M44 - right.M44 			);
		}
		
		public static Matrix4x4f operator-(Matrix4x4f left, Matrix4x4f right)
		{
			return new Matrix4x4f(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M34 - right.M34 , left.M41 - right.M41 , left.M42 - right.M42 , left.M43 - right.M43 , left.M44 - right.M44 			);
		}
		
		public static Matrix4x4f operator+(Matrix4x4f value)
		{
			return value;
		}
		
		public static Matrix4x4f operator-(Matrix4x4f value)
		{
			return new Matrix4x4f(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M14 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M24 ,-value.M31 ,-value.M32 ,-value.M33 ,-value.M34 ,-value.M41 ,-value.M42 ,-value.M43 ,-value.M44 			);
		}
		
		public static bool operator==(Matrix4x4f left, Matrix4x4f right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M34 == right.M34 && left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43 && left.M44 == right.M44 ;
		}
		
		public static bool operator!=(Matrix4x4f left, Matrix4x4f right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M24 != right.M24 || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 || left.M34 != right.M34 || left.M41 != right.M41 || left.M42 != right.M42 || left.M43 != right.M43 || left.M44 != right.M44 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M14.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M24.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode() ^ M34.GetHashCode() ^ M41.GetHashCode() ^ M42.GetHashCode() ^ M43.GetHashCode() ^ M44.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix4x4f m && Equals(m);
		}

		public bool Equals(Matrix4x4f other)
		{
			return this == other;
		}

		public unsafe float this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix4x4f m = this;

				return ((float*)&m)[row * 4 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix4x4f m = this;

				((float*)&m)[row * 4 + column] = value;
			}
		}

		public unsafe Vector4f GetRow(int row)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x4f m = this;

			return ((Vector4f*)&m)[row];
		}

		public unsafe Vector4f GetColumn(int column)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x4f m = this;
			float* p = (float*)&m;

			return new Vector4f(p[0 + column], p[4 + column], p[8 + column], p[12 + column]);
		}

		public unsafe void SetRow(int row, Vector4f value)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x4f m = this;

			((Vector4f*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector4f value)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x4f m = this;
			float* p = (float*)&m;

						p[0 + column] = value.X;
						p[4 + column] = value.Y;
						p[8 + column] = value.Z;
						p[12 + column] = value.W;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

				public bool IsIdentity
			{
				get
				{
				return  this.M11 == 1 
				 &&  this.M12 == 0 
				 &&  this.M13 == 0 
				 &&  this.M14 == 0 
				 &&  this.M21 == 0 
				 &&  this.M22 == 1 
				 &&  this.M23 == 0 
				 &&  this.M24 == 0 
				 &&  this.M31 == 0 
				 &&  this.M32 == 0 
				 &&  this.M33 == 1 
				 &&  this.M34 == 0 
				 &&  this.M41 == 0 
				 &&  this.M42 == 0 
				 &&  this.M43 == 0 
				 &&  this.M44 == 1 
				;
				}
			}
		
		public static Vector4f Transform(Vector4f left, Matrix4x4f right)
		{
			return new Vector4f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43, left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34 + left.W * right.M44
			);
		}

		public Vector4f Transform(Vector4f value)
		{
			return new Vector4f(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W, M31 * value.X + M32 * value.Y + M33 * value.Z + M34 * value.W, M41 * value.X + M42 * value.Y + M43 * value.Z + M44 * value.W
			);
		}

		public static Vector4f Transform(Matrix4x4f left, Vector4f right)
		{
			return new Vector4f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34 * right.W, left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z + left.M44 * right.W
			);
		}

		public static Point4f Transform(Point4f left, Matrix4x4f right)
		{
			return new Point4f(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43, left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34 + left.W * right.M44
			);
		}

		public Point4f Transform(Point4f value)
		{
			return new Point4f(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W, M31 * value.X + M32 * value.Y + M33 * value.Z + M34 * value.W, M41 * value.X + M42 * value.Y + M43 * value.Z + M44 * value.W
			);
		}

		public static Point4f Transform(Matrix4x4f left, Point4f right)
		{
			return new Point4f(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34 * right.W, left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z + left.M44 * right.W
			);
		}
		
		public static Matrix4x4f FromRows(Vector4f row1, Vector4f row2, Vector4f row3, Vector4f row4)
		{
			return new Matrix4x4f(
			row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W, row3.X, row3.Y, row3.Z, row3.W, row4.X, row4.Y, row4.Z, row4.W
			);
		}
		
		public static Matrix4x4f FromRows(Vector4f[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 4)
				throw new ArgumentException("Expected a number of 4 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2], rows[3]);
		}
		
		public static Matrix4x4f FromColumns(Vector4f column1, Vector4f column2, Vector4f column3, Vector4f column4)
		{
			return new Matrix4x4f(
			column1.X, column1.Y, column1.Z, column1.W, column2.X, column2.Y, column2.Z, column2.W, column3.X, column3.Y, column3.Z, column3.W, column4.X, column4.Y, column4.Z, column4.W
			);
		}
		
		public static Matrix4x4f FromColumns(Vector4f[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 4)
				throw new ArgumentException("Expected a number of 4 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2], columns[3]);
		}

		public static Matrix4x2f operator*(Matrix4x4f left, Matrix4x2f right)
		{
			return new Matrix4x2f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31  +  left.M44 * right.M41 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32  +  left.M44 * right.M42 			);
		}
		public static Matrix4x3f operator*(Matrix4x4f left, Matrix4x3f right)
		{
			return new Matrix4x3f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33  +  left.M34 * right.M43 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31  +  left.M44 * right.M41 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32  +  left.M44 * right.M42 ,  left.M41 * right.M13  +  left.M42 * right.M23  +  left.M43 * right.M33  +  left.M44 * right.M43 			);
		}
		public static Matrix4x4f operator*(Matrix4x4f left, Matrix4x4f right)
		{
			return new Matrix4x4f(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34  +  left.M14 * right.M44 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34  +  left.M24 * right.M44 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33  +  left.M34 * right.M43 ,  left.M31 * right.M14  +  left.M32 * right.M24  +  left.M33 * right.M34  +  left.M34 * right.M44 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31  +  left.M44 * right.M41 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32  +  left.M44 * right.M42 ,  left.M41 * right.M13  +  left.M42 * right.M23  +  left.M43 * right.M33  +  left.M44 * right.M43 ,  left.M41 * right.M14  +  left.M42 * right.M24  +  left.M43 * right.M34  +  left.M44 * right.M44 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix2x2r : System.IEquatable<Matrix2x2r>
	{
		public static readonly Matrix2x2r Zero = new Matrix2x2r();

				public static readonly Matrix2x2r Identity = new Matrix2x2r(1, 0, 0, 1);

		
		public double M11  , M12  , M21  , M22  ;

		public Matrix2x2r( double m11  ,double m12  ,double m21  ,double m22 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M21 = m21;
						this.M22 = m22;
					}

		public Matrix2x2r Transpose()
		{
			return new Matrix2x2r(
				M11 , M21 , M12 , M22 			);
		}
		
		public static Matrix2x2r operator*(Matrix2x2r left, double right)
		{
			return new Matrix2x2r(
		 left.M11 * right , left.M12 * right , left.M21 * right , left.M22 * right 			);
		}
		
		public static Matrix2x2r operator*(double left, Matrix2x2r right)
		{
			return new Matrix2x2r(
		 left * right.M11 , left * right.M12 , left * right.M21 , left * right.M22 			);
		}
		
		public static Matrix2x2r operator/(Matrix2x2r left, double right)
		{
			return new Matrix2x2r(
		 left.M11 / right , left.M12 / right , left.M21 / right , left.M22 / right 			);
		}
		
		public static Matrix2x2r operator+(Matrix2x2r left, Matrix2x2r right)
		{
			return new Matrix2x2r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 			);
		}
		
		public static Matrix2x2r operator-(Matrix2x2r left, Matrix2x2r right)
		{
			return new Matrix2x2r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 			);
		}
		
		public static Matrix2x2r operator+(Matrix2x2r value)
		{
			return value;
		}
		
		public static Matrix2x2r operator-(Matrix2x2r value)
		{
			return new Matrix2x2r(
		-value.M11 ,-value.M12 ,-value.M21 ,-value.M22 			);
		}
		
		public static bool operator==(Matrix2x2r left, Matrix2x2r right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M21 == right.M21 && left.M22 == right.M22 ;
		}
		
		public static bool operator!=(Matrix2x2r left, Matrix2x2r right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M21 != right.M21 || left.M22 != right.M22 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix2x2r m && Equals(m);
		}

		public bool Equals(Matrix2x2r other)
		{
			return this == other;
		}

		public unsafe double this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix2x2r m = this;

				return ((double*)&m)[row * 2 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix2x2r m = this;

				((double*)&m)[row * 2 + column] = value;
			}
		}

		public unsafe Vector2r GetRow(int row)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x2r m = this;

			return ((Vector2r*)&m)[row];
		}

		public unsafe Vector2r GetColumn(int column)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x2r m = this;
			double* p = (double*)&m;

			return new Vector2r(p[0 + column], p[2 + column]);
		}

		public unsafe void SetRow(int row, Vector2r value)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x2r m = this;

			((Vector2r*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector2r value)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x2r m = this;
			double* p = (double*)&m;

						p[0 + column] = value.X;
						p[2 + column] = value.Y;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

				public bool IsIdentity
			{
				get
				{
				return  this.M11 == 1 
				 &&  this.M12 == 0 
				 &&  this.M21 == 0 
				 &&  this.M22 == 1 
				;
				}
			}
		
		public static Vector2r Transform(Vector2r left, Matrix2x2r right)
		{
			return new Vector2r(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22
			);
		}

		public Vector2r Transform(Vector2r value)
		{
			return new Vector2r(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y
			);
		}

		public static Vector2r Transform(Matrix2x2r left, Vector2r right)
		{
			return new Vector2r(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y
			);
		}

		public static Point2r Transform(Point2r left, Matrix2x2r right)
		{
			return new Point2r(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22
			);
		}

		public Point2r Transform(Point2r value)
		{
			return new Point2r(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y
			);
		}

		public static Point2r Transform(Matrix2x2r left, Point2r right)
		{
			return new Point2r(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y
			);
		}
		
		public static Matrix2x2r FromRows(Vector2r row1, Vector2r row2)
		{
			return new Matrix2x2r(
			row1.X, row1.Y, row2.X, row2.Y
			);
		}
		
		public static Matrix2x2r FromRows(Vector2r[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 2)
				throw new ArgumentException("Expected a number of 2 rows.", nameof(rows));

			return FromRows(rows[0], rows[1]);
		}
		
		public static Matrix2x2r FromColumns(Vector2r column1, Vector2r column2)
		{
			return new Matrix2x2r(
			column1.X, column1.Y, column2.X, column2.Y
			);
		}
		
		public static Matrix2x2r FromColumns(Vector2r[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 2)
				throw new ArgumentException("Expected a number of 2 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1]);
		}

		public static Matrix2x2r operator*(Matrix2x2r left, Matrix2x2r right)
		{
			return new Matrix2x2r(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 			);
		}
		public static Matrix2x3r operator*(Matrix2x2r left, Matrix2x3r right)
		{
			return new Matrix2x3r(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 			);
		}
		public static Matrix2x4r operator*(Matrix2x2r left, Matrix2x4r right)
		{
			return new Matrix2x4r(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M11 * right.M14  +  left.M12 * right.M24 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M21 * right.M14  +  left.M22 * right.M24 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix2x3r : System.IEquatable<Matrix2x3r>
	{
		public static readonly Matrix2x3r Zero = new Matrix2x3r();

		
		public double M11  , M12  , M13  , M21  , M22  , M23  ;

		public Matrix2x3r( double m11  ,double m12  ,double m13  ,double m21  ,double m22  ,double m23 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
					}

		public Matrix3x2r Transpose()
		{
			return new Matrix3x2r(
				M11 , M21 , M12 , M22 , M13 , M23 			);
		}
		
		public static Matrix2x3r operator*(Matrix2x3r left, double right)
		{
			return new Matrix2x3r(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M21 * right , left.M22 * right , left.M23 * right 			);
		}
		
		public static Matrix2x3r operator*(double left, Matrix2x3r right)
		{
			return new Matrix2x3r(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M21 , left * right.M22 , left * right.M23 			);
		}
		
		public static Matrix2x3r operator/(Matrix2x3r left, double right)
		{
			return new Matrix2x3r(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M21 / right , left.M22 / right , left.M23 / right 			);
		}
		
		public static Matrix2x3r operator+(Matrix2x3r left, Matrix2x3r right)
		{
			return new Matrix2x3r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 			);
		}
		
		public static Matrix2x3r operator-(Matrix2x3r left, Matrix2x3r right)
		{
			return new Matrix2x3r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 			);
		}
		
		public static Matrix2x3r operator+(Matrix2x3r value)
		{
			return value;
		}
		
		public static Matrix2x3r operator-(Matrix2x3r value)
		{
			return new Matrix2x3r(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M21 ,-value.M22 ,-value.M23 			);
		}
		
		public static bool operator==(Matrix2x3r left, Matrix2x3r right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 ;
		}
		
		public static bool operator!=(Matrix2x3r left, Matrix2x3r right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix2x3r m && Equals(m);
		}

		public bool Equals(Matrix2x3r other)
		{
			return this == other;
		}

		public unsafe double this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix2x3r m = this;

				return ((double*)&m)[row * 2 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix2x3r m = this;

				((double*)&m)[row * 2 + column] = value;
			}
		}

		public unsafe Vector3r GetRow(int row)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x3r m = this;

			return ((Vector3r*)&m)[row];
		}

		public unsafe Vector2r GetColumn(int column)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x3r m = this;
			double* p = (double*)&m;

			return new Vector2r(p[0 + column], p[3 + column]);
		}

		public unsafe void SetRow(int row, Vector3r value)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x3r m = this;

			((Vector3r*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector2r value)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x3r m = this;
			double* p = (double*)&m;

						p[0 + column] = value.X;
						p[3 + column] = value.Y;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector3r Transform(Vector2r left, Matrix2x3r right)
		{
			return new Vector3r(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22, left.X * right.M13 + left.Y * right.M23
			);
		}

		public Vector2r Transform(Vector3r value)
		{
			return new Vector2r(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z
			);
		}

		public static Vector2r Transform(Matrix2x3r left, Vector3r right)
		{
			return new Vector2r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z
			);
		}

		public static Point3r Transform(Point2r left, Matrix2x3r right)
		{
			return new Point3r(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22, left.X * right.M13 + left.Y * right.M23
			);
		}

		public Point2r Transform(Point3r value)
		{
			return new Point2r(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z
			);
		}

		public static Point2r Transform(Matrix2x3r left, Point3r right)
		{
			return new Point2r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z
			);
		}
		
		public static Matrix2x3r FromRows(Vector3r row1, Vector3r row2)
		{
			return new Matrix2x3r(
			row1.X, row1.Y, row1.Z, row2.X, row2.Y, row2.Z
			);
		}
		
		public static Matrix2x3r FromRows(Vector3r[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 2)
				throw new ArgumentException("Expected a number of 2 rows.", nameof(rows));

			return FromRows(rows[0], rows[1]);
		}
		
		public static Matrix2x3r FromColumns(Vector2r column1, Vector2r column2, Vector2r column3)
		{
			return new Matrix2x3r(
			column1.X, column1.Y, column2.X, column2.Y, column3.X, column3.Y
			);
		}
		
		public static Matrix2x3r FromColumns(Vector2r[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 3)
				throw new ArgumentException("Expected a number of 3 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2]);
		}

		public static Matrix2x2r operator*(Matrix2x3r left, Matrix3x2r right)
		{
			return new Matrix2x2r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 			);
		}
		public static Matrix2x3r operator*(Matrix2x3r left, Matrix3x3r right)
		{
			return new Matrix2x3r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 			);
		}
		public static Matrix2x4r operator*(Matrix2x3r left, Matrix3x4r right)
		{
			return new Matrix2x4r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix2x4r : System.IEquatable<Matrix2x4r>
	{
		public static readonly Matrix2x4r Zero = new Matrix2x4r();

		
		public double M11  , M12  , M13  , M14  , M21  , M22  , M23  , M24  ;

		public Matrix2x4r( double m11  ,double m12  ,double m13  ,double m14  ,double m21  ,double m22  ,double m23  ,double m24 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M14 = m14;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M24 = m24;
					}

		public Matrix4x2r Transpose()
		{
			return new Matrix4x2r(
				M11 , M21 , M12 , M22 , M13 , M23 , M14 , M24 			);
		}
		
		public static Matrix2x4r operator*(Matrix2x4r left, double right)
		{
			return new Matrix2x4r(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M14 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M24 * right 			);
		}
		
		public static Matrix2x4r operator*(double left, Matrix2x4r right)
		{
			return new Matrix2x4r(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M14 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M24 			);
		}
		
		public static Matrix2x4r operator/(Matrix2x4r left, double right)
		{
			return new Matrix2x4r(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M14 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M24 / right 			);
		}
		
		public static Matrix2x4r operator+(Matrix2x4r left, Matrix2x4r right)
		{
			return new Matrix2x4r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 			);
		}
		
		public static Matrix2x4r operator-(Matrix2x4r left, Matrix2x4r right)
		{
			return new Matrix2x4r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 			);
		}
		
		public static Matrix2x4r operator+(Matrix2x4r value)
		{
			return value;
		}
		
		public static Matrix2x4r operator-(Matrix2x4r value)
		{
			return new Matrix2x4r(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M14 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M24 			);
		}
		
		public static bool operator==(Matrix2x4r left, Matrix2x4r right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24 ;
		}
		
		public static bool operator!=(Matrix2x4r left, Matrix2x4r right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M24 != right.M24 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M14.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M24.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix2x4r m && Equals(m);
		}

		public bool Equals(Matrix2x4r other)
		{
			return this == other;
		}

		public unsafe double this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix2x4r m = this;

				return ((double*)&m)[row * 2 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix2x4r m = this;

				((double*)&m)[row * 2 + column] = value;
			}
		}

		public unsafe Vector4r GetRow(int row)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x4r m = this;

			return ((Vector4r*)&m)[row];
		}

		public unsafe Vector2r GetColumn(int column)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x4r m = this;
			double* p = (double*)&m;

			return new Vector2r(p[0 + column], p[4 + column]);
		}

		public unsafe void SetRow(int row, Vector4r value)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix2x4r m = this;

			((Vector4r*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector2r value)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix2x4r m = this;
			double* p = (double*)&m;

						p[0 + column] = value.X;
						p[4 + column] = value.Y;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 2)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector4r Transform(Vector2r left, Matrix2x4r right)
		{
			return new Vector4r(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22, left.X * right.M13 + left.Y * right.M23, left.X * right.M14 + left.Y * right.M24
			);
		}

		public Vector2r Transform(Vector4r value)
		{
			return new Vector2r(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W
			);
		}

		public static Vector2r Transform(Matrix2x4r left, Vector4r right)
		{
			return new Vector2r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W
			);
		}

		public static Point4r Transform(Point2r left, Matrix2x4r right)
		{
			return new Point4r(
			left.X * right.M11 + left.Y * right.M21, left.X * right.M12 + left.Y * right.M22, left.X * right.M13 + left.Y * right.M23, left.X * right.M14 + left.Y * right.M24
			);
		}

		public Point2r Transform(Point4r value)
		{
			return new Point2r(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W
			);
		}

		public static Point2r Transform(Matrix2x4r left, Point4r right)
		{
			return new Point2r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W
			);
		}
		
		public static Matrix2x4r FromRows(Vector4r row1, Vector4r row2)
		{
			return new Matrix2x4r(
			row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W
			);
		}
		
		public static Matrix2x4r FromRows(Vector4r[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 2)
				throw new ArgumentException("Expected a number of 2 rows.", nameof(rows));

			return FromRows(rows[0], rows[1]);
		}
		
		public static Matrix2x4r FromColumns(Vector2r column1, Vector2r column2, Vector2r column3, Vector2r column4)
		{
			return new Matrix2x4r(
			column1.X, column1.Y, column2.X, column2.Y, column3.X, column3.Y, column4.X, column4.Y
			);
		}
		
		public static Matrix2x4r FromColumns(Vector2r[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 4)
				throw new ArgumentException("Expected a number of 4 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2], columns[3]);
		}

		public static Matrix2x2r operator*(Matrix2x4r left, Matrix4x2r right)
		{
			return new Matrix2x2r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 			);
		}
		public static Matrix2x3r operator*(Matrix2x4r left, Matrix4x3r right)
		{
			return new Matrix2x3r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 			);
		}
		public static Matrix2x4r operator*(Matrix2x4r left, Matrix4x4r right)
		{
			return new Matrix2x4r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34  +  left.M14 * right.M44 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34  +  left.M24 * right.M44 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix3x2r : System.IEquatable<Matrix3x2r>
	{
		public static readonly Matrix3x2r Zero = new Matrix3x2r();

		
		public double M11  , M12  , M21  , M22  , M31  , M32  ;

		public Matrix3x2r( double m11  ,double m12  ,double m21  ,double m22  ,double m31  ,double m32 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M21 = m21;
						this.M22 = m22;
						this.M31 = m31;
						this.M32 = m32;
					}

		public Matrix2x3r Transpose()
		{
			return new Matrix2x3r(
				M11 , M21 , M31 , M12 , M22 , M32 			);
		}
		
		public static Matrix3x2r operator*(Matrix3x2r left, double right)
		{
			return new Matrix3x2r(
		 left.M11 * right , left.M12 * right , left.M21 * right , left.M22 * right , left.M31 * right , left.M32 * right 			);
		}
		
		public static Matrix3x2r operator*(double left, Matrix3x2r right)
		{
			return new Matrix3x2r(
		 left * right.M11 , left * right.M12 , left * right.M21 , left * right.M22 , left * right.M31 , left * right.M32 			);
		}
		
		public static Matrix3x2r operator/(Matrix3x2r left, double right)
		{
			return new Matrix3x2r(
		 left.M11 / right , left.M12 / right , left.M21 / right , left.M22 / right , left.M31 / right , left.M32 / right 			);
		}
		
		public static Matrix3x2r operator+(Matrix3x2r left, Matrix3x2r right)
		{
			return new Matrix3x2r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 , left.M31 - right.M31 , left.M32 - right.M32 			);
		}
		
		public static Matrix3x2r operator-(Matrix3x2r left, Matrix3x2r right)
		{
			return new Matrix3x2r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 , left.M31 - right.M31 , left.M32 - right.M32 			);
		}
		
		public static Matrix3x2r operator+(Matrix3x2r value)
		{
			return value;
		}
		
		public static Matrix3x2r operator-(Matrix3x2r value)
		{
			return new Matrix3x2r(
		-value.M11 ,-value.M12 ,-value.M21 ,-value.M22 ,-value.M31 ,-value.M32 			);
		}
		
		public static bool operator==(Matrix3x2r left, Matrix3x2r right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M21 == right.M21 && left.M22 == right.M22 && left.M31 == right.M31 && left.M32 == right.M32 ;
		}
		
		public static bool operator!=(Matrix3x2r left, Matrix3x2r right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M21 != right.M21 || left.M22 != right.M22 || left.M31 != right.M31 || left.M32 != right.M32 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix3x2r m && Equals(m);
		}

		public bool Equals(Matrix3x2r other)
		{
			return this == other;
		}

		public unsafe double this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix3x2r m = this;

				return ((double*)&m)[row * 3 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix3x2r m = this;

				((double*)&m)[row * 3 + column] = value;
			}
		}

		public unsafe Vector2r GetRow(int row)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x2r m = this;

			return ((Vector2r*)&m)[row];
		}

		public unsafe Vector3r GetColumn(int column)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x2r m = this;
			double* p = (double*)&m;

			return new Vector3r(p[0 + column], p[2 + column], p[4 + column]);
		}

		public unsafe void SetRow(int row, Vector2r value)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x2r m = this;

			((Vector2r*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector3r value)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x2r m = this;
			double* p = (double*)&m;

						p[0 + column] = value.X;
						p[2 + column] = value.Y;
						p[4 + column] = value.Z;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector2r Transform(Vector3r left, Matrix3x2r right)
		{
			return new Vector2r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32
			);
		}

		public Vector3r Transform(Vector2r value)
		{
			return new Vector3r(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y, M31 * value.X + M32 * value.Y
			);
		}

		public static Vector3r Transform(Matrix3x2r left, Vector2r right)
		{
			return new Vector3r(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y, left.M31 * right.X + left.M32 * right.Y
			);
		}

		public static Point2r Transform(Point3r left, Matrix3x2r right)
		{
			return new Point2r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32
			);
		}

		public Point3r Transform(Point2r value)
		{
			return new Point3r(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y, M31 * value.X + M32 * value.Y
			);
		}

		public static Point3r Transform(Matrix3x2r left, Point2r right)
		{
			return new Point3r(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y, left.M31 * right.X + left.M32 * right.Y
			);
		}
		
		public static Matrix3x2r FromRows(Vector2r row1, Vector2r row2, Vector2r row3)
		{
			return new Matrix3x2r(
			row1.X, row1.Y, row2.X, row2.Y, row3.X, row3.Y
			);
		}
		
		public static Matrix3x2r FromRows(Vector2r[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 3)
				throw new ArgumentException("Expected a number of 3 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2]);
		}
		
		public static Matrix3x2r FromColumns(Vector3r column1, Vector3r column2)
		{
			return new Matrix3x2r(
			column1.X, column1.Y, column1.Z, column2.X, column2.Y, column2.Z
			);
		}
		
		public static Matrix3x2r FromColumns(Vector3r[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 2)
				throw new ArgumentException("Expected a number of 2 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1]);
		}

		public static Matrix3x2r operator*(Matrix3x2r left, Matrix2x2r right)
		{
			return new Matrix3x2r(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 			);
		}
		public static Matrix3x3r operator*(Matrix3x2r left, Matrix2x3r right)
		{
			return new Matrix3x3r(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M31 * right.M13  +  left.M32 * right.M23 			);
		}
		public static Matrix3x4r operator*(Matrix3x2r left, Matrix2x4r right)
		{
			return new Matrix3x4r(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M11 * right.M14  +  left.M12 * right.M24 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M21 * right.M14  +  left.M22 * right.M24 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M31 * right.M13  +  left.M32 * right.M23 ,  left.M31 * right.M14  +  left.M32 * right.M24 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix3x3r : System.IEquatable<Matrix3x3r>
	{
		public static readonly Matrix3x3r Zero = new Matrix3x3r();

				public static readonly Matrix3x3r Identity = new Matrix3x3r(1, 0, 0, 0, 1, 0, 0, 0, 1);

		
		public double M11  , M12  , M13  , M21  , M22  , M23  , M31  , M32  , M33  ;

		public Matrix3x3r( double m11  ,double m12  ,double m13  ,double m21  ,double m22  ,double m23  ,double m31  ,double m32  ,double m33 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M31 = m31;
						this.M32 = m32;
						this.M33 = m33;
					}

		public Matrix3x3r Transpose()
		{
			return new Matrix3x3r(
				M11 , M21 , M31 , M12 , M22 , M32 , M13 , M23 , M33 			);
		}
		
		public static Matrix3x3r operator*(Matrix3x3r left, double right)
		{
			return new Matrix3x3r(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M31 * right , left.M32 * right , left.M33 * right 			);
		}
		
		public static Matrix3x3r operator*(double left, Matrix3x3r right)
		{
			return new Matrix3x3r(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M31 , left * right.M32 , left * right.M33 			);
		}
		
		public static Matrix3x3r operator/(Matrix3x3r left, double right)
		{
			return new Matrix3x3r(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M31 / right , left.M32 / right , left.M33 / right 			);
		}
		
		public static Matrix3x3r operator+(Matrix3x3r left, Matrix3x3r right)
		{
			return new Matrix3x3r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 			);
		}
		
		public static Matrix3x3r operator-(Matrix3x3r left, Matrix3x3r right)
		{
			return new Matrix3x3r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 			);
		}
		
		public static Matrix3x3r operator+(Matrix3x3r value)
		{
			return value;
		}
		
		public static Matrix3x3r operator-(Matrix3x3r value)
		{
			return new Matrix3x3r(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M31 ,-value.M32 ,-value.M33 			);
		}
		
		public static bool operator==(Matrix3x3r left, Matrix3x3r right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 ;
		}
		
		public static bool operator!=(Matrix3x3r left, Matrix3x3r right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix3x3r m && Equals(m);
		}

		public bool Equals(Matrix3x3r other)
		{
			return this == other;
		}

		public unsafe double this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix3x3r m = this;

				return ((double*)&m)[row * 3 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix3x3r m = this;

				((double*)&m)[row * 3 + column] = value;
			}
		}

		public unsafe Vector3r GetRow(int row)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x3r m = this;

			return ((Vector3r*)&m)[row];
		}

		public unsafe Vector3r GetColumn(int column)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x3r m = this;
			double* p = (double*)&m;

			return new Vector3r(p[0 + column], p[3 + column], p[6 + column]);
		}

		public unsafe void SetRow(int row, Vector3r value)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x3r m = this;

			((Vector3r*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector3r value)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x3r m = this;
			double* p = (double*)&m;

						p[0 + column] = value.X;
						p[3 + column] = value.Y;
						p[6 + column] = value.Z;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

				public bool IsIdentity
			{
				get
				{
				return  this.M11 == 1 
				 &&  this.M12 == 0 
				 &&  this.M13 == 0 
				 &&  this.M21 == 0 
				 &&  this.M22 == 1 
				 &&  this.M23 == 0 
				 &&  this.M31 == 0 
				 &&  this.M32 == 0 
				 &&  this.M33 == 1 
				;
				}
			}
		
		public static Vector3r Transform(Vector3r left, Matrix3x3r right)
		{
			return new Vector3r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33
			);
		}

		public Vector3r Transform(Vector3r value)
		{
			return new Vector3r(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z, M31 * value.X + M32 * value.Y + M33 * value.Z
			);
		}

		public static Vector3r Transform(Matrix3x3r left, Vector3r right)
		{
			return new Vector3r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z
			);
		}

		public static Point3r Transform(Point3r left, Matrix3x3r right)
		{
			return new Point3r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33
			);
		}

		public Point3r Transform(Point3r value)
		{
			return new Point3r(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z, M31 * value.X + M32 * value.Y + M33 * value.Z
			);
		}

		public static Point3r Transform(Matrix3x3r left, Point3r right)
		{
			return new Point3r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z
			);
		}
		
		public static Matrix3x3r FromRows(Vector3r row1, Vector3r row2, Vector3r row3)
		{
			return new Matrix3x3r(
			row1.X, row1.Y, row1.Z, row2.X, row2.Y, row2.Z, row3.X, row3.Y, row3.Z
			);
		}
		
		public static Matrix3x3r FromRows(Vector3r[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 3)
				throw new ArgumentException("Expected a number of 3 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2]);
		}
		
		public static Matrix3x3r FromColumns(Vector3r column1, Vector3r column2, Vector3r column3)
		{
			return new Matrix3x3r(
			column1.X, column1.Y, column1.Z, column2.X, column2.Y, column2.Z, column3.X, column3.Y, column3.Z
			);
		}
		
		public static Matrix3x3r FromColumns(Vector3r[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 3)
				throw new ArgumentException("Expected a number of 3 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2]);
		}

		public static Matrix3x2r operator*(Matrix3x3r left, Matrix3x2r right)
		{
			return new Matrix3x2r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 			);
		}
		public static Matrix3x3r operator*(Matrix3x3r left, Matrix3x3r right)
		{
			return new Matrix3x3r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33 			);
		}
		public static Matrix3x4r operator*(Matrix3x3r left, Matrix3x4r right)
		{
			return new Matrix3x4r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33 ,  left.M31 * right.M14  +  left.M32 * right.M24  +  left.M33 * right.M34 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix3x4r : System.IEquatable<Matrix3x4r>
	{
		public static readonly Matrix3x4r Zero = new Matrix3x4r();

		
		public double M11  , M12  , M13  , M14  , M21  , M22  , M23  , M24  , M31  , M32  , M33  , M34  ;

		public Matrix3x4r( double m11  ,double m12  ,double m13  ,double m14  ,double m21  ,double m22  ,double m23  ,double m24  ,double m31  ,double m32  ,double m33  ,double m34 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M14 = m14;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M24 = m24;
						this.M31 = m31;
						this.M32 = m32;
						this.M33 = m33;
						this.M34 = m34;
					}

		public Matrix4x3r Transpose()
		{
			return new Matrix4x3r(
				M11 , M21 , M31 , M12 , M22 , M32 , M13 , M23 , M33 , M14 , M24 , M34 			);
		}
		
		public static Matrix3x4r operator*(Matrix3x4r left, double right)
		{
			return new Matrix3x4r(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M14 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M24 * right , left.M31 * right , left.M32 * right , left.M33 * right , left.M34 * right 			);
		}
		
		public static Matrix3x4r operator*(double left, Matrix3x4r right)
		{
			return new Matrix3x4r(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M14 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M24 , left * right.M31 , left * right.M32 , left * right.M33 , left * right.M34 			);
		}
		
		public static Matrix3x4r operator/(Matrix3x4r left, double right)
		{
			return new Matrix3x4r(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M14 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M24 / right , left.M31 / right , left.M32 / right , left.M33 / right , left.M34 / right 			);
		}
		
		public static Matrix3x4r operator+(Matrix3x4r left, Matrix3x4r right)
		{
			return new Matrix3x4r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M34 - right.M34 			);
		}
		
		public static Matrix3x4r operator-(Matrix3x4r left, Matrix3x4r right)
		{
			return new Matrix3x4r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M34 - right.M34 			);
		}
		
		public static Matrix3x4r operator+(Matrix3x4r value)
		{
			return value;
		}
		
		public static Matrix3x4r operator-(Matrix3x4r value)
		{
			return new Matrix3x4r(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M14 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M24 ,-value.M31 ,-value.M32 ,-value.M33 ,-value.M34 			);
		}
		
		public static bool operator==(Matrix3x4r left, Matrix3x4r right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M34 == right.M34 ;
		}
		
		public static bool operator!=(Matrix3x4r left, Matrix3x4r right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M24 != right.M24 || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 || left.M34 != right.M34 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M14.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M24.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode() ^ M34.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix3x4r m && Equals(m);
		}

		public bool Equals(Matrix3x4r other)
		{
			return this == other;
		}

		public unsafe double this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix3x4r m = this;

				return ((double*)&m)[row * 3 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix3x4r m = this;

				((double*)&m)[row * 3 + column] = value;
			}
		}

		public unsafe Vector4r GetRow(int row)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x4r m = this;

			return ((Vector4r*)&m)[row];
		}

		public unsafe Vector3r GetColumn(int column)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x4r m = this;
			double* p = (double*)&m;

			return new Vector3r(p[0 + column], p[4 + column], p[8 + column]);
		}

		public unsafe void SetRow(int row, Vector4r value)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix3x4r m = this;

			((Vector4r*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector3r value)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix3x4r m = this;
			double* p = (double*)&m;

						p[0 + column] = value.X;
						p[4 + column] = value.Y;
						p[8 + column] = value.Z;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 3)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector4r Transform(Vector3r left, Matrix3x4r right)
		{
			return new Vector4r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33, left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34
			);
		}

		public Vector3r Transform(Vector4r value)
		{
			return new Vector3r(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W, M31 * value.X + M32 * value.Y + M33 * value.Z + M34 * value.W
			);
		}

		public static Vector3r Transform(Matrix3x4r left, Vector4r right)
		{
			return new Vector3r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34 * right.W
			);
		}

		public static Point4r Transform(Point3r left, Matrix3x4r right)
		{
			return new Point4r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33, left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34
			);
		}

		public Point3r Transform(Point4r value)
		{
			return new Point3r(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W, M31 * value.X + M32 * value.Y + M33 * value.Z + M34 * value.W
			);
		}

		public static Point3r Transform(Matrix3x4r left, Point4r right)
		{
			return new Point3r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34 * right.W
			);
		}
		
		public static Matrix3x4r FromRows(Vector4r row1, Vector4r row2, Vector4r row3)
		{
			return new Matrix3x4r(
			row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W, row3.X, row3.Y, row3.Z, row3.W
			);
		}
		
		public static Matrix3x4r FromRows(Vector4r[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 3)
				throw new ArgumentException("Expected a number of 3 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2]);
		}
		
		public static Matrix3x4r FromColumns(Vector3r column1, Vector3r column2, Vector3r column3, Vector3r column4)
		{
			return new Matrix3x4r(
			column1.X, column1.Y, column1.Z, column2.X, column2.Y, column2.Z, column3.X, column3.Y, column3.Z, column4.X, column4.Y, column4.Z
			);
		}
		
		public static Matrix3x4r FromColumns(Vector3r[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 4)
				throw new ArgumentException("Expected a number of 4 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2], columns[3]);
		}

		public static Matrix3x2r operator*(Matrix3x4r left, Matrix4x2r right)
		{
			return new Matrix3x2r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 			);
		}
		public static Matrix3x3r operator*(Matrix3x4r left, Matrix4x3r right)
		{
			return new Matrix3x3r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33  +  left.M34 * right.M43 			);
		}
		public static Matrix3x4r operator*(Matrix3x4r left, Matrix4x4r right)
		{
			return new Matrix3x4r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34  +  left.M14 * right.M44 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34  +  left.M24 * right.M44 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33  +  left.M34 * right.M43 ,  left.M31 * right.M14  +  left.M32 * right.M24  +  left.M33 * right.M34  +  left.M34 * right.M44 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix4x2r : System.IEquatable<Matrix4x2r>
	{
		public static readonly Matrix4x2r Zero = new Matrix4x2r();

		
		public double M11  , M12  , M21  , M22  , M31  , M32  , M41  , M42  ;

		public Matrix4x2r( double m11  ,double m12  ,double m21  ,double m22  ,double m31  ,double m32  ,double m41  ,double m42 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M21 = m21;
						this.M22 = m22;
						this.M31 = m31;
						this.M32 = m32;
						this.M41 = m41;
						this.M42 = m42;
					}

		public Matrix2x4r Transpose()
		{
			return new Matrix2x4r(
				M11 , M21 , M31 , M41 , M12 , M22 , M32 , M42 			);
		}
		
		public static Matrix4x2r operator*(Matrix4x2r left, double right)
		{
			return new Matrix4x2r(
		 left.M11 * right , left.M12 * right , left.M21 * right , left.M22 * right , left.M31 * right , left.M32 * right , left.M41 * right , left.M42 * right 			);
		}
		
		public static Matrix4x2r operator*(double left, Matrix4x2r right)
		{
			return new Matrix4x2r(
		 left * right.M11 , left * right.M12 , left * right.M21 , left * right.M22 , left * right.M31 , left * right.M32 , left * right.M41 , left * right.M42 			);
		}
		
		public static Matrix4x2r operator/(Matrix4x2r left, double right)
		{
			return new Matrix4x2r(
		 left.M11 / right , left.M12 / right , left.M21 / right , left.M22 / right , left.M31 / right , left.M32 / right , left.M41 / right , left.M42 / right 			);
		}
		
		public static Matrix4x2r operator+(Matrix4x2r left, Matrix4x2r right)
		{
			return new Matrix4x2r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 , left.M31 - right.M31 , left.M32 - right.M32 , left.M41 - right.M41 , left.M42 - right.M42 			);
		}
		
		public static Matrix4x2r operator-(Matrix4x2r left, Matrix4x2r right)
		{
			return new Matrix4x2r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M21 - right.M21 , left.M22 - right.M22 , left.M31 - right.M31 , left.M32 - right.M32 , left.M41 - right.M41 , left.M42 - right.M42 			);
		}
		
		public static Matrix4x2r operator+(Matrix4x2r value)
		{
			return value;
		}
		
		public static Matrix4x2r operator-(Matrix4x2r value)
		{
			return new Matrix4x2r(
		-value.M11 ,-value.M12 ,-value.M21 ,-value.M22 ,-value.M31 ,-value.M32 ,-value.M41 ,-value.M42 			);
		}
		
		public static bool operator==(Matrix4x2r left, Matrix4x2r right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M21 == right.M21 && left.M22 == right.M22 && left.M31 == right.M31 && left.M32 == right.M32 && left.M41 == right.M41 && left.M42 == right.M42 ;
		}
		
		public static bool operator!=(Matrix4x2r left, Matrix4x2r right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M21 != right.M21 || left.M22 != right.M22 || left.M31 != right.M31 || left.M32 != right.M32 || left.M41 != right.M41 || left.M42 != right.M42 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M41.GetHashCode() ^ M42.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix4x2r m && Equals(m);
		}

		public bool Equals(Matrix4x2r other)
		{
			return this == other;
		}

		public unsafe double this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix4x2r m = this;

				return ((double*)&m)[row * 4 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix4x2r m = this;

				((double*)&m)[row * 4 + column] = value;
			}
		}

		public unsafe Vector2r GetRow(int row)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x2r m = this;

			return ((Vector2r*)&m)[row];
		}

		public unsafe Vector4r GetColumn(int column)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x2r m = this;
			double* p = (double*)&m;

			return new Vector4r(p[0 + column], p[2 + column], p[4 + column], p[6 + column]);
		}

		public unsafe void SetRow(int row, Vector2r value)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x2r m = this;

			((Vector2r*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector4r value)
		{
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x2r m = this;
			double* p = (double*)&m;

						p[0 + column] = value.X;
						p[2 + column] = value.Y;
						p[4 + column] = value.Z;
						p[6 + column] = value.W;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 2)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector2r Transform(Vector4r left, Matrix4x2r right)
		{
			return new Vector2r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42
			);
		}

		public Vector4r Transform(Vector2r value)
		{
			return new Vector4r(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y, M31 * value.X + M32 * value.Y, M41 * value.X + M42 * value.Y
			);
		}

		public static Vector4r Transform(Matrix4x2r left, Vector2r right)
		{
			return new Vector4r(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y, left.M31 * right.X + left.M32 * right.Y, left.M41 * right.X + left.M42 * right.Y
			);
		}

		public static Point2r Transform(Point4r left, Matrix4x2r right)
		{
			return new Point2r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42
			);
		}

		public Point4r Transform(Point2r value)
		{
			return new Point4r(
			M11 * value.X + M12 * value.Y, M21 * value.X + M22 * value.Y, M31 * value.X + M32 * value.Y, M41 * value.X + M42 * value.Y
			);
		}

		public static Point4r Transform(Matrix4x2r left, Point2r right)
		{
			return new Point4r(
			left.M11 * right.X + left.M12 * right.Y, left.M21 * right.X + left.M22 * right.Y, left.M31 * right.X + left.M32 * right.Y, left.M41 * right.X + left.M42 * right.Y
			);
		}
		
		public static Matrix4x2r FromRows(Vector2r row1, Vector2r row2, Vector2r row3, Vector2r row4)
		{
			return new Matrix4x2r(
			row1.X, row1.Y, row2.X, row2.Y, row3.X, row3.Y, row4.X, row4.Y
			);
		}
		
		public static Matrix4x2r FromRows(Vector2r[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 4)
				throw new ArgumentException("Expected a number of 4 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2], rows[3]);
		}
		
		public static Matrix4x2r FromColumns(Vector4r column1, Vector4r column2)
		{
			return new Matrix4x2r(
			column1.X, column1.Y, column1.Z, column1.W, column2.X, column2.Y, column2.Z, column2.W
			);
		}
		
		public static Matrix4x2r FromColumns(Vector4r[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 2)
				throw new ArgumentException("Expected a number of 2 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1]);
		}

		public static Matrix4x2r operator*(Matrix4x2r left, Matrix2x2r right)
		{
			return new Matrix4x2r(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M41 * right.M11  +  left.M42 * right.M21 ,  left.M41 * right.M12  +  left.M42 * right.M22 			);
		}
		public static Matrix4x3r operator*(Matrix4x2r left, Matrix2x3r right)
		{
			return new Matrix4x3r(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M31 * right.M13  +  left.M32 * right.M23 ,  left.M41 * right.M11  +  left.M42 * right.M21 ,  left.M41 * right.M12  +  left.M42 * right.M22 ,  left.M41 * right.M13  +  left.M42 * right.M23 			);
		}
		public static Matrix4x4r operator*(Matrix4x2r left, Matrix2x4r right)
		{
			return new Matrix4x4r(
				 left.M11 * right.M11  +  left.M12 * right.M21 ,  left.M11 * right.M12  +  left.M12 * right.M22 ,  left.M11 * right.M13  +  left.M12 * right.M23 ,  left.M11 * right.M14  +  left.M12 * right.M24 ,  left.M21 * right.M11  +  left.M22 * right.M21 ,  left.M21 * right.M12  +  left.M22 * right.M22 ,  left.M21 * right.M13  +  left.M22 * right.M23 ,  left.M21 * right.M14  +  left.M22 * right.M24 ,  left.M31 * right.M11  +  left.M32 * right.M21 ,  left.M31 * right.M12  +  left.M32 * right.M22 ,  left.M31 * right.M13  +  left.M32 * right.M23 ,  left.M31 * right.M14  +  left.M32 * right.M24 ,  left.M41 * right.M11  +  left.M42 * right.M21 ,  left.M41 * right.M12  +  left.M42 * right.M22 ,  left.M41 * right.M13  +  left.M42 * right.M23 ,  left.M41 * right.M14  +  left.M42 * right.M24 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix4x3r : System.IEquatable<Matrix4x3r>
	{
		public static readonly Matrix4x3r Zero = new Matrix4x3r();

		
		public double M11  , M12  , M13  , M21  , M22  , M23  , M31  , M32  , M33  , M41  , M42  , M43  ;

		public Matrix4x3r( double m11  ,double m12  ,double m13  ,double m21  ,double m22  ,double m23  ,double m31  ,double m32  ,double m33  ,double m41  ,double m42  ,double m43 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M31 = m31;
						this.M32 = m32;
						this.M33 = m33;
						this.M41 = m41;
						this.M42 = m42;
						this.M43 = m43;
					}

		public Matrix3x4r Transpose()
		{
			return new Matrix3x4r(
				M11 , M21 , M31 , M41 , M12 , M22 , M32 , M42 , M13 , M23 , M33 , M43 			);
		}
		
		public static Matrix4x3r operator*(Matrix4x3r left, double right)
		{
			return new Matrix4x3r(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M31 * right , left.M32 * right , left.M33 * right , left.M41 * right , left.M42 * right , left.M43 * right 			);
		}
		
		public static Matrix4x3r operator*(double left, Matrix4x3r right)
		{
			return new Matrix4x3r(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M31 , left * right.M32 , left * right.M33 , left * right.M41 , left * right.M42 , left * right.M43 			);
		}
		
		public static Matrix4x3r operator/(Matrix4x3r left, double right)
		{
			return new Matrix4x3r(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M31 / right , left.M32 / right , left.M33 / right , left.M41 / right , left.M42 / right , left.M43 / right 			);
		}
		
		public static Matrix4x3r operator+(Matrix4x3r left, Matrix4x3r right)
		{
			return new Matrix4x3r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M41 - right.M41 , left.M42 - right.M42 , left.M43 - right.M43 			);
		}
		
		public static Matrix4x3r operator-(Matrix4x3r left, Matrix4x3r right)
		{
			return new Matrix4x3r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M41 - right.M41 , left.M42 - right.M42 , left.M43 - right.M43 			);
		}
		
		public static Matrix4x3r operator+(Matrix4x3r value)
		{
			return value;
		}
		
		public static Matrix4x3r operator-(Matrix4x3r value)
		{
			return new Matrix4x3r(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M31 ,-value.M32 ,-value.M33 ,-value.M41 ,-value.M42 ,-value.M43 			);
		}
		
		public static bool operator==(Matrix4x3r left, Matrix4x3r right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43 ;
		}
		
		public static bool operator!=(Matrix4x3r left, Matrix4x3r right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 || left.M41 != right.M41 || left.M42 != right.M42 || left.M43 != right.M43 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode() ^ M41.GetHashCode() ^ M42.GetHashCode() ^ M43.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix4x3r m && Equals(m);
		}

		public bool Equals(Matrix4x3r other)
		{
			return this == other;
		}

		public unsafe double this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix4x3r m = this;

				return ((double*)&m)[row * 4 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix4x3r m = this;

				((double*)&m)[row * 4 + column] = value;
			}
		}

		public unsafe Vector3r GetRow(int row)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x3r m = this;

			return ((Vector3r*)&m)[row];
		}

		public unsafe Vector4r GetColumn(int column)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x3r m = this;
			double* p = (double*)&m;

			return new Vector4r(p[0 + column], p[3 + column], p[6 + column], p[9 + column]);
		}

		public unsafe void SetRow(int row, Vector3r value)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x3r m = this;

			((Vector3r*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector4r value)
		{
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x3r m = this;
			double* p = (double*)&m;

						p[0 + column] = value.X;
						p[3 + column] = value.Y;
						p[6 + column] = value.Z;
						p[9 + column] = value.W;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 3)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

		
		public static Vector3r Transform(Vector4r left, Matrix4x3r right)
		{
			return new Vector3r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43
			);
		}

		public Vector4r Transform(Vector3r value)
		{
			return new Vector4r(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z, M31 * value.X + M32 * value.Y + M33 * value.Z, M41 * value.X + M42 * value.Y + M43 * value.Z
			);
		}

		public static Vector4r Transform(Matrix4x3r left, Vector3r right)
		{
			return new Vector4r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z, left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z
			);
		}

		public static Point3r Transform(Point4r left, Matrix4x3r right)
		{
			return new Point3r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43
			);
		}

		public Point4r Transform(Point3r value)
		{
			return new Point4r(
			M11 * value.X + M12 * value.Y + M13 * value.Z, M21 * value.X + M22 * value.Y + M23 * value.Z, M31 * value.X + M32 * value.Y + M33 * value.Z, M41 * value.X + M42 * value.Y + M43 * value.Z
			);
		}

		public static Point4r Transform(Matrix4x3r left, Point3r right)
		{
			return new Point4r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z, left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z
			);
		}
		
		public static Matrix4x3r FromRows(Vector3r row1, Vector3r row2, Vector3r row3, Vector3r row4)
		{
			return new Matrix4x3r(
			row1.X, row1.Y, row1.Z, row2.X, row2.Y, row2.Z, row3.X, row3.Y, row3.Z, row4.X, row4.Y, row4.Z
			);
		}
		
		public static Matrix4x3r FromRows(Vector3r[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 4)
				throw new ArgumentException("Expected a number of 4 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2], rows[3]);
		}
		
		public static Matrix4x3r FromColumns(Vector4r column1, Vector4r column2, Vector4r column3)
		{
			return new Matrix4x3r(
			column1.X, column1.Y, column1.Z, column1.W, column2.X, column2.Y, column2.Z, column2.W, column3.X, column3.Y, column3.Z, column3.W
			);
		}
		
		public static Matrix4x3r FromColumns(Vector4r[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 3)
				throw new ArgumentException("Expected a number of 3 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2]);
		}

		public static Matrix4x2r operator*(Matrix4x3r left, Matrix3x2r right)
		{
			return new Matrix4x2r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32 			);
		}
		public static Matrix4x3r operator*(Matrix4x3r left, Matrix3x3r right)
		{
			return new Matrix4x3r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32 ,  left.M41 * right.M13  +  left.M42 * right.M23  +  left.M43 * right.M33 			);
		}
		public static Matrix4x4r operator*(Matrix4x3r left, Matrix3x4r right)
		{
			return new Matrix4x4r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33 ,  left.M31 * right.M14  +  left.M32 * right.M24  +  left.M33 * right.M34 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32 ,  left.M41 * right.M13  +  left.M42 * right.M23  +  left.M43 * right.M33 ,  left.M41 * right.M14  +  left.M42 * right.M24  +  left.M43 * right.M34 			);
		}
			}


	[System.Serializable()]
	public partial struct Matrix4x4r : System.IEquatable<Matrix4x4r>
	{
		public static readonly Matrix4x4r Zero = new Matrix4x4r();

				public static readonly Matrix4x4r Identity = new Matrix4x4r(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

		
		public double M11  , M12  , M13  , M14  , M21  , M22  , M23  , M24  , M31  , M32  , M33  , M34  , M41  , M42  , M43  , M44  ;

		public Matrix4x4r( double m11  ,double m12  ,double m13  ,double m14  ,double m21  ,double m22  ,double m23  ,double m24  ,double m31  ,double m32  ,double m33  ,double m34  ,double m41  ,double m42  ,double m43  ,double m44 )
		{
					this.M11 = m11;
						this.M12 = m12;
						this.M13 = m13;
						this.M14 = m14;
						this.M21 = m21;
						this.M22 = m22;
						this.M23 = m23;
						this.M24 = m24;
						this.M31 = m31;
						this.M32 = m32;
						this.M33 = m33;
						this.M34 = m34;
						this.M41 = m41;
						this.M42 = m42;
						this.M43 = m43;
						this.M44 = m44;
					}

		public Matrix4x4r Transpose()
		{
			return new Matrix4x4r(
				M11 , M21 , M31 , M41 , M12 , M22 , M32 , M42 , M13 , M23 , M33 , M43 , M14 , M24 , M34 , M44 			);
		}
		
		public static Matrix4x4r operator*(Matrix4x4r left, double right)
		{
			return new Matrix4x4r(
		 left.M11 * right , left.M12 * right , left.M13 * right , left.M14 * right , left.M21 * right , left.M22 * right , left.M23 * right , left.M24 * right , left.M31 * right , left.M32 * right , left.M33 * right , left.M34 * right , left.M41 * right , left.M42 * right , left.M43 * right , left.M44 * right 			);
		}
		
		public static Matrix4x4r operator*(double left, Matrix4x4r right)
		{
			return new Matrix4x4r(
		 left * right.M11 , left * right.M12 , left * right.M13 , left * right.M14 , left * right.M21 , left * right.M22 , left * right.M23 , left * right.M24 , left * right.M31 , left * right.M32 , left * right.M33 , left * right.M34 , left * right.M41 , left * right.M42 , left * right.M43 , left * right.M44 			);
		}
		
		public static Matrix4x4r operator/(Matrix4x4r left, double right)
		{
			return new Matrix4x4r(
		 left.M11 / right , left.M12 / right , left.M13 / right , left.M14 / right , left.M21 / right , left.M22 / right , left.M23 / right , left.M24 / right , left.M31 / right , left.M32 / right , left.M33 / right , left.M34 / right , left.M41 / right , left.M42 / right , left.M43 / right , left.M44 / right 			);
		}
		
		public static Matrix4x4r operator+(Matrix4x4r left, Matrix4x4r right)
		{
			return new Matrix4x4r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M34 - right.M34 , left.M41 - right.M41 , left.M42 - right.M42 , left.M43 - right.M43 , left.M44 - right.M44 			);
		}
		
		public static Matrix4x4r operator-(Matrix4x4r left, Matrix4x4r right)
		{
			return new Matrix4x4r(
		 left.M11 - right.M11 , left.M12 - right.M12 , left.M13 - right.M13 , left.M14 - right.M14 , left.M21 - right.M21 , left.M22 - right.M22 , left.M23 - right.M23 , left.M24 - right.M24 , left.M31 - right.M31 , left.M32 - right.M32 , left.M33 - right.M33 , left.M34 - right.M34 , left.M41 - right.M41 , left.M42 - right.M42 , left.M43 - right.M43 , left.M44 - right.M44 			);
		}
		
		public static Matrix4x4r operator+(Matrix4x4r value)
		{
			return value;
		}
		
		public static Matrix4x4r operator-(Matrix4x4r value)
		{
			return new Matrix4x4r(
		-value.M11 ,-value.M12 ,-value.M13 ,-value.M14 ,-value.M21 ,-value.M22 ,-value.M23 ,-value.M24 ,-value.M31 ,-value.M32 ,-value.M33 ,-value.M34 ,-value.M41 ,-value.M42 ,-value.M43 ,-value.M44 			);
		}
		
		public static bool operator==(Matrix4x4r left, Matrix4x4r right)
		{
			return  left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M34 == right.M34 && left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43 && left.M44 == right.M44 ;
		}
		
		public static bool operator!=(Matrix4x4r left, Matrix4x4r right)
		{
			return  left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14 || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M24 != right.M24 || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 || left.M34 != right.M34 || left.M41 != right.M41 || left.M42 != right.M42 || left.M43 != right.M43 || left.M44 != right.M44 ;
		}

		public override int GetHashCode()
		{
			return M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M14.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M24.GetHashCode() ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode() ^ M34.GetHashCode() ^ M41.GetHashCode() ^ M42.GetHashCode() ^ M43.GetHashCode() ^ M44.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Matrix4x4r m && Equals(m);
		}

		public bool Equals(Matrix4x4r other)
		{
			return this == other;
		}

		public unsafe double this[int row, int column]
		{
			get 
			{
				CheckCoordinate(row, column);

				Matrix4x4r m = this;

				return ((double*)&m)[row * 4 + column];
				
			}
			set 
			{ 
				CheckCoordinate(row, column);

				Matrix4x4r m = this;

				((double*)&m)[row * 4 + column] = value;
			}
		}

		public unsafe Vector4r GetRow(int row)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x4r m = this;

			return ((Vector4r*)&m)[row];
		}

		public unsafe Vector4r GetColumn(int column)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x4r m = this;
			double* p = (double*)&m;

			return new Vector4r(p[0 + column], p[4 + column], p[8 + column], p[12 + column]);
		}

		public unsafe void SetRow(int row, Vector4r value)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
				
			Matrix4x4r m = this;

			((Vector4r*)&m)[row] = value;
		}

		public unsafe void SetColumn(int column, Vector4r value)
		{
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
				
			Matrix4x4r m = this;
			double* p = (double*)&m;

						p[0 + column] = value.X;
						p[4 + column] = value.Y;
						p[8 + column] = value.Z;
						p[12 + column] = value.W;
					}

		private static void CheckCoordinate(int row, int column)
		{
			if (row < 0 || row >= 4)
				throw new ArgumentOutOfRangeException(nameof(row), row, "The indicated row does not fall into the matrix.");
			if (column < 0 || column >= 4)
				throw new ArgumentOutOfRangeException(nameof(column), column, "The indicated column does not fall into the matrix.");
		}

				public bool IsIdentity
			{
				get
				{
				return  this.M11 == 1 
				 &&  this.M12 == 0 
				 &&  this.M13 == 0 
				 &&  this.M14 == 0 
				 &&  this.M21 == 0 
				 &&  this.M22 == 1 
				 &&  this.M23 == 0 
				 &&  this.M24 == 0 
				 &&  this.M31 == 0 
				 &&  this.M32 == 0 
				 &&  this.M33 == 1 
				 &&  this.M34 == 0 
				 &&  this.M41 == 0 
				 &&  this.M42 == 0 
				 &&  this.M43 == 0 
				 &&  this.M44 == 1 
				;
				}
			}
		
		public static Vector4r Transform(Vector4r left, Matrix4x4r right)
		{
			return new Vector4r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43, left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34 + left.W * right.M44
			);
		}

		public Vector4r Transform(Vector4r value)
		{
			return new Vector4r(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W, M31 * value.X + M32 * value.Y + M33 * value.Z + M34 * value.W, M41 * value.X + M42 * value.Y + M43 * value.Z + M44 * value.W
			);
		}

		public static Vector4r Transform(Matrix4x4r left, Vector4r right)
		{
			return new Vector4r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34 * right.W, left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z + left.M44 * right.W
			);
		}

		public static Point4r Transform(Point4r left, Matrix4x4r right)
		{
			return new Point4r(
			left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41, left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42, left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43, left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34 + left.W * right.M44
			);
		}

		public Point4r Transform(Point4r value)
		{
			return new Point4r(
			M11 * value.X + M12 * value.Y + M13 * value.Z + M14 * value.W, M21 * value.X + M22 * value.Y + M23 * value.Z + M24 * value.W, M31 * value.X + M32 * value.Y + M33 * value.Z + M34 * value.W, M41 * value.X + M42 * value.Y + M43 * value.Z + M44 * value.W
			);
		}

		public static Point4r Transform(Matrix4x4r left, Point4r right)
		{
			return new Point4r(
			left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W, left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W, left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34 * right.W, left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z + left.M44 * right.W
			);
		}
		
		public static Matrix4x4r FromRows(Vector4r row1, Vector4r row2, Vector4r row3, Vector4r row4)
		{
			return new Matrix4x4r(
			row1.X, row1.Y, row1.Z, row1.W, row2.X, row2.Y, row2.Z, row2.W, row3.X, row3.Y, row3.Z, row3.W, row4.X, row4.Y, row4.Z, row4.W
			);
		}
		
		public static Matrix4x4r FromRows(Vector4r[] rows)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (rows.Length != 4)
				throw new ArgumentException("Expected a number of 4 rows.", nameof(rows));

			return FromRows(rows[0], rows[1], rows[2], rows[3]);
		}
		
		public static Matrix4x4r FromColumns(Vector4r column1, Vector4r column2, Vector4r column3, Vector4r column4)
		{
			return new Matrix4x4r(
			column1.X, column1.Y, column1.Z, column1.W, column2.X, column2.Y, column2.Z, column2.W, column3.X, column3.Y, column3.Z, column3.W, column4.X, column4.Y, column4.Z, column4.W
			);
		}
		
		public static Matrix4x4r FromColumns(Vector4r[] columns)
		{
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));

			if (columns.Length != 4)
				throw new ArgumentException("Expected a number of 4 columns.", nameof(columns));

			return FromColumns(columns[0], columns[1], columns[2], columns[3]);
		}

		public static Matrix4x2r operator*(Matrix4x4r left, Matrix4x2r right)
		{
			return new Matrix4x2r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31  +  left.M44 * right.M41 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32  +  left.M44 * right.M42 			);
		}
		public static Matrix4x3r operator*(Matrix4x4r left, Matrix4x3r right)
		{
			return new Matrix4x3r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33  +  left.M34 * right.M43 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31  +  left.M44 * right.M41 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32  +  left.M44 * right.M42 ,  left.M41 * right.M13  +  left.M42 * right.M23  +  left.M43 * right.M33  +  left.M44 * right.M43 			);
		}
		public static Matrix4x4r operator*(Matrix4x4r left, Matrix4x4r right)
		{
			return new Matrix4x4r(
				 left.M11 * right.M11  +  left.M12 * right.M21  +  left.M13 * right.M31  +  left.M14 * right.M41 ,  left.M11 * right.M12  +  left.M12 * right.M22  +  left.M13 * right.M32  +  left.M14 * right.M42 ,  left.M11 * right.M13  +  left.M12 * right.M23  +  left.M13 * right.M33  +  left.M14 * right.M43 ,  left.M11 * right.M14  +  left.M12 * right.M24  +  left.M13 * right.M34  +  left.M14 * right.M44 ,  left.M21 * right.M11  +  left.M22 * right.M21  +  left.M23 * right.M31  +  left.M24 * right.M41 ,  left.M21 * right.M12  +  left.M22 * right.M22  +  left.M23 * right.M32  +  left.M24 * right.M42 ,  left.M21 * right.M13  +  left.M22 * right.M23  +  left.M23 * right.M33  +  left.M24 * right.M43 ,  left.M21 * right.M14  +  left.M22 * right.M24  +  left.M23 * right.M34  +  left.M24 * right.M44 ,  left.M31 * right.M11  +  left.M32 * right.M21  +  left.M33 * right.M31  +  left.M34 * right.M41 ,  left.M31 * right.M12  +  left.M32 * right.M22  +  left.M33 * right.M32  +  left.M34 * right.M42 ,  left.M31 * right.M13  +  left.M32 * right.M23  +  left.M33 * right.M33  +  left.M34 * right.M43 ,  left.M31 * right.M14  +  left.M32 * right.M24  +  left.M33 * right.M34  +  left.M34 * right.M44 ,  left.M41 * right.M11  +  left.M42 * right.M21  +  left.M43 * right.M31  +  left.M44 * right.M41 ,  left.M41 * right.M12  +  left.M42 * right.M22  +  left.M43 * right.M32  +  left.M44 * right.M42 ,  left.M41 * right.M13  +  left.M42 * right.M23  +  left.M43 * right.M33  +  left.M44 * right.M43 ,  left.M41 * right.M14  +  left.M42 * right.M24  +  left.M43 * right.M34  +  left.M44 * right.M44 			);
		}
			}

}