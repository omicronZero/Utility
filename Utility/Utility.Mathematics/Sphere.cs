using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mathematics
{
    [Serializable]
    public struct Sphere : IEquatable<Sphere>
    {
        public static Sphere Zero { get; } = default(Sphere);

        public Vector3r Position { get; set; }
        public double Radius { get; set; }

        public Sphere(Vector3r position, double radius)
        {
            Position = position;
            Radius = radius;
        }

        public bool IsEmpty
        {
            get { return Radius == 0; }
        }

        public bool Contains(Vector3r position)
        {
            return (Position - position).LengthSq() <= Radius * Radius;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineSuspensionPoint">The suspension point of the line.</param>
        /// <param name="lineDirection">The direction of the line.</param>
        /// <param name="distance1">The offset of the point of intersection to the suspension point, if there was a intersection. <see cref="Vector3r.Zero"/> otherwise.</param>
        /// <param name="distance2">The offset of the point of intersection to the suspension point, if there was a intersection. <see cref="Vector3r.Zero"/> otherwise.</param>
        /// <returns></returns>
        public bool Intersect(Point3r lineSuspensionPoint, Vector3r lineDirection, out double distance1, out double distance2)
        {
            Vector3r r = lineSuspensionPoint - Position;
            double p = r.Dot(lineDirection);
            double disc = p * p - lineDirection.LengthSq() * r.LengthSq() - Radius * Radius;

            if (disc < 0)
            {
                distance1 = 0;
                distance2 = 0;
                return false;
            }

            double d = Math.Sqrt(disc) / lineDirection.LengthSq();

            distance1 = d - lineDirection.Dot(r);
            distance2 = -d - lineDirection.Dot(r);

            return true;
        }

        public bool Intersect(Vector3r lineSuspensionPoint, Vector3r lineDirection)
        {
            Vector3r r = lineSuspensionPoint - Position;
            double p = r.Dot(lineDirection);
            double disc = p * p - lineDirection.LengthSq() * r.LengthSq() - Radius * Radius;

            return disc >= 0;
        }

        public bool Intersect(Ray3Dr line, out double distance1, out double distance2)
        {
            return Intersect(line.SuspensionPoint, line.Direction, out distance1, out distance2);
        }

        public bool Intersect(Ray3Dr line)
        {
            return Intersect(line.SuspensionPoint, line.Direction);
        }

        public override string ToString()
        {
            return $"Position: { Position.ToString() }, Radius: { Radius }";
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Radius.GetHashCode();
        }

        public void Normalize()
        {
            if (Radius < 0)
                Radius = -Radius;
        }

        public bool IntersectsWith(Sphere sphere)
        {
            return (sphere.Position - Position).LengthSq() <= MathUtil.Pow2(sphere.Radius + Radius);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Sphere s && Equals(s);
        }

        public bool Equals(Sphere other)
        {
            return Position == other.Position && Radius == other.Radius;
        }

        public static Sphere GetBoundingSphere(IEnumerable<Point3r> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            using (IEnumerator<Point3r> enr = points.GetEnumerator())
            {
                if (!enr.MoveNext())
                    return Sphere.Zero;

                Point3r v = enr.Current;
                double rsq = 0;

                while (enr.MoveNext())
                {
                    double crsq = (enr.Current - v).LengthSq();

                    if (crsq > rsq) //point lies outside of the currently computed sphere
                    {
                        v = (v + (enr.Current - v) / 2); //set sphere position to the middle of the two points
                        rsq = crsq; //set squared radius of the sphere to the squared distance of the two points
                    }
                }

                return new Sphere(v, Math.Sqrt(rsq));
            }
        }

        public static bool operator ==(Sphere left, Sphere right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Sphere left, Sphere right)
        {
            return !(left == right);
        }
    }
}
