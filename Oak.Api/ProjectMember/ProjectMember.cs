using Common.Shared;

namespace Oak.Api.ProjectMember;

public interface IProjectMemberApi
{
    public Task<ProjectMember> Add(Add arg);
    public Task<ProjectMember> GetOne(Exact arg);
    public Task<SetRes<ProjectMember>> Get(Get arg);
    public Task<ProjectMember> Update(Update arg);
    public System.Threading.Tasks.Task Remove(Exact arg);
}

public class ProjectMemberApi : IProjectMemberApi
{
    private readonly IRpcClient _client;

    public ProjectMemberApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<ProjectMember> Add(Add arg) => _client.Do(ProjectMemberRpcs.Add, arg);

    public Task<ProjectMember> GetOne(Exact arg) => _client.Do(ProjectMemberRpcs.GetOne, arg);

    public Task<SetRes<ProjectMember>> Get(Get arg) => _client.Do(ProjectMemberRpcs.Get, arg);

    public Task<ProjectMember> Update(Update arg) => _client.Do(ProjectMemberRpcs.Update, arg);

    public System.Threading.Tasks.Task Remove(Exact arg) =>
        _client.Do(ProjectMemberRpcs.Remove, arg);
}

public static class ProjectMemberRpcs
{
    public static readonly Rpc<Add, ProjectMember> Add = new("/project_member/add");
    public static readonly Rpc<Exact, ProjectMember> GetOne = new("/project_member/get_one");
    public static readonly Rpc<Get, SetRes<ProjectMember>> Get = new("/project_member/get");
    public static readonly Rpc<Update, ProjectMember> Update = new("/project_member/update");
    public static readonly Rpc<Exact, Nothing> Remove = new("/project_member/remove");
}

public record ProjectMember(
    string Org,
    string Project,
    string Id,
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
