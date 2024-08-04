// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

#nullable enable

using Common.Shared;
using MessagePack;


namespace Oak.Api.Task;

public interface ITaskApi
{
    public Task<CreateRes> Create(Create arg, CancellationToken ctkn = default);
    public Task<Task> GetOne(Exact arg, CancellationToken ctkn = default);
    public Task<List<Task>> GetAncestors(Exact arg, CancellationToken ctkn = default);
    public Task<List<Task>> GetChildren(GetChildren arg, CancellationToken ctkn = default);
    public Task<InitView> GetInitView(Exact arg, CancellationToken ctkn = default);
    public Task<List<Task>> GetAllDescendants(Exact arg, CancellationToken ctkn = default);
    public Task<UpdateRes> Update(Update arg, CancellationToken ctkn = default);
    public Task<Task> Delete(Exact arg, CancellationToken ctkn = default);
    
}

public class TaskApi : ITaskApi
{
    private readonly IRpcClient _client;

    public TaskApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<CreateRes> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.Create, arg, ctkn);
    
    public Task<Task> GetOne(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.GetOne, arg, ctkn);
    
    public Task<List<Task>> GetAncestors(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.GetAncestors, arg, ctkn);
    
    public Task<List<Task>> GetChildren(GetChildren arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.GetChildren, arg, ctkn);
    
    public Task<InitView> GetInitView(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.GetInitView, arg, ctkn);
    
    public Task<List<Task>> GetAllDescendants(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.GetAllDescendants, arg, ctkn);
    
    public Task<UpdateRes> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.Update, arg, ctkn);
    
    public Task<Task> Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(TaskRpcs.Delete, arg, ctkn);
    
    
}

public static class TaskRpcs
{
    public static readonly Rpc<Create, CreateRes> Create = new("/task/create");
    public static readonly Rpc<Exact, Task> GetOne = new("/task/getOne");
    public static readonly Rpc<Exact, List<Task>> GetAncestors = new("/task/getAncestors");
    public static readonly Rpc<GetChildren, List<Task>> GetChildren = new("/task/getChildren");
    public static readonly Rpc<Exact, InitView> GetInitView = new("/task/getInitView");
    public static readonly Rpc<Exact, List<Task>> GetAllDescendants = new("/task/getAllDescendants");
    public static readonly Rpc<Update, UpdateRes> Update = new("/task/update");
    public static readonly Rpc<Exact, Task> Delete = new("/task/delete");
    
}



[MessagePackObject]
public record Task
{
    public Task(
        string org,
        string project,
        string id,
        string? parent,
        string? firstChild,
        string? nextSib,
        string? user,
        string name,
        string description,
        string createdBy,
        DateTime createdOn,
        ulong timeEst,
        ulong timeInc,
        ulong timeSubMin,
        ulong timeSubEst,
        ulong timeSubInc,
        ulong costEst,
        ulong costInc,
        ulong costSubEst,
        ulong costSubInc,
        ulong fileN,
        ulong fileSize,
        ulong fileSubN,
        ulong fileSubSize,
        ulong childN,
        ulong descN,
        bool isParallel
        
    )
    {
        Org = org;
        Project = project;
        Id = id;
        Parent = parent;
        FirstChild = firstChild;
        NextSib = nextSib;
        User = user;
        Name = name;
        Description = description;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        TimeEst = timeEst;
        TimeInc = timeInc;
        TimeSubMin = timeSubMin;
        TimeSubEst = timeSubEst;
        TimeSubInc = timeSubInc;
        CostEst = costEst;
        CostInc = costInc;
        CostSubEst = costSubEst;
        CostSubInc = costSubInc;
        FileN = fileN;
        FileSize = fileSize;
        FileSubN = fileSubN;
        FileSubSize = fileSubSize;
        ChildN = childN;
        DescN = descN;
        IsParallel = isParallel;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Id { get; set; }
    [Key(3)]
    public string? Parent { get; set; }
    [Key(4)]
    public string? FirstChild { get; set; }
    [Key(5)]
    public string? NextSib { get; set; }
    [Key(6)]
    public string? User { get; set; }
    [Key(7)]
    public string Name { get; set; }
    [Key(8)]
    public string Description { get; set; }
    [Key(9)]
    public string CreatedBy { get; set; }
    [Key(10)]
    public DateTime CreatedOn { get; set; }
    [Key(11)]
    public ulong TimeEst { get; set; }
    [Key(12)]
    public ulong TimeInc { get; set; }
    [Key(13)]
    public ulong TimeSubMin { get; set; }
    [Key(14)]
    public ulong TimeSubEst { get; set; }
    [Key(15)]
    public ulong TimeSubInc { get; set; }
    [Key(16)]
    public ulong CostEst { get; set; }
    [Key(17)]
    public ulong CostInc { get; set; }
    [Key(18)]
    public ulong CostSubEst { get; set; }
    [Key(19)]
    public ulong CostSubInc { get; set; }
    [Key(20)]
    public ulong FileN { get; set; }
    [Key(21)]
    public ulong FileSize { get; set; }
    [Key(22)]
    public ulong FileSubN { get; set; }
    [Key(23)]
    public ulong FileSubSize { get; set; }
    [Key(24)]
    public ulong ChildN { get; set; }
    [Key(25)]
    public ulong DescN { get; set; }
    [Key(26)]
    public bool IsParallel { get; set; }
    
}



[MessagePackObject]
public record CreateRes
{
    public CreateRes(
        Task parent,
        Task created
        
    )
    {
        Parent = parent;
        Created = created;
        
    }
    
    [Key(0)]
    public Task Parent { get; set; }
    [Key(1)]
    public Task Created { get; set; }
    
}



[MessagePackObject]
public record Create
{
    public Create(
        string org,
        string project,
        string parent,
        string? prevSib,
        string name,
        string description = "",
        bool isParallel = false,
        string? user = null,
        ulong timeEst = 0,
        ulong costEst = 0
        
    )
    {
        Org = org;
        Project = project;
        Parent = parent;
        PrevSib = prevSib;
        Name = name;
        Description = description;
        IsParallel = isParallel;
        User = user;
        TimeEst = timeEst;
        CostEst = costEst;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Parent { get; set; }
    [Key(3)]
    public string? PrevSib { get; set; }
    [Key(4)]
    public string Name { get; set; }
    [Key(5)]
    public string Description { get; set; } = "";
    [Key(6)]
    public bool IsParallel { get; set; } = false;
    [Key(7)]
    public string? User { get; set; } = null;
    [Key(8)]
    public ulong TimeEst { get; set; } = 0;
    [Key(9)]
    public ulong CostEst { get; set; } = 0;
    
}



[MessagePackObject]
public record Update
{
    public Update(
        string org,
        string project,
        string id,
        string? parent = null,
        NSet<string>? prevSib = null,
        string? name = null,
        string? description = null,
        bool? isParallel = null,
        NSet<string>? user = null,
        ulong? timeEst = null,
        ulong? costEst = null
        
    )
    {
        Org = org;
        Project = project;
        Id = id;
        Parent = parent;
        PrevSib = prevSib;
        Name = name;
        Description = description;
        IsParallel = isParallel;
        User = user;
        TimeEst = timeEst;
        CostEst = costEst;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Id { get; set; }
    [Key(3)]
    public string? Parent { get; set; } = null;
    [Key(4)]
    public NSet<string>? PrevSib { get; set; } = null;
    [Key(5)]
    public string? Name { get; set; } = null;
    [Key(6)]
    public string? Description { get; set; } = null;
    [Key(7)]
    public bool? IsParallel { get; set; } = null;
    [Key(8)]
    public NSet<string>? User { get; set; } = null;
    [Key(9)]
    public ulong? TimeEst { get; set; } = null;
    [Key(10)]
    public ulong? CostEst { get; set; } = null;
    
}



[MessagePackObject]
public record UpdateRes
{
    public UpdateRes(
        Task task,
        Task? oldParent,
        Task? newParent
        
    )
    {
        Task = task;
        OldParent = oldParent;
        NewParent = newParent;
        
    }
    
    [Key(0)]
    public Task Task { get; set; }
    [Key(1)]
    public Task? OldParent { get; set; }
    [Key(2)]
    public Task? NewParent { get; set; }
    
}



[MessagePackObject]
public record GetChildren
{
    public GetChildren(
        string org,
        string project,
        string id,
        string? after
        
    )
    {
        Org = org;
        Project = project;
        Id = id;
        After = after;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Id { get; set; }
    [Key(3)]
    public string? After { get; set; }
    
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



[MessagePackObject]
public record InitView
{
    public InitView(
        Task task,
        List<Task> children,
        List<Task> ancestors
        
    )
    {
        Task = task;
        Children = children;
        Ancestors = ancestors;
        
    }
    
    [Key(0)]
    public Task Task { get; set; }
    [Key(1)]
    public List<Task> Children { get; set; }
    [Key(2)]
    public List<Task> Ancestors { get; set; }
    
}



