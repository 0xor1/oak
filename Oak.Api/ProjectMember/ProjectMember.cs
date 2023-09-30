using Common.Shared;
using Oak.Api.OrgMember;

namespace Oak.Api.ProjectMember;

public interface IProjectMemberApi
{
    public Task<ProjectMember> Add(Add arg, CancellationToken ctkn = default);
    public Task<Maybe<ProjectMember>> GetOne(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<ProjectMember>> Get(Get arg, CancellationToken ctkn = default);
    public Task<ProjectMember> Update(Update arg, CancellationToken ctkn = default);
    public System.Threading.Tasks.Task Remove(Exact arg, CancellationToken ctkn = default);
}

public class ProjectMemberApi : IProjectMemberApi
{
    private readonly IRpcClient _client;

    public ProjectMemberApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<ProjectMember> Add(Add arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectMemberRpcs.Add, arg, ctkn);

    public Task<Maybe<ProjectMember>> GetOne(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectMemberRpcs.GetOne, arg, ctkn);

    public Task<SetRes<ProjectMember>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectMemberRpcs.Get, arg, ctkn);

    public Task<ProjectMember> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectMemberRpcs.Update, arg, ctkn);

    public System.Threading.Tasks.Task Remove(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectMemberRpcs.Remove, arg, ctkn);
}

public static class ProjectMemberRpcs
{
    public static readonly Rpc<Add, ProjectMember> Add = new("/project_member/add");
    public static readonly Rpc<Exact, Maybe<ProjectMember>> GetOne = new("/project_member/get_one");
    public static readonly Rpc<Get, SetRes<ProjectMember>> Get = new("/project_member/get");
    public static readonly Rpc<Update, ProjectMember> Update = new("/project_member/update");
    public static readonly Rpc<Exact, Nothing> Remove = new("/project_member/remove");
}

public record ProjectMember(
    string Org,
    string Project,
    string Id,
    bool IsActive,
    OrgMemberRole OrgRole,
    string Name,
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

public record Add(string Org, string Project, string Id, ProjectMemberRole Role);

public record Get(
    string Org,
    string Project,
    bool? IsActive = null,
    ProjectMemberRole? Role = null,
    string? NameStartsWith = null,
    string? After = null,
    ProjectMemberOrderBy OrderBy = ProjectMemberOrderBy.Role,
    bool Asc = true
);

public record Update(string Org, string Project, string Id, ProjectMemberRole Role);

public record Exact(string Org, string Project, string Id);

public enum ProjectMemberOrderBy
{
    Role,
    Name
}
