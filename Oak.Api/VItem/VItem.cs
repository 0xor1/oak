using System.ComponentModel;
using System.Runtime.Serialization;
using Common.Shared;
using MessagePack;

namespace Oak.Api.VItem;

public interface ICreatable
{
    public string CreatedBy { get; }
    public DateTime CreatedOn { get; }
}

public interface IVItemApi
{
    public Task<VItemRes> Create(Create arg, CancellationToken ctkn = default);
    public Task<VItemRes> Update(Update arg, CancellationToken ctkn = default);
    public Task<Task.Task> Delete(Exact arg, CancellationToken ctkn = default);
    public Task<SetRes<VItem>> Get(Get arg, CancellationToken ctkn = default);
}

public class VItemApi : IVItemApi
{
    private readonly IRpcClient _client;

    public VItemApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<VItemRes> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(VItemRpcs.Create, arg, ctkn);

    public Task<VItemRes> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(VItemRpcs.Update, arg, ctkn);

    public Task<Task.Task> Delete(Exact arg, CancellationToken ctkn = default) =>
        _client.Do(VItemRpcs.Delete, arg, ctkn);

    public Task<SetRes<VItem>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(VItemRpcs.Get, arg, ctkn);
}

public static class VItemRpcs
{
    public static readonly Rpc<Create, VItemRes> Create = new("/vitem/create");
    public static readonly Rpc<Update, VItemRes> Update = new("/vitem/update");
    public static readonly Rpc<Exact, Task.Task> Delete = new("/vitem/delete");
    public static readonly Rpc<Get, SetRes<VItem>> Get = new("/vitem/get");
}

[MessagePackObject]
public record VItem(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] VItemType Type,
    [property: Key(4)] string Id,
    [property: Key(5)] string CreatedBy,
    [property: Key(6)] DateTime CreatedOn,
    [property: Key(7)] ulong Inc,
    [property: Key(8)] string Note
) : ICreatable;

[MessagePackObject]
public record Create(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] VItemType Type,
    [property: Key(4)] ulong? Est,
    [property: Key(5)] ulong Inc,
    [property: Key(6)] string Note
);

[MessagePackObject]
public record Update(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] VItemType Type,
    [property: Key(4)] string Id,
    [property: Key(5)] ulong Inc,
    [property: Key(6)] string Note
);

[MessagePackObject]
public record Exact(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] VItemType Type,
    [property: Key(4)] string Id
);

[MessagePackObject]
public record Get(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] VItemType Type,
    [property: Key(3)] string? Task = null,
    [property: Key(4)] MinMax<DateTime>? CreatedOn = null,
    [property: Key(5)] string? CreatedBy = null,
    [property: Key(6)] string? After = null,
    [property: Key(7)] bool Asc = false
);

[MessagePackObject]
public record VItemRes([property: Key(0)] Task.Task Task, [property: Key(1)] VItem Item);

public enum VItemType
{
    [EnumMember(Value = "time")]
    [Description("time")]
    Time,

    [EnumMember(Value = "cost")]
    [Description("cost")]
    Cost
}
