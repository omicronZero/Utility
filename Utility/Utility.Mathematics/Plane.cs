
using System;


namespace Utility.Mathematics
{
	[Serializable]
	public struct PlaneF : IEquatable<PlaneF>
	{
		public float N1, N2, N3, D;

		//TODO: test
		public PlaneF(Vector3f n, float d)
			: this(n.X, n.Y, n.Z, d)
		{ }

		public PlaneF(float n1, float n2, float n3, float d)
		{
			N1 = n1;
			N2 = n2;
			N3 = n3;
			D = d;
		}

		public Vector3f ProjectNormal(Vector3f value)
		{
			return value + Normal * SignedDistanceTo(value);
		}

		public float NormalizedSignedDistanceTo(Vector3f value)
		{
			float dist = 1 / (float)Math.Sqrt(N1 * N1 + N2 * N2 + N3 * N3);

			return SignedDistanceTo(value) * dist;
		}

		public float NormalizedDistanceTo(Vector3f value)
		{
			return Math.Abs(NormalizedSignedDistanceTo(value));
		}

		public float DistanceTo(Vector3f value)
		{
			return Math.Abs(SignedDistanceTo(value));
		}

		public float SignedDistanceTo(Vector3f value)
		{
			return value.X * N1 + value.Y * N2 + value.Z * N3 + D;
		}

		public override string ToString()
		{
			return $"N1 = { N1 }, N2 = { N2 }, N3 = { N3 }, D = { D }";
		}

		public Vector3f Normal
		{
			get { return new Vector3f(N1, N2, N3); }
		}

		public void Normalize()
		{
			float linv = Math.Sign(D) / (float)Math.Sqrt(N1 * N1 + N2 * N2 + N3 * N3);

			N1 *= linv;
			N2 *= linv;
			N3 *= linv;
			D *= linv;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as PlaneF?;

			if (p == null)
				return false;

			return Equals(p.Value);
		}

		public bool Equals(PlaneF other)
		{
			return N1 == other.N1 && N2 == other.N2 && N3 == other.N3 && D == other.D;
		}

		public override int GetHashCode()
		{
			return N1.GetHashCode() ^ N2.GetHashCode() ^ N3.GetHashCode() ^ D.GetHashCode();
		}

		public static PlaneF FromVectors(Vector3f v1, Vector3f v2, Vector3f v3)
		{
			Vector3f n = Vector3f.Cross(v2 - v1, v3 - v1);

			return new PlaneF(n, -n.Dot(v1));
		}

		public static PlaneF FromDirections(Vector3f v, Vector3f d1, Vector3f d2)
		{
			Vector3f n = Vector3f.Cross(d1, d2);

			return new PlaneF(n, -n.Dot(v));
		}

		public static bool operator==(PlaneF left, PlaneF right)
		{
			return left.N1 == right.N1 && left.N2 == right.N2 && left.N3 == right.N3 && left.D == right.D;
		}

		public static bool operator!=(PlaneF left, PlaneF right)
		{
			return left.N1 != right.N1 || left.N2 != right.N2 || left.N3 != right.N3 || left.D != right.D;
		}
	}
}


namespace Utility.Mathematics
{
	[Serializable]
	public struct PlaneR : IEquatable<PlaneR>
	{
		public double N1, N2, N3, D;

		//TODO: test
		public PlaneR(Vector3r n, double d)
			: this(n.X, n.Y, n.Z, d)
		{ }

		public PlaneR(double n1, double n2, double n3, double d)
		{
			N1 = n1;
			N2 = n2;
			N3 = n3;
			D = d;
		}

		public Vector3r ProjectNormal(Vector3r value)
		{
			return value + Normal * SignedDistanceTo(value);
		}

		public double NormalizedSignedDistanceTo(Vector3r value)
		{
			double dist = 1 / Math.Sqrt(N1 * N1 + N2 * N2 + N3 * N3);

			return SignedDistanceTo(value) * dist;
		}

		public double NormalizedDistanceTo(Vector3r value)
		{
			return Math.Abs(NormalizedSignedDistanceTo(value));
		}

		public double DistanceTo(Vector3r value)
		{
			return Math.Abs(SignedDistanceTo(value));
		}

		public double SignedDistanceTo(Vector3r value)
		{
			return value.X * N1 + value.Y * N2 + value.Z * N3 + D;
		}

		public override string ToString()
		{
			return $"N1 = { N1 }, N2 = { N2 }, N3 = { N3 }, D = { D }";
		}

		public Vector3r Normal
		{
			get { return new Vector3r(N1, N2, N3); }
		}

		public void Normalize()
		{
			double linv = Math.Sign(D) / Math.Sqrt(N1 * N1 + N2 * N2 + N3 * N3);

			N1 *= linv;
			N2 *= linv;
			N3 *= linv;
			D *= linv;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var p = obj as PlaneR?;

			if (p == null)
				return false;

			return Equals(p.Value);
		}

		public bool Equals(PlaneR other)
		{
			return N1 == other.N1 && N2 == other.N2 && N3 == other.N3 && D == other.D;
		}

		public override int GetHashCode()
		{
			return N1.GetHashCode() ^ N2.GetHashCode() ^ N3.GetHashCode() ^ D.GetHashCode();
		}

		public static PlaneR FromVectors(Vector3r v1, Vector3r v2, Vector3r v3)
		{
			Vector3r n = Vector3r.Cross(v2 - v1, v3 - v1);

			return new PlaneR(n, -n.Dot(v1));
		}

		public static PlaneR FromDirections(Vector3r v, Vector3r d1, Vector3r d2)
		{
			Vector3r n = Vector3r.Cross(d1, d2);

			return new PlaneR(n, -n.Dot(v));
		}

		public static bool operator==(PlaneR left, PlaneR right)
		{
			return left.N1 == right.N1 && left.N2 == right.N2 && left.N3 == right.N3 && left.D == right.D;
		}

		public static bool operator!=(PlaneR left, PlaneR right)
		{
			return left.N1 != right.N1 || left.N2 != right.N2 || left.N3 != right.N3 || left.D != right.D;
		}
	}
}

