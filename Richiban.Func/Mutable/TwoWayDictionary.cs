using System.Collections;
using System.Collections.Generic;

namespace Richiban.Func
{
    public class TwoWayDictionary<T, U> : IDictionary<T, U>
    {
        private readonly Dictionary<T, U> _forwards;
        private readonly Dictionary<U, T> _backwards;

        public TwoWayDictionary()
        {
            _forwards = new Dictionary<T, U>();
            _backwards = new Dictionary<U, T>();
            Reversed = new TwoWayDictionary<U, T>(this);
        }

        private TwoWayDictionary(TwoWayDictionary<U, T> reversed)
        {
            _forwards = reversed._backwards;
            _backwards = reversed._forwards;
            Reversed = reversed;
        }

        public TwoWayDictionary<U, T> Reversed { get; }

        public U this[T key]
        {
            get => _forwards[key];
            set
            {
                _forwards[key] = value;
                _backwards[value] = key;
            }
        }

        public ICollection<T> Keys => _forwards.Keys;
        public ICollection<U> Values => _backwards.Keys;

        public int Count => _forwards.Count;

        public bool IsReadOnly => ((IDictionary<T, U>) _forwards).IsReadOnly;

        public void Add(T key, U value)
        {
            _forwards.Add(key, value);
            _backwards.Add(value, key);
        }

        public void Add(KeyValuePair<T, U> item)
        {
            ((IDictionary<T, U>) _forwards).Add(item);

            ((IDictionary<U, T>) _backwards).Add(
                new KeyValuePair<U, T>(item.Value, item.Key));
        }

        public void Clear()
        {
            _forwards.Clear();
            _backwards.Clear();
        }

        public bool Contains(KeyValuePair<T, U> item) => ((IDictionary<T, U>) _forwards).Contains(item);

        public bool ContainsKey(T key) => _forwards.ContainsKey(key);

        public void CopyTo(KeyValuePair<T, U>[] array, int arrayIndex) => ((IDictionary<T, U>) _forwards).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<T, U>> GetEnumerator() => ((IDictionary<T, U>) _forwards).GetEnumerator();

        public bool Remove(T key)
        {
            if (!_forwards.ContainsKey(key)) return false;

            return _backwards.Remove(_forwards[key]) && _forwards.Remove(key);
        }

        public bool Remove(KeyValuePair<T, U> item) => ((IDictionary<T, U>) _forwards).Remove(item);

        public bool TryGetValue(T key, out U value) => _forwards.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<T, U>) _forwards).GetEnumerator();
    }
}