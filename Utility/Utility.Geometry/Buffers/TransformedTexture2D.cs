using System;
using System.Collections.Generic;
using System.Text;
using Utility.Mathematics;

namespace Utility.Geometry.Buffers
{
    public abstract class TransformedTexture2D<T> : Texture2D<T>
    {
        private static readonly Matrix2x3r IdentityMatrix = new Matrix2x3r(1, 0, 0,
                                                                           0, 1, 0);

        public Matrix2x3r Transform { get; set; }

        protected abstract T Get(double transformedX, double transformedY);

        protected TransformedTexture2D(Matrix2x3r transform)
        {
            Transform = transform;
        }

        protected TransformedTexture2D()
            : this(IdentityMatrix)
        { }

        public override T this[double x, double y]
        {
            get
            {
                Vector2r transformed = Vector2r.Transform(Transform, new Vector2r(x, y));
                return Get(transformed.X, transformed.Y);
            }
        }

        public static TransformedTexture2D<T> TransformTexture(Texture2D<T> underlyingTexture)
        {
            return TransformTexture(underlyingTexture, IdentityMatrix);
        }

            public static TransformedTexture2D<T> TransformTexture(Texture2D<T> underlyingTexture, Matrix2x3r transform)
        {
            if (underlyingTexture == null)
                throw new ArgumentNullException(nameof(underlyingTexture));

            return new TransformedTexture(underlyingTexture, transform);
        }

        private sealed class TransformedTexture : TransformedTexture2D<T>
        {
            private readonly Texture2D<T> _underlyingTexture;

            public TransformedTexture(Texture2D<T> underlyingTexture)
                : this(underlyingTexture, IdentityMatrix)
            { }

            public TransformedTexture(Texture2D<T> underlyingTexture, Matrix2x3r transform)
                : base(transform)
            {
                if (underlyingTexture == null)
                    throw new ArgumentNullException(nameof(underlyingTexture));

                _underlyingTexture = underlyingTexture;
            }

            protected override T Get(double transformedX, double transformedY)
            {
                return _underlyingTexture[transformedX, transformedY];
            }
        }
    }
}
