using System;
using System.Collections;
using System.Collections.Generic;

namespace Richiban.Func
{
    public struct Option<T> : IEnumerable<T>
    {
        public static readonly Option<T> None = new Option<T>();
        private readonly T _value;
        public bool IsSome { get; }

        public Option(T value)
        {
            IsSome = value != null;

            _value = value;
        }

        public T Force()
        {
            if (IsSome)
            {
                return _value;
            }

            throw new InvalidOperationException("The Optional does not have a value");
        }

        public static Option<T> Create(T value) => new Option<T>(value);

        public static implicit operator Option<T>(T value) => new Option<T>(value);

        public Option<TResult> Select<TResult>(Func<T, TResult> func)
        {
            if (IsSome)
                return new Option<TResult>(func(_value));

            return new Option<TResult>();
        }

        public Option<TResult> SelectMany<TResult>(Func<T, Option<TResult>> func)
        {
            if (IsSome)
                return func(_value);

            return new Option<TResult>();
        }

        public Option<TResult> SelectMany<TCollection, TResult>(
            Func<T, Option<TCollection>> intermediateSelector,
            Func<T, TCollection, TResult> resultSelector)
        {
            if (IsSome)
            {
                var inner = intermediateSelector(_value);

                if (inner.IsSome)
                {
                    return resultSelector(_value, inner._value);
                }
            }

            return new Option<TResult>();
        }

        public Option<T> Where(Func<T, bool> func)
        {
            if (IsSome && func(_value))
            {
                return this;
            }

            return new Option<T>();
        }

        public void Iter(Action<T> action)
        {
            if (IsSome)
            {
                action(_value);
            }
        }

        public T GetValueOrDefault(T defaultValue = default(T))
        {
            if (IsSome)
                return _value;

            return defaultValue;
        }

        public TResult Match<TResult>(Func<TResult> none, Func<T, TResult> some)
        {
            if (IsSome)
                return some(_value);

            return none();
        }

        public void Switch(Action none, Action<T> some)
        {
            if (IsSome)
                some(_value);

            none();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (IsSome)
                yield return Force();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            if (IsSome)
            {
                return _value.ToString();
            }

            return "<none>";
        }
    }
}