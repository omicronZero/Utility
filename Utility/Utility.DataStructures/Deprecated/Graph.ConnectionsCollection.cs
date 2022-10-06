//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;

//namespace Utility
//{
//    public partial class Graph<TNode>
//    {
//        public sealed class ConnectionsCollection : ICollection<TNode>
//        {
//            private readonly Graph<TNode> _graph;
//            private readonly int _node;
//            private readonly List<int> _connectedNodes;

//            public bool Detached { get; private set; }
//            public bool IsTarget { get; }

//            internal ConnectionsCollection(Graph<TNode> graph, int node, bool isTarget)
//            {
//                if (graph == null)
//                    throw new ArgumentNullException(nameof(graph));

//                _graph = graph;
//                _node = node;
//                _connectedNodes = new List<int>();
//                IsTarget = isTarget;
//            }

//            internal int NodeIndex
//            {
//                get { return _node; }
//            }

//            public TNode Node
//            {
//                get
//                {
//                    ThrowDetached();

//                    TNode nd;
//                    _graph.TryGetNode(_node, out nd);
//                    return nd;
//                }
//            }

//            //TODO: make struct
//            internal IEnumerable<int> GetConnectedNodes()
//            {
//                return _connectedNodes;
//            }

//            internal void Detach()
//            {
//                Detached = true;
//            }

//            public int Count
//            {
//                get
//                {
//                    ThrowDetached();
//                    return _connectedNodes.Count;
//                }
//            }

//            bool ICollection<TNode>.IsReadOnly => false;

//            private Edge<TNode> GetEdge(TNode node)
//            {
//                return IsTarget ? new Edge<TNode>(node, Node) : new Edge<TNode>(Node, node);
//            }

//            void ICollection<TNode>.Add(TNode item)
//            {
//                Add(item);
//            }

//            internal void InternalAdd(int node)
//            {
//                _connectedNodes.Add(node);
//            }

//            internal void InternalClear()
//            {
//                _connectedNodes.Clear();
//            }

//            internal bool InternalRemove(int node)
//            {
//                return _connectedNodes.Remove(node);
//            }

//            private void ThrowDetached()
//            {
//                if (Detached)
//                    throw new InvalidOperationException("Collection of connected nodes has been detached from the graph.");
//            }

//            public bool Add(TNode item)
//            {
//                ThrowDetached();

//                return _graph.AddEdge(GetEdge(item));
//            }

//            public void Clear()
//            {
//                ThrowDetached();

//                ClearCore(true);
//            }

//            internal void ClearCore(bool decreaseEdgeCount)
//            {
//                if (_graph.IsDirected)
//                {
//                    if (IsTarget)
//                        _graph.ClearConnectionsTo(this, decreaseEdgeCount);
//                    else
//                        _graph.ClearConnectionsFrom(this, decreaseEdgeCount);
//                }
//                else
//                {
//                    _graph.ClearConnectionsTo(this, decreaseEdgeCount);
//                    _graph.ClearConnectionsFrom(this, decreaseEdgeCount);
//                }
//            }

//            public bool Contains(TNode item)
//            {
//                ThrowDetached();

//                return _graph.ContainsEdge(GetEdge(item));
//            }

//            public void CopyTo(TNode[] array, int arrayIndex)
//            {
//                if (array == null)
//                    throw new ArgumentNullException(nameof(array));
//                if (arrayIndex < 0)
//                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Non-negative array index expected.");
//                if (array.Length + arrayIndex > Count)
//                    throw new ArgumentException("Amount of entries in the collection exceeds indicated array range.");

//                foreach (TNode t in this)
//                    array[arrayIndex++] = t;
//            }

//            public override string ToString()
//            {
//                if (Detached)
//                    return "";

//                StringBuilder stringBuilder = new StringBuilder();

//                stringBuilder.Append('{');

//                bool notfirst = false;

//                foreach (TNode nd in this)
//                {
//                    if (notfirst)
//                        stringBuilder.Append(',').Append(' ');

//                    notfirst = true;

//                    stringBuilder.Append(nd);
//                }

//                stringBuilder.Append('}');

//                return stringBuilder.ToString();
//            }

//            //TODO: make struct
//            public IEnumerator<TNode> GetEnumerator()
//            {
//                foreach (int s in _connectedNodes)
//                {
//                    if (!_graph.TryGetNode(s, out TNode n))
//                        throw new InvalidOperationException("Graph integrity violated.");

//                    yield return n;
//                }
//            }

//            public bool Remove(TNode item)
//            {
//                ThrowDetached();

//                return _graph.RemoveEdge(GetEdge(item));
//            }

//            IEnumerator IEnumerable.GetEnumerator()
//            {
//                return GetEnumerator();
//            }
//        }
//    }
//}
