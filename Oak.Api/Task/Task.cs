using Common.Shared;

namespace Oak.Api.Task;

public interface ITaskApi
{
    public Task<Task> Create(Create arg);
    public Task<Task> GetOne(Exact arg);
    public Task<IReadOnlyList<Task>> Get(Get arg);
    public Task<Task> Update(Update arg);
    public System.Threading.Tasks.Task Delete(Exact arg);
}

public class TaskApi : ITaskApi
{
    private readonly IRpcClient _client;

    public TaskApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Task> Create(Create arg) => _client.Do(TaskRpcs.Create, arg);

    public Task<Task> GetOne(Exact arg) => _client.Do(TaskRpcs.GetOne, arg);

    public Task<IReadOnlyList<Task>> Get(Get arg) => _client.Do(TaskRpcs.Get, arg);

    public Task<Task> Update(Update arg) => _client.Do(TaskRpcs.Update, arg);

    public System.Threading.Tasks.Task Delete(Exact arg) => _client.Do(TaskRpcs.Delete, arg);
}

public static class TaskRpcs
{
    public static readonly Rpc<Create, Task> Create = new("/task/create");
    public static readonly Rpc<Exact, Task> GetOne = new("/task/get_one");
    public static readonly Rpc<Get, IReadOnlyList<Task>> Get = new("/task/get");
    public static readonly Rpc<Update, Task> Update = new("/task/update");
    public static readonly Rpc<Exact, Nothing> Delete = new("/task/delete");
}

public record Task(
    string Org,
    string Project,
    string Id,
    string? Parent,
    string? FirstChild,
    string? NextSib,
    string? User,
    string Name,
    string Description,
    string CreatedBy,
    DateTime CreatedOn,
    ulong TimeEst,
    ulong TimeInc,
    ulong TimeSubMin,
    ulong TimeSubEst,
    ulong TimeSubInc,
    ulong CostEst,
    ulong CostInc,
    ulong CostSubEst,
    ulong CostSubInc,
    ulong FileN,
    ulong FileSize,
    ulong FileSubN,
    ulong FileSubSize,
    ulong ChildN,
    ulong DescN,
    bool IsParallel
);

public record Create(
    string Org,
    string Project,
    string Parent,
    string? PrevSib,
    string Name,
    string Description = "",
    bool IsParallel = false,
    string? User = null,
    ulong TimeEst = 0,
    ulong CostEst = 0
);

public record Get(
    string Org,
    string Project,
    string? NameStartsWith = null,
    string? After = null,
    bool Asc = true
);

public record Update(string Org, string Project, string Id);

public record Exact(string Org, string Project, string Id);
