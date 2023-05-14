using Common.Shared;

namespace Oak.Api.File;

public interface IFileApi
{
    public Task<FileRes> Upload(Upload arg);
    public Task<HasStream> Download(Download arg);
    public Task<Task.Task> Delete(Exact arg);
    public Task<SetRes<File>> Get(Get arg);
}

public class FileApi : IFileApi
{
    private readonly IRpcClient _client;

    public FileApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<FileRes> Upload(Upload arg) => _client.Do(FileRpcs.Upload, arg);

    public Task<HasStream> Download(Download arg) => _client.Do(FileRpcs.Download, arg);

    public Task<Task.Task> Delete(Exact arg) => _client.Do(FileRpcs.Delete, arg);

    public Task<SetRes<File>> Get(Get arg) => _client.Do(FileRpcs.Get, arg);
}

public static class FileRpcs
{
    public static readonly Rpc<Upload, FileRes> Upload = new("/file/upload");
    public static readonly Rpc<Download, HasStream> Download = new("/file/download");
    public static readonly Rpc<Exact, Task.Task> Delete = new("/file/delete");
    public static readonly Rpc<Get, SetRes<File>> Get = new("/file/get");
}

public record File(
    string Org,
    string Project,
    string Task,
    string Id,
    string Name,
    string CreatedBy,
    DateTime CreatedOn,
    ulong Size,
    string Type
);

public record Upload(string Org, string Project, string Task) : HasStream;

public record Download(string Org, string Project, string Task, string Id, bool IsDownload);

public record Exact(string Org, string Project, string Task, string Id);

public record Get(
    string Org,
    string Project,
    string? Task = null,
    MinMax<DateTime>? CreatedOn = null,
    string? CreatedBy = null,
    string? After = null,
    bool Asc = true
);

public record FileRes(Task.Task Task, File File);

public static class Size
{
    public const ulong KB = 1024;
    public const ulong MB = KB * KB;
    public const ulong GB = KB * MB;
    public const ulong TB = KB * GB;
    public const ulong PB = KB * TB;
}
