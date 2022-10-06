using Utility.Geometry.Buffers;

namespace Utility.Geometry.Buffers
{
    internal class Buffer3DMock<T> : Buffer3D<T>
    {
        private AccessRangeView3D<T> _view;

        public Buffer3DMock(AccessRangeView3D<T> view)
        {
            _view = view;
        }

        public override T this[int x, int y, int z]
        {
            get => _view[x, y, z];
            set => _view[x, y, z] = value;
        }

        public override int Width => (int)(_view.IntervalX.End - _view.IntervalX.Start);

        public override int Height => (int)(_view.IntervalY.End - _view.IntervalY.Start);
        public override int Depth => (int)(_view.IntervalZ.End - _view.IntervalZ.Start);

        public override bool IsReadOnly => _view.IsReadOnly;
    }
}