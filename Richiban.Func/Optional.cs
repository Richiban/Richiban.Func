using System;
using System.Collections;
using System.Collections.Generic;

namespace Richiban.Func
{
    public struct Optional<T> : IEnumerable<T>
    {
        public static readonly Optional<T> None = new Optional<T>();
        private readonly T _value;
        public readonly bool HasValue;

        public Optional(T value)
        {
            HasValue = value != null;

            _value = value;
        }

        public T Value
        {
            get
            {
                if (HasValue)
                {
                    return _value;
                }
                throw new InvalidOperationException("The Optional does not have a value");
            }
        }

        public static Optional<T> Create(T value)
        {
            return new Optional<T>(value);
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public Optional<TResult> Select<TResult>(Func<T, TResult> func)
        {
            if (HasValue)
                return new Optional<TResult>(func(_value));

            return new Optional<TResult>();
        }

        public Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> func)
        {
            if (HasValue)
                return func(_value);

            return new Optional<TResult>();
        }

        public Optional<TResult> SelectMany<TCollection, TResult>(
            Func<T, Optional<TCollection>> intermediateSelector,
            Func<T, TCollection, TResult> resultSelector)
        {
            if (HasValue)
            {
                var inner = intermediateSelector(_value);

                if (inner.HasValue)
                {
                    return resultSelector(_value, inner._value);
                }
            }

            return new Optional<TResult>();
        }

        public Optional<T> Where(Func<T, bool> func)
        {
            if (HasValue && func(_value))
            {
                return this;
            }

            return new Optional<T>();
        }

        public void Iter(Action<T> action)
        {
            if (HasValue)
            {
                action(_value);
            }
        }

        public T GetValueOrDefault(T defaultValue = default(T))
        {
            if (HasValue)
                return _value;
            return defaultValue;
        }

        public TResult Match<TResult>(Func<TResult> none, Func<T, TResult> some)
        {
            if (HasValue)
                return some(_value);
            return none();
        }

        public void Switch(Action none, Action<T> some)
        {
            if (HasValue)
                some(_value);

            none();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue)
                yield return Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (HasValue)
            {
                return _value.ToString();
            }
            return "<none>";
        }
    }
}