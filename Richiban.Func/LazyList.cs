using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Richiban.Func
{
    /// <summary>
    /// A class that wraps an IEnumerable and stores each returned element of that IEnumerable
    /// in a list, as it is evaluated.
    /// </summary>
    public class LazyList<T> : IEnumerable<T>, IReadOnlyList<T>
    {
        private readonly IEnumerator<T> _sourceEnumerator;
        private readonly IList<T> _cache = new List<T>();
        private bool _finishedEnumerating = false;

        public LazyList(IEnumerable<T> source)
        {
            _sourceEnumerator = source.GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                var shouldAdvance = index >= _cache.Count && !_finishedEnumerating;

                if (!shouldAdvance)
                    return _cache[index];

                var advanceByN = index - _cache.Count;

                return AdvanceEnumeration().ElementAt(advanceByN);
            }
        }

        public int Count => _finishedEnumerating ? _cache.Count : Enumerate().Count();
        public IEnumerator<T> GetEnumerator() => Enumerate().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerable<T> Enumerate() =>
            _finishedEnumerating ? _cache : _cache.Concat(AdvanceEnumeration());

        private IEnumerable<T> AdvanceEnumeration()
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
