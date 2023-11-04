using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Utility.Collections;

namespace Utility.Collections.Labeled
{
    public class LabeledCollection<T> : Collection<T>, ILabeled
    {
        private static readonly Comparer<Label> Comparer = Comparer<Label>.Create((l, r) => l.Index.CompareTo(r.Index));
        private Label[] _labels;
        private int _labelCount;

        public LabeledCollection()
        {
            _labels = new Label[1];
        }

        public LabeledCollection(IList<T> internalList)
            : base(internalList)
        {
            _labels = new Label[1];
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            //decrement all label indices that point to an item after the removed one
            for (int l = _labelCount - 1; l >= 0 && _labels[l].Index > index; l--)
                _labels[l].SetIndexInternal(_labels[l].Index - 1, index);
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            //set all labels' indices to 0
            foreach (Label l in _labels.Take(_labelCount))
                l.SetIndexInternal(0, l.Index);
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            //increment all label indices that pointed to an item after the newly inserted one
            for (int l = _labelCount - 1; l >= 0 && _labels[l].Index >= index; l--)
                _labels[l].SetIndexInternal(_labels[l].Index + 1, index);
        }

        private void AdjustLabelCapacity(int deltaCount)
        {
            int newElementCount = _labelCount + deltaCount;

            int requiredSize = _labels.Length;

            //make array bigger (requiredSize will be 2^k)
            while (requiredSize < newElementCount)
                requiredSize *= 2;

            //only up to half of the array entries should be unused
            while (requiredSize > newElementCount * 4)
                requiredSize /= 2;

            //an empty label array is not permitted
            if (requiredSize == 0)
                requiredSize = 1;

            if (_labels.Length != requiredSize)
            {
                Array.Resize(ref _labels, requiredSize);
            }
        }

        public Label CreateLabel(int index)
        {
            ValidateIndex(index);

            Label l = new Label(this, index);

            //a new label will be added. Thus, delta is 1
            AdjustLabelCapacity(1);

            //keep the label array sorted when inserting a new one
            int ind = Array.BinarySearch(_labels, 0, _labelCount, l, Comparer);

            if (ind < 0)
                ind = ~ind;

            //create a slot for the new label. Multiple labels are allowed that point to the same index
            Array.Copy(_labels, ind, _labels, ind + 1, _labelCount - ind);

            _labels[ind] = l;
            _labelCount++;

            return l;
        }

        public T this[Label label]
        {
            get
            {
                ThrowInvalidLabel(label);

                return this[label.Index];
            }
            set
            {
                ThrowInvalidLabel(label);

                this[label.Index] = value;
            }
        }

        public Label CreateLabel()
        {
            return CreateLabel(Count);
        }

        public void RemoveLabel(Label label)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            ThrowInvalidLabel(label);

            //find label index in label array that is sorted by the index the label points to
            int ind = Array.BinarySearch(_labels, 0, _labelCount, label, Comparer);

            Array.Copy(_labels, ind + 1, _labels, ind, _labelCount - ind - 1);

            _labelCount--;
            _labels[_labelCount] = null;

            AdjustLabelCapacity(0);

            label.Removed();
        }

        private void ThrowInvalidLabel(Label label)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            if (label.Labeled != this)
                if (label.Labeled == null)
                    throw new ArgumentException("Label has already been deleted.", nameof(label));
                else
                    throw new ArgumentException("Label is not assigned to this collection.", nameof(label));
        }

        internal void UpdateLabel(Label label, int newIndex)
        {
            ValidateIndex(newIndex);

            int labelOldIndex = label.Index;

            //get the old index in the label collection
            int oldInd = Array.BinarySearch(_labels, 0, _labelCount, label, Comparer);

            label.SetIndexInternal(newIndex, -1, false);
            int newInd = Array.BinarySearch(_labels, 0, _labelCount, label, Comparer);

            //oldInd should not be negative as the element should always lie in the set of labels 

            if (newInd < 0)
                newInd = ~newInd;

            //the range depends on the value of oldInd < newInd as the item will
            //either be removed from the end and inserted at the beginning or vice versa

            //thus: copy the range...
            if (oldInd < newInd)
                Array.Copy(_labels, oldInd + 1, _labels, oldInd, newInd - oldInd - 1);
            else if (oldInd > newInd)
                Array.Copy(_labels, newInd, _labels, newInd + 1, oldInd - newInd - 1);

            //... and move the item to the new position
            _labels[newInd] = label;

            label.RaiseChanged(labelOldIndex, -1);
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative and not exceed the item count.");
        }

        void ILabeled.UpdateLabel(Label label, int newIndex)
        {
            UpdateLabel(label, newIndex);
        }

        public IEnumerable<int> EnumerateIndicesLabeled()
        {
            Label l = CreateLabel(0);
            try
            {
                for (; l.Index < Count; l.Index++)
                    yield return l.Index;
            }
            finally
            {
                RemoveLabel(l);
            }
        }

        public IEnumerable<T> EnumerateLabeled()
        {
            return EnumerateIndicesLabeled().Select((ind) => this[ind]);
        }
    }
}
