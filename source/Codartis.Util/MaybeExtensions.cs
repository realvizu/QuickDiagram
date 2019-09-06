using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.Util
{
    public static class MaybeExtensions
    {
        public static Maybe<TResult> Select<T, TResult>(this Maybe<T> a, [NotNull] Func<T, TResult> fn)
        {
            return a.Match(fn, () => default).ToMaybe();
        }

        [NotNull]
        public static IEnumerable<Maybe<TResult>> Select<T, TResult>([NotNull] this IEnumerable<Maybe<T>> maybes, [NotNull] Func<T, TResult> selector)
        {
            return maybes.Select(maybe => maybe.Select(selector));
        }

        public static T? ToNullable<T>(this Maybe<T> maybe)
            where T : struct
        {
            return maybe.HasValue ? maybe.Value : (T?)null;
        }

        public static T FromMaybe<T>(this Maybe<T> maybe) => maybe.HasValue ? maybe.Value : default;

        public static Maybe<T> ToMaybe<T>([CanBeNull] this T? o, [CanBeNull] Predicate<T> predicate = null)
            where T : struct
        {
            return o != null && predicate?.Invoke(o.Value) != false
                ? new Maybe<T>(o.Value)
                : Maybe<T>.Nothing;
        }

        public static Maybe<T> ToMaybe<T>([CanBeNull] this T o, [CanBeNull] Predicate<T> predicate = null)
        {
            return o != null && predicate?.Invoke(o) != false
                ? new Maybe<T>(o)
                : Maybe<T>.Nothing;
        }

        public static Maybe<TB> Cast<TA, TB>(this Maybe<TA> a)
            where TB : class
        {
            return a.HasValue ? a.Select(i => i as TB) : Maybe<TB>.Nothing;
        }

        public static TResult Match<T, TResult>(this Maybe<T> maybe, [NotNull] Func<T, TResult> some, [NotNull] Func<TResult> none)
        {
            return maybe.HasValue ? some(maybe.Value) : none();
        }

        public static TResult Match<T, TResult>(this Maybe<T> maybe, [NotNull] Func<T, TResult> some, [NotNull] Exception exceptionForNone)
        {
            return maybe.HasValue ? some(maybe.Value) : throw exceptionForNone;
        }

        public static void Match<T>(this Maybe<T> maybe, [NotNull] Action<T> some, [NotNull] Action none = null)
        {
            if (maybe.HasValue)
                some(maybe.Value);
            else
                none?.Invoke();
        }

        [NotNull]
        public static async Task MatchAsync<T>(this Maybe<T> maybe, [NotNull] Func<T, Task> asyncSome, [NotNull] Func<Task> asyncNone = null)
        {
            if (maybe.HasValue)
                await asyncSome(maybe.Value);
            else
                await asyncNone?.Invoke();
        }
    }
}