using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Utility
{
    public static class ArrayExtensions
    {
        public static T[] ShallowCopy<T>(this T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            return (T[])array.Clone();
        }

        public static ReadOnlyCollection<T> ReadOnlyShallowCopy<T>(this T[] array)
        {
            return Array.AsReadOnly(ShallowCopy(array));
        }
    }
}
