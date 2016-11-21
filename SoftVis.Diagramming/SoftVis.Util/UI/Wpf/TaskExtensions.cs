using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codartis.SoftVis.Util.UI.Wpf
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Creates a task that executes on an STA thread.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="taskFactory">Not used, it just enables the extension method syntax.</param>
        /// <param name="func">The func to be executed by the task.</param>
        /// <returns>A task created from the given func.</returns>
        public static Task<T> StartSTA<T>(this TaskFactory taskFactory, Func<T> func)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            var thread = new Thread(() => 
            {
                try
                {
                    taskCompletionSource.SetResult(func());
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

        /// <summary>
        /// Adds a continuation that will execute in the  current SynchronizationContext.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="task">A task.</param>
        /// <param name="action">The continuation action.</param>
        /// <returns>The task object to enable the fluent interface style.</returns>
        public static Task<T> ContinueInCurrentContext<T>(this Task<T> task,  Action<Task<T>> action)
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            task.ContinueWith(action, scheduler);
            return task;
        }
    }
}
