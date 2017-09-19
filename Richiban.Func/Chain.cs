using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Richiban.Func
{
    /// <summary>
    /// A Chain is a lazily-evaluated singly linked list.
    /// 
    /// It also supports deconstruction into a tuple of the head and tail.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Chain<T> : IEnumerable<T>
    {
        public Chain(IEnumerable<T> source) : this(source.GetEnumerator()) { }

        private Chain(IEnumerator<T> enumerator)
        {
            _isEmpty = new Lazy<bool>(() => !enumerator.MoveNext());
            _head = new Lazy<T>(() => enumerator.Current);
            _tail = new Lazy<Chain<T>>(() => new Chain<T>(enumerator));
        }

        private readonly Lazy<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        private readonly Lazy<T> _head;

        public T Head
        {
            get
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Chain is empty");
                else
                    return _head.Value;
            }
        }

        private readonly Lazy<Chain<T>> _tail;
        public Chain<T> Tail => _tail.Value;

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

        public void Deconstruct(out T head, out Chain<T> tail)
        {
            head = Head;
            tail = Tail;
        }
    }
}
