namespace Utility.Dynamic.Disassembly
{
    public struct ArrayShape
    {
        public uint Rank { get; }

        private uint[] _sizes;
        private int[] _loBounds;

        public uint SizeCount
        {
            get { return (uint)_sizes.LongLength; }
        }

        public uint LowerBoundsCount
        {
            get { return (uint)_loBounds.LongLength; }
        }

        public int GetSize(int index)
        {
            //TODO: validate index

            return unchecked((int)_sizes[index]);
        }

        public int GetLowerBound(int index)
        {
            //TODO: validate index

            return _loBounds[index];
        }

        public ArrayShape(uint rank, uint[] sizes, int[] lowerBounds)
        {
            Rank = rank;
            _sizes = sizes;
            _loBounds = lowerBounds;
        }
    }
}