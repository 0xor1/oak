// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

#nullable enable

using Common.Shared;
using MessagePack;


namespace Oak.Api.Timer;

public interface ITimerApi
{
    public Task<List<Timer>> Create(Create arg, CancellationToken ctkn = default);
    public Task<SetRes<Timer>> Get(Get arg, CancellationToken ctkn = default);
    public Task<List<Timer>> Update(Update arg, CancellationToken ctkn = default);
    public Task<List<Timer>> Delete(Delete arg, CancellationToken ctkn = default);
    
}

public class TimerApi : ITimerApi
{
    private readonly IRpcClient _client;

    public TimerApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<List<Timer>> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Create, arg, ctkn);
    
    public Task<SetRes<Timer>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Get, arg, ctkn);
    
    public Task<List<Timer>> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Update, arg, ctkn);
    
    public Task<List<Timer>> Delete(Delete arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Delete, arg, ctkn);
    
    
}

public static class TimerRpcs
{
    public static readonly Rpc<Create, List<Timer>> Create = new("/timer/create");
    public static readonly Rpc<Get, SetRes<Timer>> Get = new("/timer/get");
    public static readonly Rpc<Update, List<Timer>> Update = new("/timer/update");
    public static readonly Rpc<Delete, List<Timer>> Delete = new("/timer/delete");
    
}



[MessagePackObject]
public record Timer
{
    public Timer(
        string org,
        string project,
        string task,
        string user,
        string taskName,
        ulong inc,
        DateTime lastStartedOn,
        bool isRunning
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        User = user;
        TaskName = taskName;
        Inc = inc;
        LastStartedOn = lastStartedOn;
        IsRunning = isRunning;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public string User { get; set; }
    [Key(4)]
    public string TaskName { get; set; }
    [Key(5)]
    public ulong Inc { get; set; }
    [Key(6)]
    public DateTime LastStartedOn { get; set; }
    [Key(7)]
    public bool IsRunning { get; set; }
    
}



[MessagePackObject]
public record Create
{
    public Create(
        string org,
        string project,
        string task
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    
}



[MessagePackObject]
public record Get
{
    public Get(
        string org,
        string project,
        string? task = null,
        string? user = null,
        bool asc = false
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        User = user;
        Asc = asc;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string? Task { get; set; } = null;
    [Key(3)]
    public string? User { get; set; } = null;
    [Key(4)]
    public bool Asc { get; set; } = false;
    
}



[MessagePackObject]
public record Update
{
    public Update(
        string org,
        string project,
        string task,
        bool isRunning
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        IsRunning = isRunning;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    [Key(3)]
    public bool IsRunning { get; set; }
    
}



[MessagePackObject]
public record Delete
{
    public Delete(
        string org,
        string project,
        string task
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        
    }
    
    [Key(0)]
    public string Org { get; set; }
    [Key(1)]
    public string Project { get; set; }
    [Key(2)]
    public string Task { get; set; }
    
}



