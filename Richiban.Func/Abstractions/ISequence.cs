using System.Collections.Generic;

namespace Richiban.Func
{
    public interface ISequence<out T> : IEnumerable<T> { }
}