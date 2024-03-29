﻿using Common.Shared;

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

public record Comment(
    string Org,
    string Project,
    string Task,
    string Id,
    string CreatedBy,
    DateTime CreatedOn,
    string Body
);

public record Create(string Org, string Project, string Task, string Body);

public record Update(string Org, string Project, string Task, string Id, string Body);

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
