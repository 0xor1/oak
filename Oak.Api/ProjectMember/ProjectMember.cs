using Common.Shared;
using MessagePack;
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

[MessagePackObject]
public record ProjectMember(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Id,
    [property: Key(3)] bool IsActive,
    [property: Key(4)] OrgMemberRole OrgRole,
    [property: Key(5)] string Name,
    [property: Key(6)] ProjectMemberRole Role,
    [property: Key(7)] ulong TimeEst,
    [property: Key(8)] ulong TimeInc,
    [property: Key(9)] ulong CostEst,
    [property: Key(10)] ulong CostInc,
    [property: Key(11)] ulong FileN,
    [property: Key(12)] ulong FileSize,
    [property: Key(13)] ulong TaskN
);

public enum ProjectMemberRole
{
    Admin,
    Writer,
    Reader
}

[MessagePackObject]
public record Add(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Id,
    [property: Key(3)] ProjectMemberRole Role
);

[MessagePackObject]
public record Get(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] bool? IsActive = null,
    [property: Key(3)] ProjectMemberRole? Role = null,
    [property: Key(4)] string? NameStartsWith = null,
    [property: Key(5)] string? After = null,
    [property: Key(6)] ProjectMemberOrderBy OrderBy = ProjectMemberOrderBy.Role,
    [property: Key(7)] bool Asc = true
);

[MessagePackObject]
public record Update(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Id,
    [property: Key(3)] ProjectMemberRole Role
);

[MessagePackObject]
public record Exact(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Id
);

public enum ProjectMemberOrderBy
{
    Role,
    Name
}
