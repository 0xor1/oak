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

    public string DownloadUrl(Download arg) => _client.GetUrl(FileRpcs.Download, arg);

    public Task<Task.Task> Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(FileRpcs.Delete, arg, ctkn);

    public Task<SetRes<File>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(FileRpcs.Get, arg, ctkn);
}

public static class FileRpcs
{
    public static readonly Rpc<Upload, FileRes> Upload = new("/file/upload", 50 * Size.MB);
    public static readonly Rpc<Download, HasStream> Download = new("/file/download");
    public static readonly Rpc<Exact, Task.Task> Delete = new("/file/delete");
    public static readonly Rpc<Get, SetRes<File>> Get = new("/file/get");
}

[MessagePackObject]
public record File(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] string Id,
    [property: Key(4)] string Name,
    [property: Key(5)] string CreatedBy,
    [property: Key(6)] DateTime CreatedOn,
    [property: Key(7)] ulong Size,
    [property: Key(8)] string Type
) : ICreatable;

[MessagePackObject]
public record Upload(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task
) : HasStream;

[MessagePackObject]
public record Download(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] string Id,
    [property: Key(4)] bool IsDownload
);

[MessagePackObject]
public record Exact(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] string Id
);

[MessagePackObject]
public record Get(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string? Task = null,
    [property: Key(3)] MinMax<DateTime>? CreatedOn = null,
    [property: Key(4)] string? CreatedBy = null,
    [property: Key(5)] string? After = null,
    [property: Key(6)] bool Asc = false
);

[MessagePackObject]
public record FileRes([property: Key(0)] Task.Task Task, [property: Key(1)] File File);
