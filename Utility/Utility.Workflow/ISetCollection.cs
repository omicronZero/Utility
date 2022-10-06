using System.Collections.Generic;

namespace Utility
{
    public interface ISetCollection<T> : ICollection<T>
    {
        new bool Add(T item);
    }
}