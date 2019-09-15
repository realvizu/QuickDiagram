using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions.Events;
using FluentAssertions.Execution;

namespace Codartis.SoftVis.UnitTests
{
    public static class FluentAssertionsHelper
    {
        /// <summary>
        /// An alternative version of FluentAssertions' WithArgs that handles different types of event on the same recorder.
        /// </summary>
        public static IEventRecorder WithArgs2<T>(this IEventRecorder eventRecorder, Expression<Func<T, bool>> predicate)
        {
            Func<T, bool> compiledPredicate = predicate.Compile();

            if (eventRecorder.All(i => !i.Parameters.OfType<T>().Any()))
            {
                throw new ArgumentException("No argument of event " + eventRecorder.EventName + " is of type <" + typeof(T) + ">.");
            }

            if (eventRecorder.All(recordedEvent => !recordedEvent.Parameters.OfType<T>().Any(parameter => compiledPredicate(parameter))))
            {
                Execute.Assertion
                    .FailWith("Expected at least one event with arguments matching {0}, but found none.", predicate.Body);
            }

            return eventRecorder;
        }
    }
}