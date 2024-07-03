using Common.Shared;
using MessagePack;
using Newtonsoft.Json;

namespace Oak.Api.Timer;

public interface ITimerApi
{
    public Task<IReadOnlyList<Timer>> Create(Create arg, CancellationToken ctkn = default);
    public Task<SetRes<Timer>> Get(Get arg, CancellationToken ctkn = default);
    public Task<IReadOnlyList<Timer>> Update(Update arg, CancellationToken ctkn = default);
    public Task<IReadOnlyList<Timer>> Delete(Delete arg, CancellationToken ctkn = default);
}

public class TimerApi : ITimerApi
{
    private readonly IRpcClient _client;

    public TimerApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<IReadOnlyList<Timer>> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Create, arg, ctkn);

    public Task<SetRes<Timer>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Get, arg, ctkn);

    public Task<IReadOnlyList<Timer>> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Update, arg, ctkn);

    public Task<IReadOnlyList<Timer>> Delete(Delete arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Delete, arg, ctkn);
}

public static class TimerRpcs
{
    public static readonly Rpc<Create, IReadOnlyList<Timer>> Create = new("/timer/create");
    public static readonly Rpc<Get, SetRes<Timer>> Get = new("/timer/get");
    public static readonly Rpc<Update, IReadOnlyList<Timer>> Update = new("/timer/update");
    public static readonly Rpc<Delete, IReadOnlyList<Timer>> Delete = new("/timer/delete");
}

[MessagePackObject]
public record Timer(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] string User,
    [property: Key(4)] string TaskName,
    [property: Key(5)] ulong Inc,
    [property: Key(6)] DateTime LastStartedOn,
    [property: Key(7)] bool IsRunning
)
{
    [JsonIgnore]
    [IgnoreMember]
    public ulong FullInc =>
        !IsRunning
            ? Inc
            : Inc + (ulong)DateTimeExt.UtcNowMilli().Subtract(LastStartedOn).TotalSeconds;
}

[MessagePackObject]
public record Create(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task
);

[MessagePackObject]
public record Get(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string? Task = null,
    [property: Key(3)] string? User = null,
    [property: Key(4)] bool Asc = false
);

[MessagePackObject]
public record Update(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task,
    [property: Key(3)] bool IsRunning
);

[MessagePackObject]
public record Delete(
    [property: Key(0)] string Org,
    [property: Key(1)] string Project,
    [property: Key(2)] string Task
);
