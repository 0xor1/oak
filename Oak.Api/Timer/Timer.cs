using Common.Shared;

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

public record Timer(
    string Org,
    string Project,
    string Task,
    string User,
    ulong Inc,
    DateTime LastStartedOn,
    bool IsRunning
);

public record Create(string Org, string Project, string Task);

public record Get(
    string Org,
    string Project,
    string? Task = null,
    string? User = null,
    bool Asc = false
);

public record Update(string Org, string Project, string Task, bool IsRunning);

public record Delete(string Org, string Project, string Task);
