using Common.Shared;

namespace Oak.Api.Project;

public interface IProjectApi
{
    public Task<Project> Create(Create arg);
    public Task<IReadOnlyList<Project>> Get(Get arg);
    public Task<Project> Update(Update arg);
}

public class ProjectApi : IProjectApi
{
    private readonly IRpcClient _client;

    public ProjectApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Project> Create(Create arg) => _client.Do(ProjectRpcs.Create, arg);

    public Task<IReadOnlyList<Project>> Get(Get arg) => _client.Do(ProjectRpcs.Get, arg);

    public Task<Project> Update(Update arg) => _client.Do(ProjectRpcs.Update, arg);
}

public static class ProjectRpcs
{
    public static readonly Rpc<Create, Project> Create = new("/project/create");
    public static readonly Rpc<Get, IReadOnlyList<Project>> Get = new("/project/get");
    public static readonly Rpc<Update, Project> Update = new("/project/update");
    public static readonly Rpc<Delete, Project> Delete = new("/project/delete");
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
    bool IsArchived,
    bool IsPublic,
    string? Id = null,
    string? NameStartsWith = null,
    MinMax<DateTime>? CreatedOn = null,
    MinMax<DateTime>? StartOn = null,
    MinMax<DateTime>? EndOn = null,
    ProjectOrderBy OrderBy = ProjectOrderBy.Name,
    bool Asc = true
);

public record Update();

public record Delete();

public enum ProjectOrderBy
{
    Name,
    CreatedOn,
    StartOn,
    EndOn
}
