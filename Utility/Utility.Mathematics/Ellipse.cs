using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mathematics
{
    [Serializable()]
    public struct Ellipse
    {
        public Vector2r Position { get; set; }
        public Vector2r Orientation { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Ellipse(Vector2r position, Vector2r direction, Vector2r size)
            : this(position, direction, size.X, size.Y)
        { }

        public Ellipse(Vector2r position, Vector2r orientation, double width, double height)
        {
            Position = position;
            Orientation = orientation;
            Width = width;
            Height = height;
        }

        public Vector2r Span => (Width, Height);

        public double Eccentricity => Math.Sqrt(1 - MathUtil.Pow2(Height / Width));

        /* the ellipse is a transformed unit circle (position = (0, 0), radius = 1)
         * by transforming the circle's points by
         *      width 0
         *      0   height
         * then rotating it by the rotation defined by direction (direction = (c, s) = (cos alpha, sin alpha)
         *      c   -s
         *      s   c
         * and translating it by position
         */
        public Matrix2x2r GetTransformFromUnitCircle()
        {
            //R * S
            return new Matrix2x2r(0.5 * Orientation.X * Width, 0.5 * Orientation.Y * Height,
                                  -0.5 * Orientation.Y * Width, 0.5 * Orientation.X * Height);
        }

        public Matrix2x2r GetTransformToUnitCircle()
        {
            //inverse of (R * S)
            return new Matrix2x2r(2 * Orientation.X / Width, -2 * Orientation.Y / Width,
                                  2 * Orientation.Y / Height, 2 * Orientation.X / Height);
        }

        //WARNING: test
        public bool Intersects(Ray2Dr ray, out double offset1, out double offset2)
        {
            Matrix2x2r m = GetTransformToUnitCircle();
            ray = new Ray2Dr(m.Transform(ray.SuspensionPoint - Position), m.Transform(ray.Direction));


            return Circle.UnitCircle.Intersect(ray, out offset1, out offset2);
        }

        public static Ellipse FromAngle(Vector2r position, Vector2r span, double angle)
        {
            return new Ellipse(position, MathUtil.CosSin(angle * MathUtil.AngleToRad), span);
        }

        public double Area
        {
            get { return Height * Width * Math.PI * 0.25; }
        }

        public static implicit operator Ellipse(Circle circle)
        {
            return new Ellipse(circle.Position, Vector2r.AxisX, circle.Radius, circle.Radius);
        }
    }
}
