// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

#nullable enable

using Common.Shared;
using MessagePack;
using Oak.Api.VItem;


namespace Oak.Api.File;

public interface IFileApi
{
    public Task<FileRes> Upload(Upload arg, CancellationToken ctkn = default);
    public Task<HasStream> Download(Download arg, CancellationToken ctkn = default);
    public string DownloadUrl(Download arg);
    public Task<Task.Task> Delete(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<File>> Get(Get arg, CancellationToken ctkn = default);
    
}

public class FileApi : IFileApi
{
    private readonly IRpcClient _client;

    public FileApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<FileRes> Upload(Upload arg, CancellationToken ctkn = default) =>
        _client.Do(FileRpcs.Upload, arg, ctkn);
    
    public Task<HasStream> Download(Download arg, CancellationToken ctkn = default) =>
        _client.Do(FileRpcs.Download, arg, ctkn);
    
    public string DownloadUrl(Download arg) =>
        _client.GetUrl(FileRpcs.Download, arg);
    
    public Task<Task.Task> Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(FileRpcs.Delete, arg, ctkn);
    
    public Task<SetRes<File>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(FileRpcs.Get, arg, ctkn);
    
    
}

public static class FileRpcs
{
    public static readonly Rpc<Upload, FileRes> Upload = new("/file/upload");
    public static readonly Rpc<Download, HasStream> Download = new("/file/download");
    public static readonly Rpc<Exact, Task.Task> Delete = new("/file/delete");
    public static readonly Rpc<Get, SetRes<File>> Get = new("/file/get");
    
}



[MessagePackObject]
public record File : ICreatable
{
    public File(
        string org,
        string project,
        string task,
        string id,
        string name,
        string createdBy,
        DateTime createdOn,
        ulong size,
        string type
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Id = id;
        Name = name;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        Size = size;
        Type = type;
        
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
    public string Name { get; set; }
    [Key(5)]
    public string CreatedBy { get; set; }
    [Key(6)]
    public DateTime CreatedOn { get; set; }
    [Key(7)]
    public ulong Size { get; set; }
    [Key(8)]
    public string Type { get; set; }
    
}



[MessagePackObject]
public record Upload : HasStream
{
    public Upload(
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
public record Download
{
    public Download(
        string org,
        string project,
        string task,
        string id,
        bool isDownload
        
    )
    {
        Org = org;
        Project = project;
        Task = task;
        Id = id;
        IsDownload = isDownload;
        
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
    public bool IsDownload { get; set; }
    
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
        string? task,
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
    public string? Task { get; set; }
    [Key(3)]
    public MinMax<DateTime>? CreatedOn { get; set; } = null;
    [Key(4)]
    public string? CreatedBy { get; set; } = null;
    [Key(5)]
    public string? After { get; set; } = null;
    [Key(6)]
    public bool Asc { get; set; } = false;
    
}



[MessagePackObject]
public record FileRes
{
    public FileRes(
        Task.Task task,
        File file
        
    )
    {
        Task = task;
        File = file;
        
    }
    
    [Key(0)]
    public Task.Task Task { get; set; }
    [Key(1)]
    public File File { get; set; }
    
}



