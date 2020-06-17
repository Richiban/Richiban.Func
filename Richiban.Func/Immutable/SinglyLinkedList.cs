using System.Collections;
using System.Collections.Generic;
using Richiban.Func.Enumerators;

namespace Richiban.Func
{
    public sealed class SinglyLinkedList<T> : IChain<T>
    {
        public SinglyLinkedList()
        {
            IsEmpty = true;
        }

        public SinglyLinkedList(T head)
        {
            Head = head;
            IsEmpty = false;
        }

        public IEnumerator<T> GetEnumerator() => new ChainEnumerator<T>(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T Head { get; }
        public bool IsEmpty { get; }
        public SinglyLinkedList<T> Tail { get; set; }
        IChain<T> IChain<T>.Tail => Tail;

        public void Deconstruct(out T head, out SinglyLinkedList<T> tail) => (head, tail) = (Head, Tail);
    }
}