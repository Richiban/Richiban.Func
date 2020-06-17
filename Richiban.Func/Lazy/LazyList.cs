using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Richiban.Func
{
    /// <summary>
    /// A class that wraps an IEnumerable and stores each yielded element of that IEnumerable
    /// in a list, as it is evaluated.
    /// </summary>
    public sealed class LazyList<T> : IReadOnlyList<T>
    {
        private readonly IEnumerator<T> _sourceEnumerator;
        private readonly IList<T> _cache = new List<T>();
        private bool _finishedEnumerating = false;

        public LazyList(IEnumerable<T> source) : this(source.GetEnumerator()) { }

        private LazyList(IEnumerator<T> sourceEnumerator)
        {
            _sourceEnumerator = sourceEnumerator;
        }

        public T this[int index]
        {
            get
            {
                var indexHasBeenCached = index < _cache.Count || _finishedEnumerating;

                if (indexHasBeenCached)
                    return _cache[index];

                var numItemsToAdvance = index - _cache.Count;

                return RemainingEnumeration().ElementAt(numItemsToAdvance);
            }
        }

        public int Count => _finishedEnumerating ? _cache.Count : Enumerate().Count();
        public IEnumerator<T> GetEnumerator() => Enumerate().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerable<T> Enumerate() =>
            _finishedEnumerating ? _cache : _cache.Concat(RemainingEnumeration());

        private IEnumerable<T> RemainingEnumeration()
        {
            while (_sourceEnumerator.MoveNext())
            {
                _cache.Add(_sourceEnumerator.Current);
                yield return _sourceEnumerator.Current;
            }

            _finishedEnumerating = true;
            _sourceEnumerator.Dispose();
        }
    }
}