using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utility.Collections;
using Utility.Collections.Tools;
using Utility.DataStructures;

namespace Utility
{
    public class GenericHyperGraph<TNode> : IHyperGraph<TNode>
    {
        private readonly IHyperGraph<TNode> _graph;

        private NodeCollection _nodes;
        private EdgeCollection _edges;

        public GenericHyperGraph(IHyperGraph<TNode> hyperGraph)
        {
            _graph = hyperGraph ?? throw new ArgumentNullException(nameof(hyperGraph));
        }

        public GenericHyperGraph(bool isDirected)
            : this(new HyperGraph<TNode>(isDirected))
        { }

        public GenericHyperGraph(bool isDirected, IEqualityComparer<TNode> nodeComparer)
            : this(new HyperGraph<TNode>(isDirected, nodeComparer))
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
                    //nodes must be initialized first
                    _ = Nodes;
                    _edges = CreateEdges() ?? throw new InvalidOperationException("The nodes-collection initializer must return a valid reference to an object.");
                    _edges.Initialize(this);
                }

                return _edges;
            }
        }

        protected virtual NodeCollection CreateNodes() => new NodeCollection();
        protected virtual EdgeCollection CreateEdges() => new EdgeCollection();

        public IEnumerable<HyperEdge<TNode>> GetInEdges(TNode node)
        {
            return _graph.GetInEdges(node).AsReadonlyAuto();
        }

        public IEnumerable<HyperEdge<TNode>> GetOutEdges(TNode node)
        {
            return _graph.GetOutEdges(node).AsReadonlyAuto();
        }

        IEnumerable<HyperEdge<TNode>> IHyperGraph<TNode>.GetInEdges(TNode node) => GetInEdges(node);
        IEnumerable<HyperEdge<TNode>> IHyperGraph<TNode>.GetOutEdges(TNode node) => GetOutEdges(node);

        ISetCollection<HyperEdge<TNode>> IHyperGraph<TNode>.Edges => Edges;
        ISetCollection<TNode> IHyperGraph<TNode>.Nodes => Nodes;

        protected virtual bool AddNode(TNode node)
        {
            return _graph.Nodes.Add(node);
        }

        protected virtual bool RemoveNode(TNode node)
        {
            return _graph.Nodes.Remove(node);
        }

        protected virtual void ClearNodes()
        {
            _graph.Nodes.Clear();
        }

        protected virtual bool RemoveEdge(HyperEdge<TNode> item)
        {
            return _graph.Edges.Remove(item);
        }

        protected virtual bool AddEdge(HyperEdge<TNode> item)
        {
            return _graph.Edges.Add(item);
        }

        protected virtual void ClearEdges()
        {
            _graph.Edges.Clear();
        }

        private static Exception UninitializedException() => new InvalidOperationException("The current collection is not bound to a graph and can thus not be modified.");

        //TODO: we have a lot of redundancy between here and GenericGraph.NodeCollection, can we reasonably refactor?
        public class NodeCollection : ISetCollection<TNode>
        {
            internal GenericHyperGraph<TNode> Graph { get; private set; }

            internal void Initialize(GenericHyperGraph<TNode> graph)
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

            internal void InternalOnNodeAdded(TNode item)
            {
                OnNodeAdded(item);
            }

            internal void InternalOnNodeRemoved(TNode node)
            {
                OnNodeRemoved(node);
            }

            internal void InternalOnNodesCleared(TNode[] nodes)
            {
                OnNodesCleared(Array.AsReadOnly(nodes));
            }

            protected virtual void OnNodeAdded(TNode item)
            { }

            protected virtual void OnNodeRemoved(TNode node)
            { }

            protected virtual void OnNodesCleared(IList<TNode> nodes)
            { }

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
                Graph.Nodes.CopyTo(array, arrayIndex);
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

        public class EdgeCollection : ISetCollection<HyperEdge<TNode>>
        {
            internal GenericHyperGraph<TNode> Graph { get; private set; }

            internal void Initialize(GenericHyperGraph<TNode> graph)
            {
                Graph = graph;
            }

            private void CheckInitialized()
            {
                if (Graph == null)
                    throw UninitializedException();
            }

            protected virtual bool OnRemoveEdge(HyperEdge<TNode> item)
            {
                return Graph.RemoveEdge(item);
            }

            protected virtual bool OnAddEdge(HyperEdge<TNode> item)
            {
                return Graph.AddEdge(item);
            }

            protected virtual void OnClearEdges()
            {
                Graph.ClearEdges();
            }

            protected virtual bool ContainsCore(HyperEdge<TNode> item) => Graph._graph.Edges.Contains(item);

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

            public bool Add(HyperEdge<TNode> item)
            {
                CheckInitialized();
                return OnAddEdge(item);
            }

            public void Clear()
            {
                CheckInitialized();
                OnClearEdges();
            }

            public bool Contains(HyperEdge<TNode> item)
            {
                CheckInitialized();
                return ContainsCore(item);
            }

            public void CopyTo(HyperEdge<TNode>[] array, int arrayIndex)
            {
                CheckInitialized();
                Graph.Edges.CopyTo(array, arrayIndex);
            }

            public IEnumerator<HyperEdge<TNode>> GetEnumerator()
            {
                CheckInitialized();
                return Graph._graph.Edges.GetEnumerator();
            }

            public bool Remove(HyperEdge<TNode> item)
            {
                CheckInitialized();
                return OnRemoveEdge(item);
            }

            void ICollection<HyperEdge<TNode>>.Add(HyperEdge<TNode> item)
            {
                CheckInitialized();
                Add(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
