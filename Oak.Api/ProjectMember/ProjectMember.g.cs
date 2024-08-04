// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

#nullable enable

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
    public static readonly Rpc<Add, ProjectMember> Add = new("/projectMember/add");
    public static readonly Rpc<Exact, Maybe<ProjectMember>> GetOne = new("/projectMember/getOne");
    public static readonly Rpc<Get, SetRes<ProjectMember>> Get = new("/projectMember/get");
    public static readonly Rpc<Update, ProjectMember> Update = new("/projectMember/update");
    public static readonly Rpc<Exact, Nothing> Remove = new("/projectMember/remove");
    
}



[MessagePackObject]
public record ProjectMember
{
    public ProjectMember(
        string org,
        string project,
        string id,
        bool isActive,
        OrgMemberRole orgRole,
        string name,
        ProjectMemberRole role,
        ulong timeEst,
        ulong timeInc,
        ulong costEst,
        ulong costInc,
        ulong fileN,
        ulong fileSize,
        ulong taskN
        
    )
    {
        Org = org;
        Project = project;
        Id = id;
        IsActive = isActive;
        OrgRole = orgRole;
        Name = name;
        Role = role;
        TimeEst = timeEst;
        TimeInc = timeInc;
        CostEst = costEst;
        CostInc = costInc;
        FileN = fileN;
        FileSize = fileSize;
        TaskN = taskN;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Id { get; set; }
    [Key(3)]
    public bool IsActive { get; set; }
    [Key(4)]
    public OrgMemberRole OrgRole { get; set; }
    [Key(5)]
    public string Name { get; set; }
    [Key(6)]
    public ProjectMemberRole Role { get; set; }
    [Key(7)]
    public ulong TimeEst { get; set; }
    [Key(8)]
    public ulong TimeInc { get; set; }
    [Key(9)]
    public ulong CostEst { get; set; }
    [Key(10)]
    public ulong CostInc { get; set; }
    [Key(11)]
    public ulong FileN { get; set; }
    [Key(12)]
    public ulong FileSize { get; set; }
    [Key(13)]
    public ulong TaskN { get; set; }
    
}



[MessagePackObject]
public record Add
{
    public Add(
        string org,
        string project,
        string id,
        ProjectMemberRole role
        
    )
    {
        Org = org;
        Project = project;
        Id = id;
        Role = role;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Id { get; set; }
    [Key(3)]
    public ProjectMemberRole Role { get; set; }
    
}



[MessagePackObject]
public record Get
{
    public Get(
        string org,
        string project,
        bool? isActive = null,
        ProjectMemberRole? role = null,
        string? nameStartsWith = null,
        string? after = null,
        ProjectMemberOrderBy orderBy = ProjectMemberOrderBy.Role,
        bool asc = true
        
    )
    {
        Org = org;
        Project = project;
        IsActive = isActive;
        Role = role;
        NameStartsWith = nameStartsWith;
        After = after;
        OrderBy = orderBy;
        Asc = asc;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public bool? IsActive { get; set; } = null;
    [Key(3)]
    public ProjectMemberRole? Role { get; set; } = null;
    [Key(4)]
    public string? NameStartsWith { get; set; } = null;
    [Key(5)]
    public string? After { get; set; } = null;
    [Key(6)]
    public ProjectMemberOrderBy OrderBy { get; set; } = ProjectMemberOrderBy.Role;
    [Key(7)]
    public bool Asc { get; set; } = true;
    
}



[MessagePackObject]
public record Update
{
    public Update(
        string org,
        string project,
        string id,
        ProjectMemberRole role
        
    )
    {
        Org = org;
        Project = project;
        Id = id;
        Role = role;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Id { get; set; }
    [Key(3)]
    public ProjectMemberRole Role { get; set; }
    
}



[MessagePackObject]
public record Exact
{
    public Exact(
        string org,
        string project,
        string id
        
    )
    {
        Org = org;
        Project = project;
        Id = id;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Id { get; set; }
    
}




public enum ProjectMemberRole
{
    Admin,
    Writer,
    Reader
    
}

public enum ProjectMemberOrderBy
{
    Role,
    Name
    
}
