using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Utility
{
    public class LinkedComposite<TComposite> : LinkedCompositeElement
        where TComposite : LinkedCompositeElement
    {
        private readonly CompositeCollection<TComposite> _children;

        public LinkedComposite()
        {
            _children = new CompositeCollection<TComposite>(this);
        }

        internal bool InternalIsValidChild(TComposite item)
        {
            return IsValidChild(item);
        }

        protected virtual bool IsValidChild(TComposite item)
        {
            return item != null;
        }

        internal void InternalItemRemoved(int index, TComposite oldItem)
        {
            oldItem.InternalSetParent(null, -1);

            for (int i = index; i < _children.Count; i++)
                _children[i].InternalSetIndex(i);

            OnItemRemoved(index, oldItem);
        }

        internal void InternalItemSet(int index, TComposite oldItem, TComposite item)
        {
            oldItem.InternalSetParent(null, -1);

            item.InternalSetIndex(index);
            item.InternalSetParent(this);

            OnItemSet(index, oldItem, item);
        }

        internal void InternalItemInserted(int index, TComposite item)
        {
            _children[index].InternalSetParent(item, index);

            for (int i = index + 1; i < _children.Count; i++)
                _children[i].InternalSetIndex(i);

            OnItemInserted(index, item);
        }

        internal void InternalItemsClearing()
        {
            OnItemsClearing();
        }

        internal void InternalItemsCleared(IList<TComposite> oldItems)
        {
            foreach (TComposite c in oldItems)
                c.InternalSetParent(null, -1);

            OnItemsCleared(oldItems.AsReadonlyList());
        }

        protected override bool IsValidChild(LinkedCompositeElement element)
        {
            if (element == null)
                return false;

            TComposite c = element as TComposite;

            if (c == null)
                return false;

            return IsValidChild(c);
        }

        protected override int AddChild(LinkedCompositeElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            TComposite elem = element as TComposite;

            _children.Add(elem);

            return _children.Count;
        }

        protected override void RemoveChild(LinkedCompositeElement element, int index)
        {
            _children.RemoveAt(index);
        }

        protected virtual void OnItemRemoved(int index, TComposite oldItem)
        { }

        protected virtual void OnItemSet(int index, TComposite oldItem, TComposite item)
        { }

        protected virtual void OnItemInserted(int index, TComposite item)
        { }

        protected virtual void OnItemsClearing()
        { }

        protected virtual void OnItemsCleared(IList<TComposite> oldItems)
        { }
    }
}
