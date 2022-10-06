using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics.MathObjects
{
    internal static class AlgebraicProperties
    {
        private static readonly ConcurrentDictionary<string, int> _ids;
        private static int _maxId;
        private static string[] _propertyStrings;

        static AlgebraicProperties()
        {
            _ids = new ConcurrentDictionary<string, int>();
            _propertyStrings = Array.Empty<string>();
        }

        public static int Resolve(string name)
        {
            return _ids.GetOrAdd(name, NextId);
        }

        private static int NextId(string name)
        {
            int id = System.Threading.Interlocked.Increment(ref _maxId);

            if (id == 0)
                throw new InvalidOperationException("Maximum number of properties exceeded.");

            id = unchecked(id - 1);

            while (id >= _propertyStrings.Length)
            {
                Array.Resize(ref _propertyStrings, Math.Max(1, _propertyStrings.Length * 2));
            }

            _propertyStrings[id] = name;

            return id;
        }
    }
}
