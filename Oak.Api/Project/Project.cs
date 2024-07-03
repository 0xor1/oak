using Common.Shared;
using MessagePack;

namespace Oak.Api.Project;

public interface IProjectApi
{
    public Task<Project> Create(Create arg, CancellationToken ctkn = default);
    public Task<Project> GetOne(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<Project>> Get(Get arg, CancellationToken ctkn = default);
    public Task<Project> Update(Update arg, CancellationToken ctkn = default);
    public System.Threading.Tasks.Task Delete(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<Activity>> GetActivities(
        GetActivities arg,
        CancellationToken ctkn = default
    );
}

public class ProjectApi : IProjectApi
{
    private readonly IRpcClient _client;

    public ProjectApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Project> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.Create, arg, ctkn);

    public Task<Project> GetOne(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.GetOne, arg, ctkn);

    public Task<SetRes<Project>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.Get, arg, ctkn);

    public Task<Project> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.Update, arg, ctkn);

    public System.Threading.Tasks.Task Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.Delete, arg, ctkn);

    public Task<SetRes<Activity>> GetActivities(
        GetActivities arg,
        CancellationToken ctkn = default
    ) => _client.Do(ProjectRpcs.GetActivities, arg, ctkn);
}

public static class ProjectRpcs
{
    public static readonly Rpc<Create, Project> Create = new("/project/create");
    public static readonly Rpc<Exact, Project> GetOne = new("/project/get_one");
    public static readonly Rpc<Get, SetRes<Project>> Get = new("/project/get");
    public static readonly Rpc<Update, Project> Update = new("/project/update");
    public static readonly Rpc<Exact, Nothing> Delete = new("/project/delete");
    public static readonly Rpc<GetActivities, SetRes<Activity>> GetActivities =
        new("/project/get_activities");
}

[MessagePackObject]
public record Project(
    [property: Key(0)] string Org,
    [property: Key(1)] string Id,
    [property: Key(2)] bool IsArchived,
    [property: Key(3)] bool IsPublic,
    [property: Key(4)] string Name,
    [property: Key(5)] DateTime CreatedOn,
    [property: Key(6)] string CurrencySymbol,
    [property: Key(7)] string CurrencyCode,
    [property: Key(8)] uint HoursPerDay,
    [property: Key(9)] uint DaysPerWeek,
    [property: Key(10)] DateTime? StartOn,
    [property: Key(11)] DateTime? EndOn,
    [property: Key(12)] ulong FileLimit,
    [property: Key(13)] Task.Task Task
);

[MessagePackObject]
public record Create(
    [property: Key(0)] string Org,
    [property: Key(1)] bool IsPublic,
    [property: Key(2)] string Name,
    [property: Key(3)] string CurrencySymbol,
    [property: Key(4)] string CurrencyCode,
    [property: Key(5)] uint HoursPerDay,
    [property: Key(6)] uint DaysPerWeek,
    [property: Key(7)] DateTime? StartOn,
    [property: Key(8)] DateTime? EndOn,
    [property: Key(9)] ulong FileLimit
);

[MessagePackObject]
public record Get(
    [property: Key(0)] string Org,
    [property: Key(1)] bool? IsPublic = null,
    [property: Key(2)] string? NameStartsWith = null,
    [property: Key(3)] MinMax<DateTime>? CreatedOn = null,
    [property: Key(4)] MinMax<DateTime>? StartOn = null,
    [property: Key(5)] MinMax<DateTime>? EndOn = null,
    [property: Key(6)] bool IsArchived = false,
    [property: Key(7)] string? After = null,
    [property: Key(8)] ProjectOrderBy OrderBy = ProjectOrderBy.Name,
    [property: Key(9)] bool Asc = true
);

[MessagePackObject]
public record Update(
    [property: Key(0)] string Org,
    [property: Key(1)] string Id,
    [property: Key(2)] bool? IsPublic = null,
    [property: Key(3)] string? Name = null,
    [property: Key(4)] string? CurrencySymbol = null,
    [property: Key(5)] string? CurrencyCode = null,
    [property: Key(6)] uint? HoursPerDay = null,
    [property: Key(7)] uint? DaysPerWeek = null,
    [property: Key(8)] DateTime? StartOn = null,
    [property: Key(9)] DateTime? EndOn = null,
    [property: Key(10)] ulong? FileLimit = null
);

[MessagePackObject]
public record Exact([property: Key(0)] string Org, [property: Key(1)] string Id);

public enum ProjectOrderBy
{
    Name,
    CreatedOn,
    StartOn,
    EndOn
}

[MessagePackObject]
public record Activity(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string? Task,
    [property: Key(3)] DateTime OccurredOn,
    [property: Key(4)] string User,
    [property: Key(5)] string Item,
    [property: Key(6)] ActivityItemType ItemType,
    [property: Key(7)] bool TaskDeleted,
    [property: Key(8)] bool ItemDeleted,
    [property: Key(9)] ActivityAction Action,
    [property: Key(10)] string? TaskName,
    [property: Key(11)] string? ItemName,
    [property: Key(12)] string? ExtraInfo
);

[MessagePackObject]
public record GetActivities(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] bool ExcludeDeletedItem = true,
    [property: Key(3)] string? Task = null,
    [property: Key(4)] string? Item = null,
    [property: Key(5)] string? User = null,
    [property: Key(6)] MinMax<DateTime>? OccuredOn = null,
    [property: Key(7)] bool Asc = false
);
