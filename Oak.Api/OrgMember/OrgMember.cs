using Common.Shared;

namespace Oak.Api.OrgMember;

public interface IOrgMemberApi
{
    public Task<OrgMember> Invite(Invite arg, CancellationToken ctkn = default);
    Task<Maybe<OrgMember>> GetOne(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<OrgMember>> Get(Get arg, CancellationToken ctkn = default);
    public Task<OrgMember> Update(Update arg, CancellationToken ctkn = default);
}

public class OrgMemberApi : IOrgMemberApi
{
    private readonly IRpcClient _client;

    public OrgMemberApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<OrgMember> Invite(Invite arg, CancellationToken ctkn = default) =>
        _client.Do(OrgMemberRpcs.Invite, arg, ctkn);

    public Task<Maybe<OrgMember>> GetOne(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(OrgMemberRpcs.GetOne, arg, ctkn);

    public Task<SetRes<OrgMember>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(OrgMemberRpcs.Get, arg, ctkn);

    public Task<OrgMember> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(OrgMemberRpcs.Update, arg, ctkn);
}

public static class OrgMemberRpcs
{
    public static readonly Rpc<Invite, OrgMember> Invite = new("/org_member/invite");
    public static readonly Rpc<Exact, Maybe<OrgMember>> GetOne = new("/org_member/get_one");
    public static readonly Rpc<Get, SetRes<OrgMember>> Get = new("/org_member/get");
    public static readonly Rpc<Update, OrgMember> Update = new("/org_member/update");
}

public record OrgMember(string Org, string Id, bool IsActive, string Name, OrgMemberRole Role);

public record Invite(string Org, string Email, string Name, OrgMemberRole Role);

public record Get(
    string Org,
    bool? IsActive = null,
    string? NameStartsWith = null,
    OrgMemberRole? Role = null,
    string? After = null,
    OrgMemberOrderBy OrderBy = OrgMemberOrderBy.Role,
    bool Asc = true
);

public record Update(string Org, string Id, bool? IsActive, string? Name, OrgMemberRole? Role);

public record Exact(string Org, string Id);

public enum OrgMemberRole
{
    Owner,
    Admin,
    WriteAllProjects,
    ReadAllProjects,
    PerProject
}

public enum OrgMemberOrderBy
{
    Role,
    Name
}
