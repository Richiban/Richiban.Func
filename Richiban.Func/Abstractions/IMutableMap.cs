namespace Richiban.Func
{
    public interface IMutableMap<in TKey, TValue> : IMap<TKey, TValue>
    {
        new TValue this[TKey key] { get; set; }
    }
}