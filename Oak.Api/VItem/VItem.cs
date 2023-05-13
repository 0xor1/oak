using System.ComponentModel;
using System.Runtime.Serialization;
using Common.Shared;

namespace Oak.Api.VItem;

public interface IVItemApi
{
    public Task<VItemRes> Create(Create arg);
    public Task<VItemRes> Update(Update arg);
    public Task<Task.Task> Delete(Exact arg);
    public Task<SetRes<VItem>> Get(Get arg);
}

public class VItemApi : IVItemApi
{
    private readonly IRpcClient _client;

    public VItemApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<VItemRes> Create(Create arg) => _client.Do(VItemRpcs.Create, arg);

    public Task<VItemRes> Update(Update arg) => _client.Do(VItemRpcs.Update, arg);

    public Task<Task.Task> Delete(Exact arg) => _client.Do(VItemRpcs.Delete, arg);

    public Task<SetRes<VItem>> Get(Get arg) => _client.Do(VItemRpcs.Get, arg);
}

public static class VItemRpcs
{
    public static readonly Rpc<Create, VItemRes> Create = new("/vitem/create");
    public static readonly Rpc<Update, VItemRes> Update = new("/vitem/update");
    public static readonly Rpc<Exact, Task.Task> Delete = new("/vitem/delete");
    public static readonly Rpc<Get, SetRes<VItem>> Get = new("/vitem/get");
}

public record VItem(
    string Org,
    string Project,
    string Task,
    VItemType Type,
    string Id,
    string CreatedBy,
    DateTime CreatedOn,
    ulong Inc,
    string Note
);

public record Create(
    string Org,
    string Project,
    string Task,
    VItemType Type,
    ulong? Est,
    ulong Inc,
    string Note
);

public record Update(
    string Org,
    string Project,
    string Task,
    VItemType Type,
    string Id,
    ulong Inc,
    string Note
);

public record Exact(string Org, string Project, string Task, VItemType Type, string Id);

public record Get(
    string Org,
    string Project,
    VItemType Type,
    string? Task = null,
    MinMax<DateTime>? CreatedOn = null,
    string? CreatedBy = null,
    string? After = null,
    bool Asc = true
);

public record VItemRes(Task.Task Task, VItem Item);

public enum VItemType
{
    [EnumMember(Value = "time")]
    [Description("time")]
    Time,

    [EnumMember(Value = "cost")]
    [Description("cost")]
    Cost
}
