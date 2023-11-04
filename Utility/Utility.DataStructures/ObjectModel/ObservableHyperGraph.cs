using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Utility.ObjectModel
{
    internal class ObservableHyperGraph<TNode>
    {
        public class NodeCollection
        {
        }

        public class EdgeCollection
        {

            protected virtual void OnEdgeAdded(HyperEdge<TNode> item)
            { }

            protected virtual void OnEdgeRemoved(HyperEdge<TNode> item)
            { }

            protected virtual void OnEdgesCleared(ICollection<HyperEdge<TNode>> edges = null)
            { }

            internal void InternalOnEdgeAdded(HyperEdge<TNode> item)
            {
                OnEdgeAdded(item);
            }

            internal void InternalOnEdgeRemoved(HyperEdge<TNode> item)
            {
                OnEdgeRemoved(item);
            }

            internal void InternalOnEdgesCleared(ICollection<HyperEdge<TNode>> edges = null)
            {
                OnEdgesCleared(edges);
            }
        }
    }
}
