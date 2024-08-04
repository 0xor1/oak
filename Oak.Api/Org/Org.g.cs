// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

#nullable enable

using Common.Shared;
using MessagePack;


namespace Oak.Api.Org;

public interface IOrgApi
{
    public Task<Org> Create(Create arg, CancellationToken ctkn = default);
    public Task<Org> GetOne(Exact arg, CancellationToken ctkn = default);
    public Task<List<Org>> Get(Get arg, CancellationToken ctkn = default);
    public Task<Org> Update(Update arg, CancellationToken ctkn = default);
    public System.Threading.Tasks.Task Delete(Exact arg, CancellationToken ctkn = default);
    
}

public class OrgApi : IOrgApi
{
    private readonly IRpcClient _client;

    public OrgApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Org> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(OrgRpcs.Create, arg, ctkn);
    
    public Task<Org> GetOne(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(OrgRpcs.GetOne, arg, ctkn);
    
    public Task<List<Org>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(OrgRpcs.Get, arg, ctkn);
    
    public Task<Org> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(OrgRpcs.Update, arg, ctkn);
    
    public System.Threading.Tasks.Task Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(OrgRpcs.Delete, arg, ctkn);
    
    
}

public static class OrgRpcs
{
    public static readonly Rpc<Create, Org> Create = new("/org/create");
    public static readonly Rpc<Exact, Org> GetOne = new("/org/getOne");
    public static readonly Rpc<Get, List<Org>> Get = new("/org/get");
    public static readonly Rpc<Update, Org> Update = new("/org/update");
    public static readonly Rpc<Exact, Nothing> Delete = new("/org/delete");
    
}



[MessagePackObject]
public record Org
{
    public Org(
        string id,
        string name,
        DateTime createdOn,
        OrgMember.OrgMember? member
        
    )
    {
        Id = id;
        Name = name;
        CreatedOn = createdOn;
        Member = member;
        
    }
    
    [Key(0)]
    public string Id { get; set; }
    [Key(1)]
    public string Name { get; set; }
    [Key(2)]
    public DateTime CreatedOn { get; set; }
    [Key(3)]
    public OrgMember.OrgMember? Member { get; set; }
    
}



[MessagePackObject]
public record Create
{
    public Create(
        string name,
        string ownerMemberName
        
    )
    {
        Name = name;
        OwnerMemberName = ownerMemberName;
        
    }
    
    [Key(0)]
    public string Name { get; set; }
    [Key(1)]
    public string OwnerMemberName { get; set; }
    
}



[MessagePackObject]
public record Get
{
    public Get(
        OrgOrderBy orderBy = OrgOrderBy.Name,
        bool asc = true
        
    )
    {
        OrderBy = orderBy;
        Asc = asc;
        
    }
    
    [Key(0)]
    public OrgOrderBy OrderBy { get; set; } = OrgOrderBy.Name;
    [Key(1)]
    public bool Asc { get; set; } = true;
    
}



[MessagePackObject]
public record Update
{
    public Update(
        string id,
        string name
        
    )
    {
        Id = id;
        Name = name;
        
    }
    
    [Key(0)]
    public string Id { get; set; }
    [Key(1)]
    public string Name { get; set; }
    
}



[MessagePackObject]
public record Exact
{
    public Exact(
        string id
        
    )
    {
        Id = id;
        
    }
    
    [Key(0)]
    public string Id { get; set; }
    
}




public enum OrgOrderBy
{
    Name,
    CreatedOn
    
}
