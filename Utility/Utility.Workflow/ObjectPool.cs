using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Represents an object pool that manages reusable objects. See remarks for usage hints.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// The <see cref="ObjectPool{T}"/> class can be used to keep objects alive that are expensive to create and otherwise would otherwise be finalized by the garbage collection.
    /// This typically is the case when allocating a new object frequently happens or if it is desirable to avoid the cost of such an allocation.
    /// 
    /// The recommended use is, to not expose a reference to the objects handled by the object pool, e.g., by non-public references to the pooled instance. Otherwise, a user may actually
    /// use it to use, for example, a dictionary or otherwise do reference-based comparisons in case <see cref="T"/> represents a reference type (which would typically be the case).
    /// 
    /// An object can be queried from the pool via the <see cref="ObjectPool{T}.Allocate(out T)"/> method. Calling <see cref="Disposable.Dispose"/> on the returned object will put it back
    /// into the object pool or push it to the garbage collection. The first parameter will receive the allocated object.
    /// Multiple objects can be queried at once via <see cref="ObjectPool{T}.Allocate(T[], int, int)"/>. However, disposing the returned object will put ALL instances back into the pool
    /// (or free some of them). Note that a call to this method is expensive and should not be used unless the pooled objects are expensive to create.
    /// 
    /// When an object pool object is put back, the delegate supplied in <see cref="ObjectPool{T}.ObjectPool(Func{T}, Action{T}, int, int, ThreadingType)"/> will be called. This may be used
    /// to create a default state on the object. Note that this call is synchronous with the disposing thread and only happens if the object is queued for reuse.
    /// Reinitializing a state can be implemented by a method that is exposed on <see cref="T"/> but is not explicitly supported by the <see cref="ObjectPool{T}"/> class.
    /// 
    /// Typically, you will choose excess count to be greater than preallocationCount as, otherwise, all previously allocated objects will be finalized unless the reallocation was slower than the
    /// method that grabbed the excess object (this will probably lead to an unrequired allocation of a new object unless the management of the object pool is synchronous, i.e., 
    /// <see cref="ThreadingType.None"/>).
    /// 
    /// In general, testing whether efficiency actually increases using the <see cref="ObjectPool{T}"/> is recommended.
    /// 
    /// In general, thread-safety is guaranteed by the <see cref="ObjectPool{T}"/> class.
    /// </remarks>
    public class ObjectPool<T> : IDisposable
    {
        //TODO: do not use System.Threading.Monitor for efficiency. Rather use spin locks with a Monitor fallback after a few thousand yielding iterations or similar.
        private readonly Func<T> _allocationFunction;
        private readonly Action<T> _clearHandler;
        private readonly Queue<T> _queue;
        private readonly object _syncRoot;

        private readonly int _excessCount;

        private int _pendingCount;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
        /// </summary>
        /// <param name="allocationFunction">A delegate to the factory function that allocates a new instance of the pooled object type <see cref="T"/>.</param>
        /// <param name="preallocationCount">The number of elements that should be asynchronously be made available to make it possible to quickly allocate a new pool object.
        ///     If <paramref name="managementType"/> is set to <see cref="ThreadingType.None"/>, this value is ignored.</param>
        /// <param name="excessCount">The maximum number of objects to keep allocated in the object pool. Values beyond this number will be marked for finalization after being cleared, if necessary.</param>
        /// <param name="managementType">The way to manage the current object pool. <see cref="ThreadingType.None"/> applies synchronous management</param>
        public ObjectPool(Func<T> allocationFunction, int preallocationCount, int excessCount, ThreadingType managementType)
            : this(allocationFunction, null, preallocationCount, excessCount, managementType)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
        /// </summary>
        /// <param name="allocationFunction">A delegate to the factory function that allocates a new instance of the pooled object type <see cref="T"/>.</param>
        /// <param name="clearHandler">A delegate to call when a previously allocated pool object is added back to the pool.</param>
        /// <param name="preallocationCount">The number of elements that should be asynchronously be made available to make it possible to quickly allocate a new pool object.
        ///     If <paramref name="managementType"/> is set to <see cref="ThreadingType.None"/>, this value is ignored.</param>
        /// <param name="excessCount">The maximum number of objects to keep allocated in the object pool. Values beyond this number will be marked for finalization after being cleared, if necessary.</param>
        /// <param name="managementType">The way to manage the current object pool. <see cref="ThreadingType.None"/> applies synchronous management</param>
        public ObjectPool(Func<T> allocationFunction, Action<T> clearHandler, int preallocationCount, int excessCount, ThreadingType managementType)
        {
            if (allocationFunction == null)
                throw new ArgumentNullException(nameof(allocationFunction));

            if (preallocationCount < 0)
                throw new ArgumentOutOfRangeException(nameof(preallocationCount), preallocationCount, "Non-negative amount of preallocations expected.");

            if (excessCount < 0)
                throw new ArgumentOutOfRangeException(nameof(excessCount), excessCount, "Non-negative amount of excess expected.");

            _queue = new Queue<T>();
            _allocationFunction = allocationFunction;
            _clearHandler = clearHandler;
            _excessCount = excessCount;
            _syncRoot = new object();

            _pendingCount = preallocationCount;

            if (managementType == ThreadingType.ThreadPool)
            {
                ThreadPool.QueueUserWorkItem((o) => Handle());
            }
            else if (managementType == ThreadingType.Thread)
            {
                new Thread(() => Handle()) { IsBackground = true }.Start();
            }
        }

        /// <summary>
        /// Gets the number of currently allocated instances.
        /// </summary>
        public int AllocatedInstances
        {
            get
            {
                lock (_syncRoot) 
                    return _queue.Count;
            }
        }

        private void Handle()
        {
            Thread ct = Thread.CurrentThread;
            bool isBackgroundThread = ct.IsBackground;

            void allocationHandler(int i)
            {
                var thread = Thread.CurrentThread;
                bool isBackground = thread.IsBackground;

                thread.IsBackground = false;

                try
                {
                    _queue.Enqueue(_allocationFunction());
                }
                finally
                {
                    thread.IsBackground = isBackground;
                }
                _pendingCount--;
            }

            while (!_disposed)
            {
                lock (_syncRoot)
                {
                    if (_pendingCount == 0)
                    {
                        ct.IsBackground = isBackgroundThread;

                        do
                        {
                            Monitor.Wait(_syncRoot);
                        } while (_pendingCount == 0);

                        ct.IsBackground = false;
                    }
                    Parallel.For(0, _pendingCount, allocationHandler);
                }
            }
        }

        /// <summary>
        /// Allocates a single instance from the current instance and returns an <see cref="IDisposable"/> object used to put the allocated
        /// instance back into the pool.
        /// </summary>
        /// <param name="instance">Receives the allocated instance.</param>
        /// <returns>An <see cref="IDisposable"/> object used to put the allocated instance back into the pool.</returns>
        public Disposable Allocate(out T instance)
        {
            if (_disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            lock (_syncRoot)
            {
                if (_queue.Count == 0)
                {
                    SynchronizedAllocateExcess();
                    instance = _allocationFunction();
                }
                else
                {
                    instance = _queue.Dequeue();
                }
            }

            return new Disposable(this, instance);
        }

        private void SynchronizedAllocateExcess()
        {
            if (_pendingCount == 0)
            {
                int newPending = _excessCount / 2;

                if (newPending > 0)
                {
                    _pendingCount = newPending;
                    Monitor.PulseAll(_syncRoot);
                }
            }
        }

        /// <summary>
        /// Allocates multiple instances from the current instance and returns a single <see cref="IDisposable"/> object used to put
        /// all of the allocated instances back into the pool. See the remarks on <see cref="ObjectPool{T}"/> for further usage
        /// information and suggestions.
        /// </summary>
        /// <param name="instances">The buffer that receives the allocated instances.</param>
        /// <param name="index">The first index that receives the allocated instances.</param>
        /// <param name="count">The number of allocated instances and the size of the region in <paramref name="instances"/> that
        ///     receives them.</param>
        /// <returns>An <see cref="IDisposable"/> object used to at once put all of the allocated instances back into the pool.</returns>
        public Disposable Allocate(T[] instances, int index, int count)
        {
            if (_disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            Util.ValidateNamedRange(instances, index, count, arrayName: nameof(instances));

            if (count == 0)
                return Disposable.Empty;

            T[] alloc = new T[count];
            int c = 0;

            lock (_syncRoot)
            {
                int qc = _queue.Count;


                for (int i = 0; i < qc && count > 0; i++, count--)
                {
                    T v = _queue.Dequeue();
                    instances[index++] = v;
                    alloc[c++] = v;
                }

                if (_queue.Count == 0)
                    SynchronizedAllocateExcess();

                if (count > 0)
                {
                    Parallel.For(0, count, (i) =>
                    {
                        T v = _allocationFunction();
                        instances[index + i] = v;
                        alloc[c + i] = v;
                    });
                }
            }

            return new Disposable(this, alloc);
        }

        private void Free(T instance)
        {
            lock (_syncRoot)
            {
                if (_disposed || _queue.Count >= _excessCount)
                {
                    if (instance is IDisposable d)
                        d.Dispose();
                    else if (instance is IAsyncDisposable ad)
                        ad.DisposeAsync();

                    return;
                }
                else
                {
                    _clearHandler?.Invoke(instance);
                    _queue.Enqueue(instance);
                }
            }
        }

        private void Free(T[] instances)
        {
            lock (_syncRoot)
            {
                Action<T> clearAction = _clearHandler;

                foreach (T v in instances)
                {
                    clearAction?.Invoke(v);
                    if (_disposed || _queue.Count > _excessCount)
                    {
                        if (v is IDisposable d)
                            d.Dispose();
                        else if (v is IAsyncDisposable ad)
                            ad.DisposeAsync();
                    }
                    else
                    {
                        _queue.Enqueue(v);
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            //TODO: check correctness
            _disposed = true;
        }

        /// <summary>
        /// Disposes the current object pool and frees all objects currently allocated in it.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Used to put allocated instances back into the pool.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Usually not directly receiving calls except via using and not used anywhere else.")]
        public struct Disposable : IDisposable, IEquatable<Disposable>
        {
            internal static Disposable Empty => default;

            private readonly long _generation;
            private DisposableHelp _help;

            internal Disposable(ObjectPool<T> pool, T instance)
            {
                if (pool == null)
                    throw new ArgumentNullException(nameof(pool));

                _help = DisposableHelp.Allocate();

                _help.SetData(pool, instance, out _generation);
            }

            internal Disposable(ObjectPool<T> pool, T[] instances)
            {
                if (pool == null)
                    throw new ArgumentNullException(nameof(pool));

                if (instances == null)
                    throw new ArgumentNullException(nameof(instances));

                _help = DisposableHelp.Allocate();

                _help.SetData(pool, instances, out _generation);
            }

            /// <summary>
            /// Called to put previously allocated instances back into the pool.
            /// </summary>
            public void Dispose()
            {
                _help?.Free(_generation);
                _help = null;
            }


            public bool Equals(Disposable other) => _generation == other._generation && other._help == _help;

            public override bool Equals(object obj)
            {
                return obj != null && obj is Disposable other && Equals(other);
            }

            public override int GetHashCode()
            {
                return (_help?.GetHashCode() ?? 0) ^ _generation.GetHashCode();
            }

            /// <summary>
            /// Returns, whether two <see cref="Disposable"/> objects are equal.
            /// </summary>
            /// <param name="left">The first <see cref="Disposable"/> object to compare.</param>
            /// <param name="right">The second <see cref="Disposable"/> object to compare.</param>
            /// <returns>Returns, whether both supplied <see cref="Disposable"/> objects are equal.</returns>
            public static bool operator ==(Disposable left, Disposable right)
            {
                return left.Equals(right);
            }

            /// <summary>
            /// Returns, whether two <see cref="Disposable"/> objects are unequal.
            /// </summary>
            /// <param name="left">The first <see cref="Disposable"/> object to compare.</param>
            /// <param name="right">The second <see cref="Disposable"/> object to compare.</param>
            /// <returns>Returns, whether both supplied <see cref="Disposable"/> objects are unequal.</returns>
            public static bool operator !=(Disposable left, Disposable right)
            {
                return !(left == right);
            }
        }

        //internal disposable pool. _generation serves as a value that validates Disposable references to prevent
        //subsequent calls from destroying the reused DisposableHelp-object (generation will increase after calling the
        //Dispose-method on Disposable).
        private sealed class DisposableHelp
        {
            private const int Limit = 256;

            private static readonly ConcurrentBag<DisposableHelp> FreeDisposables = new ConcurrentBag<DisposableHelp>();

            private ObjectPool<T> _pool;
            private T _instance;
            private T[] _instances;
            private long _generation;

            private DisposableHelp()
            { }

            public void SetData(ObjectPool<T> pool, T instance, out long generation)
            {
                _pool = pool;
                _instance = instance;
                generation = _generation;
            }

            public void SetData(ObjectPool<T> pool, T[] instances, out long generation)
            {
                _pool = pool;
                _instances = instances;
                generation = _generation;
            }

            public void Free(long generation)
            {
                if (Interlocked.CompareExchange(ref _generation, generation + 1, generation) != generation)
                    return;

                ObjectPool<T> pool = Interlocked.Exchange(ref _pool, null);

                if (pool == null)
                    return;

                if (_instances == null)
                    pool.Free(_instance);
                else
                    pool.Free(_instances);

                _instance = default;
                _instances = null;

                FreeDisposables.Add(this);

                if (FreeDisposables.Count > Limit)
                {
                    FreeDisposables.TryTake(out _);
                }
            }

            public static DisposableHelp Allocate()
            {
                DisposableHelp result;

                if (!FreeDisposables.TryTake(out result))
                {
                    result = new DisposableHelp();
                }

                return result;
            }
        }
    }
}
