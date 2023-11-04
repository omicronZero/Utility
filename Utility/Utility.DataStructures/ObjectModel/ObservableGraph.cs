using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Utility.DataStructures;

namespace Utility.ObjectModel
{
    public class ObservableGraph<TNode> : GenericGraph<TNode>
    {
        public ObservableGraph(IGraph<TNode> innerGraph)
            : base(innerGraph)
        { }

        public ObservableGraph(bool isDirected)
            : this(new Graph<TNode>(isDirected))
        { }

        public ObservableGraph(bool isDirected, IEqualityComparer<TNode> nodeComparer)
            : this(new Graph<TNode>(isDirected, nodeComparer))
        { }

        new public NodeCollection Nodes => (NodeCollection)base.Nodes;
        new public EdgeCollection Edges => (EdgeCollection)base.Edges;
        new public NodeEdgesCollection GetOuts(TNode node) => (NodeEdgesCollection)base.GetOuts(node);
        new public NodeEdgesCollection GetIns(TNode node) => (NodeEdgesCollection)base.GetIns(node);

        protected override bool AddNode(TNode node)
        {
            if (!base.AddNode(node))
                return false;

            Nodes.InternalOnNodeAdded(node);

            return true;
        }

        protected override bool RemoveNode(TNode node)
        {
            if (!base.RemoveNode(node))
                return false;

            Nodes.InternalOnNodeRemoved(node);
            return true;
        }

        protected override void ClearNodes()
        {
            var state = BeforeClearNodes();

            var nodes = Nodes.ToArray();
            var edges = Edges.ToArray();

            base.ClearNodes();

            Nodes.InternalOnNodesCleared(nodes);
            Edges.InternalOnEdgesCleared(edges);

            AfterNodesCleared(
                state,
                Array.AsReadOnly(nodes),
                Array.AsReadOnly(edges));
        }

        protected override bool AddEdge(Edge<TNode> edge)
        {
            if (!base.AddEdge(edge))
                return false;

            Edges.InternalOnEdgeAdded(edge);

            GetOuts(edge.Node1).InternalOnNodeAdded(edge);
            GetIns(edge.Node2).InternalOnNodeAdded(edge);

            if (!IsDirected)
            {
                edge = edge.Reversed();
                GetOuts(edge.Node2).InternalOnNodeAdded(edge);
                GetIns(edge.Node1).InternalOnNodeAdded(edge);
            }

            return true;
        }

        protected override bool RemoveEdge(Edge<TNode> edge)
        {
            if (!base.RemoveEdge(edge))
                return false;

            Edges.InternalOnEdgeRemoved(edge);

            GetOuts(edge.Node1).InternalOnNodeRemoved(edge);
            GetIns(edge.Node2).InternalOnNodeRemoved(edge);

            if (!IsDirected)
            {
                edge = edge.Reversed();
                GetOuts(edge.Node2).InternalOnNodeRemoved(edge);
                GetIns(edge.Node1).InternalOnNodeRemoved(edge);
            }

            return true;
        }

        protected virtual object BeforeClearNodes()
        {
            var state = new List<(NodeEdgesCollection Ins, NodeEdgesCollection Outs, object inState, object outState)>();

            foreach (var kvp in NodeInsOuts)
            {
                var (ins, outs) = ((NodeEdgesCollection)kvp.Value.Ins, (NodeEdgesCollection)kvp.Value.Outs);

                ins.InternalBeforeClear();
                outs.InternalBeforeClear();

                //if unbound, we don't save the state
                var inState = ins.InternalClearRequiresState ? ins.InternalGetClearState() : null;
                var outState = outs.InternalClearRequiresState ? outs.InternalGetClearState() : null;

                state.Add((ins, outs, inState, outState));
            }

            return Association.Create(this, state.ToArray());
        }

        /// <summary>
        /// Called after the nodes have been cleared. Works in combination with <see cref="BeforeClearNodes"/>.
        /// <br/>
        /// Handles the notification of the <see cref="NodeEdgesCollection"/>'s
        /// <see cref="NodeEdgesCollection.OnNodesCleared"/>-method.
        /// </summary>
        /// <param name="state">The state returned by <see cref="BeforeClearNodes"/>.</param>
        /// <param name="nodes">The nodes that were removed.</param>
        /// <param name="edges">The edges that were removed.</param>
        protected virtual void AfterNodesCleared(
            object state,
            ICollection<TNode> nodes,
            ICollection<Edge<TNode>> edges)
        {
            var values = Association.Cast<(NodeEdgesCollection Ins, NodeEdgesCollection Outs, object inState, object outState)[]>(this, state);

            foreach (var (ins, outs, inState, outState) in values)
            {
                ins.InternalOnNodesCleared(inState);
                outs.InternalOnNodesCleared(outState);
            }
        }

        protected override GenericGraph<TNode>.NodeCollection CreateNodes()
        {
            return new NodeCollection();
        }

        protected override GenericGraph<TNode>.EdgeCollection CreateEdges()
        {
            return new EdgeCollection();
        }

        protected override void CreateInOuts(TNode node, out GenericGraph<TNode>.NodeEdgesCollection ins, out GenericGraph<TNode>.NodeEdgesCollection outs)
        {
            ins = new NodeEdgesCollection();
            outs = new NodeEdgesCollection();
        }

        new public class NodeCollection : GenericGraph<TNode>.NodeCollection, INotifyCollectionChanged
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            internal NodeCollection() { }

            protected override bool OnAddNode(TNode item)
            {
                bool added = base.OnAddNode(item);

                if (added)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));

                return added;
            }

            protected override bool OnRemoveNode(TNode item)
            {
                bool removed = base.OnRemoveNode(item);

                if (removed)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

                return removed;
            }

            protected override void OnClearNodes()
            {
                var nodes = Array.AsReadOnly(this.ToArray());

                base.OnClearNodes();

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, nodes));
            }

            protected override void OnNodesCleared(IList<TNode> nodes)
            {
                base.OnNodesCleared(nodes);
            }

            protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                CollectionChanged?.Invoke(this, e);
            }
        }

        new public class EdgeCollection : GenericGraph<TNode>.EdgeCollection, INotifyCollectionChanged
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            internal EdgeCollection() { }

            protected override bool OnAddEdge(Edge<TNode> item)
            {
                bool added = base.OnAddEdge(item);

                if (added)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));

                return added;
            }

            protected override bool OnRemoveEdge(Edge<TNode> item)
            {
                bool removed = base.OnRemoveEdge(item);

                if (removed)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

                return removed;
            }

            protected override void OnClearEdges()
            {
                var edges = Array.AsReadOnly(this.ToArray());

                base.OnClearEdges();

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, edges));
            }

            protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                CollectionChanged?.Invoke(this, e);
            }
        }

        new public class NodeEdgesCollection : GenericGraph<TNode>.NodeEdgesCollection, INotifyCollectionChanged
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            internal NodeEdgesCollection() { }

            internal bool InternalClearRequiresState => ClearRequiresState;
            internal object InternalGetClearState() => GetClearState();

            protected bool HasCollectionChangedHandlers => CollectionChanged != null;
            protected virtual bool ClearRequiresState => HasCollectionChangedHandlers;

            protected virtual object GetClearState()
            {
                return Association.Create(this, Array.AsReadOnly(this.ToArray()));
            }

            protected override bool OnAddNode(Edge<TNode> item)
            {
                bool added = base.OnAddNode(item);

                if (added)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));

                return added;
            }

            protected override bool OnRemoveNode(Edge<TNode> item)
            {
                bool removed = base.OnRemoveNode(item);

                if (removed)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

                return removed;
            }

            protected override void OnNodesCleared(object state = null)
            {
                if (state != null)
                {
                    var nodes = Association.Cast<ReadOnlyCollection<TNode>>(this, state);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, nodes));
                }
                else
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }

                base.OnNodesCleared(state);
            }

            protected override void OnNodeAdded(Edge<TNode> edge)
            {
                base.OnNodeAdded(edge);
            }

            protected override void OnNodeRemoved(Edge<TNode> edge)
            {
                base.OnNodeRemoved(edge);
            }

            protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                CollectionChanged?.Invoke(this, e);
            }

            protected virtual void BeforeClear()
            { }

            internal void InternalOnNodesCleared(object state)
            {
                OnNodesCleared(state);
            }

            internal void InternalBeforeClear() => BeforeClear();
        }
    }
}
