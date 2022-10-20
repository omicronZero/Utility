using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Workflow.Collections.Adapters;

namespace Utility.DataStructures.Specialized
{
    public class GraphInt64 : IGraph<long>
    {
        //TODO: merge IdMappingInt64 with the Id management in Utility.Experimental.Utils as soon as they are no longer experimental

        //TODO: share HashSet on undirected nodes' edge collection
        public NodeCollection Nodes { get; }

        public EdgeCollection Edges { get; }

        public bool IsDirected { get; }

        private readonly Dictionary<long, NodeEdgePair> _nodes;
        private readonly HashSet<Edge<long>> _edges;

        public GraphInt64(bool isDirected)
        {
            IsDirected = isDirected;
            _nodes = new Dictionary<long, NodeEdgePair>();
            _edges = new HashSet<Edge<long>>();
            Nodes = new NodeCollection(this);
            Edges = new EdgeCollection(this);
        }

        public NodeEdgeCollection GetIns(long node)
        {
            if (!_nodes.TryGetValue(node, out var pair))
                throw GraphDoesNotContainNode();

            return pair.Ins;
        }

        public NodeEdgeCollection GetOuts(long node)
        {
            if (!_nodes.TryGetValue(node, out var pair))
                throw GraphDoesNotContainNode();

            return pair.Outs;
        }

        public override string ToString()
        {
            return $"({Nodes}, {Edges})";
        }

        ISetCollection<long> IGraph<long>.Nodes => Nodes;

        ISetCollection<Edge<long>> IGraph<long>.Edges => Edges;

        IEnumerable<long> IGraph<long>.GetIns(long node) => GetIns(node);

        IEnumerable<long> IGraph<long>.GetOuts(long node) => GetOuts(node);

        private bool InternalAddEdge(Edge<long> item)
        {
            if (InternalContainsEdge(item))
                return false;

            _edges.Add(item);

            var src = _nodes[item.Node1];
            var dst = _nodes[item.Node2];

            src.Outs.AttachedNodes.Add(item.Node2);
            dst.Ins.AttachedNodes.Add(item.Node1);

            if (!IsDirected && item.Node1 != item.Node2)
            {
                dst.Outs.AttachedNodes.Add(item.Node1);
                src.Ins.AttachedNodes.Add(item.Node2);
            }

            return true;
        }

        private bool InternalRemoveEdge(Edge<long> item)
        {
            bool InternalRemoveEdgeCore(Edge<long> edge)
            {
                if (!_edges.Remove(edge))
                    return false;

                var (src, dst) = edge;
                var ndSrc = _nodes[src];
                var ndDst = _nodes[dst];

                ndSrc.Outs.AttachedNodes.Remove(dst);
                ndDst.Ins.AttachedNodes.Remove(src);

                if (!IsDirected)
                {
                    ndSrc.Ins.AttachedNodes.Remove(dst);
                    ndDst.Outs.AttachedNodes.Remove(src);
                }

                return true;
            }

            if (IsDirected)
                return InternalRemoveEdgeCore(item);
            else
            {
                if (InternalRemoveEdgeCore(item))
                    return true;

                if (item.Node1 != item.Node2 && InternalRemoveEdgeCore(item.Reversed()))
                    return true;

                return false;
            }
        }

        private bool InternalContainsEdge(Edge<long> item)
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
                edgePair.Ins.AttachedNodes.Clear();
                edgePair.Outs.AttachedNodes.Clear();
            }
        }

        private IEnumerator<Edge<long>> InternalGetEdgeEnumerator()
        {
            return _edges.GetEnumerator();
        }

        private int InternalGetEdgeCount()
        {
            return _edges.Count;
        }

        private bool InternalRemoveNode(long item)
        {
            if (!_nodes.TryGetValue(item, out var pair))
                return false;

            foreach (var dst in pair.Outs.AttachedNodes)
            {
                _edges.Remove((item, dst));
                _nodes[dst].Ins.AttachedNodes.Remove(item);
            }

            foreach (var src in pair.Ins.AttachedNodes)
            {
                _edges.Remove((src, item));
                _nodes[src].Outs.AttachedNodes.Remove(item);
            }

            _nodes.Remove(item);

            pair.Ins.Detach();
            pair.Outs.Detach();

            return true;
        }

        private bool InternalContainsNode(long item)
        {
            return _nodes.ContainsKey(item);
        }

        private void InternalClearNodes()
        {
            foreach (var nd in _nodes.Values)
            {
                nd.Ins.Detach();
                nd.Outs.Detach();
            }

            _nodes.Clear();
        }

        private bool InternalAddNode(long item)
        {
            if (_nodes.ContainsKey(item))
                return false;

            _nodes.Add(item, new NodeEdgePair(
                new NodeEdgeCollection(this, item, true),
                new NodeEdgeCollection(this, item, false)));

            return true;
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

        public sealed class EdgeCollection : CollectionBase<Edge<long>>, ISetCollection<Edge<long>>
        {
            //propagates its calls to the graph itself
            private readonly GraphInt64 _graph;

            internal EdgeCollection(GraphInt64 graph)
            {
                _graph = graph ?? throw new ArgumentNullException(nameof(graph));
            }

            public override int Count => _graph.InternalGetEdgeCount();

            public override bool IsReadOnly => false;

            public override void Add(Edge<long> item)
            {
                if (!_graph.InternalAddEdge(item))
                    throw new ArgumentException("The graph contains this edge already.", nameof(item));
            }

            public bool TryAdd(Edge<long> item)
            {
                return _graph.InternalAddEdge(item);
            }

            public override void Clear()
            {
                _graph.InternalClearEdges();
            }

            public override bool Contains(Edge<long> item)
            {
                return _graph.InternalContainsEdge(item);
            }

            public override IEnumerator<Edge<long>> GetEnumerator()
            {
                return _graph.InternalGetEdgeEnumerator();
            }

            public override bool Remove(Edge<long> item)
            {
                return _graph.InternalRemoveEdge(item);
            }

            public override string ToString()
            {
                if (_graph.IsDirected)
                    return $"{{{string.Join(", ", this.Select((s) => $"({s.Node1}, {s.Node2})"))}}}";
                else
                    return $"{{{string.Join(", ", this.Select((s) => $"{{{s.Node1}, {s.Node2}}}"))}}}";
            }

            bool ISetCollection<Edge<long>>.Add(Edge<long> item)
            {
                return TryAdd(item);
            }
        }

        public sealed class NodeCollection : CollectionBase<long>, ISetCollection<long>
        {
            //propagates its calls to the graph itself
            private readonly GraphInt64 _graph;

            internal NodeCollection(GraphInt64 graph)
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

        public sealed class NodeEdgeCollection : CollectionBase<long>, ISetCollection<long>
        {
            //the current class contains only wrapper methods propagated to the edge collection
            //AttachedNodes is to be kept synchronized with Edges
            public long Node { get; }

            public bool IsTarget { get; }

            internal HashSet<long> AttachedNodes { get; private set; }
            private GraphInt64 _graph;

            internal NodeEdgeCollection(GraphInt64 graph, long node, bool isTarget)
            {
                _graph = graph ?? throw new ArgumentNullException(nameof(graph));

                Node = node;
                IsTarget = isTarget;
                AttachedNodes = new HashSet<long>();
            }

            public bool IsDetached => AttachedNodes == null;

            public bool IsSource => !IsTarget;

            public override int Count
            {
                get
                {
                    ThrowDetached();
                    return AttachedNodes.Count;
                }
            }

            public override bool IsReadOnly => false;

            public override void Add(long item)
            {
                ThrowDetached();

                if (IsTarget)
                    _graph.Edges.Add((item, Node));
                else
                    _graph.Edges.Add((Node, item));
            }

            public bool TryAdd(long item)
            {
                ThrowDetached();

                if (IsTarget)
                    return _graph.Edges.TryAdd((item, Node));
                else
                    return _graph.Edges.TryAdd((Node, item));
            }

            public override void Clear()
            {
                ThrowDetached();

                if (IsTarget)
                {
                    foreach (long v in AttachedNodes.ToArray())
                        _graph.Edges.Remove((v, Node));
                }
                else
                {
                    foreach (long v in AttachedNodes.ToArray())
                        _graph.Edges.Remove((Node, v));
                }
            }

            public override bool Contains(long item)
            {
                ThrowDetached();

                if (IsTarget)
                {
                    return _graph.Edges.Contains((item, Node));
                }
                else
                {
                    return _graph.Edges.Contains((Node, item));
                }
            }

            public override IEnumerator<long> GetEnumerator()
            {
                ThrowDetached();

                return AttachedNodes.GetEnumerator();
            }

            public override bool Remove(long item)
            {
                ThrowDetached();

                if (IsTarget)
                {
                    return _graph.Edges.Remove((item, Node));
                }
                else
                {
                    return _graph.Edges.Remove((Node, item));
                }
            }

            internal void Detach()
            {
                AttachedNodes = null;
                _graph = null;
            }

            private void ThrowDetached()
            {
                if (IsDetached)
                    throw new InvalidOperationException("The current instance is detached.");
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

        private struct NodeEdgePair
        {
            public NodeEdgeCollection Ins { get; }
            public NodeEdgeCollection Outs { get; }

            public NodeEdgePair(NodeEdgeCollection ins, NodeEdgeCollection outs)
            {
                Ins = ins;
                Outs = outs;
            }
        }
    }
}
