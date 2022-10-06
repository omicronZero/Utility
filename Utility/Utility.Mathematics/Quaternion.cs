
using System;

namespace Utility.Mathematics
{
    [Serializable()]
    public partial struct QuaternionF : IEquatable<QuaternionF>
    {
        /// <summary>
        /// The real part of the quaternion.
        /// </summary>
        public float S;
        /// <summary>
        /// The x component of the imaginary part of the quaternion.
        /// </summary>
        public float X;
        /// <summary>
        /// The y component of the imaginary part of the quaternion.
        /// </summary>
        public float Y;
        /// <summary>
        /// The z component of the imaginary part of the quaternion.
        /// </summary>
        public float Z;

        public QuaternionF(float s, Vector3f v)
            : this(s, v.X, v.Y, v.Z)
        { }

        public QuaternionF(float s, float x, float y, float z)
        {
            S = s;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Returns the imaginary part of the quaternion.
        /// </summary>
        public Vector3f V
        {
            get { return new Vector3f(X, Y, Z); }
        }

        public void Invert()
        {
            this = 1 / NormSq() * ~this;
        }

        public float NormSq()
        {
            return S * S + X * X + Y * Y + Z * Z;
        }

        public float Norm()
        {
            return (float)System.Math.Sqrt(NormSq());
        }

		public void Normalize()
		{
			this = this / Norm();
		}

		public double GetAngle()
		{
			return System.Math.Acos(S / (Norm() * 2));
		}

		public double GetAngleNormalized()
		{
			return System.Math.Acos(S / 2);
		}

		public void Transform(Vector3f[] values, int index, int count)
		{
			var p = this;
			p.Normalize();
			p.TransformNormalized(values, index, count);
		}

		public Vector3f Transform(Vector3f value)
		{
			var p = this;
			p.Normalize();
			return (p * new QuaternionF(0, value) * ~p).V;
		}

		public void TransformNormalized(Vector3f[] values, int index, int count)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values));
			Util.ValidateRange(index, count, values.Length);
			
			int d = index + count;
			for(int i = index; i < d; i++)
				values[i] = Transform(values[i]);
		}

		public Vector3f TransformNormalized(Vector3f value)
		{
			return (this * new QuaternionF(0, value) * ~this).V;
		}

        public override bool Equals(object obj)
        {
			if (obj == null)
				return false;

			var q = obj as QuaternionF?;

			if (q == null)
				return false;

			return Equals(q.Value);
        }

        public bool Equals(QuaternionF other)
        {
            return S == other.S && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            return S.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override string ToString()
        {
            return $"({S}, {X}, {Y}, {Z})";
        }

		public static bool operator==(QuaternionF left, QuaternionF right)
		{
			return left.Equals(right);
		}

		public static bool operator!=(QuaternionF left, QuaternionF right)
		{
			return !left.Equals(right);
		}

        public static float Dot(QuaternionF left, QuaternionF right)
        {
            return left.S * right.S + left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static QuaternionF Cross(QuaternionF left, QuaternionF right)
        {
            return new QuaternionF(0, Vector3f.Cross(left.V, right.V));
        }

        public static QuaternionF operator +(QuaternionF left, QuaternionF right)
        {
            return new QuaternionF(left.S + right.S, left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static QuaternionF operator -(QuaternionF left, QuaternionF right)
        {
            return new QuaternionF(left.S - right.S, left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static QuaternionF operator *(QuaternionF left, QuaternionF right)
        {
            return new QuaternionF(left.S * right.S - left.X * right.X - left.Y * right.Y - left.Z * right.Z,
										left.S * right.X + left.X * right.S + left.Y * right.Z + left.Z * right.Y,
										left.S * right.Y + left.X * right.Z + left.Y * right.S + left.Z * right.X,
										left.S * right.Z + left.X * right.Y + left.Y * right.X + left.Z * right.S);
        }

        public static QuaternionF operator *(float left, QuaternionF right)
        {
            return new QuaternionF(left * right.S, left * right.X, left * right.Y, left * right.Z);
        }

        public static QuaternionF operator *(QuaternionF left, float right)
        {
            return new QuaternionF(left.S * right, left.X * right, left.Y * right, left.Z * right);
        }

        public static QuaternionF operator /(QuaternionF left, float right)
        {
            return new QuaternionF(left.S / right, left.X / right, left.Y / right, left.Z / right);
        }

        public static QuaternionF operator ~(QuaternionF value)
        {
            return new QuaternionF(value.S, -value.V);
        }

		public static explicit operator QuaternionF(float value)
		{
			return new QuaternionF(value, 0, 0, 0);
		}

		public static explicit operator QuaternionF(Vector4f value)
		{
			return new QuaternionF(value.W, value.X, value.Y, value.Z);
		}

		public static explicit operator Vector4f(QuaternionF value)
		{
			return new Vector4f(value.X, value.Y, value.Z, value.S);
		}

		public static QuaternionF FromYawPitchRoll(float yaw, float pitch, float roll)
		{    
			
			float sps = (float)System.Math.Sin(yaw / 2);
			float sth = (float)System.Math.Sin(pitch / 2);
			float sph = (float)System.Math.Sin(roll / 2);
			float cps = (float)System.Math.Cos(yaw / 2);
			float cth = (float)System.Math.Cos(pitch / 2);
			float cph = (float)System.Math.Cos(roll / 2);

			return new QuaternionF(cph * cth * cps + sph * sth * sps,
									    sph * cth * cps - cph * sth * sps,
										cph * sth * cps + sph * cth * sps,
										cph * cth * sps - sph * sth * cps);
		}

		public Vector3f YawPitchRollNormalized
		{
			get
			{
				return new Vector3f((float)System.Math.Atan2(2 * (S * Z + X * Y), 1 - 2 * (Y * Y + Z * Z)),
										  (float)System.Math.Asin(2 * (S * Y + Z * X)),
										  (float)System.Math.Atan2(2 * (S * X + Y * Z), 1 - 2 * (X * X + Y * Y)));
			}
		}

		public Vector3f YawPitchRoll
		{
			get
			{
				var tq = this;
				tq.Normalize();
				return tq.YawPitchRollNormalized;
			}
		}

		//TODO: Important: Test. This method is not proved
		public static QuaternionF LookAt(Vector3f direction)
		{
			
			double x = (double)direction.Z / direction.X;
			double y = (double)direction.Z / direction.Y;

			float cth = (float)(1 / (System.Math.Sqrt(x * x + 1) + 1));
			float sth = (float)(x / 2 / System.Math.Sqrt(x * x + 1) / cth);
			float cps = (float)(1 / (System.Math.Sqrt(y * y + 1) + 1));
			float sps = (float)(y / 2 / System.Math.Sqrt(y * y + 1) / cps);

			return new QuaternionF(cth * cps, - sth * sps, sth * cps, cth * sps);
		}
    }


    [Serializable()]
    public partial struct QuaternionR : IEquatable<QuaternionR>
    {
        /// <summary>
        /// The real part of the quaternion.
        /// </summary>
        public double S;
        /// <summary>
        /// The x component of the imaginary part of the quaternion.
        /// </summary>
        public double X;
        /// <summary>
        /// The y component of the imaginary part of the quaternion.
        /// </summary>
        public double Y;
        /// <summary>
        /// The z component of the imaginary part of the quaternion.
        /// </summary>
        public double Z;

        public QuaternionR(double s, Vector3r v)
            : this(s, v.X, v.Y, v.Z)
        { }

        public QuaternionR(double s, double x, double y, double z)
        {
            S = s;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Returns the imaginary part of the quaternion.
        /// </summary>
        public Vector3r V
        {
            get { return new Vector3r(X, Y, Z); }
        }

        public void Invert()
        {
            this = 1 / NormSq() * ~this;
        }

        public double NormSq()
        {
            return S * S + X * X + Y * Y + Z * Z;
        }

        public double Norm()
        {
            return (double)System.Math.Sqrt(NormSq());
        }

		public void Normalize()
		{
			this = this / Norm();
		}

		public double GetAngle()
		{
			return System.Math.Acos(S / (Norm() * 2));
		}

		public double GetAngleNormalized()
		{
			return System.Math.Acos(S / 2);
		}

		public void Transform(Vector3r[] values, int index, int count)
		{
			var p = this;
			p.Normalize();
			p.TransformNormalized(values, index, count);
		}

		public Vector3r Transform(Vector3r value)
		{
			var p = this;
			p.Normalize();
			return (p * new QuaternionR(0, value) * ~p).V;
		}

		public void TransformNormalized(Vector3r[] values, int index, int count)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values));
			Util.ValidateRange(index, count, values.Length);
			
			int d = index + count;
			for(int i = index; i < d; i++)
				values[i] = Transform(values[i]);
		}

		public Vector3r TransformNormalized(Vector3r value)
		{
			return (this * new QuaternionR(0, value) * ~this).V;
		}

        public override bool Equals(object obj)
        {
			if (obj == null)
				return false;

			var q = obj as QuaternionR?;

			if (q == null)
				return false;

			return Equals(q.Value);
        }

        public bool Equals(QuaternionR other)
        {
            return S == other.S && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            return S.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override string ToString()
        {
            return $"({S}, {X}, {Y}, {Z})";
        }

		public static bool operator==(QuaternionR left, QuaternionR right)
		{
			return left.Equals(right);
		}

		public static bool operator!=(QuaternionR left, QuaternionR right)
		{
			return !left.Equals(right);
		}

        public static double Dot(QuaternionR left, QuaternionR right)
        {
            return left.S * right.S + left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static QuaternionR Cross(QuaternionR left, QuaternionR right)
        {
            return new QuaternionR(0, Vector3r.Cross(left.V, right.V));
        }

        public static QuaternionR operator +(QuaternionR left, QuaternionR right)
        {
            return new QuaternionR(left.S + right.S, left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static QuaternionR operator -(QuaternionR left, QuaternionR right)
        {
            return new QuaternionR(left.S - right.S, left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static QuaternionR operator *(QuaternionR left, QuaternionR right)
        {
            return new QuaternionR(left.S * right.S - left.X * right.X - left.Y * right.Y - left.Z * right.Z,
										left.S * right.X + left.X * right.S + left.Y * right.Z + left.Z * right.Y,
										left.S * right.Y + left.X * right.Z + left.Y * right.S + left.Z * right.X,
										left.S * right.Z + left.X * right.Y + left.Y * right.X + left.Z * right.S);
        }

        public static QuaternionR operator *(double left, QuaternionR right)
        {
            return new QuaternionR(left * right.S, left * right.X, left * right.Y, left * right.Z);
        }

        public static QuaternionR operator *(QuaternionR left, double right)
        {
            return new QuaternionR(left.S * right, left.X * right, left.Y * right, left.Z * right);
        }

        public static QuaternionR operator /(QuaternionR left, double right)
        {
            return new QuaternionR(left.S / right, left.X / right, left.Y / right, left.Z / right);
        }

        public static QuaternionR operator ~(QuaternionR value)
        {
            return new QuaternionR(value.S, -value.V);
        }

		public static explicit operator QuaternionR(double value)
		{
			return new QuaternionR(value, 0, 0, 0);
		}

		public static explicit operator QuaternionR(Vector4r value)
		{
			return new QuaternionR(value.W, value.X, value.Y, value.Z);
		}

		public static explicit operator Vector4r(QuaternionR value)
		{
			return new Vector4r(value.X, value.Y, value.Z, value.S);
		}

		public static QuaternionR FromYawPitchRoll(double yaw, double pitch, double roll)
		{    
			
			double sps = System.Math.Sin(yaw / 2);
			double sth = System.Math.Sin(pitch / 2);
			double sph = System.Math.Sin(roll / 2);
			double cps = System.Math.Cos(yaw / 2);
			double cth = System.Math.Cos(pitch / 2);
			double cph = System.Math.Cos(roll / 2);

			return new QuaternionR(cph * cth * cps + sph * sth * sps,
									    sph * cth * cps - cph * sth * sps,
										cph * sth * cps + sph * cth * sps,
										cph * cth * sps - sph * sth * cps);
		}

		public Vector3r YawPitchRollNormalized
		{
			get
			{
				return new Vector3r(System.Math.Atan2(2 * (S * Z + X * Y), 1 - 2 * (Y * Y + Z * Z)),
										  System.Math.Asin(2 * (S * Y + Z * X)),
										  System.Math.Atan2(2 * (S * X + Y * Z), 1 - 2 * (X * X + Y * Y)));
			}
		}

		public Vector3r YawPitchRoll
		{
			get
			{
				var tq = this;
				tq.Normalize();
				return tq.YawPitchRollNormalized;
			}
		}

		//TODO: Important: Test. This method is not proved
		public static QuaternionR LookAt(Vector3r direction)
		{
			
			double x = (double)direction.Z / direction.X;
			double y = (double)direction.Z / direction.Y;

			double cth = (1 / (System.Math.Sqrt(x * x + 1) + 1));
			double sth = (x / 2 / System.Math.Sqrt(x * x + 1) / cth);
			double cps = (1 / (System.Math.Sqrt(y * y + 1) + 1));
			double sps = (y / 2 / System.Math.Sqrt(y * y + 1) / cps);

			return new QuaternionR(cth * cps, - sth * sps, sth * cps, cth * sps);
		}
    }



}