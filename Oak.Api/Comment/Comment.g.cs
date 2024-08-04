// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

#nullable enable

using Common.Shared;
using MessagePack;


namespace Oak.Api.Comment;

public interface ICommentApi
{
    public Task<Comment> Create(Create arg, CancellationToken ctkn = default);
    public Task<Comment> Update(Update arg, CancellationToken ctkn = default);
    public System.Threading.Tasks.Task Delete(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<Comment>> Get(Get arg, CancellationToken ctkn = default);
    
}

public class CommentApi : ICommentApi
{
    private readonly IRpcClient _client;

    public CommentApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Comment> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(CommentRpcs.Create, arg, ctkn);
    
    public Task<Comment> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(CommentRpcs.Update, arg, ctkn);
    
    public System.Threading.Tasks.Task Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(CommentRpcs.Delete, arg, ctkn);
    
    public Task<SetRes<Comment>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(CommentRpcs.Get, arg, ctkn);
    
    
}

public static class CommentRpcs
{
    public static readonly Rpc<Create, Comment> Create = new("/comment/create");
    public static readonly Rpc<Update, Comment> Update = new("/comment/update");
    public static readonly Rpc<Exact, Nothing> Delete = new("/comment/delete");
    public static readonly Rpc<Get, SetRes<Comment>> Get = new("/comment/get");
    
}



[MessagePackObject]
public record Comment
{
    public Comment(
        string org,
        string project,
        string task,
        string id,
        string createdBy,
        DateTime createdOn,
        string body
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Id = id;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        Body = body;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public string Id { get; set; }
    [Key(4)]
    public string CreatedBy { get; set; }
    [Key(5)]
    public DateTime CreatedOn { get; set; }
    [Key(6)]
    public string Body { get; set; }
    
}



[MessagePackObject]
public record Create
{
    public Create(
        string org,
        string project,
        string task,
        string body
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Body = body;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public string Body { get; set; }
    
}



[MessagePackObject]
public record Update
{
    public Update(
        string org,
        string project,
        string task,
        string id,
        string body
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Id = id;
        Body = body;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public string Id { get; set; }
    [Key(4)]
    public string Body { get; set; }
    
}



[MessagePackObject]
public record Exact
{
    public Exact(
        string org,
        string project,
        string task,
        string id
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Id = id;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public string Id { get; set; }
    
}



[MessagePackObject]
public record Get
{
    public Get(
        string org,
        string project,
        string? task = null,
        MinMax<DateTime>? createdOn = null,
        string? createdBy = null,
        string? after = null,
        bool asc = false
        
    )
    {
        Org = org;
        Project = project;
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
    public string? Task { get; set; } = null;
    [Key(3)]
    public MinMax<DateTime>? CreatedOn { get; set; } = null;
    [Key(4)]
    public string? CreatedBy { get; set; } = null;
    [Key(5)]
    public string? After { get; set; } = null;
    [Key(6)]
    public bool Asc { get; set; } = false;
    
}



