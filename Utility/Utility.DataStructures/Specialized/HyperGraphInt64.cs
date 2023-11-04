using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Utility.Collections.Adapters;
using Utility.Collections.Tools;

namespace Utility.DataStructures.Specialized
{
    public class HyperGraphInt64 : IHyperGraph<long>
    {
        //TODO: merge IdMappingInt64 with the Id management in Utility.Experimental.Utils as soon as they are no longer experimental
        //TODO: change Id type from long to Id
        public bool IsDirected { get; }
        public NodeCollection Nodes { get; }
        public EdgeCollection Edges { get; }

        private readonly Dictionary<long, AttachedEdges> _nodes;
        private readonly HashSet<HyperEdge<long>> _edges;

        public HyperGraphInt64(bool isDirected)
        {
            IsDirected = isDirected;
            Nodes = new NodeCollection(this);
            Edges = new EdgeCollection(this);
            _nodes = new Dictionary<long, AttachedEdges>();
            _edges = new HashSet<HyperEdge<long>>();
        }

        public ICollection<HyperEdge<long>> GetInEdges(long node)
        {
            if (!_nodes.TryGetValue(node, out var attached))
                throw GraphDoesNotContainNode();

            return attached.InEdgesReadOnly;
        }

        public ICollection<HyperEdge<long>> GetOutEdges(long node)
        {
            if (!_nodes.TryGetValue(node, out var attached))
                throw GraphDoesNotContainNode();

            return attached.OutEdgesReadOnly;
        }

        ISetCollection<long> IHyperGraph<long>.Nodes => Nodes;

        ISetCollection<HyperEdge<long>> IHyperGraph<long>.Edges => Edges;

        IEnumerable<HyperEdge<long>> IHyperGraph<long>.GetInEdges(long node)
        {
            return GetInEdges(node);
        }

        IEnumerable<HyperEdge<long>> IHyperGraph<long>.GetOutEdges(long node)
        {
            return GetOutEdges(node);
        }

        private bool InternalAddEdge(HyperEdge<long> item)
        {
            if (InternalContainsEdge(item))
                return false;

            foreach (var node in item.Node1.Concat(item.Node2))
            {
                if (!_nodes.ContainsKey(node))
                    throw new ArgumentException("The graph does not contain all nodes of the edge.", nameof(item));
            }

            _edges.Add(item);

            foreach (var targetNode in item.Node2)
                _nodes[targetNode].InEdges.AttachedEdges.Add(item);

            foreach (var sourceNode in item.Node1)
                _nodes[sourceNode].OutEdges.AttachedEdges.Add(item);

            if (!IsDirected)
            {
                item = item.Reversed();
                foreach (var targetNode in item.Node2)
                    _nodes[targetNode].InEdges.AttachedEdges.Add(item);

                foreach (var sourceNode in item.Node1)
                    _nodes[sourceNode].OutEdges.AttachedEdges.Add(item);
            }

            return true;
        }

        private bool InternalRemoveEdge(HyperEdge<long> item)
        {
            bool InternalRemoveEdgeCore(HyperEdge<long> edge)
            {

                foreach (var node in item.Node1)
                    _nodes[node].OutEdges.AttachedEdges.Remove(edge);

                foreach (var node in item.Node2)
                    _nodes[node].InEdges.AttachedEdges.Remove(edge);

                return true;
            }

            if (!(_edges.Remove(item)
                || (item.Node1 != item.Node2 && _edges.Remove(item.Reversed()))))
                return false;

            if (IsDirected)
                return InternalRemoveEdgeCore(item);
            else
            {
                InternalRemoveEdgeCore(item);

                if (item.Node1 != item.Node2)
                    InternalRemoveEdgeCore(item.Reversed());

                return true;
            }
        }

        private bool InternalContainsEdge(HyperEdge<long> item)
        {
            if (IsDirected)
                return _edges.Contains(item);
            else
                return _edges.Contains(item) || (item.Node1 != item.Node2 && _edges.Contains(item.Reversed()));
        }

        private void InternalClearEdges()
        {
            _edges.Clear();

            foreach (var edgePair in _nodes.Values)
            {
                edgePair.InEdges.AttachedEdges.Clear();
                edgePair.OutEdges.AttachedEdges.Clear();
            }
        }

        private IEnumerator<HyperEdge<long>> InternalGetEdgeEnumerator()
        {
            return _edges.GetEnumerator();
        }

        private int InternalGetEdgeCount()
        {
            return _edges.Count;
        }

        private bool InternalRemoveNode(long item)
        {
            if (!_nodes.TryGetValue(item, out var attached))
                return false;

            foreach (var edge in attached.OutEdges)
                _edges.Remove(edge);

            foreach (var edge in attached.InEdges)
                _edges.Remove(edge);

            _nodes.Remove(item);

            attached.Detach();

            return true;
        }

        private bool InternalContainsNode(long item)
        {
            return _nodes.ContainsKey(item);
        }

        private void InternalClearNodes()
        {
            foreach (var attached in _nodes.Values)
            {
                attached.Detach();
            }

            _edges.Clear();
            _nodes.Clear();
        }

        private bool InternalAddNode(long item)
        {
            return _nodes.TryAdd(item, new AttachedEdges(this));
        }

        private IEnumerator<long> InternalGetNodeEnumerator()
        {
            return _nodes.Keys.GetEnumerator();
        }

        private int InternalGetNodeCount()
        {
            return _nodes.Count;
        }

        private static Exception GraphDoesNotContainNode()
        {
            return new ArgumentException("The current graph does not contain the indicated node.", "node");
        }

        public sealed class NodeCollection : CollectionBase<long>, ISetCollection<long>
        {
            //propagates its calls to the graph itself
            private readonly HyperGraphInt64 _graph;

            internal NodeCollection(HyperGraphInt64 graph)
            {
                _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            }

            public override int Count => _graph.InternalGetNodeCount();

            public override bool IsReadOnly => false;

            public override void Add(long item)
            {
                if (!_graph.InternalAddNode(item))
                    throw new ArgumentException("The graph contains this edge already.", nameof(item));
            }

            public bool TryAdd(long item)
            {
                return _graph.InternalAddNode(item);
            }

            public override void Clear()
            {
                _graph.InternalClearNodes();
            }

            public override bool Contains(long item)
            {
                return _graph.InternalContainsNode(item);
            }

            public override IEnumerator<long> GetEnumerator()
            {
                return _graph.InternalGetNodeEnumerator();
            }

            public override bool Remove(long item)
            {
                return _graph.InternalRemoveNode(item);
            }

            public override string ToString()
            {
                return $"{{{string.Join(", ", this)}}}";
            }

            bool ISetCollection<long>.Add(long item)
            {
                return TryAdd(item);
            }
        }

        public sealed class EdgeCollection : CollectionBase<HyperEdge<long>>, ISetCollection<HyperEdge<long>>
        {
            //propagates its calls to the graph itself
            private readonly HyperGraphInt64 _graph;

            internal EdgeCollection(HyperGraphInt64 graph)
            {
                _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            }

            public override int Count => _graph.InternalGetEdgeCount();

            public override bool IsReadOnly => false;

            public override void Add(HyperEdge<long> item)
            {
                if (!_graph.InternalAddEdge(item))
                    throw new ArgumentException("The graph contains this edge already.", nameof(item));
            }

            public bool TryAdd(HyperEdge<long> item)
            {
                return _graph.InternalAddEdge(item);
            }

            public override void Clear()
            {
                _graph.InternalClearEdges();
            }

            public override bool Contains(HyperEdge<long> item)
            {
                return _graph.InternalContainsEdge(item);
            }

            public override IEnumerator<HyperEdge<long>> GetEnumerator()
            {
                return _graph.InternalGetEdgeEnumerator();
            }

            public override bool Remove(HyperEdge<long> item)
            {
                return _graph.InternalRemoveEdge(item);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();

                int c = 0;
                if (_graph.IsDirected)
                {
                    foreach (var edge in this)
                    {
                        if (c++ > 0)
                            sb.Append(", ");

                        sb.Append('(');
                        edge.Node1.ToString(sb);
                        sb.Append(", ");
                        edge.Node2.ToString(sb);
                        sb.Append(')');
                    }
                }
                else
                {
                    foreach (var edge in this)
                    {
                        if (c++ > 0)
                            sb.Append(", ");

                        sb.Append('{');
                        edge.Node1.ToString(sb);
                        sb.Append(", ");
                        edge.Node2.ToString(sb);
                        sb.Append('}');
                    }
                }

                return sb.ToString();
            }

            bool ISetCollection<HyperEdge<long>>.Add(HyperEdge<long> item)
            {
                return TryAdd(item);
            }
        }
        private readonly struct AttachedEdges
        {
            public AttachedSet InEdges { get; }
            public AttachedSet OutEdges { get; }

            public ICollection<HyperEdge<long>> InEdgesReadOnly { get; }

            public ICollection<HyperEdge<long>> OutEdgesReadOnly { get; }

            public AttachedEdges(HyperGraphInt64 hyperGraph)
            {
                InEdges = new AttachedSet(hyperGraph);
                OutEdges = new AttachedSet(hyperGraph);

                InEdgesReadOnly = InEdges.AsReadonlyCollection();
                OutEdgesReadOnly = OutEdges.AsReadonlyCollection();
            }

            internal void Detach()
            {
                InEdges.Detach();
                OutEdges.Detach();
            }
        }

        private class AttachedSet : ReadOnlyCollectionBase<HyperEdge<long>>, ISetCollection<HyperEdge<long>>
        {
            private readonly HyperGraphInt64 _hyperGraph;
            internal HashSet<HyperEdge<long>> AttachedEdges { get; private set; }

            public AttachedSet(HyperGraphInt64 hyperGraph)
            {
                _hyperGraph = hyperGraph ?? throw new ArgumentNullException(nameof(hyperGraph));
                AttachedEdges = new HashSet<HyperEdge<long>>();
            }

            internal void Detach()
            {
                AttachedEdges = null;
            }

            private void ThrowDetached()
            {
                if (IsDetached)
                    throw new InvalidOperationException("The current instance is detached.");
            }

            public bool IsDetached => AttachedEdges == null;

            public override int Count
            {
                get
                {
                    ThrowDetached();
                    return AttachedEdges.Count;
                }
            }

            public override bool Contains(HyperEdge<long> item)
            {
                ThrowDetached();

                return AttachedEdges.Contains(item) ||
                    (!_hyperGraph.IsDirected && AttachedEdges.Contains(item.Reversed()));
            }

            public override void CopyTo(HyperEdge<long>[] array, int arrayIndex)
            {
                ThrowDetached();

                AttachedEdges.CopyTo(array, arrayIndex);
            }

            public override IEnumerator<HyperEdge<long>> GetEnumerator()
            {
                ThrowDetached();
                return AttachedEdges.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            bool ISetCollection<HyperEdge<long>>.Add(HyperEdge<long> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }
        }
    }
}
