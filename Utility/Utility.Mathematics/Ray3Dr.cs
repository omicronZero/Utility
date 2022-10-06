using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mathematics
{
    //TODO: test
    [Serializable()]
    public struct Ray3Dr : IEquatable<Ray3Dr>
    {
        public static readonly Ray3Dr XAxis = new Ray3Dr(Point3r.Zero, new Vector3r(1, 0, 0));
        public static readonly Ray3Dr YAxis = new Ray3Dr(Point3r.Zero, new Vector3r(0, 1, 0));
        public static readonly Ray3Dr ZAxis = new Ray3Dr(Point3r.Zero, new Vector3r(0, 0, 1));

        public Point3r SuspensionPoint { get; set; }
        public Vector3r Direction { get; set; }

        public Ray3Dr(Point3r suspensionPoint, Vector3r direction)
        {
            SuspensionPoint = suspensionPoint;
            Direction = direction;
        }

        public bool Equals(Ray3Dr other)
        {
            return SuspensionPoint == other.SuspensionPoint && Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var o = obj as Ray3Dr?;

            if (o == null)
                return false;

            return Equals(o.Value);
        }

        public override int GetHashCode()
        {
            return SuspensionPoint.GetHashCode() ^ ~Direction.GetHashCode();
        }

        public bool IsElement(Vector3r value)
        {
            return (value.X - SuspensionPoint.X) * Direction.Y == (value.Y - SuspensionPoint.Y) * Direction.X;
        }

        public double ProjectOrthogonalOffset(Point3r value)
        {
            return (value - SuspensionPoint).Dot(Direction) / Direction.LengthSq();
        }

        public Point3r Project(Point3r value, Vector3r direction)
        {
            return SuspensionPoint + Direction *
                ((value.Y - SuspensionPoint.Y) * direction.X - (value.X - SuspensionPoint.X) * direction.Y) /
                (direction.X * Direction.Y - direction.Y * direction.X);
        }

        public double GetNearestOffset(Ray3Dr other)
        {
            return GetNearestOffset(other, out _);
        }

        public double GetNearestOffset(Ray3Dr other, out double otherOffset)
        {
            Vector3r r = other.SuspensionPoint - SuspensionPoint;

            Vector3r d1 = Direction;
            Vector3r d2 = other.Direction;
            double d1d2 = d1.Dot(d2);

            double d1sq = d1.LengthSq();
            double d2sq = d2.LengthSq();

            double rd1 = r.Dot(d1);
            double rd2 = r.Dot(d2);

            otherOffset = (d1d2 * rd1 - rd2 * d1sq) / (d1d2 + d1sq * d2sq);
            return -(otherOffset * d2sq + rd2) / d1d2;
        }

        public Point3r GetNearestPoint(Ray3Dr other)
        {
            return this[GetNearestOffset(other)];
        }

        public Point3r GetNearestPoint(Ray3Dr other, out double otherOffset)
        {
            return this[GetNearestOffset(other, out otherOffset)];
        }

        public Point3r GetNearestPoint(Ray3Dr other, out Point3r otherPoint)
        {
            double  otheroffs;

            double offs = GetNearestOffset(other, out otheroffs);

            otherPoint = other[otheroffs];

            return this[offs];
        }

        public double GetDistance(Ray3Dr other)
        {
            return Math.Sqrt(GetDistanceSq(other));
        }

        public double GetDistanceSq(Ray3Dr other)
        {
            //TODO: find other solution

            Point3r r;

            Point3r l = GetNearestPoint(other, out r);

            return (l - r).LengthSq();
        }

        public Point3r ProjectOrthogonal(Point3r value)
        {
            return SuspensionPoint + Direction * (value - SuspensionPoint).Dot(Direction) / Direction.LengthSq();
        }

        public Point3r this[double offset]
        {
            get { return SuspensionPoint + Direction * offset; }
        }

        public static bool operator ==(Ray3Dr left, Ray3Dr right)
        {
            return left.SuspensionPoint == right.SuspensionPoint && left.Direction == right.Direction;
        }

        public static bool operator !=(Ray3Dr left, Ray3Dr right)
        {
            return left.SuspensionPoint != right.SuspensionPoint || left.Direction != right.Direction;
        }
    }
}
