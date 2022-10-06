using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Geometry.Buffers
{
    public interface IBuffer<T>
    {
        int Dimensions { get; }
        bool IsReadOnly { get; }
        int GetLength(int dimension);
        T Get(params int[] index);
    }

    public interface IBuffer<T, TIndex> : IBuffer<T>
    {
        T this[TIndex index] { get; set; }
    }
}
