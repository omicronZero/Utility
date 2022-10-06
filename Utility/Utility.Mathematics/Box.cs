using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics
{
    [Serializable]
    public struct Box : IEquatable<Box>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }

        public Box(Vector3r position, Vector3r size)
            : this(position.X, position.Y, position.Z, size.X, size.Y, size.Z)
        { }

        public Box(double x, double y, double z, double width, double height, double depth)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
            Depth = depth;
        }

        public double Right
        {
            get => X + Width;
            set => X = value - Width;
        }

        public double Bottom
        {
            get => Y + Height;
            set => Y = value - Height;
        }

        public double Back
        {
            get => Z + Depth;
            set => Z = value - Depth;
        }

        public Point3r Position
        {
            get => (X, Y, Z);
            set => (X, Y, Z) = value;
        }

        public Vector3r Size
        {
            get => (Width, Height, Depth);
            set => (Width, Height, Depth) = value;
        }

        public Point3r Center
        {
            get => (X + Width / 2, Y + Height / 2, Z + Depth / 2);
            set => (X, Y, Z) = value - Size / 2;
        }

        public Point3r Far
        {
            get { return Position + Size; }
            set { Position = value - Size; }
        }

        public bool Equals(Box other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && Width == other.Width && Height == other.Height && Depth == other.Depth;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var o = obj as Box?;

            if (o == null)
                return false;

            return Equals(o.Value);
        }

        public bool Contains(Vector3r value)
        {
            return X <= value.X && value.X <= Right &&
                   Y <= value.Y && value.Y <= Bottom &&
                   Z <= value.Z && value.Z <= Back;
        }

        public bool Contains(Box other)
        {
            return X <= other.X && Right >= other.Right &&
                   Y <= other.Y && Bottom >= other.Bottom &&
                   Y <= other.Z && Back >= other.Back;
        }

        public bool IntersectsWith(Box other)
        {
            return other.X <= Right && other.Right >= X &&
                   other.Y <= Bottom && other.Bottom >= Y &&
                   other.Z <= Back && other.Back >= Z;
        }

        public double Volume
        {
            get => Width * Height * Depth;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ ~(Width.GetHashCode() ^ Height.GetHashCode() ^ Depth.GetHashCode());
        }

        public override string ToString()
        {
            return $"Position: {Position}, Size: {Size}";
        }

        public Vector3r GetPositionLerp(Vector3r linear)
        {
            return GetPositionLerp(linear.X, linear.Y, linear.Z);
        }

        public Vector3r GetPositionLerp(double linearX, double linearY, double linearZ)
        {
            return (X + linearX * Width, Y + linearY * Height, Z + linearZ * Depth);
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
            if (Depth < 0)
            {
                Z += Depth;
                Depth = -Depth;
            }
        }

        public static Box FromPoints(Point3r corner1, Point3r corner2)
        {
            Vector3r pos = (Math.Min(corner1.X, corner2.X),
                Math.Min(corner1.Y, corner2.Y),
                Math.Min(corner1.Z, corner2.Z));

            return new Box(
                pos,
                (Math.Max(corner1.X, corner2.X) - pos.X,
                Math.Max(corner1.Y, corner2.Y) - pos.Y,
                Math.Max(corner1.Z, corner2.Z) - pos.Z)
                );
        }

        public static bool operator ==(Box left, Box right) => left.Equals(right);
        public static bool operator !=(Box left, Box right) => !(left == right);
    }
}
