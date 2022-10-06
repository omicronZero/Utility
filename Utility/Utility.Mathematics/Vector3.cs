using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics
{
    partial struct Vector3f
    {
        public static Vector3f Cross(Vector3f left, Vector3f right)
        {
            return new Vector3f(left.Y * right.Z - left.Z * right.Y,
                                left.Z * right.X - left.X * right.Z,
                                left.X * right.Y - left.Y * right.X);
        }
    }

    partial struct Vector3r
    {
        public static Vector3r Cross(Vector3r left, Vector3r right)
        {
            return new Vector3r(left.Y * right.Z - left.Z * right.Y,
                                left.Z * right.X - left.X * right.Z,
                                left.X * right.Y - left.Y * right.X);
        }
    }
}
