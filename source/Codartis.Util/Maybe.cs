using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Codartis.Util
{
    public readonly struct Maybe<T> : IEquatable<Maybe<T>>
    {
        public static readonly Maybe<T> Nothing = default;

        public bool HasValue { get; }
        private readonly T _value;

        internal Maybe([NotNull] T value)
        {
            HasValue = true;
            _value = value;
        }

        [NotNull]
        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("value is not present");

                return _value;
            }
        }

        public override string ToString() => !HasValue ? "<Nothing>" : Value.ToString();

        public static implicit operator Maybe<T>(Maybe<Maybe<T>> doubleMaybe) => doubleMaybe.HasValue ? doubleMaybe.Value : Nothing;

        public bool Equals(Maybe<T> other)
        {
            return !HasValue && !other.HasValue ||
                   HasValue && other.HasValue && EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        public bool Equals(T otherValue)
        {
            return !HasValue && otherValue == null ||
                   HasValue && otherValue != null && EqualityComparer<T>.Default.Equals(_value, otherValue);
        }

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

        public Maybe<TResult> OfType<TResult>()
        {
            return Value is TResult result ? new Maybe<TResult>(result) : Maybe<TResult>.Nothing;
        }
    }

    public static class Maybe
    {
        public static Maybe<T> Create<T>(T some) => some == null ? Maybe<T>.Nothing : new Maybe<T>(some);
    }
}