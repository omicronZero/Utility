using System;
using System.Collections.Generic;
using System.Text;
using Utility.DataStructures;
using System.Linq;

namespace Utility
{
    public static class GraphExtensions
    {
        public static bool Add<T>(this ISetCollection<Edge<T>> edges, T node1, T node2)
        {
            return edges.Add(new Edge<T>(node1, node2));
        }

        public static bool Contains<T>(this IEnumerable<Edge<T>> edges, T node1, T node2)
        {
            return edges.Contains(new Edge<T>(node1, node2));
        }

        public static bool Remove<T>(this ICollection<Edge<T>> edges, T node1, T node2)
        {
            return edges.Remove(new Edge<T>(node1, node2));
        }

        public static IGraph<TNode> AsReadonly<TNode>(this IGraph<TNode> instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new ReadonlyGraph<TNode>(instance);
        }

        public static IWeightedGraph<TNode, TWeight> AsReadonly<TNode, TWeight>(this IWeightedGraph<TNode, TWeight> instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new ReadonlyWeightedGraph<TNode, TWeight>(instance);
        }

        private sealed class ReadonlyGraph<TNode> : IGraph<TNode>
        {
            private readonly IGraph<TNode> _underlyingGraph;

            public ISetCollection<TNode> Nodes { get; }
            public ISetCollection<Edge<TNode>> Edges { get; }

            public bool IsDirected => _underlyingGraph.IsDirected;

            public ReadonlyGraph(IGraph<TNode> underlyingGraph)
            {
                if (underlyingGraph == null)
                    throw new ArgumentNullException(nameof(underlyingGraph));

                _underlyingGraph = underlyingGraph;

                Nodes = (ISetCollection<TNode>)underlyingGraph.Nodes.AsReadonlyAuto();
                Edges = (ISetCollection<Edge<TNode>>)underlyingGraph.Edges.AsReadonlyAuto();
            }

            public IEnumerable<TNode> GetIns(TNode node)
            {
                return _underlyingGraph.GetIns(node).AsReadonlyAuto();
            }

            public IEnumerable<TNode> GetOuts(TNode node)
            {
                return _underlyingGraph.GetOuts(node).AsReadonlyAuto();
            }
        }

        private sealed class ReadonlyWeightedGraph<TNode, TWeight> : IWeightedGraph<TNode, TWeight>
        {
            private readonly IWeightedGraph<TNode, TWeight> _underlyingWeightedGraph;

            public ICollection<TNode> Nodes { get; }
            public IDictionary<Edge<TNode>, TWeight> EdgeMap { get; }

            public ReadonlyWeightedGraph(IWeightedGraph<TNode, TWeight> underlyingWeightedGraph)
            {
                if (underlyingWeightedGraph == null)
                    throw new ArgumentNullException(nameof(underlyingWeightedGraph));

                _underlyingWeightedGraph = underlyingWeightedGraph;

                Nodes = (ICollection<TNode>)underlyingWeightedGraph.Nodes.AsReadonlyAuto();
                EdgeMap = (IDictionary<Edge<TNode>, TWeight>)underlyingWeightedGraph.EdgeMap.AsReadonlyDictionary();
            }

            public bool IsDirected => _underlyingWeightedGraph.IsDirected;
            public bool WeightsReadonly => true;

            public IDictionary<TNode, TWeight> GetIns(TNode node)
            {
                return _underlyingWeightedGraph.GetIns(node).AsReadonlyDictionary();
            }

            public IDictionary<TNode, TWeight> GetOuts(TNode node)
            {
                return _underlyingWeightedGraph.GetOuts(node).AsReadonlyDictionary();
            }
        }
    }
}
