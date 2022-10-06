using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public static partial class ProgressingIterator
    {
        public static ProgressingIterator<TResult, TItem, TProgress, TProgressAccumulate> CreateInstance<TResult, TItem, TProgress, TProgressAccumulate>(
            IEnumerable<TItem> enumerable,
            TProgress zero,
            Func<TItem, TProgress> maximumProgressEvaluator,
            Func<TProgress, TProgressAccumulate, TProgress> progressAdd,
            Func<TProgress, TProgress, TProgressAccumulate> progressSubtract,
            Comparison<TProgress> progressComparison,
            Func<TItem, TProgress, TResult> resultSelector)
        {
            return new ProgressingIterator<TResult, TItem, TProgress, TProgressAccumulate>(
                    enumerable,
                    zero,
                    maximumProgressEvaluator,
                    progressAdd,
                    progressSubtract,
                    progressComparison,
                    resultSelector);
        }

        public static ProgressingIterator<TResult, TItem, TProgress, TProgressAccumulate> CreateInstance<TResult, TItem, TProgress, TProgressAccumulate>(
                IEnumerable<TItem> enumerable,
                TProgress zero,
                Func<TItem, TProgress> maximumProgressEvaluator,
                Func<TProgress, TProgressAccumulate, TProgress> progressAdd,
                Func<TProgress, TProgress, TProgressAccumulate> progressSubtract,
                Func<TItem, TProgress, TResult> resultSelector)
            where TProgress : IComparable<TProgress>
        {
            return new ProgressingIterator<TResult, TItem, TProgress, TProgressAccumulate>(
                    enumerable,
                    zero,
                    maximumProgressEvaluator,
                    progressAdd,
                    progressSubtract,
                    (l, r) => l.CompareTo(r),
                    resultSelector);
        }
    }

    public class ProgressingIterator<TResult, TItem, TProgress, TProgressAccumulate> : IDisposable
    {
        private IEnumerator<TItem> _enumerator;
        private Func<TItem, TProgress> _maximumProgressEvaluator;
        private Func<TProgress, TProgressAccumulate, TProgress> _progressAdd;
        private Func<TProgress, TProgress, TProgressAccumulate> _progressSubtract;
        private Comparison<TProgress> _progressComparison;
        private Func<TItem, TProgress, TResult> _resultSelector;
        private TProgress _zero;

        private TProgress _currentProgress;
        private TResult _currentResult;
        private bool _resultComputed;

        protected bool IsDisposed { get; private set; }

        public bool Ended { get; private set; }

        public ProgressingIterator(
            IEnumerable<TItem> enumeration,
            TProgress zero,
            Func<TItem, TProgress> maximumProgressEvaluator,
            Func<TProgress, TProgressAccumulate, TProgress> progressAdd,
            Func<TProgress, TProgress, TProgressAccumulate> progressSubtract,
            Comparison<TProgress> progressComparison,
            Func<TItem, TProgress, TResult> resultSelector)
        {
            if (enumeration == null)
                throw new ArgumentNullException(nameof(enumeration));
            if (maximumProgressEvaluator == null)
                throw new ArgumentNullException(nameof(maximumProgressEvaluator));
            if (progressAdd == null)
                throw new ArgumentNullException(nameof(progressAdd));
            if (progressSubtract == null)
                throw new ArgumentNullException(nameof(progressSubtract));
            if (progressComparison == null)
                throw new ArgumentNullException(nameof(progressComparison));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            _enumerator = enumeration.GetEnumerator();
            Ended = !_enumerator.MoveNext();

            _maximumProgressEvaluator = maximumProgressEvaluator;
            _progressSubtract = progressSubtract;
            _progressAdd = progressAdd;
            _progressComparison = progressComparison;
            _zero = zero;
            _resultSelector = resultSelector;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                _enumerator.Dispose();
                _enumerator = null;
                _maximumProgressEvaluator = null;
                _progressSubtract = null;
                _progressAdd = null;
                _progressComparison = null;
                _zero = default(TProgress);
                _resultSelector = null;
                _currentResult = default(TResult);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public TResult CurrentResult
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(this.GetType().Name);

                if (!_resultComputed)
                    _currentResult = _resultSelector(_enumerator.Current, _currentProgress);

                return _currentResult;
            }
        }

        public bool Progress(TProgressAccumulate value, out TProgressAccumulate remainder, out bool newValue)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);

            if (Ended)
            {
                remainder = value;
                newValue = false;
                return false;
            }

            TProgress maxProgress = _maximumProgressEvaluator(_enumerator.Current);
            TProgress newProgress = _progressAdd(_currentProgress, value);

            if (_progressComparison(newProgress, maxProgress) > 0)
            {
                remainder = _progressSubtract(newProgress, maxProgress);

                _currentProgress = _zero;

                newValue = true;

                TItem previous = _enumerator.Current;

                if (!_enumerator.MoveNext())
                {
                    _currentResult = _resultSelector(previous, maxProgress);
                    _resultComputed = true;

                    Ended = true;
                    return false;
                }

                _currentResult = default(TResult);
                _resultComputed = false;
            }
            else
            {
                _currentProgress = newProgress;

                remainder = default(TProgressAccumulate);
                newValue = false;
            }

            return true;
        }

        public bool ProgressAll(TProgressAccumulate value, out TProgressAccumulate remainder, ICollection<TResult> progressHandler, out bool newValue)
        {
            return ProgressAll(value, out remainder, progressHandler == null ? (Action<TResult>)null : progressHandler.Add, out newValue);
        }

        public bool ProgressAll(TProgressAccumulate value, out TProgressAccumulate remainder, Action<TResult> progressHandler, out bool newValue)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);

            newValue = false;

            bool localNewValue;
            bool going;

            do
            {
                going = Progress(value, out value, out localNewValue);
                remainder = value;

                progressHandler?.Invoke(CurrentResult);

                newValue |= localNewValue;
            } while (localNewValue);

            return going;
        }
    }
}
