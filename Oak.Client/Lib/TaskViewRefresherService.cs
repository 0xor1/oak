using ATask = Oak.Api.Task.Task;

namespace Oak.Client.Lib;

public interface ITaskViewRefresherService
{
    void OnTaskChanged(Func<ATask, Task> fn);
    Task TaskChanged(ATask t);
}

public class TaskViewRefresherService : ITaskViewRefresherService
{
    private Func<ATask, Task>? _fn = null;

    public void OnTaskChanged(Func<ATask, Task> fn) => _fn = fn;

    public Task TaskChanged(ATask t) => _fn?.Invoke(t) ?? Task.CompletedTask;
}
