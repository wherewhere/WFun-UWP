using System;
using System.Threading.Tasks;

namespace WFunUWP.Core.Helpers
{
    public static class TasksHelper
    {
        [Obsolete("Use FireAndForget instead")]
        public static void RunAndForget(this Task task)
        {
        }

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
