using System;

namespace Utility.Mathematics
{
    public static partial class MathUtil
    {
        public const double AngleToRad = Math.PI / 180;
        public const double RadToAngle = 180 / Math.PI;
        public const float AngleToRadF = (float)(Math.PI / 180);
        public const float RadToAngleF = (float)(180 / Math.PI);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector2r SinCos(double angle)
        {
            return (Math.Sin(angle), Math.Cos(angle));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Vector2r CosSin(double angle)
        {
            return (Math.Cos(angle), Math.Sin(angle));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float Pow2(float v)
        {
            return v * v;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float Pow3(float v)
        {
            return v * v * v;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float Pow4(float v)
        {
            return (v * v) * (v * v);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float Pow5(float v)
        {
            return (v * v) * (v * v) * v;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double Pow2(double v)
        {
            return v * v;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double Pow3(double v)
        {
            return v * v * v;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double Pow4(double v)
        {
            return (v * v) * (v * v);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double Pow5(double v)
        {
            return (v * v) * (v * v) * v;
        }

        public static bool SolveQuadratic(float a, float b, float c, out float first, out float second)
        {
            float r = b * b - 4 * a * c;
            if (r < 0)
            {
                first = 0;
                second = 0;
                return false;
            }

            r = (float)Math.Sqrt(r);
            first = (-b + r) / (2 * a);
            second = (-b - r) / (2 * a);

            return true;
        }

        public static bool SolveQuadratic(float a, float b, float c)
        {
            return b * b - 4 * a * c >= 0;
        }

        public static bool SolveQuadratic(double a, double b, double c, out double first, out double second)
        {
            double r = b * b - 4 * a * c;
            if (r < 0)
            {
                first = 0;
                second = 0;
                return false;
            }

            r = Math.Sqrt(r);
            first = (-b + r) / (2 * a);
            second = (-b - r) / (2 * a);

            return true;
        }

        public static bool SolveQuadratic(double a, double b, double c)
        {
            return b * b - 4 * a * c >= 0;
        }
    }
}