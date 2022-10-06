using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics
{
    [Serializable]
    public struct Circle : IEquatable<Circle>
    {
        public static Circle Zero => new Circle(Point2r.Zero, 0);
        public static Circle UnitCircle => new Circle(Point2r.Zero, 1);

        public Point2r Position { get; set; }
        public double Radius { get; set; }

        public Circle(Point2r position, double radius)
        {
            Position = position;
            Radius = radius;
        }

        public double X => Position.X;
        public double Y => Position.Y;

        public bool Contains(Point2r point)
        {
            return (point - Position).LengthSq() <= Radius * Radius;
        }

        public bool IntersectsWith(Ray2Dr ray)
        {
            Vector2r relative = ray.SuspensionPoint - Position;

            double a = ray.Direction.LengthSq();
            double b = 2 * ray.Direction.Dot(relative);
            double c = relative.LengthSq();

            return MathUtil.SolveQuadratic(a, b, c);
        }

        public bool Intersect(Point2r raySuspensionPoint, Vector2r rayDirection, out double offset1, out double offset2)
        {
            return Intersect(new Ray2Dr(raySuspensionPoint, rayDirection), out offset1, out offset2);
        }

        public bool IntersectsWith(Circle other)
        {
            return (other.Position - Position).LengthSq() <= MathUtil.Pow2(Radius + other.Radius);
        }

        public bool Intersect(Ray2Dr ray, out double offset1, out double offset2)
        {
            Vector2r relative = ray.SuspensionPoint - Position;

            double a = ray.Direction.LengthSq();
            double b = 2 * ray.Direction.Dot(relative);
            double c = relative.LengthSq();

            return MathUtil.SolveQuadratic(a, b, c, out offset1, out offset2);
        }

        public override string ToString()
        {
            return $"Position: {Position}, Radius: {Radius}";
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Circle c && Equals(c);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Radius.GetHashCode();
        }

        public bool Equals(Circle other)
        {
            return Position == other.Position && Radius == other.Radius;
        }

        public static bool operator ==(Circle left, Circle right)
        {
            return left.Position == right.Position && left.Radius == right.Radius;
        }

        public static bool operator !=(Circle left, Circle right)
        {
            return left.Position != right.Position || left.Radius != right.Radius;
        }

        public static Circle GetBoundingSphere(IEnumerable<Point2r> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            using (IEnumerator<Point2r> enr = points.GetEnumerator())
            {
                if (!enr.MoveNext())
                    return Circle.Zero;

                Point2r v = enr.Current;
                double rsq = 0;

                while (enr.MoveNext())
                {
                    double crsq = (enr.Current - v).LengthSq();

                    if (crsq > rsq) //point lies outside of the currently computed ellipse
                    {
                        v = (v + (enr.Current - v) / 2); //set ellipse position to the middle of the two points
                        rsq = crsq; //set squared radius of the ellipse to the squared distance of the two points
                    }
                }

                return new Circle(v, Math.Sqrt(rsq));
            }
        }
    }
}
