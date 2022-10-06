using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Utility.Data
{
    public sealed class AssembledObject<T>
    {
        private readonly int _totalBytes;
        private readonly T[] _data;
        private int _currentBytes;
        private GCHandle _bufferHandle;

        internal AssembledObject(int count)
            : this(new T[count < 0 ? throw new ArgumentOutOfRangeException(nameof(count), "Non-negative count expected.") : count])
        { }

        internal AssembledObject(T[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _totalBytes = typeof(T).IsPrimitive ? Buffer.ByteLength(data) : Marshal.SizeOf<T>() * data.Length;
            _data = data;

            if (data.Length > 0)
                _bufferHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
        }

        public bool IsComplete
        {
            get { return _currentBytes == _totalBytes; }
        }

        public T[] AsArray()
        {
            if (!IsComplete)
                throw new InvalidOperationException("Object is incomplete.");

            return _data;
        }

        public T AsSingleton()
        {
            if (_data.Length != 1)
                throw new InvalidOperationException("Object is not a singleton.");

            return AsArray()[0];
        }

        public int RemainingBytes
        {
            get { return _totalBytes - _currentBytes; }
        }

        internal void Write(byte[] buffer, int index, int count)
        {
            if (count > RemainingBytes)
                throw new ArgumentException("Object does not accept this amount of bytes.");

            if (count == 0)
                return;

            IntPtr bptr = _bufferHandle.AddrOfPinnedObject();

            Marshal.Copy(buffer, index, bptr + _currentBytes, count);

            _currentBytes += count;

            if (IsComplete)
                _bufferHandle.Free();
        }

        ~AssembledObject()
        {
            if (!IsComplete)
                _bufferHandle.Free();
        }

        public static explicit operator T(AssembledObject<T> instance)
        {
            if (instance == null)
                return default(T);
            else
                return instance.AsSingleton();
        }

        public static explicit operator T[] (AssembledObject<T> instance)
        {
            if (instance == null)
                return null;
            else
                return instance.AsArray();
        }
    }
}
