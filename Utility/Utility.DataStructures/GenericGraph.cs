using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.DataStructures
{
    public class GenericGraph<TNode> : IGraph<TNode>
    {
        private readonly IGraph<TNode> _graph;

        private NodeCollection _nodes;
        private EdgeCollection _edges;

        private readonly Dictionary<TNode, (NodeEdgesCollection Ins, NodeEdgesCollection Outs)> _nodeInsOuts;

        public GenericGraph(IGraph<TNode> graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            _nodeInsOuts = new Dictionary<TNode, (NodeEdgesCollection, NodeEdgesCollection)>();
        }

        public GenericGraph(bool isDirected)
            : this(new Graph<TNode>(isDirected))
        { }

        public GenericGraph(bool isDirected, IEqualityComparer<TNode> nodeComparer)
            : this(new Graph<TNode>(isDirected, nodeComparer))
        { }
        public bool IsDirected => _graph.IsDirected;

        public NodeCollection Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = CreateNodes() ?? throw new InvalidOperationException("The nodes-collection initializer must return a valid reference to an object.");
                    _nodes.Initialize(this);
                }

                return _nodes;
            }
        }

        public EdgeCollection Edges
        {
            get
            {
                if (_edges == null)
                {
                    //we demand nodes to be initialized first
                    _ = Nodes;
                    _edges = CreateEdges() ?? throw new InvalidOperationException(message: "The edges-collection initializer must return a valid reference to an object.");
                    _edges.Initialize(this);
                }

                return _edges;
            }
        }

        private void GetInsOuts(TNode node, out NodeEdgesCollection ins, out NodeEdgesCollection outs)
        {
            if (!_nodeInsOuts.TryGetValue(node, out var nodeInsOuts))
            {
                var innerIns = _graph.GetIns(node);
                var innerOuts = _graph.GetOuts(node);

                if (innerIns == null)
                    throw new InvalidOperationException(message: "The in-collection of the node returned from the underlying graph must not be null.");

                if (innerOuts == null)
                    throw new InvalidOperationException(message: "The out-collection of the node returned from the underlying graph must not be null.");

                if (innerIns is ICollection<TNode> innerInsCollection &&
                    innerOuts is ICollection<TNode> innerOutsCollection)
                {
                    CreateInOuts(node, out ins, out outs);

                    if (ins == null)
                        throw new InvalidOperationException(message: "The in-edges collection initializer must return a valid reference to an object or both, the in-edges and the out-edges collection must be null.");

                    if (outs == null)
                        throw new InvalidOperationException(message: "The out-edges collection initializer must return a valid reference to an object or both, the in-edges and the out-edges collection must be null.");

                    ins.Initialize(this, node, innerInsCollection, true);
                    outs.Initialize(this, node, innerOutsCollection, false);
                }
                else
                {
                    ins = outs = null;
                }

                _nodeInsOuts.Add(node, (ins, outs));
            }

            (ins, outs) = nodeInsOuts;
        }

        public IEnumerable<TNode> GetIns(TNode node)
        {
            GetInsOuts(node, out var value, out _);

            return value;
        }

        public IEnumerable<TNode> GetOuts(TNode node)
        {
            GetInsOuts(node, out _, out var value);

            return value;
        }

        protected virtual bool AddNode(TNode node)
        {
            return _graph.Nodes.Add(node);
        }

        protected virtual bool RemoveNode(TNode node)
        {
            if (!_graph.Nodes.Remove(node))
                return false;

            _nodeInsOuts.Remove(node);
            return true;
        }

        protected virtual void ClearNodes()
        {
            _graph.Nodes.Clear();
        }

        protected virtual bool AddEdge(Edge<TNode> edge)
        {
            return _graph.Edges.Add(edge);
        }

        protected virtual bool RemoveEdge(Edge<TNode> edge)
        {
            return _graph.Edges.Remove(edge);
        }

        protected virtual void RemoveEdges(IEnumerable<Edge<TNode>> edges)
        {
            foreach (var e in edges.ToArray())
                RemoveEdge(e);
        }

        protected virtual void ClearEdges()
        {
            _graph.Edges.Clear();
        }

        protected virtual NodeCollection CreateNodes()
        {
            return new NodeCollection();
        }

        protected virtual EdgeCollection CreateEdges()
        {
            return new EdgeCollection();
        }

        protected virtual void CreateInOuts(TNode node, out NodeEdgesCollection ins, out NodeEdgesCollection outs)
        {
            ins = new NodeEdgesCollection();
            outs = new NodeEdgesCollection();
        }

        private static Exception UninitializedException() => new InvalidOperationException("The current collection is not bound to a graph and can thus not be modified.");

        ISetCollection<TNode> IGraph<TNode>.Nodes => Nodes;

        ISetCollection<Edge<TNode>> IGraph<TNode>.Edges => Edges;

        public class NodeCollection : ISetCollection<TNode>
        {
            internal GenericGraph<TNode> Graph { get; private set; }

            internal void Initialize(GenericGraph<TNode> graph)
            {
                Graph = graph;
            }

            private void CheckInitialized()
            {
                if (Graph == null)
                    throw UninitializedException();
            }

            protected virtual bool OnRemoveNode(TNode item)
            {
                return Graph.RemoveNode(item);
            }

            protected virtual bool OnAddNode(TNode item)
            {
                return Graph.AddNode(item);
            }

            protected virtual void OnClearNodes()
            {
                Graph.ClearNodes();
            }

            protected virtual bool ContainsCore(TNode item) => Graph._graph.Nodes.Contains(item);

            public int Count
            {
                get
                {
                    CheckInitialized();
                    return Graph._graph.Nodes.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    CheckInitialized();
                    return Graph._graph.Nodes.IsReadOnly;
                }
            }

            public bool Add(TNode item)
            {
                CheckInitialized();
                return OnAddNode(item);
            }

            public void Clear()
            {
                CheckInitialized();
                OnClearNodes();
            }

            public bool Contains(TNode item)
            {
                CheckInitialized();
                return ContainsCore(item);
            }

            public void CopyTo(TNode[] array, int arrayIndex)
            {
                CheckInitialized();
            }

            public IEnumerator<TNode> GetEnumerator()
            {
                CheckInitialized();
                return Graph._graph.Nodes.GetEnumerator();
            }

            public bool Remove(TNode item)
            {
                CheckInitialized();
                return OnRemoveNode(item);
            }

            void ICollection<TNode>.Add(TNode item)
            {
                CheckInitialized();
                Add(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class EdgeCollection : ISetCollection<Edge<TNode>>
        {
            internal GenericGraph<TNode> Graph { get; private set; }

            internal void Initialize(GenericGraph<TNode> graph)
            {
                Graph = graph;
            }

            private void CheckInitialized()
            {
                if (Graph == null)
                    throw UninitializedException();
            }

            protected virtual bool OnRemoveEdge(Edge<TNode> item)
            {
                return Graph.RemoveEdge(item);
            }

            protected virtual bool OnAddEdge(Edge<TNode> item)
            {
                return Graph.AddEdge(item);
            }

            protected virtual void OnClearEdges()
            {
                Graph.ClearNodes();
            }

            protected virtual bool ContainsCore(Edge<TNode> item) => Graph._graph.Edges.Contains(item);

            public int Count
            {
                get
                {
                    CheckInitialized();
                    return Graph._graph.Edges.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    CheckInitialized();
                    return Graph._graph.Edges.IsReadOnly;
                }
            }

            public bool Add(Edge<TNode> item)
            {
                CheckInitialized();
                return OnAddEdge(item);
            }

            public void Clear()
            {
                CheckInitialized();
                OnClearEdges();
            }

            public bool Contains(Edge<TNode> item)
            {
                CheckInitialized();
                return ContainsCore(item);
            }

            public void CopyTo(Edge<TNode>[] array, int arrayIndex)
            {
                CheckInitialized();
            }

            public IEnumerator<Edge<TNode>> GetEnumerator()
            {
                CheckInitialized();
                return Graph._graph.Edges.GetEnumerator();
            }

            public bool Remove(Edge<TNode> item)
            {
                CheckInitialized();
                return OnRemoveEdge(item);
            }

            void ICollection<Edge<TNode>>.Add(Edge<TNode> item)
            {
                CheckInitialized();
                Add(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class NodeEdgesCollection : ISetCollection<TNode>
        {
            private bool _isTarget;
            internal GenericGraph<TNode> Graph { get; private set; }
            internal ICollection<TNode> _innerEnumerable;
            private TNode _node;

            internal void Initialize(GenericGraph<TNode> graph, TNode node, ICollection<TNode> collection, bool isTarget)
            {
                _isTarget = isTarget;
                Graph = graph;
                _node = node;
                _innerEnumerable = collection;
            }

            protected ICollection<TNode> InnerEnumerable
            {
                get
                {
                    CheckInitialized();
                    return _innerEnumerable;
                }
            }

            protected TNode Node
            {
                get
                {
                    CheckInitialized();
                    return _node;
                }
            }

            private void CheckInitialized()
            {
                if (Graph == null)
                    throw UninitializedException();
            }

            protected virtual bool OnAddNode(Edge<TNode> edge)
            {
                return Graph.AddEdge(edge);
            }

            protected virtual bool OnRemoveNode(Edge<TNode> edge)
            {
                return Graph.RemoveEdge(edge);
            }

            protected virtual void OnClearNodes()
            {
                Graph.RemoveEdges(this.Select(GetEdge));
            }

            protected virtual bool ContainsCore(Edge<TNode> edge) => Graph._graph.Edges.Contains(edge);

            public bool IsSource
            {
                get
                {
                    CheckInitialized();
                    return !_isTarget;
                }
            }

            public bool IsTarget
            {
                get
                {
                    CheckInitialized();
                    return _isTarget;
                }
            }

            public int Count
            {
                get
                {
                    CheckInitialized();
                    return _innerEnumerable.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    CheckInitialized();
                    return _innerEnumerable.IsReadOnly;
                }
            }

            private Edge<TNode> GetEdge(TNode other) => IsSource ? (_node, other) : (other, _node);

            public bool Add(TNode item)
            {
                CheckInitialized();
                return OnAddNode(GetEdge(item));
            }

            void ICollection<TNode>.Add(TNode item)
            {
                Add(item);
            }

            public void Clear()
            {
                CheckInitialized();
                OnClearNodes();
            }

            public bool Contains(TNode item)
            {
                CheckInitialized();
                return ContainsCore(GetEdge(item));
            }

            public void CopyTo(TNode[] array, int arrayIndex)
            {
                CheckInitialized();
                _innerEnumerable.CopyTo(array, arrayIndex);
            }

            public bool Remove(TNode item)
            {
                CheckInitialized();
                return OnRemoveNode(GetEdge(item));
            }

            public IEnumerator<TNode> GetEnumerator()
            {
                CheckInitialized();
                return _innerEnumerable.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
