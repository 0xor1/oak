using Common.Shared;

namespace Oak.Api.ProjectMember;

public interface IProjectMemberApi
{
    public Task<ProjectMember> Create(Create arg);
    public Task<ProjectMember> GetOne(Exact arg);
    public Task<IReadOnlyList<ProjectMember>> Get(Get arg);
    public Task<ProjectMember> Update(Update arg);
    public System.Threading.Tasks.Task Delete(Exact arg);
}

public class ProjectMemberApi : IProjectMemberApi
{
    private readonly IRpcClient _client;

    public ProjectMemberApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<ProjectMember> Create(Create arg) => _client.Do(ProjectMemberRpcs.Create, arg);

    public Task<ProjectMember> GetOne(Exact arg) => _client.Do(ProjectMemberRpcs.GetOne, arg);

    public Task<IReadOnlyList<ProjectMember>> Get(Get arg) =>
        _client.Do(ProjectMemberRpcs.Get, arg);

    public Task<ProjectMember> Update(Update arg) => _client.Do(ProjectMemberRpcs.Update, arg);

    public System.Threading.Tasks.Task Delete(Exact arg) =>
        _client.Do(ProjectMemberRpcs.Delete, arg);
}

public static class ProjectMemberRpcs
{
    public static readonly Rpc<Create, ProjectMember> Create = new("/project_member/create");
    public static readonly Rpc<Exact, ProjectMember> GetOne = new("/project_member/get_one");
    public static readonly Rpc<Get, IReadOnlyList<ProjectMember>> Get = new("/project_member/get");
    public static readonly Rpc<Update, ProjectMember> Update = new("/project_member/update");
    public static readonly Rpc<Exact, Nothing> Delete = new("/project_member/delete");
}

public record ProjectMember(
    string Org,
    string Project,
    string Id,
    ProjectMemberRole Role,
    ulong TimeEst,
    ulong TimeInc,
    ulong CostEst,
    ulong CostInc,
    ulong FileN,
    ulong FileSize,
    ulong TaskN
);

public enum ProjectMemberRole
{
    Admin,
    Writer,
    Reader
}

public record Create(
    string Org,
    string Project,
    string Id,
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
    ProjectMemberOrderBy OrderBy = ProjectMemberOrderBy.Name,
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

public record Exact(string Org, string Project, string Id);

public enum ProjectMemberOrderBy
{
    Role,
    Name
}
