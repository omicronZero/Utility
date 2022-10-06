//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Utility
//{
//    public partial class Graph<TNode> : IGraph<TNode>
//    {
//        private readonly Dictionary<Edge<int?>, object> _edgeDictionary;
//        //NodeIndexPair maps indices to their corresponding node and vice versa. Either the node is set or the index (iif the index is null, the node is set)
//        //the dictionary thus contains two entries where (node, null) obtains (null, index) and (null, index) returns (node, null) where null is the default value of node or index
//        //this is done to reduce the number of dictionaries
//        private readonly Dictionary<NodeIndexPair, NodeIndexPair> _nodeMap;

//        //keeps track of the number of nodes that have been added since the creation of the graph
//        //the index will be incremented upon adding items and otherwise be left untouched

//        //TODO: set to long instead, change all related nodes
//        private int _currentIndex;
//        private int _edgeCount;

//        public bool IsDirected { get; }

//        public NodeCollection Nodes { get; }
//        public EdgeCollection Edges { get; }

//        ICollection<TNode> IGraph<TNode>.Nodes => Nodes;
//        ICollection<Edge<TNode>> IGraph<TNode>.Edges => Edges;

//        public Graph(bool isDirected)
//        {
//            //TODO: use a key comparer on edge that maps the sequential indices to other values!
//            Nodes = new NodeCollection(this);
//            Edges = new EdgeCollection(this);

//            _edgeDictionary = new Dictionary<Edge<int?>, object>();
//            _nodeMap = new Dictionary<NodeIndexPair, NodeIndexPair>();

//            IsDirected = isDirected;
//        }

//        public bool TryGetIns(TNode node, out ConnectionsCollection ins)
//        {
//            int i;

//            if (!GetNodeIndex(node, false, out i, out _))
//            {
//                ins = null;
//                return false;
//            }

//            var e = new Edge<int?>(null, i);

//            if (!_edgeDictionary.TryGetValue(e, out object v))
//            {
//                ins = null;
//                return false;
//            }

//            ins = (ConnectionsCollection)v;
//            return true;
//        }

//        public bool TryGetOuts(TNode node, out ConnectionsCollection outs)
//        {
//            int i;

//            if (!GetNodeIndex(node, false, out i, out _))
//            {
//                outs = null;
//                return false;
//            }

//            var e = new Edge<int?>(i, null);

//            if (!_edgeDictionary.TryGetValue(e, out object v))
//            {
//                outs = null;
//                return false;
//            }

//            outs = (ConnectionsCollection)v;
//            return true;
//        }

//        public ConnectionsCollection GetIns(TNode node)
//        {
//            int n = GetNodeIndex(node, false, out _);
//            var e = new Edge<int?>(null, n);
//            ConnectionsCollection c;

//            if (!_edgeDictionary.TryGetValue(e, out object v))
//            {
//                c = new ConnectionsCollection(this, n, true);
//                _edgeDictionary.Add(e, c);
//            }
//            else
//                c = (ConnectionsCollection)v;

//            return c;
//        }

//        public ConnectionsCollection GetOuts(TNode node)
//        {
//            int n = GetNodeIndex(node, false, out _);
//            var e = new Edge<int?>(n, null);
//            ConnectionsCollection c;

//            if (!_edgeDictionary.TryGetValue(e, out object v))
//            {
//                c = new ConnectionsCollection(this, n, false);
//                _edgeDictionary.Add(e, c);
//            }
//            else
//                c = (ConnectionsCollection)v;

//            return c;
//        }

//        IEnumerable<TNode> IGraph<TNode>.GetIns(TNode node)
//        {
//            return GetIns(node);
//        }

//        IEnumerable<TNode> IGraph<TNode>.GetOuts(TNode node)
//        {
//            return GetOuts(node);
//        }

//        internal int GetNodeIndex(TNode node, bool create, out bool created)
//        {
//            int ind;

//            if (!GetNodeIndex(node, create, out ind, out created))
//                throw new ArgumentException("Node is not a member of the collection.", nameof(node));

//            return ind;
//        }

//        internal bool TryGetNodeIndex(TNode node, out int index)
//        {
//            NodeIndexPair p;
//            if (_nodeMap.TryGetValue(node, out p))
//            {
//                index = p.Index.Value;
//                return true;
//            }
//            else
//            {
//                index = 0;
//                return false;
//            }
//        }

//        internal bool TryGetNode(int index, out TNode node)
//        {
//            NodeIndexPair p;
//            if (_nodeMap.TryGetValue(index, out p))
//            {
//                node = p.Node;
//                return true;
//            }
//            else
//            {
//                node = default;
//                return false;
//            }
//        }

//        internal bool GetNodeIndex(TNode node, bool create, out int index, out bool created)
//        {
//            created = false;
//            if (!TryGetNodeIndex(node, out index))
//            {
//                if (!create)
//                {
//                    return false;
//                }

//                index = _currentIndex++;
//                _nodeMap.Add(node, index);
//                _nodeMap.Add(index, node);
//                created = true;
//            }
//            return true;
//        }

//        protected virtual bool AddNode(TNode node)
//        {
//            bool created;
//            GetNodeIndex(node, true, out _, out created);
//            return created;
//        }

//        protected virtual bool RemoveNode(TNode node)
//        {
//            int nodeIndex;

//            if (!GetNodeIndex(node, false, out nodeIndex, out _))
//                return false;

//            object cobj;
//            ConnectionsCollection c2;
//            bool edgesWereDecreased = false;

//            //if there is a collection that stores nodes that have <node> as a destination...
//            if (_edgeDictionary.TryGetValue(new Edge<int?>(null, nodeIndex), out cobj))
//            {
//                c2 = (ConnectionsCollection)cobj;

//                c2.ClearCore(true);
//                edgesWereDecreased = true;

//                ////... iterate through all node indices that are connected to <node>
//                //foreach (int source in c2.GetConnectedNodes())
//                //{
//                //    //remove connection from the "connected to"-collection of the target item
//                //    if (_edgeDictionary.TryGetValue(new Edge<int?>(source, null), out cobj))
//                //    {
//                //        var c1 = (ConnectionsCollection)cobj;
//                //        c1.InternalRemove(nodeIndex);
//                //    }

//                //    //remove edge from edge dictionary
//                //    _edgeDictionary.Remove(new Edge<int?>(nodeIndex, source));
//                //}

//                //detach and clear "connected from"-collection itself
//                c2.Detach();
//                //c2.InternalClear();
//            }

//            //deal with the other direction. If there is a collection that stores nodes from <node>...
//            if (_edgeDictionary.TryGetValue(new Edge<int?>(nodeIndex, null), out cobj))
//            {
//                c2 = (ConnectionsCollection)cobj;
//                c2.ClearCore(IsDirected || !edgesWereDecreased);
//                //... iterate through all node indices that are connected from <node>
//                //foreach (int destination in c2.GetConnectedNodes())
//                //{
//                //    //remove connection from the "connected from"-collection of the target item
//                //    if (_edgeDictionary.TryGetValue(new Edge<int?>(null, destination), out cobj))
//                //    {
//                //        var c1 = (ConnectionsCollection)cobj;
//                //        c1.InternalRemove(nodeIndex);
//                //    }

//                //    //remove edge from edge dictionary
//                //    _edgeDictionary.Remove(new Edge<int?>(destination, nodeIndex));
//                //}

//                ////remove connected to
//                c2.Detach();
//                //c2.InternalClear();
//            }

//            _nodeMap.Remove(new NodeIndexPair(node));
//            _nodeMap.Remove(new NodeIndexPair(nodeIndex));

//            return true;
//        }

//        protected virtual void ClearNodes()
//        {
//            foreach (var e in _edgeDictionary)
//                (e.Value as ConnectionsCollection)?.Detach();

//            _edgeDictionary.Clear();
//            _nodeMap.Clear();
//            _edgeDictionary.Clear();
//            _edgeCount = 0;
//            _currentIndex = 0;
//        }

//        protected virtual bool AddEdge(Edge<TNode> edge)
//        {
//            int n1 = GetNodeIndex(edge.Node1, false, out _);
//            int n2 = GetNodeIndex(edge.Node2, false, out _);

//            //check whether the edge is already a part of the graph
//            if (_edgeDictionary.ContainsKey(new Edge<int?>(n1, n2)))
//                return false;

//            //Add it if not (we use the edge dictionary as a hash set, here, by setting the value to null)
//            _edgeDictionary.Add(new Edge<int?>(n1, n2), null);

//            //we may need to add a reversed edge if we're undirected and haven't added the edge (i.e., we have an edge v <-> v)
//            if (!IsDirected && n1 != n2)
//                _edgeDictionary.Add(new Edge<int?>(n2, n1), null);

//            ConnectionsCollection c;

//            //we add the edge to both nodes' edge collections
//            //note that InternalAdd will also increase the count of the nodes on the nodes' connected-nodes collections
//            c = GetIns(edge.Node2);
//            c.InternalAdd(n1);
//            c = GetOuts(edge.Node1);
//            c.InternalAdd(n2);

//            //and, if undirected and necessary, also the reverse direction
//            if (n1 != n2 && !IsDirected)
//            {
//                c = GetIns(edge.Node1);
//                c.InternalAdd(n2);
//                c = GetOuts(edge.Node2);
//                c.InternalAdd(n1);
//            }

//            //we have added exactly one edge
//            _edgeCount++;
//            return true;
//        }

//        protected virtual bool RemoveEdge(Edge<TNode> edge)
//        {
//            if (!TryGetNodeIndex(edge.Node1, out int n1)
//                || !TryGetNodeIndex(edge.Node2, out int n2))
//                return false;

//            if (!_edgeDictionary.Remove(new Edge<int?>(n1, n2)))
//                return false;

//            //we need to remove the reverse direction if we're undirected and we have some edge u -> v with u != v
//            if (n1 != n2 && !IsDirected)
//                _edgeDictionary.Remove(new Edge<int?>(n2, n1));

//            object cobj;
//            ConnectionsCollection c;

//            //actually this should always hold. We query the list of out-going connections of node 1 and remove node 2 from it
//            //InternalRemove also adjusts the node count of that collection
//            if (_edgeDictionary.TryGetValue(new Edge<int?>(n1, null), out cobj))
//            {
//                c = (ConnectionsCollection)cobj;
//                c.InternalRemove(n2);
//            }

//            //here, we query the in-going connections of node 2 and remove node 1 from it
//            if (_edgeDictionary.TryGetValue(new Edge<int?>(null, n2), out cobj))
//            {
//                c = (ConnectionsCollection)cobj;
//                c.InternalRemove(n1);
//            }

//            //and the reverse direction if necessary
//            if (n1 != n2 && !IsDirected)
//            {
//                if (_edgeDictionary.TryGetValue(new Edge<int?>(n2, null), out cobj))
//                {
//                    c = (ConnectionsCollection)cobj;
//                    c.InternalRemove(n1);
//                }

//                if (_edgeDictionary.TryGetValue(new Edge<int?>(null, n1), out cobj))
//                {
//                    c = (ConnectionsCollection)cobj;
//                    c.InternalRemove(n2);
//                }
//            }

//            //we removed a single edge
//            _edgeCount--;

//            return true;
//        }

//        protected virtual void ClearEdges()
//        {
//            //we clear and detach all inner collections.
//            //Note that we only clear the edges, not the nodes, here.
//            //Therefore, only the inner list gets cleared and the node collection is not detached.
//            foreach (var v in _edgeDictionary.Values)
//            {
//                if (!(v is ConnectionsCollection c))
//                    continue;

//                c.InternalClear();
//            }

//            _edgeDictionary.Clear();
//            _edgeCount = 0;
//        }

//        private int NodeCount
//        {
//            get { return _nodeMap.Count / 2; } //for each node, we add two entries to the node map (index -> node, node -> index)
//        }

//        private bool ContainsNode(TNode node)
//        {
//            return _nodeMap.ContainsKey(node);
//        }

//        private void CopyNodesTo(TNode[] array, int arrayIndex)
//        {
//            Util.ValidateNamedRange(array, arrayIndex, Nodes.Count, nameof(array), nameof(arrayIndex));

//            foreach (var n in _nodeMap)
//            {
//                if (n.Key.Index != null)
//                    array[arrayIndex++] = n.Value.Node;
//            }
//        }

//        private IEnumerator<TNode> GetNodesEnumerator()
//        {
//            return _nodeMap.Where((s) => s.Value.Index == null).Select((s) => s.Value.Node).GetEnumerator();
//        }

//        private void CopyEdgesTo(Edge<TNode>[] array, int arrayIndex)
//        {
//            Util.ValidateNamedRange(array, arrayIndex, EdgeCount);

//            //we do not have an enumerable, thus we use the enumerator we implemented
//            using IEnumerator<Edge<TNode>> enr = GetEdgeEnumerator();

//            while (enr.MoveNext())
//                array[arrayIndex++] = enr.Current;
//        }

//        private int EdgeCount
//        {
//            get { return _edgeCount; }
//        }

//        private bool ContainsEdge(Edge<TNode> item)
//        {
//            int n1;
//            int n2;

//            if (!(GetNodeIndex(item.Node1, false, out n1, out _) && GetNodeIndex(item.Node2, false, out n2, out _)))
//                return false;

//            return _edgeDictionary.ContainsKey(new Edge<int?>(n1, n2));
//        }

//        //TODO: make struct
//        private IEnumerator<Edge<TNode>> GetEdgeEnumerator()
//        {
//            //we return each edge only once --> we need to take care of this in the undirected scenario because we decided to store both directions
//            HashSet<Edge<int>> edgeSet = IsDirected ? null : new HashSet<Edge<int>>();

//            foreach (var e in _edgeDictionary)
//            {
//                if (e.Key.Node1 == null || e.Key.Node2 == null)
//                    continue;

//                //for undirected graphs, the edgeSet is set to a value and shall contain all nodes that will not appear to keep redundancy from occuring
//                if (edgeSet != null)
//                    if (edgeSet.Contains((e.Key.Node1.Value, e.Key.Node2.Value)))
//                        continue;
//                    else
//                    {
//                        //it is sufficient to add the node (n2, n1) without storing (n1, n2), too as an occurence of multiple nodes has already been prevented before.
//                        edgeSet.Add((e.Key.Node2.Value, e.Key.Node1.Value));
//                    }

//                TNode n1 = _nodeMap[e.Key.Node1.Value].Node;
//                TNode n2 = _nodeMap[e.Key.Node2.Value].Node;

//                yield return (n1, n2);
//            }
//        }

//        private void ClearConnectionsTo(ConnectionsCollection target, bool decreaseEdgeCount)
//        {
//            ClearConnectionsToCore(target, decreaseEdgeCount);

//            if (!IsDirected)
//                ClearConnectionsFromCore(target, false);
//        }

//        private void ClearConnectionsToCore(ConnectionsCollection target, bool decreaseEdgeCount)
//        {
//            int n2 = target.NodeIndex;

//            var connected = target.GetConnectedNodes().ToArray(); //TODO: make more efficient
//            int edgeCount = 0;

//            foreach (int n1 in connected)
//            {
//                object c;
//                _edgeDictionary.Remove(new Edge<int?>(n1, n2));

//                if (_edgeDictionary.TryGetValue(new Edge<int?>(n1, null), out c))
//                    ((ConnectionsCollection)c).InternalRemove(n2);

//                edgeCount++;
//            }

//            if (decreaseEdgeCount)
//                _edgeCount -= edgeCount;

//            target.InternalClear();
//        }

//        private void ClearConnectionsFrom(ConnectionsCollection source, bool decreaseEdgeCount)
//        {
//            ClearConnectionsFromCore(source, decreaseEdgeCount);

//            if (!IsDirected)
//                ClearConnectionsToCore(source, false);
//        }

//        private void ClearConnectionsFromCore(ConnectionsCollection source, bool decreaseEdgeCount)
//        {
//            int n1 = source.NodeIndex;

//            var connected = source.GetConnectedNodes().ToArray(); //TODO: make more efficient
//            int edgeCount = 0;

//            foreach (int n2 in connected)
//            {
//                object c;
//                _edgeDictionary.Remove(new Edge<int?>(n1, n2));

//                if (_edgeDictionary.TryGetValue(new Edge<int?>(null, n2), out c))
//                    ((ConnectionsCollection)c).InternalRemove(n1);

//                edgeCount++;
//            }

//            if (decreaseEdgeCount)
//                _edgeCount -= edgeCount;

//            source.InternalClear();
//        }

//        public override string ToString()
//        {
//            var sb = new StringBuilder();

//            //outputs G = (V, E) with V = Nodes, E = Edges where V, E are sets
//            sb.Append('(');
//            sb.Append("Vertices: ");
//            Nodes.ToStringCore(sb);
//            sb.Append(',').Append(' ');
//            sb.Append("Edges: ");
//            Edges.ToStringCore(sb);
//            sb.Append(')');

//            return sb.ToString();
//        }

//        private struct NodeIndexPair : IEquatable<NodeIndexPair>
//        {
//            public TNode Node { get; }
//            public int? Index { get; }

//            public NodeIndexPair(int index)
//            {
//                Node = default;
//                Index = index;
//            }

//            public NodeIndexPair(TNode node)
//            {
//                Node = node;
//                Index = null;
//            }

//            public override string ToString()
//            {
//                return (Node, Index).ToString();
//            }

//            public override int GetHashCode()
//            {
//                return (Node?.GetHashCode() ?? 0) ^ (Index?.GetHashCode() ?? 0);
//            }

//            public override bool Equals(object obj)
//            {
//                if (obj == null)
//                    return false;

//                var nip = obj as NodeIndexPair?;

//                if (nip == null)
//                    return false;

//                return Equals(nip.Value);
//            }

//            public bool Equals(NodeIndexPair other)
//            {
//                return Index == other.Index && EqualityComparer<TNode>.Default.Equals(Node, other.Node);
//            }

//            public static implicit operator NodeIndexPair(int index)
//            {
//                return new NodeIndexPair(index);
//            }

//            public static implicit operator NodeIndexPair(TNode node)
//            {
//                return new NodeIndexPair(node);
//            }
//        }
//    }
//}
