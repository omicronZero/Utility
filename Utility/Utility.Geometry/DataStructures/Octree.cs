using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Utility.Mathematics;

namespace Utility.DataStructures
{
    public abstract class Octree<T> : ICollection<T>
    {
        private readonly Dictionary<T, Box> _boundaries;

        private Node _root;

        public int Count { get; private set; }

        protected abstract Box ComputeBoundaries(T item);

        protected abstract int ExpansionCount { get; }
        protected abstract int CollapsingCount { get; }

        protected Octree()
        {
            _boundaries = new Dictionary<T, Box>(ReferenceComparer<T>.Default);
        }

        public bool IsReadOnly => true;

        protected void Update(T item)
        {
            _root.Remove(item, GetBoundaries(item));
            _boundaries.Remove(item);
            GetBoundaries(item);
        }

        protected Box GetBoundaries(T item)
        {
            Box r;

            if (!_boundaries.TryGetValue(item, out r))
            {
                r = ComputeBoundaries(item);
                _boundaries.Add(item, r);
            }

            return r;
        }

        public void Add(T item)
        {
            if (_boundaries.ContainsKey(item))
                throw new ArgumentException("The specified item is already in the collection.", nameof(item));

            Box itemBounds = GetBoundaries(item);
            Node root = _root;

            if (root != null)
            {
                Box rb = root.Boundaries;

                while (!rb.Contains(itemBounds))
                {
                    //determine at least one direction, in which the indicated item's boundaries exceed the current tree root's boundaries
                    //extend in that direction

                    //dExpansion == 0: keep, dExpansion < 0: expand towards near, dExpansion > 0: expand towards far
                    int xExpansion, yExpansion, zExpansion;

                    if (itemBounds.X < rb.X)
                        xExpansion = -1;
                    else if (itemBounds.Right > rb.X)
                        xExpansion = 1;
                    else
                        xExpansion = 0;

                    if (itemBounds.Y < rb.Y)
                        yExpansion = -1;
                    else if (itemBounds.Bottom > rb.Y)
                        yExpansion = 1;
                    else
                        yExpansion = 0;

                    if (itemBounds.Z < rb.Z)
                        zExpansion = -1;
                    else if (itemBounds.Back > rb.Z)
                        zExpansion = 1;
                    else
                        zExpansion = 0;

                    //double width and height of the rectangle, choose one direction in which the extension is required
                    if (xExpansion < 0)
                    {
                        //expand only to left, if required. If xExpansion == 0, an expansion to the right is performed
                        rb.X -= rb.Width;
                    }

                    rb.Width += rb.Width;

                    if (yExpansion < 0)
                    {
                        //expand only to top, if required. If yExpansion == 0, an expansion to the bottom is performed
                        rb.Y -= rb.Height;
                    }

                    if (yExpansion < 0)
                    {
                        //expand only to front, if required. If zExpansion == 0, an expansion to the back is performed
                        rb.Z -= rb.Depth;
                    }

                    rb.Height += rb.Height;

                    int newRootChildIndex = (xExpansion <= 0 ? 0 : 1) + (yExpansion <= 0 ? 0 : 2) + (zExpansion <= 0 ? 0 : 4);

                    Node newRoot = new Node(this, rb, root, newRootChildIndex);

                    root = newRoot;
                }
            }
            else
            {
                root = new Node(this, itemBounds);
            }
            _root = root;

            root.Insert(item, itemBounds);
        }

        public IEnumerable<T> GetItems(Box bounds)
        {
            if (_root == null || !_root.Boundaries.IntersectsWith(bounds))
                return Enumerable.Empty<T>();

            return _root.GetIntersectingItems(bounds);
        }

        public void Clear()
        {
            _root = null;
            _boundaries.Clear();
        }

        public bool Contains(T item)
        {
            return _boundaries.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _boundaries.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (_root == null)
                return false;

            if (_boundaries.Remove(item))
            {
                _root.Remove(item, GetBoundaries(item));
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _boundaries.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class Node
        {
            private Octree<T> Tree { get; }

            private Node[] Children { get; set; }
            public Box Boundaries { get; }
            internal List<(T, Box)> Items { get; }

            public int TotalCount { get; private set; }

            internal Node(Octree<T> tree, Box boundaries)
            {
                if (tree == null)
                    throw new ArgumentNullException(nameof(tree));

                Tree = tree;
                Boundaries = boundaries;
                Items = new List<(T, Box)>();
            }

            internal Node(Octree<T> tree, Box boundaries, Node child, int childIndex)
                : this(tree, boundaries)
            {
                CreateChildren();
                Children[childIndex] = child;
                TotalCount += child.TotalCount;
            }

            public bool IsExpanded => Children != null;

            public Node GetChild(bool right, bool bottom, bool back)
            {
                return Children?[GetChildIndex(right, bottom, back)];
            }

            public static int GetChildIndex(bool right, bool bottom, bool back)
            {
                return (right ? 0 : 1) + (bottom ? 0 : 2) + (back ? 0 : 4);
            }

            public int GetChildIndex(Box box)
            {
                return GetChildIndex(box, out _, out _, out _);
            }

            public int GetChildIndex(Box rectangle, out bool right, out bool bottom, out bool back)
            {
                Point3r center = Boundaries.Center;

                if (center.X <= rectangle.X)
                {
                    right = true;
                }
                else if (rectangle.Right < center.X)
                {
                    right = false;
                }
                else
                {
                    bottom = false;
                    right = false;
                    back = false;
                    return -1;
                }

                if (center.Y <= rectangle.Y)
                {
                    bottom = true;
                }
                else if (rectangle.Bottom < center.Y)
                {
                    bottom = false;
                }
                else
                {
                    bottom = false;
                    right = false;
                    back = false;
                    return -1;
                }

                if (center.Z <= rectangle.Z)
                {
                    back = true;
                }
                else if (rectangle.Back < center.Z)
                {
                    back = false;
                }
                else
                {
                    bottom = false;
                    right = false;
                    back = false;
                    return -1;
                }

                return GetChildIndex(right, bottom, back);
            }

            public Node GetContainer(Box box)
            {
                box.Normalize();
                return GetContainerCore(box);
            }

            private Node GetContainerCore(Box box)
            {
                if (Children == null)
                    return this;

                bool right, bottom, back;

                if (GetChildIndex(box, out right, out bottom, out back) == -1)
                    return this;

                return GetChild(right, bottom, back);
            }

            public Box GetBoundaries(int index)
            {
                if (index < 0 || index >= 4)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index in the range of 0 to 3 expected.");

                return GetBoundaries((index & 1) == 1, ((index >> 1) & 0x1) == 1, (index >> 2) == 1);
            }

            public Box GetBoundaries(bool right, bool bottom, bool back)
            {
                if (back)
                {
                    if (right)
                    {
                        if (bottom)
                            return Box.FromPoints(Boundaries.Center, Boundaries.Far);
                        else
                            return Box.FromPoints((Boundaries.X + Boundaries.Width / 2, Boundaries.Y, Boundaries.Z + Boundaries.Depth / 2), Boundaries.Center);
                    }
                    else
                    {
                        if (bottom)
                            return Box.FromPoints((Boundaries.X, Boundaries.Y + Boundaries.Height, Boundaries.Z + Boundaries.Depth / 2), Boundaries.Far);
                        else
                            return Box.FromPoints(Boundaries.Position, Boundaries.Center);
                    }
                }
                else
                {
                    if (right)
                    {
                        if (bottom)
                            return Box.FromPoints(Boundaries.Center, Boundaries.Far);
                        else
                            return Box.FromPoints((Boundaries.X + Boundaries.Width / 2, Boundaries.Y, Boundaries.Z), Boundaries.Center);
                    }
                    else
                    {
                        if (bottom)
                            return Box.FromPoints((Boundaries.X, Boundaries.Y + Boundaries.Height, Boundaries.Z), Boundaries.Far);
                        else
                            return Box.FromPoints(Boundaries.Position, Boundaries.Center);
                    }
                }
            }

            private void Expand()
            {
                if (Children != null)
                    return;

                (T v, Box rectangle)[] it = Items.ToArray();
                Items.Clear();

                CreateChildren();

                foreach ((T v, Box box) in it)
                {
                    bool right, bottom, back;

                    int index = GetChildIndex(box, out right, out bottom, out back);

                    if (index == -1)
                        continue;

                    Node c = Children[index];

                    if (c == null)
                    {
                        c = new Node(Tree, GetBoundaries(right, bottom, back));
                        Children[index] = c;
                    }

                    c.Insert(v, box);
                }
            }

            private IEnumerable<(T, Box)> GetNodesCore()
            {
                IEnumerable<(T, Box)> items = Items;

                if (Children != null)
                {
                    IEnumerable<(T, Box)> children = Children.Where((nd) => nd != null).SelectMany((child) => child.GetNodesCore());

                    items = children.Concat(items);
                }

                return items;
            }

            private void CreateChildren()
            {
                if (Children == null)
                {
                    Children = new Node[8];
                }
            }

            private IEnumerable<T> GetNodes()
            {
                return GetNodesCore().Select((s) => s.Item1);
            }

            private void Collapse()
            {
                if (Children == null)
                    return;

                var items = new HashSet<T>(Items.SelectList((s) => s.Item1), ReferenceComparer<T>.Default);

                foreach ((T v, Box box) in GetNodesCore())
                {
                    if (!items.Add(v))
                        continue;

                    Items.Add((v, box));
                }

                Children = null;
            }

            internal void Remove(T item, Box itemBounds)
            {
                TotalCount--;

                int index = GetChildIndex(itemBounds);

                if (index == -1 || Children == null || Children[index] == null)
                {
                    Items.Remove((item, itemBounds));
                }
                else
                    Children[index].Remove(item, itemBounds);

                if (TotalCount <= Tree.CollapsingCount && TotalCount < Tree.ExpansionCount)
                {
                    Collapse();
                }

                for (int i = 0; i < Children.Length; i++)
                {
                    if (Children[i].TotalCount == 0)
                        Children[i] = null;
                }
            }

            internal void Insert(T item, Box itemBounds)
            {
                TotalCount++;

                int index = GetChildIndex(itemBounds);

                if (Children == null)
                {
                    if (TotalCount >= Tree.ExpansionCount && TotalCount > Tree.CollapsingCount)
                    {
                        Expand();
                    }
                }

                if (index == -1 || Children == null)
                {
                    Items.Add((item, itemBounds));
                }
                else
                {
                    Node c = Children[index];

                    if (c == null)
                    {
                        c = new Node(Tree, GetBoundaries(index));
                        Children[index] = c;
                    }

                    c.Insert(item, itemBounds);
                }
            }

            public IEnumerable<T> GetIntersectingItems(Box bounds)
            {
                return GetNodesCore(bounds).Where((p) => bounds.IntersectsWith(p.Item2)).Select((s) => s.Item1);
            }

            private IEnumerable<(T, Box)> GetNodesCore(Box bounds)
            {

                int ind = GetChildIndex(bounds);
                IEnumerable<(T, Box)> it = Items;

                if (Children == null)
                    return it;

                if (ind == -1)
                {
                    it = it.Concat(Children.Where((t) => t != null).SelectMany((m) => m.GetNodesCore(bounds)));
                }
                else if (Children[ind] != null)
                {
                    it = it.Concat(Children[ind].GetNodesCore(bounds));
                }

                return it;
            }
        }
    }
}
