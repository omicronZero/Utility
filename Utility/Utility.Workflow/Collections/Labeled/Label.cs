using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility.Workflow.Collections.Labeled
{
    public class Label
    {
        private int _index;

        public event EventHandler<LabelChangedEventArgs> LabelChanged;

        internal ILabeled Labeled { get; private set; }

        internal Label(ILabeled listInstance, int index)
        {
            if (listInstance == null)
                throw new ArgumentNullException(nameof(listInstance));

            Labeled = listInstance;

            _index = index;
        }

        public bool IsEnd
        {
            get { return Labeled.Count == _index; }
        }

        public int Index
        {
            get { return _index; }
            set
            {
                ThrowDeleted();
                Labeled.UpdateLabel(this, value);
            }
        }

        public bool IsAttached
        {
            get { return Labeled != null; }
        }

        private void ThrowDeleted()
        {
            if (Labeled == null)
                throw new InvalidOperationException("Label has been removed from the collection.");
        }

        internal void SetIndexInternal(int index, int changedItemIndex, bool raise = true)
        {
            int oldIndex = _index;
            _index = index;

            if (raise)
                RaiseChanged(oldIndex, changedItemIndex);
        }

        internal void RaiseChanged(int oldIndex, int changedItemIndex)
        {
            LabelChanged?.Invoke(this, new LabelChangedEventArgs(oldIndex, changedItemIndex));
        }

        internal void Removed()
        {
            Labeled = null;
        }
    }
}