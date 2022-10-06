using System;
using System.Collections.Generic;
using System.Text;
using Utility.ObjectDescription.ObservableQuery.Queries;

namespace Utility.ObjectDescription.ObservableQuery
{
    public static class ObservationListQueryExtensions
    {
        public static WhereQuery<TSource> Where<TSource>(this IObservationListQuery<TSource> source, Func<TSource, bool> predicate)
        {
            return new WhereQuery<TSource>(source, predicate);
        }

        public static SelectorQuery<TSource, TResult> Select<TSource, TResult>(this IObservationListQuery<TSource> source, Func<TSource, TResult> selector)
        {
            return Select(source, selector, false);
        }

        public static SelectorQuery<TSource, TResult> Select<TSource, TResult>(this IObservationListQuery<TSource> source, Func<TSource, TResult> selector, bool convertOnce)
        {
            return new SelectorQuery<TSource, TResult>(source, selector, convertOnce);
        }

        public static AggregateQuery<TSource, TSource, TSource> Aggregate<TSource>(this IObservationListQuery<TSource> source, Func<TSource, TSource, TSource> func)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            bool initialized = false;

            return Aggregate(source, () =>
                {
                    initialized = false;
                    return default;
                },
                (value, current) =>
                {
                    if (initialized)
                        return func(value, current);
                    else
                    {
                        initialized = true;
                        return current;
                    }
                },
                Helper<TSource>.Identity);
        }

        public static AggregateQuery<TSource, TAccumulate, TAccumulate> Aggregate<TSource, TAccumulate>(this IObservationListQuery<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return Aggregate(source, () => seed, func);
        }

        public static AggregateQuery<TSource, TAccumulate, TResult> Aggregate<TSource, TAccumulate, TResult>(this IObservationListQuery<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            return Aggregate(source, () => seed, func, resultSelector);
        }

        public static AggregateQuery<TSource, TAccumulate, TAccumulate> Aggregate<TSource, TAccumulate>(this IObservationListQuery<TSource> source, Func<TAccumulate> seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return Aggregate(source, seed, func, Helper<TAccumulate>.Identity);
        }

        public static AggregateQuery<TSource, TAccumulate, TResult> Aggregate<TSource, TAccumulate, TResult>(this IObservationListQuery<TSource> source, Func<TAccumulate> seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            return Aggregate(source, seed, AggregateHelper<TAccumulate, TSource>.Create(func).Handle, resultSelector);
        }

        public static AggregateQuery<TSource, TAccumulate, TAccumulate> Aggregate<TSource, TAccumulate>(this IObservationListQuery<TSource> source, TAccumulate seed, AggregateQueryHandler<TAccumulate, TSource> func)
        {
            return Aggregate(source, () => seed, func);
        }

        public static AggregateQuery<TSource, TAccumulate, TResult> Aggregate<TSource, TAccumulate, TResult>(this IObservationListQuery<TSource> source, TAccumulate seed, AggregateQueryHandler<TAccumulate, TSource> func, Func<TAccumulate, TResult> resultSelector)
        {
            return Aggregate(source, () => seed, func, resultSelector);
        }

        public static AggregateQuery<TSource, TAccumulate, TAccumulate> Aggregate<TSource, TAccumulate>(this IObservationListQuery<TSource> source, Func<TAccumulate> seed, AggregateQueryHandler<TAccumulate, TSource> func)
        {
            return Aggregate(source, seed, func, Helper<TAccumulate>.Identity);
        }

        public static AggregateQuery<TSource, TAccumulate, TResult> Aggregate<TSource, TAccumulate, TResult>(this IObservationListQuery<TSource> source, Func<TAccumulate> seed, AggregateQueryHandler<TAccumulate, TSource> func, Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (seed == null)
                throw new ArgumentNullException(nameof(seed));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return new AggregateQuery<TSource, TAccumulate, TResult>(source, seed, func, resultSelector);
        }

        public static AggregateQuery<TSource, bool, bool> Contains<TSource>(this IObservationListQuery<TSource> source, TSource value)
        {
            return Contains(source, value, null);
        }

        public static AggregateQuery<TSource, bool, bool> Contains<TSource>(this IObservationListQuery<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            return Any(source, (l) => comparer.Equals(value, l));
        }

        public static AggregateQuery<TSource, bool, bool> Any<TSource>(this IObservationListQuery<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return Aggregate(source, false, new AggregateHelper<bool, TSource>((a, v) =>
            {
                if (a)
                    return (false, true);

                bool result = predicate(v);
                return (result, result);
            }).Handle);
        }

        public static AggregateQuery<TSource, bool, bool> All<TSource>(this IObservationListQuery<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return Aggregate(source, true, new AggregateHelper<bool, TSource>((a, v) =>
            {
                if (!a)
                    return (false, false);

                bool result = predicate(v);
                return (!result, result);
            }).Handle);
        }

        private static class Helper<T>
        {
            public static Func<T, T> Identity { get; } = (v) => v;
        }

        private sealed class AggregateHelper<TAccumulate, TSource>
        {
            private Func<TAccumulate, TSource, (bool, TAccumulate)> _func;

            public static AggregateHelper<TAccumulate, TSource> Create(Func<TAccumulate, TSource, TAccumulate> func)
            {
                if (func == null)
                    throw new ArgumentNullException(nameof(func));

                return new AggregateHelper<TAccumulate, TSource>((a, v) => (false, func(a, v)));
            }

            public AggregateHelper(Func<TAccumulate, TSource, (bool Stop, TAccumulate Accumulate)> func)
            {
                if (func == null)
                    throw new ArgumentNullException(nameof(func));

                _func = func;
            }

            public TAccumulate Handle(TAccumulate accumulate, TSource value, out bool stop)
            {
                TAccumulate v;

                (stop, v) = _func(accumulate, value);

                return v;
            }
        }
    }
}
