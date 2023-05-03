using Common.Shared;

namespace Oak.Api.Project;

public interface IProjectApi
{
    public Task<Project> Create(Create arg);
    public Task<Project> GetOne(Exact arg);
    public Task<SetRes<Project>> Get(Get arg);
    public Task<Project> Update(Update arg);
    public System.Threading.Tasks.Task Delete(Exact arg);
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
}

public static class ProjectRpcs
{
    public static readonly Rpc<Create, Project> Create = new("/project/create");
    public static readonly Rpc<Exact, Project> GetOne = new("/project/get_one");
    public static readonly Rpc<Get, SetRes<Project>> Get = new("/project/get");
    public static readonly Rpc<Update, Project> Update = new("/project/update");
    public static readonly Rpc<Exact, Nothing> Delete = new("/project/delete");
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
    uint? HoursPerDay,
    uint? DaysPerWeek,
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
    uint? HoursPerDay,
    uint? DaysPerWeek,
    DateTime? StartOn,
    DateTime? EndOn,
    ulong FileLimit
);

public record Get(
    string Org,
    bool IsPublic = false,
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
