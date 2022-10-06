using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics
{
    partial struct Matrix3x3f
    {
        public static Matrix3x3f Rotation(QuaternionF quaternion)
        {
            return new Matrix3x3f(
                1 - 2 * (quaternion.X * quaternion.X + quaternion.Z * quaternion.Z), 2 * (quaternion.X * quaternion.Y - quaternion.S * quaternion.Z), 2 * (quaternion.X * quaternion.Z + quaternion.S * quaternion.Y),
                2 * (quaternion.X * quaternion.Y + quaternion.S * quaternion.Z), 1 - 2 * (quaternion.X * quaternion.X - quaternion.Z * quaternion.Z), 2 * (quaternion.Y * quaternion.Z - quaternion.S * quaternion.X),
                2 * (quaternion.X * quaternion.Z - quaternion.S * quaternion.Y), 2 * (quaternion.Y * quaternion.Z + quaternion.S * quaternion.X), 1 - 2 * (quaternion.X * quaternion.X - quaternion.Y * quaternion.Y));
        }

        public static Matrix3x3f Scale(float x, float y, float z)
        {
            return new Matrix3x3f(x, 0, 0, 0, y, 0, 0, 0, z);
        }
    }

    partial struct Matrix3x3r
    {
        public static Matrix3x3r Rotation(QuaternionR quaternion)
        {
            return new Matrix3x3r(
                1 - 2 * (quaternion.X * quaternion.X + quaternion.Z * quaternion.Z), 2 * (quaternion.X * quaternion.Y - quaternion.S * quaternion.Z), 2 * (quaternion.X * quaternion.Z + quaternion.S * quaternion.Y),
                2 * (quaternion.X * quaternion.Y + quaternion.S * quaternion.Z), 1 - 2 * (quaternion.X * quaternion.X - quaternion.Z * quaternion.Z), 2 * (quaternion.Y * quaternion.Z - quaternion.S * quaternion.X),
                2 * (quaternion.X * quaternion.Z - quaternion.S * quaternion.Y), 2 * (quaternion.Y * quaternion.Z + quaternion.S * quaternion.X), 1 - 2 * (quaternion.X * quaternion.X - quaternion.Y * quaternion.Y));
        }

        public static Matrix3x3r Scale(double x, double y, double z)
        {
            return new Matrix3x3r(x, 0, 0, 0, y, 0, 0, 0, z);
        }
    }
}
