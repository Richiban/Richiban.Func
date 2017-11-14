using System;

namespace Richiban.Func
{
    internal struct StringSlice
    {
        public StringSlice(string stringValue) : this(stringValue, position: -1) { }

        private StringSlice(string stringValue, int position)
        {
            _stringValue = stringValue;

            if (position >= stringValue.Length)
                throw new InvalidOperationException(
                    "An attempt was made to advance the StringSlice beyond the end of the string");

            Position = position;
            Length = stringValue.Length - position;
            AtEnd = Length <= 1;

            Current = position < 0 ? default(char) : stringValue[position];
        }

        private readonly string _stringValue;
        public bool AtEnd { get; }
        public int Position { get; }
        public int Length { get; }
        public char Current { get; }

        public StringSlice Next() => new StringSlice(_stringValue, Position + 1);
    }
}