using Common.Shared;

namespace Oak.Api.Task;

public interface ITaskApi
{
    public Task<CreateRes> Create(Create arg, CancellationToken ctkn = default);
    public Task<Task> GetOne(Exact arg, CancellationToken ctkn = default);
    public Task<IReadOnlyList<Task>> GetAncestors(Exact arg, CancellationToken ctkn = default);
    public Task<IReadOnlyList<Task>> GetChildren(GetChildren arg, CancellationToken ctkn = default);
    public Task<InitView> GetInitView(Exact arg, CancellationToken ctkn = default);
    public Task<IReadOnlyList<Task>> GetAllDescendants(Exact arg, CancellationToken ctkn = default);
    public Task<UpdateRes> Update(Update arg, CancellationToken ctkn = default);
    public Task<Task> Delete(Exact arg, CancellationToken ctkn = default);
}

public class TaskApi : ITaskApi
{
    private readonly IRpcClient _client;

    public TaskApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<CreateRes> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.Create, arg, ctkn);

    public Task<Task> GetOne(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.GetOne, arg, ctkn);

    public Task<IReadOnlyList<Task>> GetAncestors(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.GetAncestors, arg, ctkn);

    public Task<IReadOnlyList<Task>> GetChildren(
        GetChildren arg,
        CancellationToken ctkn = default
    ) => _client.Do(TaskRpcs.GetChildren, arg, ctkn);

    public Task<InitView> GetInitView(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.GetInitView, arg, ctkn);

    public Task<IReadOnlyList<Task>> GetAllDescendants(
        Exact arg,
        CancellationToken ctkn = default
    ) => _client.Do(TaskRpcs.GetAllDescendants, arg, ctkn);

    public Task<UpdateRes> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.Update, arg, ctkn);

    public Task<Task> Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.Delete, arg, ctkn);
}

public static class TaskRpcs
{
    public static readonly Rpc<Create, CreateRes> Create = new("/task/create");
    public static readonly Rpc<Exact, Task> GetOne = new("/task/get_one");
    public static readonly Rpc<Exact, IReadOnlyList<Task>> GetAncestors =
        new("/task/get_ancestors");
    public static readonly Rpc<GetChildren, IReadOnlyList<Task>> GetChildren =
        new("/task/get_children");
    public static readonly Rpc<Exact, InitView> GetInitView = new("/task/get_init_view");
    public static readonly Rpc<Exact, IReadOnlyList<Task>> GetAllDescendants =
        new("/task/get_all_descendants");
    public static readonly Rpc<Update, UpdateRes> Update = new("/task/update");
    public static readonly Rpc<Exact, Task> Delete = new("/task/delete");
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

public record CreateRes(Task Parent, Task New);

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

public record Update(
    string Org,
    string Project,
    string Id,
    string? Parent = null,
    NSet<string>? PrevSib = null,
    string? Name = null,
    string? Description = null,
    bool? IsParallel = null,
    NSet<string>? User = null,
    ulong? TimeEst = null,
    ulong? CostEst = null
);

public record UpdateRes(Task Task, Task? OldParent, Task? NewParent);

public record GetChildren(string Org, string Project, string Id, string? After);

public record Exact(string Org, string Project, string Id);

public record InitView(Task Task, IReadOnlyList<Task> Children, IReadOnlyList<Task> Ancestors);
