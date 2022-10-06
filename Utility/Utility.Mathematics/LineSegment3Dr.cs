using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Mathematics
{
    [Serializable()]
    public struct LineSegment3Dr
    {
        public Point3r StartPoint;
        public Point3r EndPoint;

        public Ray3Dr Line
        {
            get { return new Ray3Dr(StartPoint, EndPoint - StartPoint); }
        }

        public LineSegment3Dr(Point3r startPoint, Point3r endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public static LineSegment3Dr FromLine(Ray3Dr line, double first, double second)
        {
            return new LineSegment3Dr(line[first], line[second]);
        }

        public static LineSegment3Dr Relative(Point3r startPoint, Vector3r endPointOffset)
        {
            return new LineSegment3Dr(startPoint, startPoint + endPointOffset);
        }

        public static LineSegment3Dr Relative(Point3r startPoint, Vector3r endPointOffset, double offsetFactor)
        {
            return new LineSegment3Dr(startPoint, startPoint + offsetFactor * endPointOffset);
        }
    }
}
