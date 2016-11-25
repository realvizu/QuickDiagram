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
        /// <returns>The task representing the async work.</returns>
        public static Task StartSTA(this TaskFactory taskFactory, Action action)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            return ExecuteOnSTAThread(taskCompletionSource, i =>
            {
                action.Invoke();
                i.SetResult(null);
            });
        }

        /// <summary>
        /// Creates a task that executes on an STA thread and returns a result.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="taskFactory">Not used, it just enables the extension method syntax.</param>
        /// <param name="func">The func to be executed by the task.</param>
        /// <returns>The task representing the async work and providing the result.</returns>
        public static Task<T> StartSTA<T>(this TaskFactory taskFactory, Func<T> func)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            return ExecuteOnSTAThread(taskCompletionSource, i => i.SetResult(func()));
        }

        private static Task<T> ExecuteOnSTAThread<T>(TaskCompletionSource<T> taskCompletionSource, Action<TaskCompletionSource<T>> action)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    action(taskCompletionSource);
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
