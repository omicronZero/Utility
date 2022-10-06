using System;
using System.Collections.Generic;
using System.Text;
using Utility.Collections;

namespace Utility
{
    public class CompositeCollection<TComposite> : Collection<TComposite>
        where TComposite : LinkedCompositeElement
    {
        public LinkedComposite<TComposite> Element { get; }

        internal CompositeCollection(LinkedComposite<TComposite> element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            Element = element;
        }

        protected override void SetItem(int index, TComposite item)
        {
            ProhibitInvalid(item);

            TComposite oldItem = this[index];

            base.SetItem(index, item);

            Element.InternalItemSet(index, oldItem, item);
        }

        protected override void ClearItems()
        {
            Element.InternalItemsClearing();

            IList<TComposite> oldItems = InternalList;

            InternalList = new List<TComposite>();

            Element.InternalItemsCleared(oldItems);
        }

        protected override int IndexOfItem(TComposite item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.Parent != Element)
                return -1;

            return item.Index;
        }

        protected override void InsertItem(int index, TComposite item)
        {
            ProhibitInvalid(item);

            base.InsertItem(index, item);

            Element.InternalItemInserted(index, item);
        }

        private void ProhibitInvalid(TComposite item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!Element.InternalIsValidChild(item))
                throw new ArgumentException("Item is not supported by the associated element.", nameof(item));
        }

        protected override void RemoveItem(int index)
        {
            TComposite oldItem = this[index];

            base.RemoveItem(index);

            Element.InternalItemRemoved(index, oldItem);
        }
    }
}
