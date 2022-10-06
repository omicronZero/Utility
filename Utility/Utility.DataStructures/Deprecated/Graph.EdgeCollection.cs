//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Text;

//namespace Utility
//{
//    public partial class Graph<TNode>
//    {
//        public sealed class EdgeCollection : ICollection<Edge<TNode>>
//        {
//            private readonly Graph<TNode> _graph;

//            internal EdgeCollection(Graph<TNode> graph)
//            {
//                if (graph == null)
//                    throw new ArgumentNullException(nameof(graph));

//                _graph = graph;
//            }

//            public int Count => _graph.EdgeCount;

//            bool ICollection<Edge<TNode>>.IsReadOnly => false;

//            public bool Add(Edge<TNode> item)
//            {
//                return _graph.AddEdge(item);
//            }

//            public bool Add(TNode node1, TNode node2) => Add((node1, node2));

//            public bool AddRange(params Edge<TNode>[] edges) => AddRange((IEnumerable<Edge<TNode>>)edges);

//            public bool AddRange(IEnumerable<Edge<TNode>> edgeEnumeration)
//            {
//                if (edgeEnumeration == null)
//                    throw new ArgumentNullException(nameof(edgeEnumeration));

//                bool success = true;

//                foreach (Edge<TNode> edge in edgeEnumeration)
//                    success &= Add(edge);

//                return success;
//            }

//            void ICollection<Edge<TNode>>.Add(Edge<TNode> item)
//            {
//                Add(item);
//            }

//            public void Clear()
//            {
//                _graph.ClearEdges();
//            }

//            public bool Contains(Edge<TNode> item)
//            {
//                return _graph.ContainsEdge(item);
//            }

//            public bool Contains(TNode node1, TNode node2) => Contains((node1, node2));

//            public void CopyTo(Edge<TNode>[] array, int arrayIndex)
//            {
//                _graph.CopyEdgesTo(array, arrayIndex);
//            }

//            public IEnumerator<Edge<TNode>> GetEnumerator()
//            {
//                return _graph.GetEdgeEnumerator();
//            }

//            public bool Remove(Edge<TNode> item)
//            {
//                return _graph.RemoveEdge(item);
//            }

//            public bool Remove(TNode node1, TNode node2) => Remove((node1, node2));

//            IEnumerator IEnumerable.GetEnumerator()
//            {
//                return GetEnumerator();
//            }

//            public override string ToString()
//            {
//                return ToStringCore(new StringBuilder()).ToString();
//            }

//            internal StringBuilder ToStringCore(StringBuilder stringBuilder)
//            {
//                char openingBracket = _graph.IsDirected ? '(' : '{';
//                char closingBracket = _graph.IsDirected ? ')' : '}';

//                bool notfirst = false;

//                stringBuilder.Append('{');

//                foreach (Edge<TNode> edge in this)
//                {
//                    if (notfirst)
//                        stringBuilder.Append(',').Append(' ');

//                    notfirst = true;

//                    //for undirected graphs, set format to {n1, n2} for directed graphs to (n1, n2)
//                    stringBuilder.Append(openingBracket);
//                    stringBuilder.Append(edge.Node1);
//                    stringBuilder.Append(',').Append(' ');
//                    stringBuilder.Append(edge.Node2);
//                    stringBuilder.Append(closingBracket);
//                }

//                stringBuilder.Append('}');

//                return stringBuilder;
//            }
//        }
//    }
//}
