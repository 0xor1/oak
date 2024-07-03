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
public record Comment(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] string Id,
    [property: Key(4)] string CreatedBy,
    [property: Key(5)] DateTime CreatedOn,
    [property: Key(6)] string Body
);

[MessagePackObject]
public record Create(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] string Body
);

[MessagePackObject]
public record Update(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] string Id,
    [property: Key(4)] string Body
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
