using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Geometry.Buffers
{
    public abstract class Texture2D<T> : ITexture<T>
    {
        public abstract T this[double x, double y] { get; }

        int ITexture<T>.Dimensions => 2;

        T ITexture<T>.Get(params double[] index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            if (index.Length != 2)
                throw new ArgumentException("Length of index is not equal to the dimension count.");

            return this[index[0], index[1]];
        }

        public static Texture2D<T> FromDelegate(Func<double, double, T> sampler)
        {
            if (sampler == null)
                throw new ArgumentNullException(nameof(sampler));

            return new DelegateTexture(sampler);
        }

        public static Texture2D<T> Wrap(Texture2D<T> texture, WrapMode wrapModeX, WrapMode wrapModeY)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            if (wrapModeX == WrapMode.None && wrapModeY == WrapMode.None)
                return texture;

            Func<double, double> xPass = TextureWrap.GetWrapFunction(wrapModeX);
            Func<double, double> yPass = TextureWrap.GetWrapFunction(wrapModeY);

            return FromDelegate((x, y) => texture[xPass(x), yPass(y)]);
        }

        private sealed class DelegateTexture : Texture2D<T>
        {
            private Func<double, double, T> _sampler;

            public override T this[double x, double y] => _sampler(x, y);

            public DelegateTexture(Func<double, double, T> sampler)
            {
                if (sampler == null)
                    throw new ArgumentNullException(nameof(sampler));

                _sampler = sampler;
            }
        }
    }
}
