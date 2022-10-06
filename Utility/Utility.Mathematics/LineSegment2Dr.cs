using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Mathematics
{
    [Serializable()]
    public struct LineSegment2Dr
    {
        public Point2r StartPoint;
        public Point2r EndPoint;
        
        public LineSegment2Dr(Point2r startPoint, Point2r endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Ray2Dr Line
        {
            get { return new Ray2Dr(StartPoint, EndPoint - StartPoint); }
        }

        public bool Intersect(Ray2Dr other, out double offset)
        {
            offset = Line.Intersect(other);

            return offset >= 0 && offset <= 1;
        }

        public bool Intersect(Ray2Dr other, out double offset, out Vector2r position)
        {
            bool v = Intersect(other, out offset);

            position = StartPoint + (EndPoint - StartPoint) * offset;

            return v;
        }

        public static LineSegment2Dr FromLine(Ray2Dr line, double first, double second)
        {
            return new LineSegment2Dr(line[first], line[second]);
        }

        public static LineSegment2Dr Project(Ray2Dr line, LineSegment2Dr segment, Vector2r direction)
        {
            return new LineSegment2Dr(
                    line.Project(segment.StartPoint, direction), 
                    line.Project(segment.EndPoint, direction)
                );
        }

        public static LineSegment2Dr ProjectOrthogonal(Ray2Dr line, LineSegment2Dr segment)
        {
            return new LineSegment2Dr(
                    line.ProjectOrthogonal(segment.StartPoint),
                    line.ProjectOrthogonal(segment.EndPoint)
                );
        }

        public static LineSegment2Dr Relative(Point2r startPoint, Vector2r endPointOffset)
        {
            return new LineSegment2Dr(startPoint, startPoint + endPointOffset);
        }

        public static LineSegment2Dr Relative(Point2r startPoint, Vector2r endPointOffset, double offsetFactor)
        {
            return new LineSegment2Dr(startPoint, startPoint + offsetFactor * endPointOffset);
        }
    }
}
