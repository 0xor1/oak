using Common.Shared;

namespace Oak.Api.Project;

public interface IProjectApi
{
    public Task<Project> Create(Create arg);
    public Task<Project> GetOne(Exact arg);
    public Task<SetRes<Project>> Get(Get arg);
    public Task<Project> Update(Update arg);
    public System.Threading.Tasks.Task Delete(Exact arg);
    public Task<SetRes<Activity>> GetActivities(GetActivities arg);
}

public class ProjectApi : IProjectApi
{
    private readonly IRpcClient _client;

    public ProjectApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Project> Create(Create arg) => _client.Do(ProjectRpcs.Create, arg);

    public Task<Project> GetOne(Exact arg) => _client.Do(ProjectRpcs.GetOne, arg);

    public Task<SetRes<Project>> Get(Get arg) => _client.Do(ProjectRpcs.Get, arg);

    public Task<Project> Update(Update arg) => _client.Do(ProjectRpcs.Update, arg);

    public System.Threading.Tasks.Task Delete(Exact arg) => _client.Do(ProjectRpcs.Delete, arg);

    public Task<SetRes<Activity>> GetActivities(GetActivities arg) =>
        _client.Do(ProjectRpcs.GetActivities, arg);
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

public record Project(
    string Org,
    string Id,
    bool IsArchived,
    bool IsPublic,
    string Name,
    DateTime CreatedOn,
    string CurrencySymbol,
    string CurrencyCode,
    uint HoursPerDay,
    uint DaysPerWeek,
    DateTime? StartOn,
    DateTime? EndOn,
    ulong FileLimit,
    Task.Task Task
);

public record Create(
    string Org,
    bool IsPublic,
    string Name,
    string CurrencySymbol,
    string CurrencyCode,
    uint HoursPerDay,
    uint DaysPerWeek,
    DateTime? StartOn,
    DateTime? EndOn,
    ulong FileLimit
);

public record Get(
    string Org,
    bool? IsPublic = null,
    string? NameStartsWith = null,
    MinMax<DateTime>? CreatedOn = null,
    MinMax<DateTime>? StartOn = null,
    MinMax<DateTime>? EndOn = null,
    bool IsArchived = false,
    string? After = null,
    ProjectOrderBy OrderBy = ProjectOrderBy.Name,
    bool Asc = true
);

public record Update(
    string Org,
    string Id,
    bool? IsPublic = null,
    string? Name = null,
    string? CurrencySymbol = null,
    string? CurrencyCode = null,
    uint? HoursPerDay = null,
    uint? DaysPerWeek = null,
    DateTime? StartOn = null,
    DateTime? EndOn = null,
    ulong? FileLimit = null
);

public record Exact(string Org, string Id);

public enum ProjectOrderBy
{
    Name,
    CreatedOn,
    StartOn,
    EndOn
}

public record Activity(
    string Org,
    string Project,
    string? Task,
    DateTime OccurredOn,
    string User,
    string Item,
    ActivityItemType ItemType,
    bool TaskDeleted,
    bool ItemDeleted,
    ActivityAction Action,
    string? TaskName,
    string? ItemName,
    string? ExtraInfo
);

public record GetActivities(
    string Org,
    string Project,
    bool ExcludeDeletedItem = true,
    string? Task = null,
    string? Item = null,
    string? User = null,
    MinMax<DateTime>? OccuredOn = null,
    bool Asc = false
);
