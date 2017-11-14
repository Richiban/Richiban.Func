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
    public sealed class MutableTrie<T> : IMutableMap<string, T>
    {
        private readonly LinkedList<MutableTrie<T>> _subTries = new LinkedList<MutableTrie<T>>();
        private readonly char _nodeChar;
        private Option<T> _value;

        private MutableTrie(char nodeChar)
        {
            _nodeChar = nodeChar;
        }

        public MutableTrie() { }

        public T this[string key]
        {
            get => Get(new StringSlice(key)).Force();
            set => Set(new StringSlice(key), value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var results = new List<T>();

            EnumerateInto(results);

            return results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Option<T> TryGet(string key) => Get(new StringSlice(key));
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>
        /// True if the value was set either in this node or one of its subtries. False otherwise
        /// </returns>
        private bool Set(StringSlice key, Option<T> value)
        {
            // If the current char in the key is not our node char then we're going down the wrong
            // branch and we can quit early.
            if (key.Current != _nodeChar)
                return false;

            // If we have reached the end of the key then we are at the correct node at which to actually
            // set the value
            if (key.AtEnd)
            {
                _value = value;

                return true;
            }

            // By this point we are on the correct branch of the trie but still have further to go.

            // Work out what the next key is
            var nextKey = key.Next();

            // Ask our subtries if they are able to set the value (i.e. the requisite subtrie is extant)
            var addedToSubTrie = _subTries.Any(t => t.Set(nextKey, value));

            // If the value was added to any of the subtries then we're all done and can exit
            if (addedToSubTrie)
                return true;

            // If the value being set is None then we don't actually need to do anything; the value we are
            // removing is already non-existent
            if (value.IsSome == false)
                return true;

            // We have reached the end of the current branch and now need to create a new subtrie to
            // extend the branch
            var newSubTrie = new MutableTrie<T>(nextKey.Current);
            _subTries.AddFirst(newSubTrie);

            // Now add the value to the new subtrie
            return newSubTrie.Set(nextKey, value);
        }

        private Option<T> Get(StringSlice key)
        {
            if (key.AtEnd)
            {
                if (key.Current == _nodeChar)
                    return _value;

                return Option<T>.None;
            }

            var nextSlice = key.Next();

            return _subTries.Select(trie => trie.Get(nextSlice)).FirstOrDefault(x => x.IsSome);
        }
    }
}