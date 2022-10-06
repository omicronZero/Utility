using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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

            protected override void OnClearNodes()
            {
                var nodes = Array.AsReadOnly(this.ToArray());

                base.OnClearNodes();

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, nodes));
            }

            protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                CollectionChanged?.Invoke(this, e);
            }
        }
    }
}
