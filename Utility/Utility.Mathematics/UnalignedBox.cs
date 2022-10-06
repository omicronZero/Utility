using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics
{
    [Serializable]
    public struct UnalignedBox : IEquatable<UnalignedBox>
    {
        public Point3r Center { get; set; }
        public Vector3r Up { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }

        public UnalignedBox(Point3r center, Vector3r up, Vector3r size)
            : this(center, up, size.X, size.Y, size.Z)
        { }

        public UnalignedBox(Point3r center, Vector3r up, double width, double height, double depth)
        {
            Center = center;
            Up = up;
            Width = width;
            Height = height;
            Depth = depth;
        }

        public double Volume => Width * Height * Depth;

        public Vector3r Size => (Width, Height, Depth);

        public override bool Equals(object obj)
        {
            return obj != null && obj is UnalignedBox b && Equals(b);
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode() ^ Up.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode() ^ Depth.GetHashCode();
        }

        public override string ToString()
        {
            return $"Center: {Center}, Up: {Up}, Width: {Width}, Height: {Height}, Depth: {Depth}";
        }

        public bool Equals(UnalignedBox other)
        {
            return Center == other.Center && Up == other.Up && Size == other.Size;
        }

        public static bool operator ==(UnalignedBox left, UnalignedBox right)
        {
            return left.Center == right.Center && left.Up == right.Up && left.Size == right.Size;
        }

        public static bool operator !=(UnalignedBox left, UnalignedBox right)
        {
            return left.Center != right.Center || left.Up != right.Up || left.Size != right.Size;
        }

        public static implicit operator UnalignedBox(Box box)
        {
            return new UnalignedBox(box.Center, Vector3r.AxisY, box.Size);
        }
    }
}
