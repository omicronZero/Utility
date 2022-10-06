using Utility.Geometry.Buffers;

namespace Utility.Geometry.Buffers
{
    internal class Buffer2DMock<T> : Buffer2D<T>
    {
        private AccessRangeView2D<T> _view;

        public Buffer2DMock(AccessRangeView2D<T> view)
        {
            _view = view;
        }

        public override T this[int x, int y]
        {
            get => _view[x, y];
            set => _view[x, y] = value;
        }

        public override int Width => (int)(_view.IntervalX.End - _view.IntervalX.Start);

        public override int Height => (int)(_view.IntervalY.End - _view.IntervalY.Start);

        public override bool IsReadOnly => _view.IsReadOnly;
    }
}