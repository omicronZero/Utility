
using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectGeneration
{
    public static partial class ObjectGeneratorExtensions
    {

        public static ObjectGenerator<T> Create<T, TParam1>(
                Func<TParam1, T> generator,
                ObjectGenerator<TParam1> param1Generator
            )
        {
			if (generator == null)
				throw new ArgumentNullException(nameof(generator));
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));

            return ObjectGenerator<T>.Create(() => generator(param1Generator.Compute()));
        }


        public static ObjectGenerator<T> Create<T, TParam1, TParam2>(
                Func<TParam1, TParam2, T> generator,
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator
            )
        {
			if (generator == null)
				throw new ArgumentNullException(nameof(generator));
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));

            return ObjectGenerator<T>.Create(() => generator(param1Generator.Compute(), param2Generator.Compute()));
        }


        public static ObjectGenerator<(TParam1, TParam2)> Combine<TParam1, TParam2>(
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator
            )
        {
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));

            return ObjectGenerator<(TParam1, TParam2)>.Create(() => (param1Generator.Compute(), param2Generator.Compute()));
        }

        public static ObjectGenerator<T> Create<T, TParam1, TParam2, TParam3>(
                Func<TParam1, TParam2, TParam3, T> generator,
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator
            )
        {
			if (generator == null)
				throw new ArgumentNullException(nameof(generator));
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));

            return ObjectGenerator<T>.Create(() => generator(param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute()));
        }


        public static ObjectGenerator<(TParam1, TParam2, TParam3)> Combine<TParam1, TParam2, TParam3>(
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator
            )
        {
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));

            return ObjectGenerator<(TParam1, TParam2, TParam3)>.Create(() => (param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute()));
        }

        public static ObjectGenerator<T> Create<T, TParam1, TParam2, TParam3, TParam4>(
                Func<TParam1, TParam2, TParam3, TParam4, T> generator,
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator
            )
        {
			if (generator == null)
				throw new ArgumentNullException(nameof(generator));
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));

            return ObjectGenerator<T>.Create(() => generator(param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute()));
        }


        public static ObjectGenerator<(TParam1, TParam2, TParam3, TParam4)> Combine<TParam1, TParam2, TParam3, TParam4>(
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator
            )
        {
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));

            return ObjectGenerator<(TParam1, TParam2, TParam3, TParam4)>.Create(() => (param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute()));
        }

        public static ObjectGenerator<T> Create<T, TParam1, TParam2, TParam3, TParam4, TParam5>(
                Func<TParam1, TParam2, TParam3, TParam4, TParam5, T> generator,
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator,
                ObjectGenerator<TParam5> param5Generator
            )
        {
			if (generator == null)
				throw new ArgumentNullException(nameof(generator));
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));
			if (param5Generator == null)
				throw new ArgumentNullException(nameof(param5Generator));

            return ObjectGenerator<T>.Create(() => generator(param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute(), param5Generator.Compute()));
        }


        public static ObjectGenerator<(TParam1, TParam2, TParam3, TParam4, TParam5)> Combine<TParam1, TParam2, TParam3, TParam4, TParam5>(
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator,
                ObjectGenerator<TParam5> param5Generator
            )
        {
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));
			if (param5Generator == null)
				throw new ArgumentNullException(nameof(param5Generator));

            return ObjectGenerator<(TParam1, TParam2, TParam3, TParam4, TParam5)>.Create(() => (param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute(), param5Generator.Compute()));
        }

        public static ObjectGenerator<T> Create<T, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
                Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, T> generator,
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator,
                ObjectGenerator<TParam5> param5Generator,
                ObjectGenerator<TParam6> param6Generator
            )
        {
			if (generator == null)
				throw new ArgumentNullException(nameof(generator));
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));
			if (param5Generator == null)
				throw new ArgumentNullException(nameof(param5Generator));
			if (param6Generator == null)
				throw new ArgumentNullException(nameof(param6Generator));

            return ObjectGenerator<T>.Create(() => generator(param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute(), param5Generator.Compute(), param6Generator.Compute()));
        }


        public static ObjectGenerator<(TParam1, TParam2, TParam3, TParam4, TParam5, TParam6)> Combine<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator,
                ObjectGenerator<TParam5> param5Generator,
                ObjectGenerator<TParam6> param6Generator
            )
        {
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));
			if (param5Generator == null)
				throw new ArgumentNullException(nameof(param5Generator));
			if (param6Generator == null)
				throw new ArgumentNullException(nameof(param6Generator));

            return ObjectGenerator<(TParam1, TParam2, TParam3, TParam4, TParam5, TParam6)>.Create(() => (param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute(), param5Generator.Compute(), param6Generator.Compute()));
        }

        public static ObjectGenerator<T> Create<T, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(
                Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, T> generator,
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator,
                ObjectGenerator<TParam5> param5Generator,
                ObjectGenerator<TParam6> param6Generator,
                ObjectGenerator<TParam7> param7Generator
            )
        {
			if (generator == null)
				throw new ArgumentNullException(nameof(generator));
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));
			if (param5Generator == null)
				throw new ArgumentNullException(nameof(param5Generator));
			if (param6Generator == null)
				throw new ArgumentNullException(nameof(param6Generator));
			if (param7Generator == null)
				throw new ArgumentNullException(nameof(param7Generator));

            return ObjectGenerator<T>.Create(() => generator(param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute(), param5Generator.Compute(), param6Generator.Compute(), param7Generator.Compute()));
        }


        public static ObjectGenerator<(TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7)> Combine<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator,
                ObjectGenerator<TParam5> param5Generator,
                ObjectGenerator<TParam6> param6Generator,
                ObjectGenerator<TParam7> param7Generator
            )
        {
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));
			if (param5Generator == null)
				throw new ArgumentNullException(nameof(param5Generator));
			if (param6Generator == null)
				throw new ArgumentNullException(nameof(param6Generator));
			if (param7Generator == null)
				throw new ArgumentNullException(nameof(param7Generator));

            return ObjectGenerator<(TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7)>.Create(() => (param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute(), param5Generator.Compute(), param6Generator.Compute(), param7Generator.Compute()));
        }

        public static ObjectGenerator<T> Create<T, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>(
                Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, T> generator,
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator,
                ObjectGenerator<TParam5> param5Generator,
                ObjectGenerator<TParam6> param6Generator,
                ObjectGenerator<TParam7> param7Generator,
                ObjectGenerator<TParam8> param8Generator
            )
        {
			if (generator == null)
				throw new ArgumentNullException(nameof(generator));
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));
			if (param5Generator == null)
				throw new ArgumentNullException(nameof(param5Generator));
			if (param6Generator == null)
				throw new ArgumentNullException(nameof(param6Generator));
			if (param7Generator == null)
				throw new ArgumentNullException(nameof(param7Generator));
			if (param8Generator == null)
				throw new ArgumentNullException(nameof(param8Generator));

            return ObjectGenerator<T>.Create(() => generator(param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute(), param5Generator.Compute(), param6Generator.Compute(), param7Generator.Compute(), param8Generator.Compute()));
        }


        public static ObjectGenerator<(TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8)> Combine<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>(
                ObjectGenerator<TParam1> param1Generator,
                ObjectGenerator<TParam2> param2Generator,
                ObjectGenerator<TParam3> param3Generator,
                ObjectGenerator<TParam4> param4Generator,
                ObjectGenerator<TParam5> param5Generator,
                ObjectGenerator<TParam6> param6Generator,
                ObjectGenerator<TParam7> param7Generator,
                ObjectGenerator<TParam8> param8Generator
            )
        {
			if (param1Generator == null)
				throw new ArgumentNullException(nameof(param1Generator));
			if (param2Generator == null)
				throw new ArgumentNullException(nameof(param2Generator));
			if (param3Generator == null)
				throw new ArgumentNullException(nameof(param3Generator));
			if (param4Generator == null)
				throw new ArgumentNullException(nameof(param4Generator));
			if (param5Generator == null)
				throw new ArgumentNullException(nameof(param5Generator));
			if (param6Generator == null)
				throw new ArgumentNullException(nameof(param6Generator));
			if (param7Generator == null)
				throw new ArgumentNullException(nameof(param7Generator));
			if (param8Generator == null)
				throw new ArgumentNullException(nameof(param8Generator));

            return ObjectGenerator<(TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8)>.Create(() => (param1Generator.Compute(), param2Generator.Compute(), param3Generator.Compute(), param4Generator.Compute(), param5Generator.Compute(), param6Generator.Compute(), param7Generator.Compute(), param8Generator.Compute()));
        }
    }
}
