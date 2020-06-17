using System;
using System.Collections;
using System.Collections.Generic;

namespace Richiban.Func.Enumerators
{
    public sealed class ChainEnumerator<T> : IEnumerator<T>
    {
        private IChain<T> _chain;
        private bool _disposed;

        public ChainEnumerator(IChain<T> chain)
        {
            _chain = chain;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _chain = null;
            _disposed = true;
        }

        public bool MoveNext()
        {
            if (_chain.IsEmpty)
                return false;

            (Current, _chain) = (_chain.Head, _chain.Tail);

            return true;
        }

        public void Reset() => throw new NotSupportedException();

        public T Current { get; private set; }
        object IEnumerator.Current => Current;
    }
}
