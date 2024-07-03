using Common.Shared;
using MessagePack;

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

[MessagePackObject]
public record OrgMember(
    [property: Key(0)] string Org,
    [property: Key(1)] string Id,
    [property: Key(2)] bool IsActive,
    [property: Key(3)] string Name,
    [property: Key(4)] OrgMemberRole Role
);

[MessagePackObject]
public record Invite(
    [property: Key(0)] string Org,
    [property: Key(1)] string Email,
    [property: Key(2)] string Name,
    [property: Key(3)] OrgMemberRole Role
);

[MessagePackObject]
public record Get(
    [property: Key(0)] string Org,
    [property: Key(1)] bool? IsActive = null,
    [property: Key(2)] string? NameStartsWith = null,
    [property: Key(3)] OrgMemberRole? Role = null,
    [property: Key(4)] string? After = null,
    [property: Key(5)] OrgMemberOrderBy OrderBy = OrgMemberOrderBy.Role,
    [property: Key(6)] bool Asc = true
);

[MessagePackObject]
public record Update(
    [property: Key(0)] string Org,
    [property: Key(1)] string Id,
    [property: Key(2)] bool? IsActive,
    [property: Key(3)] string? Name,
    [property: Key(4)] OrgMemberRole? Role
);

[MessagePackObject]
public record Exact([property: Key(0)] string Org, [property: Key(1)] string Id);

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
