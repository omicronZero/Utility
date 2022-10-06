//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;

//namespace Utility
//{
//    public partial class Graph<TNode>
//    {
//        public sealed class NodeCollection : ICollection<TNode>
//        {
//            private readonly Graph<TNode> _graph;

//            internal NodeCollection(Graph<TNode> graph)
//            {
//                if (graph == null)
//                    throw new ArgumentNullException(nameof(graph));

//                _graph = graph;
//            }

//            public int Count => _graph.NodeCount;

//            bool ICollection<TNode>.IsReadOnly => false;

//            public bool Add(TNode item)
//            {
//                return _graph.AddNode(item);
//            }

//            public bool AddRange(params TNode[] nodes) => AddRange((IEnumerable<TNode>)nodes);

//            public bool AddRange(IEnumerable<TNode> nodeEnumeration)
//            {
//                if (nodeEnumeration == null)
//                    throw new ArgumentNullException(nameof(nodeEnumeration));

//                bool success = true;

//                foreach (TNode node in nodeEnumeration)
//                    success &= Add(node);

//                return success;
//            }

//            void ICollection<TNode>.Add(TNode item)
//            {
//                Add(item);
//            }

//            public void Clear()
//            {
//                _graph.ClearNodes();
//            }

//            public bool Contains(TNode item)
//            {
//                return _graph.ContainsNode(item);
//            }

//            public void CopyTo(TNode[] array, int arrayIndex)
//            {
//                _graph.CopyNodesTo(array, arrayIndex);
//            }

//            //TODO: make struct
//            public IEnumerator<TNode> GetEnumerator()
//            {
//                return _graph.GetNodesEnumerator();
//            }

//            public bool Remove(TNode item)
//            {
//                return _graph.RemoveNode(item);
//            }

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
//                bool notfirst = false;

//                stringBuilder.Append('{');

//                foreach (TNode vertex in this)
//                {
//                    if (notfirst)
//                        stringBuilder.Append(',').Append(' ');

//                    notfirst = true;

//                    stringBuilder.Append(vertex);
//                }

//                stringBuilder.Append('}');

//                return stringBuilder;
//            }
//        }
//    }
//}
