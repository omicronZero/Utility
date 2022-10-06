using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics
{
    [Serializable]
    public struct Square
    {
        public static Square Zero => default;

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }

        public Square(Point2r position, double size)
            : this(position.X, position.Y, size)
        { }

        public Square(double x, double y, double width)
        {
            Left = x;
            Top = y;
            Width = width;
        }

        public double Right => Left + Width;
        public double Bottom => Top + Width;

        public Point2r Position
        {
            get => (Left, Top);
            set => (Left, Top) = value;
        }

        public Point2r Far => (Left + Width, Top + Width);

        public double Area
        {
            get => Width * Width;
            set => Width = Math.Sqrt(value);
        }

        public void Normalize()
        {
            if (Width < 0)
            {
                Left += Width;
                Top += Width;
                Width = -Width;
            }
        }

        public bool Contains(Point2r point)
        {
            return point.X >= Left 
                && point.Y >= Top 
                && point.X < Left + Width 
                && point.Y < Top + Width;
        }

        public bool Contains(Square other)
        {
            return other.Left >= Left
                && other.Top >= Top
                && other.Left + other.Width < Left + Width
                && other.Top + other.Width < Top + Width;
        }

        public bool IntersectsWith(Square other)
        {
            return other.Left + other.Width >= Left
                && other.Top + other.Width >= Top
                && other.Left < Left + Width
                && other.Top < Top + Width;
        }

        public void Union(Point2r point)
        {
            if (Width == 0)
            {
                Position = point;
                Width = double.Epsilon;
            }
            else
            {
                if (point.X < Left)
                {
                    Width += point.X - Left;
                    Left = point.X;
                }
                if (Right < point.X)
                {
                    Width = point.X - Left;
                }
            }
        }

        public static implicit operator RectangleR(Square square)
        {
            return new RectangleR(square.Position, (square.Width, square.Width));
        }
    }
}
