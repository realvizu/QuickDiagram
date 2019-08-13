using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codartis.Util
{
    public static class MaybeExtensions
    {
        public static Maybe<TResult> Select<T, TResult>(this Maybe<T> a, Func<T, TResult> fn)
        {
            return a.Match(fn, () => default).ToMaybe();
        }

        public static IEnumerable<Maybe<TResult>> Select<T, TResult>(this IEnumerable<Maybe<T>> maybes, Func<T, TResult> selector)
        {
            return maybes.Select(maybe => maybe.Select(selector));
        }

        public static T? ToNullable<T>(this Maybe<T> maybe)
            where T : struct
        {
            return maybe.HasValue ? maybe.Value : (T?)null;
        }
        
        public static T FromMaybe<T>(this Maybe<T> maybe) => maybe.HasValue ? maybe.Value : default;

        public static Maybe<T> ToMaybe<T>(this T o)
        {
            return o == null ? Maybe<T>.Nothing : new Maybe<T>(o);
        }

        public static Maybe<TB> Cast<TA, TB>(this Maybe<TA> a)
            where TB : class
        {
            return a.HasValue ? a.Select(i => i as TB) : Maybe<TB>.Nothing;
        }

        public static TResult Match<T, TResult>(this Maybe<T> maybe, Func<T, TResult> some, Func<TResult> none)
        {
            return maybe.HasValue ? some(maybe.Value) : none();
        }

        public static void Match<T>(this Maybe<T> maybe, Action<T> some, Action none = null)
        {
            if (maybe.HasValue)
                some(maybe.Value);
            else
                none?.Invoke();
        }

        public static async Task MatchAsync<T>(this Maybe<T> maybe, Func<T, Task> asyncSome, Func<Task> asyncNone = null)
        {
            if (maybe.HasValue)
                await asyncSome(maybe.Value);
            else
                await asyncNone?.Invoke();
        }
    }
}