using System.Collections;
using System.Collections.Generic;
using Richiban.Func.Enumerators;

namespace Richiban.Func
{
    public class Stack<T> : IChain<T>
    {
        private readonly SinglyLinkedList<T> _list;

        public Stack() : this(new SinglyLinkedList<T>()) { }

        private Stack(SinglyLinkedList<T> list)
        {
            _list = list;
        }

        public T Head => _list.Head;
        public Stack<T> Tail => new Stack<T>(_list.Tail);
        public bool IsEmpty => _list.IsEmpty;
        IChain<T> IChain<T>.Tail => Tail;

        public IEnumerator<T> GetEnumerator() => new ChainEnumerator<T>(this);

        public void Deconstruct(out T head, out IChain<T> tail) => (head, tail) = (Head, Tail);

        public void Deconstruct(out T head, out Stack<T> tail) => (head, tail) = Pop();

        public (T, Stack<T>) Pop() => (Head, Tail);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}