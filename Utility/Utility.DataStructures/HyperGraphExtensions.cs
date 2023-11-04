using System;
using System.Collections.Generic;
using System.Text;
using Utility.Collections;

namespace Utility
{
    public static class HyperGraphExtensions
    {
        public static bool Add<T>(this ISetCollection<HyperEdge<T>> set, Set<T> node1, Set<T> node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Add((node1, node2));
        }

        public static bool Add<T>(this ISetCollection<HyperEdge<T>> set, Set<T> node1, T node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Add((node1, node2));
        }

        public static bool Add<T>(this ISetCollection<HyperEdge<T>> set, T node1, Set<T> node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Add((node1, node2));
        }

        public static bool Add<T>(this ISetCollection<HyperEdge<T>> set, T node1, T node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Add((node1, node2));
        }

        public static bool Remove<T>(this ISetCollection<HyperEdge<T>> set, Set<T> node1, Set<T> node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Remove((node1, node2));
        }

        public static bool Remove<T>(this ISetCollection<HyperEdge<T>> set, Set<T> node1, T node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Remove((node1, node2));
        }

        public static bool Remove<T>(this ISetCollection<HyperEdge<T>> set, T node1, Set<T> node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Remove((node1, node2));
        }

        public static bool Remove<T>(this ISetCollection<HyperEdge<T>> set, T node1, T node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Remove((node1, node2));
        }

        public static bool Contains<T>(this ISetCollection<HyperEdge<T>> set, Set<T> node1, Set<T> node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Contains((node1, node2));
        }

        public static bool Contains<T>(this ISetCollection<HyperEdge<T>> set, Set<T> node1, T node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Contains((node1, node2));
        }

        public static bool Contains<T>(this ISetCollection<HyperEdge<T>> set, T node1, Set<T> node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Contains((node1, node2));
        }

        public static bool Contains<T>(this ISetCollection<HyperEdge<T>> set, T node1, T node2)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.Contains((node1, node2));
        }
    }
}
