using System;
using System.Collections.Generic;

namespace Codartis.Util
{
    public readonly struct Maybe<T> : IEquatable<Maybe<T>>
    {
        public static readonly Maybe<T> Nothing = default;

        private readonly T _value;
        
        internal Maybe(T value)
        {
            _value = value;
            HasValue = true;
        }

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("value is not present");

                return _value;
            }
        }

        public bool HasValue { get; }

        public override string ToString() => !HasValue ? "<Nothing>" : Value.ToString();

        public static implicit operator Maybe<T>(Maybe<Maybe<T>> doubleMaybe) => doubleMaybe.HasValue ? doubleMaybe.Value : Nothing;

        public bool Equals(Maybe<T> other) => EqualityComparer<T>.Default.Equals(_value, other._value) && HasValue.Equals(other.HasValue);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is Maybe<T> mb && Equals(mb);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(_value) * 397) ^ HasValue.GetHashCode();
            }
        }

        public static bool operator ==(Maybe<T> left, Maybe<T> right) => left.Equals(right);

        public static bool operator !=(Maybe<T> left, Maybe<T> right) => !left.Equals(right);
    }
}