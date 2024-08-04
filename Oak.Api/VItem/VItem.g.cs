// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

#nullable enable

using Common.Shared;
using MessagePack;


namespace Oak.Api.VItem;

public interface IVItemApi
{
    public Task<VItemRes> Create(Create arg, CancellationToken ctkn = default);
    public Task<VItemRes> Update(Update arg, CancellationToken ctkn = default);
    public Task<Task.Task> Delete(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<VItem>> Get(Get arg, CancellationToken ctkn = default);
    
}

public class VItemApi : IVItemApi
{
    private readonly IRpcClient _client;

    public VItemApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<VItemRes> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(VItemRpcs.Create, arg, ctkn);
    
    public Task<VItemRes> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(VItemRpcs.Update, arg, ctkn);
    
    public Task<Task.Task> Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(VItemRpcs.Delete, arg, ctkn);
    
    public Task<SetRes<VItem>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(VItemRpcs.Get, arg, ctkn);
    
    
}

public static class VItemRpcs
{
    public static readonly Rpc<Create, VItemRes> Create = new("/vItem/create");
    public static readonly Rpc<Update, VItemRes> Update = new("/vItem/update");
    public static readonly Rpc<Exact, Task.Task> Delete = new("/vItem/delete");
    public static readonly Rpc<Get, SetRes<VItem>> Get = new("/vItem/get");
    
}



[MessagePackObject]
public record VItem : ICreatable
{
    public VItem(
        string org,
        string project,
        string task,
        VItemType type,
        string id,
        string createdBy,
        DateTime createdOn,
        ulong inc,
        string note
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Type = type;
        Id = id;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        Inc = inc;
        Note = note;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public VItemType Type { get; set; }
    [Key(4)]
    public string Id { get; set; }
    [Key(5)]
    public string CreatedBy { get; set; }
    [Key(6)]
    public DateTime CreatedOn { get; set; }
    [Key(7)]
    public ulong Inc { get; set; }
    [Key(8)]
    public string Note { get; set; }
    
}



[MessagePackObject]
public record Create
{
    public Create(
        string org,
        string project,
        string task,
        VItemType type,
        ulong? est,
        ulong inc,
        string note
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Type = type;
        Est = est;
        Inc = inc;
        Note = note;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public VItemType Type { get; set; }
    [Key(4)]
    public ulong? Est { get; set; }
    [Key(5)]
    public ulong Inc { get; set; }
    [Key(6)]
    public string Note { get; set; }
    
}



[MessagePackObject]
public record Update
{
    public Update(
        string org,
        string project,
        string task,
        VItemType type,
        string id,
        ulong inc,
        string note
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Type = type;
        Id = id;
        Inc = inc;
        Note = note;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public VItemType Type { get; set; }
    [Key(4)]
    public string Id { get; set; }
    [Key(5)]
    public ulong Inc { get; set; }
    [Key(6)]
    public string Note { get; set; }
    
}



[MessagePackObject]
public record Exact
{
    public Exact(
        string org,
        string project,
        string task,
        VItemType type,
        string id
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Type = type;
        Id = id;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public VItemType Type { get; set; }
    [Key(4)]
    public string Id { get; set; }
    
}



[MessagePackObject]
public record Get
{
    public Get(
        string org,
        string project,
        VItemType type,
        string? task = null,
        MinMax<DateTime>? createdOn = null,
        string? createdBy = null,
        string? after = null,
        bool asc = false
        
    )
    {
        Org = org;
        Project = project;
        Type = type;
        Task = task;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        After = after;
        Asc = asc;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public VItemType Type { get; set; }
    [Key(3)]
    public string? Task { get; set; } = null;
    [Key(4)]
    public MinMax<DateTime>? CreatedOn { get; set; } = null;
    [Key(5)]
    public string? CreatedBy { get; set; } = null;
    [Key(6)]
    public string? After { get; set; } = null;
    [Key(7)]
    public bool Asc { get; set; } = false;
    
}



[MessagePackObject]
public record VItemRes
{
    public VItemRes(
        Task.Task task,
        VItem item
        
    )
    {
        Task = task;
        Item = item;
        
    }
    
    [Key(0)]
    public Task.Task Task { get; set; }
    [Key(1)]
    public VItem Item { get; set; }
    
}



public interface ICreatable
{
    [Key(0)]
    public string CreatedBy { get; set; }
    [Key(1)]
    public DateTime CreatedOn { get; set; }
    
}




public enum VItemType
{
    Time,
    Cost
    
}
