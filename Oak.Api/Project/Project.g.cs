// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

#nullable enable

using Common.Shared;
using MessagePack;


namespace Oak.Api.Project;

public interface IProjectApi
{
    public Task<Project> Create(Create arg, CancellationToken ctkn = default);
    public Task<Project> GetOne(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<Project>> Get(Get arg, CancellationToken ctkn = default);
    public Task<Project> Update(Update arg, CancellationToken ctkn = default);
    public System.Threading.Tasks.Task Delete(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<Activity>> GetActivities(GetActivities arg, CancellationToken ctkn = default);
    
}

public class ProjectApi : IProjectApi
{
    private readonly IRpcClient _client;

    public ProjectApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Project> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.Create, arg, ctkn);
    
    public Task<Project> GetOne(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.GetOne, arg, ctkn);
    
    public Task<SetRes<Project>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.Get, arg, ctkn);
    
    public Task<Project> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.Update, arg, ctkn);
    
    public System.Threading.Tasks.Task Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.Delete, arg, ctkn);
    
    public Task<SetRes<Activity>> GetActivities(GetActivities arg, CancellationToken ctkn = default) =>
        _client.Do(ProjectRpcs.GetActivities, arg, ctkn);
    
    
}

public static class ProjectRpcs
{
    public static readonly Rpc<Create, Project> Create = new("/project/create");
    public static readonly Rpc<Exact, Project> GetOne = new("/project/getOne");
    public static readonly Rpc<Get, SetRes<Project>> Get = new("/project/get");
    public static readonly Rpc<Update, Project> Update = new("/project/update");
    public static readonly Rpc<Exact, Nothing> Delete = new("/project/delete");
    public static readonly Rpc<GetActivities, SetRes<Activity>> GetActivities = new("/project/getActivities");
    
}



[MessagePackObject]
public record Project
{
    public Project(
        string org,
        string id,
        bool isArchived,
        bool isPublic,
        string name,
        DateTime createdOn,
        string currencySymbol,
        string currencyCode,
        uint hoursPerDay,
        uint daysPerWeek,
        DateTime? startOn,
        DateTime? endOn,
        ulong fileLimit,
        Task.Task task
        
    )
    {
        Org = org;
        Id = id;
        IsArchived = isArchived;
        IsPublic = isPublic;
        Name = name;
        CreatedOn = createdOn;
        CurrencySymbol = currencySymbol;
        CurrencyCode = currencyCode;
        HoursPerDay = hoursPerDay;
        DaysPerWeek = daysPerWeek;
        StartOn = startOn;
        EndOn = endOn;
        FileLimit = fileLimit;
        Task = task;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Id { get; set; }
    [Key(2)]
    public bool IsArchived { get; set; }
    [Key(3)]
    public bool IsPublic { get; set; }
    [Key(4)]
    public string Name { get; set; }
    [Key(5)]
    public DateTime CreatedOn { get; set; }
    [Key(6)]
    public string CurrencySymbol { get; set; }
    [Key(7)]
    public string CurrencyCode { get; set; }
    [Key(8)]
    public uint HoursPerDay { get; set; }
    [Key(9)]
    public uint DaysPerWeek { get; set; }
    [Key(10)]
    public DateTime? StartOn { get; set; }
    [Key(11)]
    public DateTime? EndOn { get; set; }
    [Key(12)]
    public ulong FileLimit { get; set; }
    [Key(13)]
    public Task.Task Task { get; set; }
    
}



[MessagePackObject]
public record Create
{
    public Create(
        string org,
        bool isPublic,
        string name,
        string currencySymbol,
        string currencyCode,
        uint hoursPerDay,
        uint daysPerWeek,
        DateTime? startOn,
        DateTime? endOn,
        ulong fileLimit
        
    )
    {
        Org = org;
        IsPublic = isPublic;
        Name = name;
        CurrencySymbol = currencySymbol;
        CurrencyCode = currencyCode;
        HoursPerDay = hoursPerDay;
        DaysPerWeek = daysPerWeek;
        StartOn = startOn;
        EndOn = endOn;
        FileLimit = fileLimit;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public bool IsPublic { get; set; }
    [Key(2)]
    public string Name { get; set; }
    [Key(3)]
    public string CurrencySymbol { get; set; }
    [Key(4)]
    public string CurrencyCode { get; set; }
    [Key(5)]
    public uint HoursPerDay { get; set; }
    [Key(6)]
    public uint DaysPerWeek { get; set; }
    [Key(7)]
    public DateTime? StartOn { get; set; }
    [Key(8)]
    public DateTime? EndOn { get; set; }
    [Key(9)]
    public ulong FileLimit { get; set; }
    
}



[MessagePackObject]
public record Get
{
    public Get(
        string org,
        bool? isPublic = null,
        string? nameStartsWith = null,
        MinMax<DateTime>? createdOn = null,
        MinMax<DateTime>? startOn = null,
        MinMax<DateTime>? endOn = null,
        bool isArchived = false,
        string? after = null,
        ProjectOrderBy orderBy = ProjectOrderBy.Name,
        bool asc = true
        
    )
    {
        Org = org;
        IsPublic = isPublic;
        NameStartsWith = nameStartsWith;
        CreatedOn = createdOn;
        StartOn = startOn;
        EndOn = endOn;
        IsArchived = isArchived;
        After = after;
        OrderBy = orderBy;
        Asc = asc;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public bool? IsPublic { get; set; } = null;
    [Key(2)]
    public string? NameStartsWith { get; set; } = null;
    [Key(3)]
    public MinMax<DateTime>? CreatedOn { get; set; } = null;
    [Key(4)]
    public MinMax<DateTime>? StartOn { get; set; } = null;
    [Key(5)]
    public MinMax<DateTime>? EndOn { get; set; } = null;
    [Key(6)]
    public bool IsArchived { get; set; } = false;
    [Key(7)]
    public string? After { get; set; } = null;
    [Key(8)]
    public ProjectOrderBy OrderBy { get; set; } = ProjectOrderBy.Name;
    [Key(9)]
    public bool Asc { get; set; } = true;
    
}



[MessagePackObject]
public record Update
{
    public Update(
        string org,
        string id,
        bool? isPublic = null,
        string? name = null,
        string? currencySymbol = null,
        string? currencyCode = null,
        uint? hoursPerDay = null,
        uint? daysPerWeek = null,
        DateTime? startOn = null,
        DateTime? endOn = null,
        ulong? fileLimit = null
        
    )
    {
        Org = org;
        Id = id;
        IsPublic = isPublic;
        Name = name;
        CurrencySymbol = currencySymbol;
        CurrencyCode = currencyCode;
        HoursPerDay = hoursPerDay;
        DaysPerWeek = daysPerWeek;
        StartOn = startOn;
        EndOn = endOn;
        FileLimit = fileLimit;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Id { get; set; }
    [Key(2)]
    public bool? IsPublic { get; set; } = null;
    [Key(3)]
    public string? Name { get; set; } = null;
    [Key(4)]
    public string? CurrencySymbol { get; set; } = null;
    [Key(5)]
    public string? CurrencyCode { get; set; } = null;
    [Key(6)]
    public uint? HoursPerDay { get; set; } = null;
    [Key(7)]
    public uint? DaysPerWeek { get; set; } = null;
    [Key(8)]
    public DateTime? StartOn { get; set; } = null;
    [Key(9)]
    public DateTime? EndOn { get; set; } = null;
    [Key(10)]
    public ulong? FileLimit { get; set; } = null;
    
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



[MessagePackObject]
public record Activity
{
    public Activity(
        string org,
        string project,
        string? task,
        DateTime occurredOn,
        string user,
        string item,
        ActivityItemType itemType,
        bool taskDeleted,
        bool itemDeleted,
        ActivityAction action,
        string? taskName,
        string? itemName,
        string? extraInfo
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        OccurredOn = occurredOn;
        User = user;
        Item = item;
        ItemType = itemType;
        TaskDeleted = taskDeleted;
        ItemDeleted = itemDeleted;
        Action = action;
        TaskName = taskName;
        ItemName = itemName;
        ExtraInfo = extraInfo;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string? Task { get; set; }
    [Key(3)]
    public DateTime OccurredOn { get; set; }
    [Key(4)]
    public string User { get; set; }
    [Key(5)]
    public string Item { get; set; }
    [Key(6)]
    public ActivityItemType ItemType { get; set; }
    [Key(7)]
    public bool TaskDeleted { get; set; }
    [Key(8)]
    public bool ItemDeleted { get; set; }
    [Key(9)]
    public ActivityAction Action { get; set; }
    [Key(10)]
    public string? TaskName { get; set; }
    [Key(11)]
    public string? ItemName { get; set; }
    [Key(12)]
    public string? ExtraInfo { get; set; }
    
}



[MessagePackObject]
public record GetActivities
{
    public GetActivities(
        string org,
        string project,
        bool excludeDeletedItem = true,
        string? task = null,
        string? item = null,
        string? user = null,
        MinMax<DateTime>? occurredOn = null,
        bool asc = false
        
    )
    {
        Org = org;
        Project = project;
        ExcludeDeletedItem = excludeDeletedItem;
        Task = task;
        Item = item;
        User = user;
        OccurredOn = occurredOn;
        Asc = asc;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public bool ExcludeDeletedItem { get; set; } = true;
    [Key(3)]
    public string? Task { get; set; } = null;
    [Key(4)]
    public string? Item { get; set; } = null;
    [Key(5)]
    public string? User { get; set; } = null;
    [Key(6)]
    public MinMax<DateTime>? OccurredOn { get; set; } = null;
    [Key(7)]
    public bool Asc { get; set; } = false;
    
}



[MessagePackObject]
public record FcmData
{
    public FcmData(
        Activity activity,
        List<string> ancestors
        
    )
    {
        Activity = activity;
        Ancestors = ancestors;
        
    }
    
    [Key(0)]
    public Activity Activity { get; set; }
    [Key(1)]
    public List<string> Ancestors { get; set; }
    
}




public enum ProjectOrderBy
{
    Name,
    CreatedOn,
    StartOn,
    EndOn
    
}

public enum ActivityItemType
{
    Org,
    Project,
    Member,
    Task,
    VItem,
    File,
    Comment
    
}

public enum ActivityAction
{
    Create,
    Update,
    Delete
    
}
