using System;
using System.Collections;
using System.Collections.Generic;

namespace Richiban.Func
{
    /// <inheritdoc />
    /// <summary>
    /// A lazily-evaluated singly linked list.
    /// Also supports deconstruction into a tuple of the head and tail.
    /// </summary>
    public sealed class Chain<T> : IEnumerable<T>
    {
        public Chain(IEnumerable<T> source) : this(source.GetEnumerator()) { }

        private Chain(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        private readonly IEnumerator<T> _enumerator;
        private bool _isMaterialized = false;
        private bool _isEmpty;
        private T _head;
        private Chain<T> _tail;

        private void EnsureMaterialized()
        {
            if (_isMaterialized)
                return;

            _isEmpty = !_enumerator.MoveNext();
            _isMaterialized = true;

            if (_isEmpty == false)
            {
                _head = _enumerator.Current;
                _tail = new Chain<T>(_enumerator);
            }
        }

        public bool IsEmpty
        {
            get
            {
                EnsureMaterialized();

                return _isEmpty;
            }
        }

        public T Head
        {
            get
            {
                EnsureMaterialized();

                if (IsEmpty)
                    throw new InvalidOperationException("Chain is empty");
                else
                    return _head;
            }
        }

        public Chain<T> Tail
        {

            get
            {
                EnsureMaterialized();

                if (IsEmpty)
                    throw new InvalidOperationException("Chain is empty");
                else
                    return _tail;
            }
        }

        public void Deconstruct(out T head, out Chain<T> tail)
        {
            head = Head;
            tail = Tail;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var chain = this;

            while (!chain.IsEmpty)
            {
                yield return chain.Head;

                chain = chain.Tail;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
