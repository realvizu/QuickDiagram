using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Util
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Creates a task that executes on an STA thread and does not return a result.
        /// </summary>
        /// <param name="taskFactory">Not used, it just enables the extension method syntax.</param>
        /// <param name="action">The action to be executed by the task.</param>
        /// <param name="cancellationToken">Optional cancellation toke.n</param>
        /// <returns>The task representing the async work.</returns>
        // ReSharper disable once InconsistentNaming
        public static Task StartSTA(this TaskFactory taskFactory, Action action,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            return ExecuteOnNewSTAThread(taskCompletionSource, i =>
            {
                action.Invoke();
                i.SetResult(null);
            }, cancellationToken);
        }

        /// <summary>
        /// Creates a task that executes on an STA thread and returns a result.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="taskFactory">Not used, it just enables the extension method syntax.</param>
        /// <param name="func">The func to be executed by the task.</param>
        /// <param name="cancellationToken">Optional cancellation toke.n</param>
        /// <returns>The task representing the async work and providing the result.</returns>
        // ReSharper disable once InconsistentNaming
        public static Task<T> StartSTA<T>(this TaskFactory taskFactory, Func<T> func,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            return ExecuteOnNewSTAThread(taskCompletionSource, i => i.SetResult(func()), cancellationToken);
        }

        private static Task<T> ExecuteOnNewSTAThread<T>(TaskCompletionSource<T> taskCompletionSource,
            Action<TaskCompletionSource<T>> action, CancellationToken cancellationToken)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(taskCompletionSource);
                }
                catch (OperationCanceledException)
                {
                    taskCompletionSource.SetCanceled();
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return taskCompletionSource.Task;
        }
    }
}
