using Common.Shared;

namespace Oak.Api.OrgMember;

public interface IOrgMemberApi
{
    private static IOrgMemberApi? _inst;
    static IOrgMemberApi Init() => _inst ??= new OrgMemberApi();
    
    public Rpc<Add, OrgMember> Add { get; }
    public Rpc<Get, IReadOnlyList<OrgMember>> Get { get; }
    public Rpc<Update, OrgMember> Update { get; }
}
public class OrgMemberApi: IOrgMemberApi
{
    public Rpc<Add, OrgMember> Add { get; } = new ("/org_member/add");
    public Rpc<Get, IReadOnlyList<OrgMember>> Get { get; } = new ("/org_member/get");
    public Rpc<Update, OrgMember> Update { get; } = new ("/org_member/update");
}

public record OrgMember(string Id, string Name);
public record Add(string Org, string Member, string Name, OrgMemberRole Role);
public record Get(string Org, string? Member, bool IsActive, string? NameStartsWith, OrgMemberRole? Role);
public record Update(string Org, string Member, bool? IsActive, string? NewName, OrgMemberRole? NewRole);

public enum OrgMemberRole
{
    Owner,
    Admin,
    WriteAllProjects,
    ReadAllProjects,
    PerProject
}