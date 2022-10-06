using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility.Collections;
using Utility.Collections.Tools;
using Utility.DataStructures.Specialized;

namespace Utility.DataStructures
{
    public class WeightedGraph<TNode, TWeight> : IWeightedGraph<TNode, TWeight>
    {
        private readonly Graph<TNode> _graph;
        private readonly Dictionary<Edge<long>, TWeight> _weightMap;

        public EdgeDictionary Edges { get; }
        public NodeCollection Nodes { get; }

        public WeightedGraph(bool isDirected)
            : this(isDirected, null)
        { }

        public WeightedGraph(bool isDirected, IEqualityComparer<TNode> nodeComparer)
        {
            _graph = new Graph<TNode>(isDirected, nodeComparer);
            Nodes = new NodeCollection(this);
            Edges = new EdgeDictionary(this);
            _weightMap = new Dictionary<Edge<long>, TWeight>();
        }

        public IDictionary<Edge<TNode>, TWeight> EdgeMap => _graph.Edges.ViewAsDictionary((e) => _weightMap[_graph.MapEdge(e)]);

        public bool IsDirected => _graph.IsDirected;

        bool IWeightedGraph<TNode, TWeight>.WeightsReadonly => false;

        public IGraph<TNode> GetGraph() => _graph.AsReadonly();

        public NodeEdgesDictionary GetIns(TNode node)
        {
            return new NodeEdgesDictionary(this, _graph.GetIns(node));
        }

        public NodeEdgesDictionary GetOuts(TNode node)
        {
            return new NodeEdgesDictionary(this, _graph.GetOuts(node));
        }

        internal void AddEdge(Edge<long> edge, TWeight weight)
        {
            _graph.InnerGraph.Edges.Add(edge);
            _weightMap.Add(edge, weight);
        }

        internal bool RemoveEdge(Edge<long> idEdge)
        {
            if (!_graph.InnerGraph.Edges.Remove(idEdge))
                return false;

            _weightMap.Remove(idEdge);
            return true;
        }

        internal void ClearEdges()
        {
            _graph.Edges.Clear();
            _weightMap.Clear();
        }

        internal void SetEdgeWeight(Edge<long> idEdge, TWeight value)
        {
            _weightMap[idEdge] = value;
        }

        internal TWeight GetEdgeWeight(Edge<long> idEdge)
        {
            return _weightMap[idEdge];
        }

        internal bool TryGetWeight(Edge<long> edge, out TWeight weight)
        {
            return _weightMap.TryGetValue(edge, out weight);
        }

        ICollection<TNode> IWeightedGraph<TNode, TWeight>.Nodes => Nodes;
        IDictionary<Edge<TNode>, TWeight> IWeightedGraph<TNode, TWeight>.EdgeMap => EdgeMap;

        IDictionary<TNode, TWeight> IWeightedGraph<TNode, TWeight>.GetIns(TNode node) => GetIns(node);
        IDictionary<TNode, TWeight> IWeightedGraph<TNode, TWeight>.GetOuts(TNode node) => GetOuts(node);        

        public class NodeCollection : ISetCollection<TNode>
        {
            private readonly Graph<TNode>.NodeCollection _nodes;
            private readonly WeightedGraph<TNode, TWeight> _graph;

            internal NodeCollection(WeightedGraph<TNode, TWeight> graph)
            {
                _graph = graph ?? throw new ArgumentNullException(nameof(graph));
                _nodes = graph._graph.Nodes;
            }

            public int Count => _nodes.Count;

            public bool Add(TNode item)
            {
                return _nodes.Add(item);
            }

            public void Clear()
            {
                _nodes.Clear();
                _graph.ClearEdges();
            }

            public bool Contains(TNode item)
            {
                return _nodes.Contains(item);
            }

            public void CopyTo(TNode[] array, int arrayIndex)
            {
                _nodes.CopyTo(array, arrayIndex);
            }

            public IEnumerator<TNode> GetEnumerator()
            {
                return _nodes.GetEnumerator();
            }

            public bool Remove(TNode item)
            {
                if (!_graph._graph.TryMapNode(item, out var id))
                    return false;

                foreach (var other in _graph._graph.GetOutsInternal(id).ConnectedNodes)
                    _graph.RemoveEdge((id, other));

                foreach (var other in _graph._graph.GetInsInternal(id).ConnectedNodes)
                    _graph.RemoveEdge((other, id));

                return _nodes.Remove(item);
            }

            void ICollection<TNode>.Add(TNode item)
            {
                _nodes.Add(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _nodes.GetEnumerator();
            }

            bool ICollection<TNode>.IsReadOnly => false;
        }

        public class EdgeDictionary : IDictionary<Edge<TNode>, TWeight>
        {
            private readonly WeightedGraph<TNode, TWeight> _graph;

            internal EdgeDictionary(WeightedGraph<TNode, TWeight> graph)
            {
                _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            }

            public ICollection<Edge<TNode>> Keys => _graph._weightMap.Keys.SelectCollection(_graph._graph.UnmapEdge);

            public ICollection<TWeight> Values => _graph._weightMap.Values;

            public int Count => _graph._weightMap.Count;

            public TWeight this[Edge<TNode> edge]
            {
                get => _graph.GetEdgeWeight(_graph._graph.MapEdge(edge));
                set => _graph.SetEdgeWeight(_graph._graph.MapEdge(edge), value);
            }

            public bool Remove(Edge<TNode> edge)
            {
                return _graph.RemoveEdge(_graph._graph.MapEdge(edge));
            }

            public bool TryGetWeight(Edge<TNode> edge, out TWeight weight)
            {
                return _graph.TryGetWeight(_graph._graph.MapEdge(edge), out weight);
            }

            public bool ContainsKey(Edge<TNode> edge)
            {
                return _graph._graph.Edges.Contains(edge);
            }

            public void Add(Edge<TNode> edge, TWeight value)
            {
                _graph.AddEdge(_graph._graph.MapEdge(edge), value);
            }

            public void Clear()
            {
                _graph.ClearEdges();
            }

            public void CopyTo(KeyValuePair<Edge<TNode>, TWeight>[] array, int arrayIndex)
            {
                CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

                foreach (var item in this)
                    array[arrayIndex++] = item;
            }

            public IEnumerator<KeyValuePair<Edge<TNode>, TWeight>> GetEnumerator()
            {
                foreach (var kvp in _graph._weightMap)
                    yield return new KeyValuePair<Edge<TNode>, TWeight>(_graph._graph.UnmapEdge(kvp.Key), kvp.Value);
            }

            bool IDictionary<Edge<TNode>, TWeight>.TryGetValue(Edge<TNode> key, out TWeight value)
            {
                return TryGetWeight(key, out value);
            }

            bool ICollection<KeyValuePair<Edge<TNode>, TWeight>>.Contains(KeyValuePair<Edge<TNode>, TWeight> item)
            {
                return TryGetWeight(item.Key, out var weight) && EqualityComparer<TWeight>.Default.Equals(weight, item.Value);
            }

            bool ICollection<KeyValuePair<Edge<TNode>, TWeight>>.Remove(KeyValuePair<Edge<TNode>, TWeight> item)
            {
                return ((ICollection<KeyValuePair<Edge<TNode>, TWeight>>)this).Contains(item) && Remove(item.Key);
            }

            void ICollection<KeyValuePair<Edge<TNode>, TWeight>>.Add(KeyValuePair<Edge<TNode>, TWeight> item)
            {
                Add(item.Key, item.Value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            bool ICollection<KeyValuePair<Edge<TNode>, TWeight>>.IsReadOnly => false;
        }

        public class NodeEdgesDictionary : IDictionary<TNode, TWeight>
        {
            private readonly WeightedGraph<TNode, TWeight> _graph;
            private readonly Graph<TNode>.NodeEdgesCollection _nodes;

            public NodeEdgesDictionary(WeightedGraph<TNode, TWeight> graph, Graph<TNode>.NodeEdgesCollection nodes)
            {
                _graph = graph;
                _nodes = nodes;
            }

            private Edge<long> GetEdge(TNode other) => IsTarget ? (_graph._graph.MapNode(other), _nodes.NodeId) : (_nodes.NodeId, _graph._graph.MapNode(other));
            private Edge<long> GetEdge(long otherId) => IsTarget ? (otherId, _nodes.NodeId) : (_nodes.NodeId, otherId);

            public bool IsTarget => _nodes.IsTarget;
            public bool IsSource => _nodes.IsSource;

            public TWeight this[TNode node]
            {
                get => _graph.GetEdgeWeight(GetEdge(node));
                set => _graph.SetEdgeWeight(GetEdge(node), value);
            }

            public ICollection<TNode> Keys => _nodes.AsReadonlyCollection();

            public ICollection<TWeight> Weights => _nodes.SelectCollection((o) => this[o]);

            public int Count => _nodes.Count;

            public void Add(TNode node, TWeight weight)
            {
                if (!_nodes.Add(node))
                    throw new ArgumentException("The indicated edge already has a weight-assignment.");

                this[node] = weight;
            }

            public void Clear()
            {
                foreach (var node in _nodes.ConnectedNodes)
                {
                    _graph.RemoveEdge(GetEdge(node));
                }
            }

            public bool ContainsKey(TNode node)
            {
                return _nodes.Contains(node);
            }

            public void CopyTo(KeyValuePair<TNode, TWeight>[] array, int arrayIndex)
            {
                CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

                foreach (var kvp in this)
                    array[arrayIndex++] = kvp;
            }

            public IEnumerator<KeyValuePair<TNode, TWeight>> GetEnumerator()
            {
                foreach (var node in _nodes.ConnectedNodes)
                    yield return new KeyValuePair<TNode, TWeight>(_graph._graph.UnmapNode(node), _graph.GetEdgeWeight(GetEdge(node)));
            }

            public bool Remove(TNode node)
            {
                return _graph.RemoveEdge(GetEdge(node));
            }

            public bool TryGetWeight(TNode node, out TWeight weight)
            {
                return _graph.TryGetWeight(GetEdge(node), out weight);
            }

            bool ICollection<KeyValuePair<TNode, TWeight>>.IsReadOnly => false;

            void ICollection<KeyValuePair<TNode, TWeight>>.Add(KeyValuePair<TNode, TWeight> item)
            {
                Add(item.Key, item.Value);
            }

            bool ICollection<KeyValuePair<TNode, TWeight>>.Contains(KeyValuePair<TNode, TWeight> item)
            {
                return TryGetWeight(item.Key, out var weight) && EqualityComparer<TWeight>.Default.Equals(weight, item.Value);
            }

            bool ICollection<KeyValuePair<TNode, TWeight>>.Remove(KeyValuePair<TNode, TWeight> item)
            {
                if (!((IDictionary<TNode, TWeight>)this).Contains(item))
                    return false;

                return Remove(item.Key);
            }

            bool IDictionary<TNode, TWeight>.TryGetValue(TNode key, out TWeight value)
            {
                return TryGetWeight(key, out value);
            }

            ICollection<TWeight> IDictionary<TNode, TWeight>.Values => Weights;

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
