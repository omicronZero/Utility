using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Utility.Collections.Tools;
using Utility.DataStructures;
using Utility.DataStructures.Specialized;

namespace Utility
{
    public class HyperGraph<TNode> : IHyperGraph<TNode>
    {
        private readonly IdMappingInt64<NodeDescriptor> _idMap;

        internal HyperGraphInt64 InnerGraph { get; }
        public NodeCollection Nodes { get; }
        public EdgeCollection Edges { get; }

        public HyperGraph(bool isDirected)
            : this(isDirected, null)
        { }

        public HyperGraph(bool isDirected, IEqualityComparer<TNode> nodeComparer)
        {
            InnerGraph = new HyperGraphInt64(isDirected);
            _idMap = new IdMappingInt64<NodeDescriptor>(new NodeComparer(nodeComparer));

            Nodes = new NodeCollection(this);
            Edges = new EdgeCollection(this);
        }
        public bool IsDirected => InnerGraph.IsDirected;

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

        internal long MapNode(TNode node)
        {
            if (!_idMap.TryGet(new NodeDescriptor(node), out var id))
                throw new ArgumentException("The indicated object is not a node of the hyper-graph.", nameof(node));

            return id;
        }

        internal Set<long> MapNodeSet(Set<TNode> nodes)
        {
            if (!TryMapNodeSet(nodes, out var idSet))
                throw new ArgumentException("Not all indicated objects are nodes of the hyper-graph.", nameof(nodes));

            return idSet;
        }

        internal HyperEdge<long> MapEdge(HyperEdge<TNode> edge)
        {
            if (!TryMapNodeSet(edge.Node1, out var idset1))
                throw new ArgumentException("Not all first endpoints of the edge are nodes of the hyper-graph.", nameof(edge));
            if (!TryMapNodeSet(edge.Node2, out var idset2))
                throw new ArgumentException("Not all second endpoints of the edge are nodes of the hyper-graph.", nameof(edge));

            return (idset1, idset2);
        }

        internal bool TryMapEdge(HyperEdge<TNode> edge, out HyperEdge<long> mappedEdge)
        {
            if (!TryMapNodeSet(edge.Node1, out var idset1)
                || !TryMapNodeSet(edge.Node2, out var idset2))
            {
                mappedEdge = default;
                return false;
            }

            mappedEdge = (idset1, idset2);

            return true;
        }

        internal bool TryMapNode(TNode node, out long id)
        {
            return _idMap.TryGet(new NodeDescriptor(node), out id);
        }

        internal bool TryMapNodeSet(Set<TNode> nodeSet, out Set<long> idSet)
        {
            if (nodeSet.IsEmpty)
            {
                idSet = Set<long>.Empty;
                return true;
            }
            else if (nodeSet.IsSingleton)
            {
                if (!TryMapNode(nodeSet.GetSingleton(), out long id))
                {
                    idSet = default;
                    return false;
                }

                idSet = new Set<long>(id);
                return true;
            }
            else
            {
                HashSet<long> mapped = new HashSet<long>();

                foreach (var nd in nodeSet)
                {
                    if (!TryMapNode(nd, out long id))
                    {
                        idSet = default;
                        return false;
                    }

                    mapped.Add(id);
                }

                idSet = new Set<long>(mapped);
                return true;
            }
        }

        internal TNode UnmapNode(long id)
        {
            return UnmapNodeDescriptor(id).Value;
        }

        internal Set<TNode> UnmapNodeSet(Set<long> id)
        {
            return Set<long>.Map(id, UnmapNode);
        }

        private NodeDescriptor UnmapNodeDescriptor(long id)
        {
            if (!_idMap.TryGetById(id, out var node))
                throw new InvalidOperationException("Implementation error.");

            return node;
        }

        internal HyperEdge<TNode> UnmapEdge(HyperEdge<long> idEdge)
        {
            return (UnmapNodeSet(idEdge.Node1), UnmapNodeSet(idEdge.Node2));
        }

        internal NodeEdgesCollection GetOutsInternal(long id)
        {
            var descriptor = UnmapNodeDescriptor(id);

            if (descriptor.Outs == null)
            {
                descriptor.Outs = new NodeEdgesCollection(this, InnerGraph.GetOutEdges(id));
                _idMap.Reassign(id, descriptor, out _);
            }

            return descriptor.Outs;
        }

        internal NodeEdgesCollection GetInsInternal(long id)
        {
            var descriptor = UnmapNodeDescriptor(id);

            if (descriptor.Ins == null)
            {
                descriptor.Ins = new NodeEdgesCollection(this, InnerGraph.GetInEdges(id));
                _idMap.Reassign(id, descriptor, out _);
            }

            return descriptor.Ins;
        }

        public NodeEdgesCollection GetInEdges(TNode node)
        {
            return GetInsInternal(MapNode(node));
        }

        public NodeEdgesCollection GetOutEdges(TNode node)
        {
            return GetOutsInternal(MapNode(node));
        }

        ISetCollection<TNode> IHyperGraph<TNode>.Nodes => Nodes;

        ISetCollection<HyperEdge<TNode>> IHyperGraph<TNode>.Edges => Edges;


        IEnumerable<HyperEdge<TNode>> IHyperGraph<TNode>.GetInEdges(TNode node)
        {
            return GetInEdges(node);
        }

        IEnumerable<HyperEdge<TNode>> IHyperGraph<TNode>.GetOutEdges(TNode node)
        {
            return GetOutEdges(node);
        }

        public class NodeCollection : ISetCollection<TNode>
        {
            private readonly HyperGraph<TNode> _hyperGraph;

            internal NodeCollection(HyperGraph<TNode> hyperGraph)
            {
                _hyperGraph = hyperGraph ?? throw new ArgumentNullException(nameof(hyperGraph));
            }

            private HyperGraphInt64.NodeCollection InnerNodes => _hyperGraph.InnerGraph.Nodes;

            public int Count => InnerNodes.Count;

            bool ICollection<TNode>.IsReadOnly => false;

            public bool Add(TNode item)
            {
                if (!_hyperGraph.AddNodeMapping(item, out var id))
                    return false;

                _hyperGraph.InnerGraph.Nodes.Add(id);

                return true;
            }

            public void Clear()
            {
                _hyperGraph.InnerGraph.Nodes.Clear();
                _hyperGraph.ClearNodeMap();
            }

            public bool Contains(TNode item)
            {
                return _hyperGraph.ContainsNodeMapping(item);
            }

            public void CopyTo(TNode[] array, int arrayIndex)
            {
                CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

                foreach (var item in _hyperGraph._idMap.GetValues())
                    array[arrayIndex++] = item.Value;
            }

            public IEnumerator<TNode> GetEnumerator()
            {
                return _hyperGraph._idMap.GetValues().Select((s) => s.Value).GetEnumerator();
            }

            public override string ToString()
            {
                return $"{{{string.Join(", ", this)}}}";
            }

            public bool Remove(TNode item)
            {
                if (!_hyperGraph.TryMapNode(item, out var id))
                    return false;

                if (InnerNodes.Remove(id)) //should actually be always true
                    _hyperGraph.RemoveNodeMapping(item);

                return true;
            }

            void ICollection<TNode>.Add(TNode item)
            {
                Add(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class EdgeCollection : ISetCollection<HyperEdge<TNode>>
        {
            private readonly HyperGraph<TNode> _hyperGraph;

            internal EdgeCollection(HyperGraph<TNode> hyperGraph)
            {
                _hyperGraph = hyperGraph ?? throw new ArgumentNullException(nameof(hyperGraph));
            }

            private HyperGraphInt64.EdgeCollection InnerEdges => _hyperGraph.InnerGraph.Edges;

            public int Count => InnerEdges.Count;

            bool ICollection<HyperEdge<TNode>>.IsReadOnly => false;

            public bool Add(HyperEdge<TNode> item)
            {
                var mappedEdge = _hyperGraph.MapEdge(item);

                return InnerEdges.TryAdd(mappedEdge);
            }

            public void Clear()
            {
                InnerEdges.Clear();
            }

            public bool Contains(HyperEdge<TNode> item)
            {
                return _hyperGraph.TryMapEdge(item, out var mappedEdge)
                    && InnerEdges.Contains(mappedEdge);
            }

            public void CopyTo(HyperEdge<TNode>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<HyperEdge<TNode>> GetEnumerator()
            {
                foreach(var mappedEdge in InnerEdges)
                    yield return _hyperGraph.UnmapEdge(mappedEdge);
            }

            public bool Remove(HyperEdge<TNode> item)
            {
                if (!_hyperGraph.TryMapEdge(item, out var mappedEdge))
                    return false;

                return InnerEdges.Remove(mappedEdge);
            }

            void ICollection<HyperEdge<TNode>>.Add(HyperEdge<TNode> item)
            {
                Add(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class NodeEdgesCollection : ReadOnlyCollectionBase<HyperEdge<TNode>>
        {
            private readonly HyperGraph<TNode> _hyperGraph;
            private readonly ICollection<HyperEdge<long>> _nodeEdges;

            internal NodeEdgesCollection(HyperGraph<TNode> hyperGraph, ICollection<HyperEdge<long>> nodeEdges)
            {
                _hyperGraph = hyperGraph ?? throw new ArgumentNullException(nameof(hyperGraph));
                _nodeEdges = nodeEdges ?? throw new ArgumentNullException(nameof(nodeEdges));
            }

            public override int Count => _nodeEdges.Count;

            public override bool Contains(HyperEdge<TNode> item)
            {
                if (!_hyperGraph.TryMapEdge(item, out var mappedEdge))
                    return false;

                return _nodeEdges.Contains(mappedEdge);
            }

            public override IEnumerator<HyperEdge<TNode>> GetEnumerator()
            {
                foreach (var edge in _nodeEdges)
                    yield return _hyperGraph.UnmapEdge(edge);
            }
        }

        private struct NodeDescriptor
        {
            public TNode Value { get; }
            public NodeEdgesCollection Ins { get; set; }
            public NodeEdgesCollection Outs { get; set; }

            public NodeDescriptor(TNode value, NodeEdgesCollection outs, NodeEdgesCollection ins)
            {
                Value = value;
                Outs = outs;
                Ins = ins;
            }

            public NodeDescriptor(TNode value)
            {
                Value = value;
                Ins = null;
                Outs = null;
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
