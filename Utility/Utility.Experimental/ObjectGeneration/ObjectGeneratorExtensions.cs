using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectGeneration
{
    public static partial class ObjectGeneratorExtensions
    {
        public static ObjectGenerator<TOut> Select<T, TOut>(this ObjectGenerator<T> generator, Func<T, TOut> selector)
        {
            if (generator == null)
                throw new ArgumentNullException(nameof(generator));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return Create(selector, generator); 
        }
    }
}
