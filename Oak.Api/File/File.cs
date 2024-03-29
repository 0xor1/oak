﻿using Common.Shared;
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
) : ICreatable;

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
    bool Asc = false
);

public record FileRes(Task.Task Task, File File);
