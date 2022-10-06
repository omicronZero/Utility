//using System;
//using System.Collections.Generic;
//using System.Text;


/*
 * Removed due to missing ease of use.
 */


//namespace Utility.Collections
//{
//    public static class ArrayExtension
//    {
//        /// <summary>
//        /// Tries to insert the specified item into the one-dimensional array and fails, if the new count would exceed the
//        /// amount of items of the array. Returns, whether the operation was successful.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item.</typeparam>
//        /// <param name="array">The array into which to insert the item.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently stored in the list's range.</param>
//        /// <param name="targetIndex">The index, relative to the list range, at which to insert the item.</param>
//        /// <param name="item">The item to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static bool TryInsert<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int targetIndex, T item)
//        {
//            if (array == null)
//                throw new ArgumentNullException(nameof(array));
//            if (arrayRangeCount < 0 || arrayRangeCount > array.Length)
//                throw new ArgumentOutOfRangeException(nameof(arrayRangeCount), "The amount of items currently stored in the array was expected to be non-negative and at most the length of the array.");
//            if (arrayRangeStartIndex < 0)
//                throw new ArgumentOutOfRangeException(nameof(arrayRangeStartIndex), "Array range start index must be non-negative.");
//            if (arrayRangeStartIndex + arrayRangeCount > array.Length)
//                throw new ArgumentException("Array range exceeded by list range.");
//            if (targetIndex < 0 || targetIndex > arrayRangeCount)
//                throw new ArgumentOutOfRangeException(nameof(targetIndex), "Target index must be non-negative and within the range 0 to arrayRangeCount.");

//            if (array.Length == arrayRangeCount)
//                return false;

//            Array.Copy(array, arrayRangeStartIndex + targetIndex, array, arrayRangeStartIndex + targetIndex + 1, arrayRangeCount - targetIndex);
//            array[arrayRangeStartIndex + targetIndex] = item;

//            return true;
//        }

//        /// <summary>
//        /// Tries to insert the specified item range into the one-dimensional array and fails, if the new count would exceed the
//        /// amount of items of the array. Returns, whether the operation was successful.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item range.</typeparam>
//        /// <param name="array">The array into which to insert the item range.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently stored in the list's range.</param>
//        /// <param name="targetIndex">The index at which to insert the item range.</param>
//        /// <param name="items">The item range to insert.</param>
//        /// <param name="itemsIndex">The index at which the item range to insert starts.</param>
//        /// <param name="itemsCount">The number of entries in the item range to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item range.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static bool TryInsert<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int targetIndex, T[] items, int itemsIndex, int itemsCount)
//        {
//            if (array == null)
//                throw new ArgumentNullException(nameof(array));
//            if (arrayRangeCount < 0 || arrayRangeCount > array.Length)
//                throw new ArgumentOutOfRangeException(nameof(arrayRangeCount), "The amount of items currently stored in the array was expected to be non-negative and at most the length of the array.");
//            if (arrayRangeStartIndex < 0)
//                throw new ArgumentOutOfRangeException(nameof(arrayRangeStartIndex), "Array range start index must be non-negative.");
//            if (arrayRangeStartIndex + arrayRangeCount > array.Length)
//                throw new ArgumentException("Array range exceeded by list range.");
//            if (targetIndex < 0 || targetIndex > arrayRangeCount)
//                throw new ArgumentOutOfRangeException(nameof(targetIndex), "Target index must be non-negative and within the range 0 to arrayRangeCount.");
//            if (items == null)
//                throw new ArgumentNullException(nameof(items));
//            if (itemsIndex < 0)
//                throw new ArgumentOutOfRangeException(nameof(itemsIndex), "Non-negative items index expected.");
//            if (itemsCount < 0)
//                throw new ArgumentOutOfRangeException(nameof(itemsCount), "Non-negative items count expected.");
//            if (itemsIndex + itemsCount > items.Length)
//                throw new ArgumentException("The indicated items range exceeds the boundaries of the items array.");

//            if (array.Length < arrayRangeCount + items.Length)
//                return false;

//            Array.Copy(array, targetIndex, array, arrayRangeStartIndex + targetIndex + items.Length, arrayRangeCount - targetIndex);
//            Array.Copy(items, itemsIndex, array, arrayRangeStartIndex + targetIndex, itemsCount);

//            return true;
//        }

//        /// <summary>
//        /// Tries to insert the specified item range into the one-dimensional array and fails, if the new count would exceed the
//        /// amount of items of the array. Returns, whether the operation was successful.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item range.</typeparam>
//        /// <param name="array">The array into which to insert the item range.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array.</param>
//        /// <param name="targetIndex">The index at which to insert the item range.</param>
//        /// <param name="items">The item range to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item range.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static bool TryInsert<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int targetIndex, T[] items)
//        {
//            return TryInsert(array, arrayRangeStartIndex, arrayRangeCount, targetIndex, items, 0, items?.Length ?? 0);
//        }

//        /// <summary>
//        /// Tries to insert the specified item into the one-dimensional array and fails, if the new count would exceed the
//        /// amount of items of the array. Returns, whether the operation was successful.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item.</typeparam>
//        /// <param name="array">The array into which to insert the item.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array. The value is increased by 1 if the operation was successful.</param>
//        /// <param name="targetIndex">The index at which to insert the item.</param>
//        /// <param name="item">The item to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static bool TryInsert<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int targetIndex, T item)
//        {
//            bool v = TryInsert(array, arrayRangeStartIndex, arrayRangeCount, targetIndex, item);

//            if (v)
//                arrayRangeCount += 1;

//            return v;
//        }

//        /// <summary>
//        /// Tries to insert the specified item range into the one-dimensional array and fails, if the new count would exceed the
//        /// amount of items of the array. Returns, whether the operation was successful.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item range.</typeparam>
//        /// <param name="array">The array into which to insert the item range.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array. The value is increased by the amount of newly added items.</param>
//        /// <param name="targetIndex">The index at which to insert the item range.</param>
//        /// <param name="items">The item range to insert.</param>
//        /// <param name="itemsIndex">The index at which the item range to insert starts.</param>
//        /// <param name="itemsCount">The number of entries in the item range to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item range.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static bool TryInsert<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int targetIndex, T[] items, int itemsIndex, int itemsCount)
//        {
//            bool v = TryInsert(array, arrayRangeStartIndex, arrayRangeCount, targetIndex, items, itemsIndex, itemsCount);

//            if (v)
//                arrayRangeCount += itemsCount;

//            return v;
//        }

//        /// <summary>
//        /// Tries to insert the specified item range into the one-dimensional array and fails, if the new count would exceed the
//        /// amount of items of the array. Returns, whether the operation was successful.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item range.</typeparam>
//        /// <param name="array">The array into which to insert the item range.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array. The value is increased by the amount of newly added items.</param>
//        /// <param name="targetIndex">The index at which to insert the item range.</param>
//        /// <param name="items">The item range to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item range.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static bool TryInsert<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int targetIndex, T[] items)
//        {
//            return TryInsert(array, arrayRangeStartIndex, ref arrayRangeCount, targetIndex, items, 0, items?.Length ?? 0);
//        }

//        /// <summary>
//        /// Inserts the specified item range into the one-dimensional array.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item.</typeparam>
//        /// <param name="array">The array into which to insert the item.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array.</param>
//        /// <param name="targetIndex">The index at which to insert the item.</param>
//        /// <param name="item">The item to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static void Insert<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int targetIndex, T item)
//        {
//            if (!TryInsert(array, arrayRangeStartIndex, arrayRangeCount, targetIndex, item))
//                throw new ArgumentException("The indicated array does not hold sufficient remaining items to store the indicated item.");
//        }

//        /// <summary>
//        /// Inserts the specified item range into the one-dimensional array.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item range.</typeparam>
//        /// <param name="array">The array into which to insert the item range.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array.</param>
//        /// <param name="targetIndex">The index at which to insert the item range.</param>
//        /// <param name="items">The item range to insert.</param>
//        /// <param name="itemsIndex">The index at which the item range to insert starts.</param>
//        /// <param name="itemsCount">The number of entries in the item range to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item range.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static void Insert<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int targetIndex, T[] items, int itemsIndex, int itemsCount)
//        {
//            if (!TryInsert(array, arrayRangeStartIndex, arrayRangeCount, targetIndex, items, itemsIndex, itemsCount))
//                throw new ArgumentException("The indicated array does not hold sufficient remaining items to store the indicated item range.");
//        }

//        /// <summary>
//        /// Inserts the specified item range into the one-dimensional array.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item range.</typeparam>
//        /// <param name="array">The array into which to insert the item range.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array.</param>
//        /// <param name="targetIndex">The index at which to insert the item range.</param>
//        /// <param name="items">The item range to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item range.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static void Insert<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int targetIndex, T[] items)
//        {
//            if (!TryInsert(array, arrayRangeStartIndex, arrayRangeCount, targetIndex, items))
//                throw new ArgumentException("The indicated array does not hold sufficient remaining items to store the indicated item range.");
//        }

//        /// <summary>
//        /// Inserts the specified item into the one-dimensional array and fails, if the new count would exceed the
//        /// amount of items of the array. Returns, whether the operation was successful.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item.</typeparam>
//        /// <param name="array">The array into which to insert the item.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array. The value is increased by 1 if the operation was successful.</param>
//        /// <param name="targetIndex">The index at which to insert the item.</param>
//        /// <param name="item">The item to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static void Insert<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int targetIndex, T item)
//        {
//            Insert(array, arrayRangeStartIndex, arrayRangeCount, targetIndex, item);

//            arrayRangeCount++;
//        }

//        /// <summary>
//        /// Inserts the specified item range into the one-dimensional array.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item range.</typeparam>
//        /// <param name="array">The array into which to insert the item range.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array. The value is increased by the amount of newly added items.</param>
//        /// <param name="targetIndex">The index at which to insert the item range.</param>
//        /// <param name="items">The item range to insert.</param>
//        /// <param name="itemsIndex">The index at which the item range to insert starts.</param>
//        /// <param name="itemsCount">The number of entries in the item range to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item range.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static void Insert<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int targetIndex, T[] items, int itemsIndex, int itemsCount)
//        {
//            Insert(array, arrayRangeStartIndex, arrayRangeCount, targetIndex, items, itemsIndex, itemsCount);
//            arrayRangeCount += itemsCount;
//        }

//        /// <summary>
//        /// Inserts the specified item range into the one-dimensional array.
//        /// </summary>
//        /// <typeparam name="T">The type of the array in which to insert the item range.</typeparam>
//        /// <param name="array">The array into which to insert the item range.</param>
//        /// <param name="arrayRangeStartIndex">The index at which the list's range starts.</param>
//        /// <param name="arrayRangeCount">The amount of items currently used in the array. The value is increased by the amount of newly added items.</param>
//        /// <param name="targetIndex">The index at which to insert the item range.</param>
//        /// <param name="items">The item range to insert.</param>
//        /// <returns>A value that determines whether sufficient items were available to store the inserted item range.
//        /// The operation was not performed and the array will not have been changed, if false.</returns>
//        public static void Insert<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int targetIndex, params T[] items)
//        {
//            Insert(array, arrayRangeStartIndex, ref arrayRangeCount, targetIndex, items);
//        }

//        public static void RemoveAt<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int index)
//        {
//            RemoveAt(array, arrayRangeStartIndex, arrayRangeCount, index, 1);
//        }

//        public static void RemoveAt<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int index, bool clear)
//        {
//            RemoveAt(array, arrayRangeStartIndex, arrayRangeCount, index, 1, clear);
//        }

//        public static void RemoveAt<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int index, int count)
//        {
//            RemoveAt(array, arrayRangeStartIndex, arrayRangeCount, index, count, true);
//        }

//        public static void RemoveAt<T>(this T[] array, int arrayRangeStartIndex, int arrayRangeCount, int index, int count, bool clear)
//        {
//            if (array == null)
//                throw new ArgumentNullException(nameof(array));
//            if (arrayRangeCount < 0 || arrayRangeCount > array.Length)
//                throw new ArgumentOutOfRangeException(nameof(arrayRangeCount), "The amount of items currently stored in the array was expected to be non-negative and at most the length of the array.");
//            if (arrayRangeStartIndex + arrayRangeCount > array.Length)
//                throw new ArgumentException("Array range exceeded by list range.");
//            if (index + count > arrayRangeCount)
//                throw new ArgumentException("The indicated items range exceeds the boundaries of the array's currently stored items.");
//            if (index < 0)
//                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative index expected.");
//            if (count < 0)
//                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative count expected.");
//            if (arrayRangeStartIndex < 0)
//                throw new ArgumentOutOfRangeException(nameof(arrayRangeStartIndex), "Array range start index must be non-negative.");

//            Array.Copy(array, arrayRangeStartIndex + index + count, array, arrayRangeStartIndex + index, count);

//            if (clear)
//                Array.Clear(array, arrayRangeStartIndex + index + count, count);
//        }

//        public static void RemoveAt<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int index)
//        {
//            RemoveAt(array, arrayRangeStartIndex, ref arrayRangeCount, index, 1);
//        }

//        public static void RemoveAt<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int index, bool clear)
//        {
//            RemoveAt(array, arrayRangeStartIndex, ref arrayRangeCount, index, 1, clear);
//        }

//        public static void RemoveAt<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int index, int count)
//        {
//            RemoveAt(array, arrayRangeStartIndex, ref arrayRangeCount, index, count, true);
//        }

//        public static void RemoveAt<T>(this T[] array, int arrayRangeStartIndex, ref int arrayRangeCount, int index, int count, bool clear)
//        {
//            RemoveAt(array, arrayRangeStartIndex, arrayRangeCount, index, count, clear);

//            arrayRangeCount -= count;
//        }

//        public static void Insert<T>(this ref ArraySegment<T> arraySegment, int targetIndex, T item)
//        {
//            int c = arraySegment.Count;
//            Insert(arraySegment.Array, arraySegment.Offset, ref c, targetIndex, item);
//            arraySegment = new ArraySegment<T>(arraySegment.Array, arraySegment.Offset, c);
//        }

//        public static void Insert<T>(this ref ArraySegment<T> arraySegment, int targetIndex, T[] items, int itemsIndex, int itemsCount)
//        {
//            int c = arraySegment.Count;
//            Insert(arraySegment.Array, arraySegment.Offset, ref c, targetIndex, items, itemsIndex, itemsCount);
//            arraySegment = new ArraySegment<T>(arraySegment.Array, arraySegment.Offset, c);
//        }

//        public static void Insert<T>(this ref ArraySegment<T> arraySegment, int targetIndex, T[] items)
//        {
//            int c = arraySegment.Count;
//            Insert(arraySegment.Array, arraySegment.Offset, ref c, targetIndex, items);
//            arraySegment = new ArraySegment<T>(arraySegment.Array, arraySegment.Offset, c);
//        }

//        public static bool TryInsert<T>(this ref ArraySegment<T> arraySegment, int targetIndex, T item)
//        {
//            int c = arraySegment.Count;
//            if (TryInsert(arraySegment.Array, arraySegment.Offset, ref c, targetIndex, item))
//            {
//                arraySegment = new ArraySegment<T>(arraySegment.Array, arraySegment.Offset, c);
//                return true;
//            }
//            return false;
//        }

//        public static bool TryInsert<T>(this ref ArraySegment<T> arraySegment, int targetIndex, T[] items, int itemsIndex, int itemsCount)
//        {
//            int c = arraySegment.Count;
//            if (TryInsert(arraySegment.Array, arraySegment.Offset, ref c, targetIndex, items, itemsIndex, itemsCount))
//            {
//                arraySegment = new ArraySegment<T>(arraySegment.Array, arraySegment.Offset, c);
//                return true;
//            }
//            return false;
//        }

//        public static bool TryInsert<T>(this ref ArraySegment<T> arraySegment, int targetIndex, T[] items)
//        {
//            int c = arraySegment.Count;
//            if (TryInsert(arraySegment.Array, arraySegment.Offset, ref c, targetIndex, items))
//            {
//                arraySegment = new ArraySegment<T>(arraySegment.Array, arraySegment.Offset, c);
//                return true;
//            }
//            return false;
//        }
//    }
//}
