using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Utility.Collections.Tools;
using Utility.DataStructures.Specialized;

namespace Utility.DataStructures
{
    public class Graph<TNode> : IGraph<TNode>
    {
        internal GraphInt64 InnerGraph { get; }
        private readonly IdMappingInt64<NodeDescriptor> _idMap;
        public NodeCollection Nodes { get; }
        public EdgeCollection Edges { get; }

        public Graph(bool isDirected)
            : this(isDirected, null)
        { }

        public Graph(bool isDirected, IEqualityComparer<TNode> nodeComparer)
        {
            InnerGraph = new GraphInt64(isDirected);
            _idMap = new IdMappingInt64<NodeDescriptor>(new NodeComparer(nodeComparer));
            Nodes = new NodeCollection(this);
            Edges = new EdgeCollection(this);
        }

        public bool IsDirected => InnerGraph.IsDirected;

        internal long MapNode(TNode node)
        {
            if (!_idMap.TryGet(new NodeDescriptor(node), out var id))
                throw new ArgumentException("The indicated object is not a node of the graph.", nameof(node));

            return id;
        }

        internal Edge<long> MapEdge(Edge<TNode> edge)
        {
            if (!TryMapNode(edge.Node1, out var id1))
                throw new ArgumentException("The first endpoint of the edge is not a node of the graph.", nameof(edge));
            if (!TryMapNode(edge.Node2, out var id2))
                throw new ArgumentException("The second endpoint of the edge is not a node of the graph.", nameof(edge));

            return (id1, id2);
        }

        internal bool TryMapNode(TNode node, out long id)
        {
            return _idMap.TryGet(new NodeDescriptor(node), out id);
        }

        internal TNode UnmapNode(long id)
        {
            return UnmapNodeDescriptor(id).Value;
        }

        private NodeDescriptor UnmapNodeDescriptor(long id)
        {
            if (!_idMap.TryGetById(id, out var node))
                throw new InvalidOperationException("Implementation error.");

            return node;
        }

        internal Edge<TNode> UnmapEdge(Edge<long> idEdge)
        {
            return (UnmapNode(idEdge.Node1), UnmapNode(idEdge.Node2));
        }

        private void ClearNodeMap()
        {
            _idMap.Clear(true);
        }

        private bool AddNodeMapping(TNode node, out long id)
        {
            if (_idMap.TryGet(new NodeDescriptor(node), out id))
                return false;

            if (!_idMap.TryAdd(new NodeDescriptor(node), out id))
                throw new InvalidOperationException("Implementation error.");

            return true;
        }

        private bool ContainsNodeMapping(TNode item)
        {
            return _idMap.Contains(new NodeDescriptor(item));
        }

        private void RemoveNodeMapping(TNode item)
        {
            if (!_idMap.TryRemove(new NodeDescriptor(item)))
                throw new ArgumentException("The indicated item is not a member of the graph.");
        }

        public NodeEdgesCollection GetIns(TNode node)
        {
            return GetInsInternal(MapNode(node));
        }

        internal NodeEdgesCollection GetInsInternal(long id)
        {
            var descriptor = UnmapNodeDescriptor(id);

            if (descriptor.Ins == null)
            {
                descriptor.Ins = new NodeEdgesCollection(this, InnerGraph.GetIns(id));
                _idMap.Reassign(id, descriptor, out _);
            }

            return descriptor.Ins;
        }

        public NodeEdgesCollection GetOuts(TNode node)
        {
            return GetOutsInternal(MapNode(node));
        }

        internal NodeEdgesCollection GetOutsInternal(long id)
        {
            var descriptor = UnmapNodeDescriptor(id);

            if (descriptor.Outs == null)
            {
                descriptor.Outs = new NodeEdgesCollection(this, InnerGraph.GetOuts(id));
                _idMap.Reassign(id, descriptor, out _);
            }

            return descriptor.Outs;
        }

        IEnumerable<TNode> IGraph<TNode>.GetIns(TNode node)
        {
            return GetIns(node);
        }

        IEnumerable<TNode> IGraph<TNode>.GetOuts(TNode node)
        {
            return GetOuts(node);
        }

        ISetCollection<TNode> IGraph<TNode>.Nodes => Nodes;

        ISetCollection<Edge<TNode>> IGraph<TNode>.Edges => Edges;

        public override string ToString()
        {
            return $"({Nodes}, {Edges})";
        }

        public class NodeCollection : ISetCollection<TNode>
        {
            private readonly Graph<TNode> _graph;

            internal NodeCollection(Graph<TNode> graph)
            {
                _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            }

            private GraphInt64.NodeCollection InnerNodes => _graph.InnerGraph.Nodes;

            public int Count => InnerNodes.Count;

            public bool Add(TNode item)
            {
                if (!_graph.AddNodeMapping(item, out var id))
                    return false;

                _graph.InnerGraph.Nodes.Add(id);

                return true;
            }

            public void Clear()
            {
                _graph.InnerGraph.Nodes.Clear();
                _graph.ClearNodeMap();
            }

            public bool Contains(TNode item)
            {
                return _graph.ContainsNodeMapping(item);
            }

            public void CopyTo(TNode[] array, int arrayIndex)
            {
                CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

                foreach (var item in _graph._idMap.GetValues())
                    array[arrayIndex++] = item.Value;
            }

            public IEnumerator<TNode> GetEnumerator()
            {
                return _graph._idMap.GetValues().Select((s) => s.Value).GetEnumerator();
            }

            public bool Remove(TNode item)
            {
                if (!_graph.TryMapNode(item, out var id))
                    return false;

                if (InnerNodes.Remove(id)) //should actually be always true
                    _graph.RemoveNodeMapping(item);

                return true;
            }

            public override string ToString()
            {
                return $"{{{string.Join(", ", this)}}}";
            }

            bool ICollection<TNode>.IsReadOnly => false;

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            void ICollection<TNode>.Add(TNode item)
            {
                Add(item);
            }
        }

        public class EdgeCollection : ISetCollection<Edge<TNode>>
        {
            private readonly Graph<TNode> _graph;

            internal EdgeCollection(Graph<TNode> graph)
            {
                _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            }

            private GraphInt64.EdgeCollection InnerEdges => _graph.InnerGraph.Edges;

            public int Count => InnerEdges.Count;

            public bool Add(Edge<TNode> item)
            {
                return InnerEdges.TryAdd(_graph.MapEdge(item));
            }

            public void Clear()
            {
                InnerEdges.Clear();
            }

            public bool Contains(Edge<TNode> item)
            {
                if (!_graph.TryMapEdge(item, out var edge))
                    return false;

                return InnerEdges.Contains(edge);
            }

            public IEnumerator<Edge<TNode>> GetEnumerator()
            {
                //as we're not buffering the T-based edges, this is quite expensive
                foreach (var (id1, id2) in InnerEdges)
                    yield return new Edge<TNode>(_graph.UnmapNode(id1), _graph.UnmapNode(id2));
            }

            public bool Remove(Edge<TNode> item)
            {
                if (!_graph.TryMapEdge(item, out var edge))
                    return false;

                return InnerEdges.Remove(edge);
            }

            public void CopyTo(Edge<TNode>[] array, int arrayIndex)
            {
                CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

                foreach (var item in this)
                    array[arrayIndex++] = item;
            }

            public override string ToString()
            {
                if (_graph.IsDirected)
                    return $"{{{string.Join(", ", this.Select((s) => $"({s.Node1}, {s.Node2})"))}}}";
                else
                    return $"{{{string.Join(", ", this.Select((s) => $"{{{s.Node1}, {s.Node2}}}"))}}}";
            }

            bool ICollection<Edge<TNode>>.IsReadOnly => false;

            void ICollection<Edge<TNode>>.Add(Edge<TNode> item)
            {
                Add(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private bool TryMapEdge(Edge<TNode> item, out Edge<long> edge)
        {
            if (!TryMapNode(item.Node1, out var id1) || !TryMapNode(item.Node2, out var id2))
            {
                edge = default;
                return false;
            }

            edge = (id1, id2);
            return true;
        }

        public class NodeEdgesCollection : ICollection<TNode>
        {
            private readonly Graph<TNode> _graph;
            internal GraphInt64.NodeEdgeCollection ConnectedNodes { get; }

            internal NodeEdgesCollection(Graph<TNode> graph, GraphInt64.NodeEdgeCollection connectedNodes)
            {
                _graph = graph ?? throw new ArgumentNullException(nameof(graph));
                ConnectedNodes = connectedNodes ?? throw new ArgumentNullException(nameof(connectedNodes));
            }

            public bool IsTarget => ConnectedNodes.IsTarget;

            public bool IsSource => ConnectedNodes.IsSource;

            public bool IsDetached => ConnectedNodes.IsDetached;

            public int Count => ConnectedNodes.Count;

            internal long NodeId => ConnectedNodes.Node;

            public bool Add(TNode item)
            {
                ThrowDetached();

                return ConnectedNodes.TryAdd(_graph.MapNode(item));
            }

            public void Clear()
            {
                ThrowDetached();

                ConnectedNodes.Clear();
            }

            public bool Contains(TNode item)
            {
                ThrowDetached();

                if (!_graph.TryMapNode(item, out var id))
                    return false;

                return ConnectedNodes.Contains(id);
            }

            public IEnumerator<TNode> GetEnumerator()
            {
                foreach (var node in ConnectedNodes)
                    yield return _graph.UnmapNode(node);
            }

            public bool Remove(TNode item)
            {
                if (!_graph.TryMapNode(item, out var id))
                    return false;

                return ConnectedNodes.Remove(id);
            }

            public void CopyTo(TNode[] array, int arrayIndex)
            {
                CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

                foreach (var item in this)
                    array[arrayIndex++] = item;
            }

            bool ICollection<TNode>.IsReadOnly => false;

            void ICollection<TNode>.Add(TNode item)
            {
                Add(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            public override string ToString()
            {
                return IsDetached ? "" : $"{{{string.Join(", ", this)}}}";
            }

            private void ThrowDetached()
            {
                if (IsDetached)
                    throw new InvalidOperationException("The current instance is detached.");
            }
        }

        private struct NodeDescriptor
        {
            public TNode Value { get; set; }
            public NodeEdgesCollection Outs { get; set; }
            public NodeEdgesCollection Ins { get; set; }

            public NodeDescriptor(TNode value, NodeEdgesCollection outs, NodeEdgesCollection ins)
            {
                Value = value;
                Outs = outs;
                Ins = ins;
            }

            public NodeDescriptor(TNode value)
            {
                Value = value;
                Outs = null;
                Ins = null;
            }
        }

        private sealed class NodeComparer : IEqualityComparer<NodeDescriptor>
        {
            public IEqualityComparer<TNode> ValueComparer { get; }

            public NodeComparer(IEqualityComparer<TNode> valueComparer)
            {
                ValueComparer = valueComparer ?? EqualityComparer<TNode>.Default;
            }

            public bool Equals(NodeDescriptor x, NodeDescriptor y)
            {
                return ValueComparer.Equals(x.Value, y.Value);
            }

            public int GetHashCode(NodeDescriptor obj)
            {
                return ValueComparer.GetHashCode(obj.Value);
            }
        }
    }
}
