using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Geometry.Buffers
{
    public abstract class Texture3D<T> : ITexture<T>
    {
        public abstract T this[double x, double y, double z] { get; }

        int ITexture<T>.Dimensions => 3;

        T ITexture<T>.Get(params double[] index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            if (index.Length != 3)
                throw new ArgumentException("Length of index is not equal to the dimension count.");

            return this[index[0], index[1], index[3]];
        }

        public static Texture3D<T> FromDelegate(Func<double, double, double, T> sampler)
        {
            if (sampler == null)
                throw new ArgumentNullException(nameof(sampler));

            return new DelegateTexture(sampler);
        }

        public static Texture3D<T> Wrap(Texture3D<T> texture, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            if (wrapModeX == WrapMode.None && wrapModeY == WrapMode.None && wrapModeZ == WrapMode.None)
                return texture;

            Func<double, double> xPass = TextureWrap.GetWrapFunction(wrapModeX);
            Func<double, double> yPass = TextureWrap.GetWrapFunction(wrapModeY);
            Func<double, double> zPass = TextureWrap.GetWrapFunction(wrapModeZ);

            return FromDelegate((x, y, z) => texture[xPass(x), yPass(y), zPass(z)]);
        }

        private sealed class DelegateTexture : Texture3D<T>
        {
            private Func<double, double, double, T> _sampler;

            public override T this[double x, double y, double z] => _sampler(x, y, z);

            public DelegateTexture(Func<double, double, double, T> sampler)
            {
                if (sampler == null)
                    throw new ArgumentNullException(nameof(sampler));

                _sampler = sampler;
            }
        }
    }
}
