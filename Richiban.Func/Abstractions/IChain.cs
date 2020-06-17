namespace Richiban.Func
{
    public interface IChain<out T> : ISequence<T>
    {
        T Head { get; }
        bool IsEmpty { get; }
        IChain<T> Tail { get; }
    }
}