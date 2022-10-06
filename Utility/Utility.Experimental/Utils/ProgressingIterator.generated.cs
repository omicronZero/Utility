
using System;
using System.Collections.Generic;

namespace Utility
{
    public static partial class ProgressingIterator
    {

			public static ProgressingIterator<TResult, TItem, double, double> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, double> maximumProgressEvaluator,
           Func<TItem, double, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, double, double>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, float, float> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, float> maximumProgressEvaluator,
           Func<TItem, float, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, float, float>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, sbyte, sbyte> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, sbyte> maximumProgressEvaluator,
           Func<TItem, sbyte, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, sbyte, sbyte>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => (sbyte)(x + y),
                    (x, y) => (sbyte)(x - y),
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, byte, byte> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, byte> maximumProgressEvaluator,
           Func<TItem, byte, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, byte, byte>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => (byte)(x + y),
                    (x, y) => (byte)(x - y),
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, short, short> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, short> maximumProgressEvaluator,
           Func<TItem, short, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, short, short>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => (short)(x + y),
                    (x, y) => (short)(x - y),
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, ushort, ushort> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, ushort> maximumProgressEvaluator,
           Func<TItem, ushort, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, ushort, ushort>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => (ushort)(x + y),
                    (x, y) => (ushort)(x - y),
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, int, int> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, int> maximumProgressEvaluator,
           Func<TItem, int, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, int, int>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, uint, uint> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, uint> maximumProgressEvaluator,
           Func<TItem, uint, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, uint, uint>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, long, long> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, long> maximumProgressEvaluator,
           Func<TItem, long, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, long, long>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, ulong, ulong> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, ulong> maximumProgressEvaluator,
           Func<TItem, ulong, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, ulong, ulong>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, char, int> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, char> maximumProgressEvaluator,
           Func<TItem, char, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, char, int>(
                    enumerable,
                    '\0',
                    maximumProgressEvaluator,
                    (x, y) => (char)(x + y),
                    (x, y) => (int)(x - y),
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
				public static ProgressingIterator<TResult, TItem, decimal, decimal> CreateInstance<TResult, TItem>(
           IEnumerable<TItem> enumerable,
           Func<TItem, decimal> maximumProgressEvaluator,
           Func<TItem, decimal, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, decimal, decimal>(
                    enumerable,
                    0,
                    maximumProgressEvaluator,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    (x, y) => x.CompareTo(y),
                    resultSelector);
        }
			}
}