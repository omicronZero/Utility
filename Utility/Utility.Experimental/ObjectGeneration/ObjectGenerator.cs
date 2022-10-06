using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectGeneration
{
    public abstract partial class ObjectGenerator<T>
    {
        /// <summary>
        /// Generates a new element.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The last item of the object generator has been reached.</exception>
        public abstract T Compute();

        public virtual bool TryCompute(out T result)
        {
            try
            {
                result = Compute();
                return true;
            }
            catch (InvalidOperationException)
            {
                result = default(T);
                return false;
            }
        }

        public static ObjectGenerator<T> Create(Func<T> generator)
        {
            if (generator == null)
                throw new ArgumentNullException(nameof(generator));

            return new DelegateGenerator(generator);
        }

        private sealed class DelegateGenerator : ObjectGenerator<T>
        {
            public Func<T> ObjectGenerator { get; }

            public DelegateGenerator(Func<T> objectGenerator)
            {
                if (objectGenerator == null)
                    throw new ArgumentNullException(nameof(objectGenerator));

                ObjectGenerator = objectGenerator;
            }

            public override T Compute()
            {
                return ObjectGenerator();
            }
        }
    }
}
