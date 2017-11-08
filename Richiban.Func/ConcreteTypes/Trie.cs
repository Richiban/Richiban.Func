using System;
using System.Collections.Generic;
using System.Linq;

namespace Richiban.Func
{
    /// <summary>
    /// A fast dictionary-style lookup
    /// </summary>
    public sealed class Trie<T>
    {
        private readonly List<Trie<T>> _subTries = new List<Trie<T>>();
        private readonly char _nodeChar;
        private T _value;
        private bool _hasValue;

        private Trie(char nodeChar, T value)
        {
            _nodeChar = nodeChar;
            _value = value;
            _hasValue = true;
        }

        private Trie(char nodeChar)
        {
            _nodeChar = nodeChar;
        }

        public Trie() { }

        public Optional<T> Retrieve(string key) => Retrieve(new StringSlice(key));

        public void Set(string key, T value) => Set(new StringSlice(key), value);

        private bool Set(StringSlice key, T value)
        {
            if (key.AtEnd)
            {
                if (key.Current == _nodeChar)
                {
                    _value = value;
                    _hasValue = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            var nextKey = key.Next().Value;

            var addedToSubTrie = _subTries.Any(t => t.Set(nextKey, value));

            if (addedToSubTrie)
            {
                return true;
            }

            var newSubTrie = new Trie<T>(nextKey.Current);

            _subTries.Add(newSubTrie);

            return newSubTrie.Set(nextKey, value);
        }

        private Optional<T> Retrieve(StringSlice slice)
        {
            if (slice.AtEnd)
            {
                if (slice.Current == _nodeChar && _hasValue)
                {
                    return _value;
                }
                else
                {
                    return Optional<T>.None;
                }
            }
            else
            {
                var result = _subTries.Select(trie => trie.Retrieve(slice.Next().Value))
                    .FirstOrDefault(x => x.HasValue);

                return result;
            }
        }

        private struct StringSlice
        {
            public StringSlice(string s) : this(s, 0) { }

            private StringSlice(string s, int pos)
            {
                S = string.IsNullOrEmpty(s) ? throw new ArgumentException("String slices cannot be empty", nameof(s)) : s;
                Pos = pos;
                Length = s.Length - pos;
                AtEnd = Length <= 1;
                Current = s[pos];
            }

            private string S { get; }
            public bool AtEnd { get; }
            public int Pos { get; }
            public int Length { get; }
            public char Current { get; }

            public StringSlice? Next()
            {
                if (AtEnd)
                    return null;
                else
                    return new StringSlice(S, Pos + 1);
            }
        }
    }
}