using Common.Shared;

namespace Oak.Api.OrgMember;

public interface IOrgMemberApi
{
    public Task<OrgMember> Add(Add arg);
    public Task<IReadOnlyList<OrgMember>> Get(Get arg);
    public Task<OrgMember> Update(Update arg);
}
public class OrgMemberApi: IOrgMemberApi
{
    private readonly IRpcClient _client;

    public OrgMemberApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<OrgMember> Add(Add arg) => _client.Do(OrgMemberRpcs.Add, arg);
    public Task<IReadOnlyList<OrgMember>> Get(Get arg) => _client.Do(OrgMemberRpcs.Get, arg);
    public Task<OrgMember> Update(Update arg) => _client.Do(OrgMemberRpcs.Update, arg);
}
public static class OrgMemberRpcs
{
    public static readonly Rpc<Add, OrgMember> Add = new ("/org_member/add");
    public static readonly Rpc<Get, IReadOnlyList<OrgMember>> Get = new ("/org_member/get");
    public static readonly Rpc<Update, OrgMember> Update = new ("/org_member/update");
}

public record OrgMember(string Org, string Member, bool IsActive, string Name, OrgMemberRole Role);
public record Add(string Org, string Member, string Name, OrgMemberRole Role);
public record Get(string Org, bool IsActive, string? Member = null, string? NameStartsWith = null, OrgMemberRole? Role = null, OrgMemberOrderBy OrderBy = OrgMemberOrderBy.Name, bool Asc = true);
public record Update(string Org, string Member, bool? IsActive, string? NewName, OrgMemberRole? NewRole);

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
    IsActive,
    Name,
    Role
}