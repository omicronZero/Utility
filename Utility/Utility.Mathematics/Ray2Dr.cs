using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mathematics
{
    //TODO: test
    [Serializable()]
    public struct Ray2Dr : IEquatable<Ray2Dr>
    {
        public static readonly Ray2Dr XAxis = new Ray2Dr(Point2r.Zero, new Vector2r(1, 0));
        public static readonly Ray2Dr YAxis = new Ray2Dr(Point2r.Zero, new Vector2r(0, 1));

        public Point2r SuspensionPoint { get; set; }
        public Vector2r Direction { get; set; }

        public Ray2Dr(Point2r suspensionPoint, Vector2r direction)
        {
            SuspensionPoint = suspensionPoint;
            Direction = direction;
        }

        public bool Equals(Ray2Dr other)
        {
            return SuspensionPoint == other.SuspensionPoint && Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var o = obj as Ray2Dr?;

            if (o == null)
                return false;

            return Equals(o.Value);
        }

        public double Intersect(Ray2Dr other)
        {
            return (other.Direction.Y * (other.SuspensionPoint.X - SuspensionPoint.X) + other.Direction.X * (other.SuspensionPoint.Y - SuspensionPoint.Y)) / (other.Direction.Y * Direction.X - Direction.Y * other.Direction.X);
        }

        public double Intersect(Ray2Dr other, out Vector2r position)
        {
            double offset = Intersect(other);

            position = this[offset];

            return offset;
        }

        public override int GetHashCode()
        {
            return SuspensionPoint.GetHashCode() ^ ~Direction.GetHashCode();
        }

        public double ProjectOrthogonalOffset(Point2r element)
        {
            return (element - SuspensionPoint).Dot(Direction) / Direction.LengthSq();
        }

        public Point2r Project(Point2r value, Vector2r direction)
        {
            return SuspensionPoint + Direction *
                ((value.Y - SuspensionPoint.Y) * direction.X - (value.X - SuspensionPoint.X) * direction.Y) /
                (direction.X * Direction.Y - direction.Y * direction.X);
        }

        public Point2r ProjectOrthogonal(Point2r value)
        {
            return SuspensionPoint + Direction * (value - SuspensionPoint).Dot(Direction) / Direction.LengthSq();
        }

        public Point2r this[double offset]
        {
            get { return SuspensionPoint + Direction * offset; }
        }

        public static bool operator ==(Ray2Dr left, Ray2Dr right)
        {
            return left.SuspensionPoint == right.SuspensionPoint && left.Direction == right.Direction;
        }

        public static bool operator !=(Ray2Dr left, Ray2Dr right)
        {
            return left.SuspensionPoint != right.SuspensionPoint || left.Direction != right.Direction;
        }
    }
}
