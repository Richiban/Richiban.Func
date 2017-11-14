using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Richiban.Func
{
    /// <summary>
    /// A fast dictionary-style [string:T] lookup.
    /// 
    /// Note that, like most dictionaries, the order in which items will be enumerated is undefined.
    /// </summary>
    public sealed class Trie<T> : IMutableMap<string, T>
    {
        private readonly List<Trie<T>> _subTries = new List<Trie<T>>();
        private readonly char _nodeChar;
        private Option<T> _value;

        private Trie(char nodeChar)
        {
            _nodeChar = nodeChar;
        }

        public Trie() { }

        public T this[string key]
        {
            get => Get(new StringSlice(key)).Force();
            set => Set(new StringSlice(key), value);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var results = new List<T>();

            EnumerateInto(results);

            return results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>) this).GetEnumerator();
        
        public void Remove(string key) => Set(new StringSlice(key), Option<T>.None);

        private void EnumerateInto(ICollection<T> result)
        {
            if(_value.IsSome)
                result.Add(_value.Force());

            foreach (var subTrie in _subTries)
            {
                subTrie.EnumerateInto(result);
            }
        }

        private bool Set(StringSlice key, Option<T> value)
        {
            if (key.Current != _nodeChar)
                return false;

            if (key.AtEnd)
            {
                _value = value;

                return true;
            }

            var nextKey = key.Next();
            var addedToSubTrie = _subTries.Any(t => t.Set(nextKey, value));

            if (addedToSubTrie)
                return true;

            if (value.IsSome == false)
            {
                return true;
            }

            var newSubTrie = new Trie<T>(nextKey.Current);
            _subTries.Add(newSubTrie);

            return newSubTrie.Set(nextKey, value);
        }

        public Option<T> TryGet(string key) => Get(new StringSlice(key));

        private Option<T> Get(StringSlice key)
        {
            if (key.AtEnd)
            {
                if (key.Current == _nodeChar && _value.IsSome)
                {
                    return _value;
                }
                else
                {
                    return Option<T>.None;
                }
            }
            else
            {
                var nextSlice = key.Next();

                return _subTries.Select(trie => trie.Get(nextSlice)).FirstOrDefault(x => x.IsSome);
            }
        }
    }
}