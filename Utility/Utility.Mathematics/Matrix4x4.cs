
namespace Utility.Mathematics
{
    partial struct Matrix4x4f
    {
        public static Matrix4x4f Rotation(QuaternionF quaternion)
        {
            return new Matrix4x4f(
                1 - 2 * (quaternion.X * quaternion.X + quaternion.Z * quaternion.Z), 2 * (quaternion.X * quaternion.Y - quaternion.S * quaternion.Z), 2 * (quaternion.X * quaternion.Z + quaternion.S * quaternion.Y), 0,
                2 * (quaternion.X * quaternion.Y + quaternion.S * quaternion.Z), 1 - 2 * (quaternion.X * quaternion.X - quaternion.Z * quaternion.Z), 2 * (quaternion.Y * quaternion.Z - quaternion.S * quaternion.X), 0,
                2 * (quaternion.X * quaternion.Z - quaternion.S * quaternion.Y), 2 * (quaternion.Y * quaternion.Z + quaternion.S * quaternion.X), 1 - 2 * (quaternion.X * quaternion.X - quaternion.Y * quaternion.Y), 0,
                0, 0, 0, 1);
        }

        public static QuaternionF ToQuaternion(Matrix4x4f rotationMatrix)
        {
            float s = (float)System.Math.Sqrt(1 + rotationMatrix.M11 + rotationMatrix.M22 + rotationMatrix.M33);
            return new QuaternionF(s / 2,
                (rotationMatrix.M32 - rotationMatrix.M23) / (2 * s),
                (rotationMatrix.M13 - rotationMatrix.M31) / (2 * s),
                (rotationMatrix.M21 - rotationMatrix.M12) / (2 * s));
        }


        public static Matrix4x4f Translation(Vector3f translation)
        {
            return Translation(translation.X, translation.Y, translation.Z);
        }

        public static Matrix4x4f Translation(float x, float y, float z)
        {
            return new Matrix4x4f(0, 0, 0, x, 0, 0, 0, y, 0, 0, 0, z, 0, 0, 0, 1);
        }

        public static Matrix4x4f Scale(Vector3f scale)
        {
            return Scale(scale.X, scale.Y, scale.Z);
        }

        public static Matrix4x4f Scale(float x, float y, float z)
        {
            return new Matrix4x4f(x, 0, 0, 0, 0, y, 0, 0, 0, 0, z, 0, 0, 0, 0, 1);
        }

        public static Matrix4x4f Reflect(PlaneF plane)
        {
            return new Matrix4x4f(1 - 2 * plane.N1 * plane.N1, -2 * plane.N1 * plane.N2, -2 * plane.N1 * plane.N3, -2 * plane.N1 * plane.D,
                                 -2 * plane.N1 * plane.N2, 1 - 2 * plane.N2 * plane.N2, -2 * plane.N2 * plane.N3, -2 * plane.N2 * plane.D,
                                 -2 * plane.N1 * plane.N3, -2 * plane.N2 * plane.N3, 1 - 2 * plane.N3 * plane.N3, -2 * plane.N3 * plane.D,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4f Reflect(Vector3f planeNormal)
        {
            return Reflect(planeNormal.X, planeNormal.Y, planeNormal.Z);
        }

        public static Matrix4x4f Reflect(float planeNormalX, float planeNormalY, float planeNormalZ)
        {
            return new Matrix4x4f(1 - 2 * planeNormalX * planeNormalX, -2 * planeNormalX * planeNormalY, -2 * planeNormalX * planeNormalZ, 0,
                                 -2 * planeNormalX * planeNormalY, 1 - 2 * planeNormalY * planeNormalY, -2 * planeNormalY * planeNormalZ, 0,
                                 -2 * planeNormalX * planeNormalZ, -2 * planeNormalY * planeNormalZ, 1 - 2 * planeNormalZ * planeNormalZ, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4f RotationAxis(Vector3f axis, float angle)
        {
            return RotationAxis(axis.X, axis.Y, axis.Z, angle);
        }

        public static Matrix4x4f RotationAxis(float axisX, float axisY, float axisZ, float angle)
        {
            float s = (float)System.Math.Sin(angle);
            float c = (float)System.Math.Cos(angle);

            return new Matrix4x4f(axisX * axisX * (1 - c) + c, axisY * axisX * (1 - c) - axisZ * s, axisZ * axisX * (1 - c) + axisY * s, 0,
                                 axisX * axisY * (1 - c) + axisZ * s, axisY * axisY * (1 - c) + c, axisZ * axisY * (1 - c) - axisX * s, 0,
                                 axisX * axisZ * (1 - c) - axisY * s, axisY * axisZ * (1 - c) + axisX * s, axisZ * axisZ * (1 - c) + c, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4f RotationX(float angle)
        {
            float s = (float)System.Math.Sin(angle);
            float c = (float)System.Math.Cos(angle);

            return new Matrix4x4f(1, 0, 0, 0,
                                 0, c, -s, 0,
                                 0, s, c, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4f RotationY(float angle)
        {
            float s = (float)System.Math.Sin(angle);
            float c = (float)System.Math.Cos(angle);

            return new Matrix4x4f(c, 0, 0, s,
                                 0, 1, 0, 0,
                                 -s, 0, c, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4f RotationZ(float angle)
        {
            float s = (float)System.Math.Sin(angle);
            float c = (float)System.Math.Cos(angle);

            return new Matrix4x4f(c, -s, 0, 0,
                                 s, c, 0, 0,
                                 0, 0, 1, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4f RotationYawPitchRoll(float yaw, float pitch, float roll)
        {
            float sinyaw = (float)System.Math.Sin(yaw);
            float cosyaw = (float)System.Math.Cos(yaw);
            float sinpitch = (float)System.Math.Sin(pitch);
            float cospitch = (float)System.Math.Cos(pitch);
            float sinroll = (float)System.Math.Sin(roll);
            float cosroll = (float)System.Math.Cos(roll);

            return new Matrix4x4f(cosyaw * cospitch, cosyaw * sinpitch * sinroll - sinyaw * cosroll, cosyaw * sinpitch * cosroll + sinyaw * sinroll, 0,
                                 sinyaw * cospitch, sinyaw * sinpitch * sinroll + cosyaw * cosroll, sinyaw * sinpitch * cosroll - cosyaw * sinroll, 0,
                                 -sinpitch, cospitch * sinroll, cospitch * cosroll, 0,
                                 0, 0, 0, 1);
        }

        //TODO: Forward&Backward vector, Rows, Columns, Translation, Scale, Determinant, Invert, Transpose, Orthogonalize, Orthonormalize, Exponent
        //TODO: LookAt L/R, Perspective L/R & opt. OffCenter & opt. Fov, Orthogonal L/R, Billboard L/R-Handed

    }
    partial struct Matrix4x4r
    {
        public static Matrix4x4r Rotation(QuaternionR quaternion)
        {
            return new Matrix4x4r(
                1 - 2 * (quaternion.X * quaternion.X + quaternion.Z * quaternion.Z), 2 * (quaternion.X * quaternion.Y - quaternion.S * quaternion.Z), 2 * (quaternion.X * quaternion.Z + quaternion.S * quaternion.Y), 0,
                2 * (quaternion.X * quaternion.Y + quaternion.S * quaternion.Z), 1 - 2 * (quaternion.X * quaternion.X - quaternion.Z * quaternion.Z), 2 * (quaternion.Y * quaternion.Z - quaternion.S * quaternion.X), 0,
                2 * (quaternion.X * quaternion.Z - quaternion.S * quaternion.Y), 2 * (quaternion.Y * quaternion.Z + quaternion.S * quaternion.X), 1 - 2 * (quaternion.X * quaternion.X - quaternion.Y * quaternion.Y), 0,
                0, 0, 0, 1);
        }

        public static QuaternionR ToQuaternion(Matrix4x4r rotationMatrix)
        {
            double s = System.Math.Sqrt(1 + rotationMatrix.M11 + rotationMatrix.M22 + rotationMatrix.M33);
            return new QuaternionR(s / 2,
                (rotationMatrix.M32 - rotationMatrix.M23) / (2 * s),
                (rotationMatrix.M13 - rotationMatrix.M31) / (2 * s),
                (rotationMatrix.M21 - rotationMatrix.M12) / (2 * s));
        }


        public static Matrix4x4r Translation(Vector3r translation)
        {
            return Translation(translation.X, translation.Y, translation.Z);
        }

        public static Matrix4x4r Translation(double x, double y, double z)
        {
            return new Matrix4x4r(0, 0, 0, x, 0, 0, 0, y, 0, 0, 0, z, 0, 0, 0, 1);
        }

        public static Matrix4x4r Scale(Vector3r scale)
        {
            return Scale(scale.X, scale.Y, scale.Z);
        }

        public static Matrix4x4r Scale(double x, double y, double z)
        {
            return new Matrix4x4r(x, 0, 0, 0, 0, y, 0, 0, 0, 0, z, 0, 0, 0, 0, 1);
        }

        public static Matrix4x4r Reflect(PlaneR plane)
        {
            return new Matrix4x4r(1 - 2 * plane.N1 * plane.N1, -2 * plane.N1 * plane.N2, -2 * plane.N1 * plane.N3, -2 * plane.N1 * plane.D,
                                 -2 * plane.N1 * plane.N2, 1 - 2 * plane.N2 * plane.N2, -2 * plane.N2 * plane.N3, -2 * plane.N2 * plane.D,
                                 -2 * plane.N1 * plane.N3, -2 * plane.N2 * plane.N3, 1 - 2 * plane.N3 * plane.N3, -2 * plane.N3 * plane.D,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4r Reflect(Vector3r planeNormal)
        {
            return Reflect(planeNormal.X, planeNormal.Y, planeNormal.Z);
        }

        public static Matrix4x4r Reflect(double planeNormalX, double planeNormalY, double planeNormalZ)
        {
            return new Matrix4x4r(1 - 2 * planeNormalX * planeNormalX, -2 * planeNormalX * planeNormalY, -2 * planeNormalX * planeNormalZ, 0,
                                 -2 * planeNormalX * planeNormalY, 1 - 2 * planeNormalY * planeNormalY, -2 * planeNormalY * planeNormalZ, 0,
                                 -2 * planeNormalX * planeNormalZ, -2 * planeNormalY * planeNormalZ, 1 - 2 * planeNormalZ * planeNormalZ, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4r RotationAxis(Vector3r axis, double angle)
        {
            return RotationAxis(axis.X, axis.Y, axis.Z, angle);
        }

        public static Matrix4x4r RotationAxis(double axisX, double axisY, double axisZ, double angle)
        {
            double s = System.Math.Sin(angle);
            double c = System.Math.Cos(angle);

            return new Matrix4x4r(axisX * axisX * (1 - c) + c, axisY * axisX * (1 - c) - axisZ * s, axisZ * axisX * (1 - c) + axisY * s, 0,
                                 axisX * axisY * (1 - c) + axisZ * s, axisY * axisY * (1 - c) + c, axisZ * axisY * (1 - c) - axisX * s, 0,
                                 axisX * axisZ * (1 - c) - axisY * s, axisY * axisZ * (1 - c) + axisX * s, axisZ * axisZ * (1 - c) + c, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4r RotationX(double angle)
        {
            double s = System.Math.Sin(angle);
            double c = System.Math.Cos(angle);

            return new Matrix4x4r(1, 0, 0, 0,
                                 0, c, -s, 0,
                                 0, s, c, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4r RotationY(double angle)
        {
            double s = System.Math.Sin(angle);
            double c = System.Math.Cos(angle);

            return new Matrix4x4r(c, 0, 0, s,
                                 0, 1, 0, 0,
                                 -s, 0, c, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4r RotationZ(double angle)
        {
            double s = System.Math.Sin(angle);
            double c = System.Math.Cos(angle);

            return new Matrix4x4r(c, -s, 0, 0,
                                 s, c, 0, 0,
                                 0, 0, 1, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4r RotationYawPitchRoll(double yaw, double pitch, double roll)
        {
            double sinyaw = System.Math.Sin(yaw);
            double cosyaw = System.Math.Cos(yaw);
            double sinpitch = System.Math.Sin(pitch);
            double cospitch = System.Math.Cos(pitch);
            double sinroll = System.Math.Sin(roll);
            double cosroll = System.Math.Cos(roll);

            return new Matrix4x4r(cosyaw * cospitch, cosyaw * sinpitch * sinroll - sinyaw * cosroll, cosyaw * sinpitch * cosroll + sinyaw * sinroll, 0,
                                 sinyaw * cospitch, sinyaw * sinpitch * sinroll + cosyaw * cosroll, sinyaw * sinpitch * cosroll - cosyaw * sinroll, 0,
                                 -sinpitch, cospitch * sinroll, cospitch * cosroll, 0,
                                 0, 0, 0, 1);
        }

        //TODO: Forward&Backward vector, Rows, Columns, Translation, Scale, Determinant, Invert, Transpose, Orthogonalize, Orthonormalize, Exponent
        //TODO: LookAt L/R, Perspective L/R & opt. OffCenter & opt. Fov, Orthogonal L/R, Billboard L/R-Handed

    }
}