using Common.Shared;
using MessagePack;

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

[MessagePackObject]
public record Task(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Id,
    [property: Key(3)] string? Parent,
    [property: Key(4)] string? FirstChild,
    [property: Key(5)] string? NextSib,
    [property: Key(6)] string? User,
    [property: Key(7)] string Name,
    [property: Key(8)] string Description,
    [property: Key(9)] string CreatedBy,
    [property: Key(10)] DateTime CreatedOn,
    [property: Key(11)] ulong TimeEst,
    [property: Key(12)] ulong TimeInc,
    [property: Key(13)] ulong TimeSubMin,
    [property: Key(14)] ulong TimeSubEst,
    [property: Key(15)] ulong TimeSubInc,
    [property: Key(16)] ulong CostEst,
    [property: Key(17)] ulong CostInc,
    [property: Key(18)] ulong CostSubEst,
    [property: Key(19)] ulong CostSubInc,
    [property: Key(20)] ulong FileN,
    [property: Key(21)] ulong FileSize,
    [property: Key(22)] ulong FileSubN,
    [property: Key(23)] ulong FileSubSize,
    [property: Key(24)] ulong ChildN,
    [property: Key(25)] ulong DescN,
    [property: Key(26)] bool IsParallel
);

[MessagePackObject]
public record CreateRes([property: Key(0)] Task Parent, [property: Key(1)] Task New);

[MessagePackObject]
public record Create(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Parent,
    [property: Key(3)] string? PrevSib,
    [property: Key(4)] string Name,
    [property: Key(5)] string Description = "",
    [property: Key(6)] bool IsParallel = false,
    [property: Key(7)] string? User = null,
    [property: Key(8)] ulong TimeEst = 0,
    [property: Key(9)] ulong CostEst = 0
);

[MessagePackObject]
public record Update(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Id,
    [property: Key(3)] string? Parent = null,
    [property: Key(4)] NSet<string>? PrevSib = null,
    [property: Key(5)] string? Name = null,
    [property: Key(6)] string? Description = null,
    [property: Key(7)] bool? IsParallel = null,
    [property: Key(8)] NSet<string>? User = null,
    [property: Key(9)] ulong? TimeEst = null,
    [property: Key(10)] ulong? CostEst = null
);

[MessagePackObject]
public record UpdateRes(
    [property: Key(0)] Task Task,
    [property: Key(1)] Task? OldParent,
    [property: Key(2)] Task? NewParent
);

[MessagePackObject]
public record GetChildren(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Id,
    [property: Key(3)] string? After
);

[MessagePackObject]
public record Exact(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Id
);

[MessagePackObject]
public record InitView(
    [property: Key(0)] Task Task,
    [property: Key(1)] IReadOnlyList<Task> Children,
    [property: Key(2)] IReadOnlyList<Task> Ancestors
);
