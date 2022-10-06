using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Utility.Collections.Tools
{
    public static class CollectionExceptions
    {
        public static Exception ReadOnlyException() => new InvalidOperationException("The collection is read-only.");

        [DebuggerHidden]
        public static void CheckCopyTo<T>(int itemCount, T[] array, int arrayIndex)
        {
            Exception ex = GetCopyToExceptions(itemCount, array, arrayIndex);

            if (ex != null)
                throw ex;
        }

        public static Exception GetCopyToExceptions<T>(int itemCount, T[] array, int arrayIndex)
        {
            if (itemCount < 0)
                return new ArgumentOutOfRangeException(nameof(itemCount), "Positive item count expected.");

            if (array == null)
                return new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex >= array.Length)
                return new ArgumentOutOfRangeException(nameof(arrayIndex), "The array index does not fall into the range of the array.");

            if (checked(arrayIndex + itemCount) > array.Length)
                return new ArgumentException("The number of elements in the source is greater than the available space from arrayIndex to the end of the destination array.");

            return null;
        }
    }
}
