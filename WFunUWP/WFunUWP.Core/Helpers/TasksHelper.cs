using System;
using System.Threading.Tasks;

namespace WFunUWP.Core.Helpers
{
    public static class TasksHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "task")]
        [Obsolete("Use FireAndForget instead")]
        public static void RunAndForget(this Task task)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "task")]
        public static void FireAndForget(this Task task)
        {
        }

        public static TResult AwaitByTaskCompleteSource<TResult>(Func<Task<TResult>> func)
        {
            TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
            Task<TResult> task1 = taskCompletionSource.Task;
            _ = Task.Run(async () =>
            {
                TResult result = await func.Invoke();
                taskCompletionSource.SetResult(result);
            });
            TResult task1Result = task1.Result;
            return task1Result;
        }
    }
}
