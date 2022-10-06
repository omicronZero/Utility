using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Utility.Contexts
{
    public static class GlobalContext<TContext>
        where TContext : class
    {
        private static TContext DefaultContext;

        private static readonly Stack<(long Generation, TContext Context)> _contexts;

        private static TContext _currentContext;
        private static long _generation;

        private static readonly ReaderWriterLockSlim Lock;
        private static readonly object SyncRoot;

        static GlobalContext()
        {
            SyncRoot = new object();
            Lock = new ReaderWriterLockSlim();

            _contexts = new Stack<(long Generation, TContext Context)>();
        }

        /// <summary>
        /// Gets a default context for the current context type. See remarks for further information.
        /// </summary>
        /// <returns>The default context for the current context type.</returns>
        /// <remarks>
        /// Requires a context initializer attribute on the current type (see <see cref="ContextInitializerAttribute"/>).
        /// Fails, if the default context cannot be created.
        /// </remarks>
        public static TContext GetDefaultContext()
        {
            TContext context = DefaultContext;

            //initialize the default context once for all threads
            //lock only after null check for efficiency reasons
            if (context == null)
            {
                lock (SyncRoot)
                {
                    //check again as DefaultContext might have changed before the lock on SyncRoot was acquired
                    context = DefaultContext;

                    if (context == null)
                    {
                        context = ContextInitializer.CreateDefault<TContext>();
                        DefaultContext = context;
                    }
                }
            }

            return context;
        }

        public static TContext CurrentContext
        {
            get
            {
                Lock.EnterReadLock();

                try
                {
                    return _currentContext ?? GetDefaultContext();
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        public static TContext GetCurrentContext(bool fallbackToDefault)
        {
            Lock.EnterReadLock();

            try
            {
                return fallbackToDefault ? _currentContext ?? GetDefaultContext() : _currentContext;
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public static ContextHandle Push(TContext context)
        {
            long generation;

            Lock.EnterWriteLock();

            try
            {
                generation = ++_generation;

                _contexts.Push((generation, context));
                _currentContext = context;
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return new ContextHandle(generation);
        }

        public struct ContextHandle : IDisposable
        {
            private readonly long _generation;

            internal ContextHandle(long generation)
            {
                _generation = generation;
            }

            public void Pop()
            {
                Lock.EnterWriteLock();
                try
                {
                    if (_contexts.Peek().Generation != _generation)
                    {
                        foreach (var (Generation, Context) in _contexts)
                        {
                            if (Generation == _generation) //other contexts were pushed beforehand
                                throw new InvalidOperationException("The current context handle does not refer to the currently active context.");
                        }

                        return; //redundant calls to Dispose for the same handle
                    }

                    _contexts.Pop();

                    _currentContext = _contexts.Count == 0 ? null : _contexts.Peek().Context;
                }
                finally
                {
                    Lock.ExitWriteLock();
                }
            }

            void IDisposable.Dispose()
            {
                Pop();
            }
        }
    }
}
