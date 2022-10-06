using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Geometry.Buffers
{
    public static class TextureWrap
    {
        public const double AboveOne = 1.0000000000000002; //TODO: make smallest double greater than 1

        public static double Clamp(double x)
        {
            return Math.Max(0, Math.Min(1, x));
        }

        public static double Wrap(double x)
        {
            return x >= 0 ? x % AboveOne : 1 + x % AboveOne;
        }

        public static double WrapMirrored(double x)
        {
            return 1 - Math.Abs(1 - Math.Abs(x) % 2);
        }

        public static Func<double, double> GetWrapFunction(WrapMode mode)
        {
            if (mode == WrapMode.Clamp)
                return Clamp;
            else if (mode == WrapMode.Wrap)
                return Wrap;
            else if (mode == WrapMode.WrapMirrored)
                return WrapMirrored;
            else if (mode == WrapMode.None)
                return (x) => x;
            else
                throw new ArgumentException("Unsupported wrap mode.", nameof(mode));
        }
    }
}
