// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

#nullable enable

using Common.Shared;
using MessagePack;


namespace Oak.Api.OrgMember;

public interface IOrgMemberApi
{
    public Task<OrgMember> Invite(Invite arg, CancellationToken ctkn = default);
    public Task<Maybe<OrgMember>> GetOne(Exact arg, CancellationToken ctkn = default);
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
    public static readonly Rpc<Invite, OrgMember> Invite = new("/orgMember/invite");
    public static readonly Rpc<Exact, Maybe<OrgMember>> GetOne = new("/orgMember/getOne");
    public static readonly Rpc<Get, SetRes<OrgMember>> Get = new("/orgMember/get");
    public static readonly Rpc<Update, OrgMember> Update = new("/orgMember/update");
    
}



[MessagePackObject]
public record OrgMember
{
    public OrgMember(
        string org,
        string id,
        bool isActive,
        string name,
        OrgMemberRole role
        
    )
    {
        Org = org;
        Id = id;
        IsActive = isActive;
        Name = name;
        Role = role;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Id { get; set; }
    [Key(2)]
    public bool IsActive { get; set; }
    [Key(3)]
    public string Name { get; set; }
    [Key(4)]
    public OrgMemberRole Role { get; set; }
    
}



[MessagePackObject]
public record Invite
{
    public Invite(
        string org,
        string email,
        string name,
        OrgMemberRole role
        
    )
    {
        Org = org;
        Email = email;
        Name = name;
        Role = role;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Email { get; set; }
    [Key(2)]
    public string Name { get; set; }
    [Key(3)]
    public OrgMemberRole Role { get; set; }
    
}



[MessagePackObject]
public record Get
{
    public Get(
        string org,
        bool? isActive = null,
        string? nameStartsWith = null,
        OrgMemberRole? role = null,
        string? after = null,
        OrgMemberOrderBy? orderBy = OrgMemberOrderBy.Role,
        bool asc = true
        
    )
    {
        Org = org;
        IsActive = isActive;
        NameStartsWith = nameStartsWith;
        Role = role;
        After = after;
        OrderBy = orderBy;
        Asc = asc;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public bool? IsActive { get; set; } = null;
    [Key(2)]
    public string? NameStartsWith { get; set; } = null;
    [Key(3)]
    public OrgMemberRole? Role { get; set; } = null;
    [Key(4)]
    public string? After { get; set; } = null;
    [Key(5)]
    public OrgMemberOrderBy? OrderBy { get; set; } = OrgMemberOrderBy.Role;
    [Key(6)]
    public bool Asc { get; set; } = true;
    
}



[MessagePackObject]
public record Update
{
    public Update(
        string org,
        string id,
        bool? isActive,
        string? name,
        OrgMemberRole? role
        
    )
    {
        Org = org;
        Id = id;
        IsActive = isActive;
        Name = name;
        Role = role;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Id { get; set; }
    [Key(2)]
    public bool? IsActive { get; set; }
    [Key(3)]
    public string? Name { get; set; }
    [Key(4)]
    public OrgMemberRole? Role { get; set; }
    
}



[MessagePackObject]
public record Exact
{
    public Exact(
        string org,
        string id
        
    )
    {
        Org = org;
        Id = id;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Id { get; set; }
    
}




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
