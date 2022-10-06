using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Geometry.Buffers
{
    public interface ITexture<T>
    {
        int Dimensions { get; }
        T Get(params double[] index);
    }

    public interface ITexture<T, TIndex> : ITexture<T>
    {
        T this[TIndex index] { get; set; }
    }
}
