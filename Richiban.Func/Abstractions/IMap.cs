using System.Dynamic;

namespace Richiban.Func
{
    public interface IMap<in TKey, out TValue> : ISequence<TValue>
    {
        TValue this[TKey key] { get; }
    }
}