using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class LinkedCompositeElement
    {
        private LinkedCompositeElement _parent;

        public int Index { get; private set; }

        protected virtual bool IsValidChild(LinkedCompositeElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return false;
        }

        private int AddChildCore(LinkedCompositeElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (IsValidChild(element))
                throw new ArgumentException("Element is not supported by the element.", nameof(element));

            return AddChild(element);
        }

        protected virtual int AddChild(LinkedCompositeElement element)
        {
            throw new NotSupportedException();
        }

        protected virtual void RemoveChild(LinkedCompositeElement element, int index)
        {
            throw new NotSupportedException();
        }

        private void RemoveChildCore(LinkedCompositeElement element, int index)
        {
            RemoveChild(element, index);

            IList<LinkedCompositeElement> l = GetChildren();

            for (int i = index; i < l.Count; i++)
                l[i].InternalSetIndex(i);
        }

        internal void InternalSetParent(LinkedCompositeElement parent)
        {
            LinkedCompositeElement previousParent = _parent;
            _parent = parent;
            OnParentChanged(previousParent);
        }

        internal void InternalSetParent(LinkedCompositeElement parent, int index)
        {
            LinkedCompositeElement previousParent = _parent;
            int previousIndex = Index;

            _parent = parent;
            Index = index;

            OnParentChanged(previousParent);
            OnIndexChanged(previousIndex);
        }

        protected virtual IList<LinkedCompositeElement> GetChildren()
        {
            return Array.Empty<LinkedCompositeElement>();
        }

        internal void InternalSetIndex(int index)
        {
            int oldIndex = Index;
            Index = index;
            OnIndexChanged(oldIndex);
        }

        protected virtual void OnParentChanged(LinkedCompositeElement previousParent)
        { }

        protected virtual void OnIndexChanged(int oldIndex)
        { }

        public LinkedCompositeElement Parent
        {
            get { return _parent; }
            set
            {
                int ind;
                if (_parent != value)
                {
                    if (_parent != null)
                    {
                        LinkedCompositeElement parent = _parent;
                        int index = Index;
                        _parent = null;
                        Index = -1;
                        parent.RemoveChildCore(this, index);
                    }

                    _parent = value;

                    try
                    {
                        ind = value.AddChildCore(this);
                    }
                    catch
                    {
                        _parent = null;
                        throw;
                    }

                    IList<LinkedCompositeElement> l = GetChildren();

                    for (int i = ind; i < l.Count; i++)
                        l[i].InternalSetIndex(i);
                }
            }
        }
    }
}
