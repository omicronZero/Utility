using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mathematics
{
    [Serializable()]
    public struct ObjectOrientedRectangle
    {
        public Vector2r Position { get; set; }
        public Vector2r Orientation { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double Area
        {
            get { return 4 * Width * Height; }
        }

        public Matrix2x2r GetTransformFromUnitSquare()
        {
            //R * S
            return new Matrix2x2r(0.5 * Orientation.X * Width, 0.5 * Orientation.Y * Height,
                                  -0.5 * Orientation.Y * Width, 0.5 * Orientation.X * Height);
        }

        public Matrix2x2r GetTransformToUnitSquare()
        {
            //inverse of (R * S)
            return new Matrix2x2r(2 * Orientation.X / Width, -2 * Orientation.Y / Width,
                                  2 * Orientation.Y / Height, 2 * Orientation.X / Height);
        }

        public void Translate(double x, double y)
        {
            Translate(new Vector2r(x, y));
        }

        public void Translate(Vector2r value)
        {
            Position += value;
        }

        public bool Contains(Vector2r value)
        {
            Vector2r v = Orientation;

            if (v.LengthSq() == 0)
                return false;

            value -= Position;

            if (!GetProjectedDistanceSquare(value, v, Width))
                return false;

            v = (v.Y, -v.X);

            return GetProjectedDistanceSquare(value, v, Height);
        }

        public bool Intersect(Ray2Dr ray, out Point2r point1, out Point2r point2)
        {
            Matrix2x2r transform = GetTransformToUnitSquare();

            ray.SuspensionPoint = transform.Transform(ray.SuspensionPoint - Position);
            ray.Direction = transform.Transform(ray.Direction);

            return new RectangleR(Point2r.Zero, new Vector2r(1, 1)).Intersects(ray, out point1, out point2);
        }

        public bool Intersect(Ray2Dr ray, double offset1, out double offset2)
        {
            Matrix2x2r transform = GetTransformToUnitSquare();

            ray.SuspensionPoint = transform.Transform(ray.SuspensionPoint - Position);
            ray.Direction = transform.Transform(ray.Direction);

            return new RectangleR(Point2r.Zero, new Vector2r(1, 1)).Intersects(ray, out offset1, out offset2);
        }

        public bool Intersect(Ray2Dr ray)
        {
            Matrix2x2r transform = GetTransformToUnitSquare();

            ray.SuspensionPoint = transform.Transform(ray.SuspensionPoint - Position);
            ray.Direction = transform.Transform(ray.Direction);

            return new RectangleR(Point2r.Zero, new Vector2r(1, 1)).Intersects(ray);
        }

        private static bool GetProjectedDistanceSquare(Vector2r value, Vector2r orientation, double span, out double offset)
        {
            offset = value.Dot(orientation);
            return offset > 0 && offset < span;
        }

        private static bool GetProjectedDistanceSquare(Vector2r value, Vector2r orientation, double span)
        {
            return GetProjectedDistanceSquare(value, orientation, span, out _);
        }
    }
}
