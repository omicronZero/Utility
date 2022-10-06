using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics
{
    [Serializable]
    public struct RectangleR : IEquatable<RectangleR>
    {
        public static RectangleR Zero => new RectangleR(0, 0, 0, 0);

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public RectangleR(Point2r position, Vector2r size)
            : this(position.X, position.Y, size.X, size.Y)
        { }

        public RectangleR(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Point2r Position => (X, Y);
        public Vector2r Size => (Width, Height);
        public double Right => X + Width;
        public double Bottom => Y + Height;
        public Point2r Far => (Right, Bottom);
        public Point2r Center => (X + Width / 2, Y + Height / 2);

        public bool Contains(Point2r value)
        {
            return value.X >= X && value.X <= X + Width &&
                   value.Y >= Y && value.Y <= Y + Height;
        }

        public bool Contains(RectangleR other)
        {
            return X <= other.X && Right >= other.Right &&
                   Y <= other.Y && Bottom >= other.Bottom;
        }

        public bool IntersectsWith(RectangleR other)
        {
            return other.X <= Right && other.Right >= X &&
                   other.Y <= Bottom && other.Bottom >= Y;
        }

        public void Intersect(RectangleR other)
        {
            this = FromPoints(Math.Max(X, other.X), Math.Max(Y, other.Y), Math.Min(Right, other.Right), Math.Min(Bottom, other.Bottom));
        }

        public void Union(Point2r point)
        {
            if (point.X < X)
                X = point.X;
            else if (point.X > Right)
                Width = point.X - X;

            if (point.Y < Y)
                Y = point.Y;
            else if (point.Y > Bottom)
                Height = point.Y - Y;
        }

        public bool Intersects(Ray2Dr line, out double offset1, out double offset2)
        {
            double temp = line[X - line.SuspensionPoint.X].Y; // check point of collision for left line
            int count = 0;

            offset1 = 0;
            offset2 = 0;

            if (temp >= Y && temp < Bottom)
            {
                count++;
                offset1 = temp;
            }

            temp = line[Right - line.SuspensionPoint.X].Y; // right

            if (temp >= Y && temp < Bottom)
            {
                if (count == 0)
                    offset1 = temp;
                else
                {
                    offset2 = temp;
                    return true;
                }
            }

            temp = line[Y - line.SuspensionPoint.Y].X; // top

            if (temp >= X && temp < Bottom)
            {
                if (count == 0)
                    offset1 = temp;
                else
                {
                    offset2 = temp;
                    return true;
                }
            }

            temp = line[Bottom - line.SuspensionPoint.Y].X; // bottom

            if (temp >= X && temp < Bottom)
            {
                if (count == 0)
                    offset1 = temp;
                else
                {
                    offset2 = temp;
                    return true;
                }
            }

            return count > 0; //should be false for any input
        }

        public bool Intersects(Ray2Dr line, out Point2r point1, out Point2r point2)
        {
            double o1, o2;

            if (Intersects(line, out o1, out o2))
            {
                point1 = line[o1];
                point2 = line[o2];
                return true;
            }
            else
            {
                point1 = default;
                point2 = default;
                return false;
            }
        }

        public bool Intersects(Ray2Dr line)
        {
            double temp = line[X - line.SuspensionPoint.X].Y;

            if (temp >= Y && temp < Bottom)
                return true;

            temp = line[Right - line.SuspensionPoint.X].Y;

            if (temp >= Y && temp < Bottom)
                return true;

            temp = line[Y - line.SuspensionPoint.Y].X;

            if (temp >= X && temp < Bottom)
                return true;

            temp = line[Bottom - line.SuspensionPoint.Y].X;

            if (temp >= X && temp < Bottom)
                return true;

            return false;
        }

        public void Union(RectangleR other)
        {
            this = FromPoints(Math.Min(other.X, X), Math.Min(other.Y, Y), Math.Max(other.X, X), Math.Max(other.Y, Y));
        }

        public void Normalize()
        {
            if (Width < 0)
            {
                X += Width;
                Width = -Width;
            }

            if (Height < 0)
            {
                Y += Height;
                Height = -Height;
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is RectangleR other && Equals(other);
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ ~Y.GetHashCode() ^ Width.GetHashCode() ^ ~Height.GetHashCode();
        }

        public bool Equals(RectangleR other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public bool IsEmpty => Width * Height == 0;

        public static RectangleR FromPoints(Vector2r point1, Vector2r point2)
        {
            return FromPoints(point1.X, point1.Y, point2.X, point2.Y);
        }

        public static RectangleR FromPoints(double x1, double y1, double x2, double y2)
        {
            double x = Math.Min(x1, x2);
            double y = Math.Min(y1, y2);

            return new RectangleR(x, y, Math.Max(x1, x2) - x, Math.Max(y1, y2) - y);
        }

        public static RectangleR GetBoundingRectangle(params Vector2r[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            return GetBoundingRectangle((IEnumerable<Vector2r>)points);
        }

        public static RectangleR GetBoundingRectangle(IEnumerable<Vector2r> pointsEnumerable)
        {
            if (pointsEnumerable == null)
                throw new ArgumentNullException(nameof(pointsEnumerable));

            double minX;
            double minY;
            double maxX;
            double maxY;

            using (var enr = pointsEnumerable.GetEnumerator())
            {
                if (!enr.MoveNext())
                    return RectangleR.Zero;

                (minX, minY) = (maxX, maxY) = enr.Current;

                while (enr.MoveNext())
                {
                    Vector2r current = enr.Current;

                    if (current.X < minX)
                        minX = current.X;
                    else if (current.X > maxX)
                        maxX = current.X;

                    if (current.Y < minY)
                        minY = current.Y;
                    else if (current.Y > maxY)
                        maxY = current.Y;
                }

                return new RectangleR(minX, minY, maxX - minX, maxY - maxY);
            }
        }

        public static bool operator ==(RectangleR left, RectangleR right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RectangleR left, RectangleR right)
        {
            return !(left == right);
        }
    }
}
