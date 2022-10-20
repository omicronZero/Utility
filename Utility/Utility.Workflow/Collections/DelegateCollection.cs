using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections.Tools;
using Utility.Workflow.Collections.Adapters;

namespace Utility.Collections
{
    public class DelegateCollection<T> : CollectionBase<T>
    {
        private readonly Func<int> _countEvaluator;
        private readonly Func<T, bool> _containsEvaluator;
        private readonly IEnumerable<T> _enumeration;

        public DelegateCollection(IEnumerable<T> enumeration, Func<int> countEvaluator)
            : this(enumeration, countEvaluator, null)
        { }

        public DelegateCollection(IEnumerable<T> enumeration, Func<int> countEvaluator, Func<T, bool> containsEvaluator)
        {
            _enumeration = enumeration ?? throw new ArgumentNullException(nameof(enumeration));
            _countEvaluator = countEvaluator ?? throw new ArgumentNullException(nameof(countEvaluator));
            _containsEvaluator = containsEvaluator;
        }

        public override int Count => _countEvaluator();

        public override bool IsReadOnly => true;

        public override void Add(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public override void Clear()
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public override bool Contains(T item)
        {
            if (_containsEvaluator == null)
                return _enumeration.Contains(item);
            else
                return _containsEvaluator(item);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return _enumeration.GetEnumerator();
        }

        public override bool Remove(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }
    }
}
