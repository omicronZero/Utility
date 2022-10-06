using System;

namespace Utility.Dynamic
{
    public class RoutingTarget<TDescription, TState>
    {
        private readonly RoutingSources<TDescription, TState> _source;
        private int _sourceIndex;

        public TState State { get; set; }
        public TDescription Description { get; private set; }

        public int SourceIndex
        {
            get => _sourceIndex;
            set
            {
                if (value < 0 || value > _source.Entries.Count)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The specified index does not fall into the range of source entries of the routing provider.");

                _sourceIndex = value;
            }
        }

        internal void Clear(TDescription description)
        {
            SourceIndex = -1;
            State = default;
            Description = description;
        }

        internal RoutingTarget(RoutingSources<TDescription, TState> source, TDescription description)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            _source = source;
            Clear(description);
        }
    }
}