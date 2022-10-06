using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public struct ILLabel
    {
        private int _index;
        private ILabelProvider _labelProvider;

        public bool IsIndex
        {
            get { return _labelProvider != null; }
        }

        /// <summary>
        /// Returns the byte address that is represented by this label. For indices, this value is absolute, otherwise it is to be relative.
        /// </summary>
        public int Address
        {
            get { return _labelProvider == null ? _index : _labelProvider.GetByteAddress(_index); }
        }

        public void MakeRelativeTo(int instructionIndex)
        {
            if (!IsIndex)
                throw new InvalidOperationException("Label must represent an index to be made relative to a specific instruction index.");

            this = new ILLabel(_labelProvider.GetRelativeAddress(_index, instructionIndex));
        }

        public void MakeRelativeTo(ILabelProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (!IsIndex)
                throw new InvalidOperationException("Label must represent an index.");

            this = new ILLabel(Index, provider);
        }

        public void MakeRelativeTo(ILabelProvider provider, int instructionAddress, bool instructionAddressIsIndex)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (IsIndex)
                throw new InvalidOperationException("Label must not represent an index.");

            if (!instructionAddressIsIndex)
                instructionAddress = provider.GetIndex(instructionAddress);

            this = new ILLabel(provider.GetIndex(instructionAddress, Address), provider);
        }

        internal int Raw
        {
            get { return _index; }
        }

        public ILabelProvider LabelProvider
        {
            get { return _labelProvider; }
        }
        
        public int Index
        {
            get
            {
                CheckIsIndex();
                return _index;
            }
            set
            {
                CheckIsIndex();

                if (value < 0 || value > _labelProvider.InstructionCount)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Index does not fall into the range of instructions.");

                _index = value;
            }
        }

        private void CheckIsIndex()
        {
            if (_labelProvider == null)
                throw new NotSupportedException("Label does not represent the index of a label.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ILLabel"/> value type taking a relative byte address to a label as used in branch instructions.
        /// </summary>
        /// <param name="relativeByteAddress">An address that is relative to the instruction that makes use of the label.</param>
        public ILLabel(int relativeByteAddress)
        {
            _labelProvider = null;
            _index = relativeByteAddress;
        }

        internal ILLabel(int offset, ILabelProvider labelProvider)
        {
            _labelProvider = labelProvider;
            _index = offset;
        }
    }
}
