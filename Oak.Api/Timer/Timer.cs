using Common.Shared;

namespace Oak.Api.Timer;

public interface ITimerApi
{
    public Task<Timer> Create(Create arg, CancellationToken ctkn = default);
    public Task<SetRes<Timer>> Get(Get arg, CancellationToken ctkn = default);
    public Task<Timer> Update(Update arg, CancellationToken ctkn = default);
    public System.Threading.Tasks.Task Delete(Delete arg, CancellationToken ctkn = default);
}

public class TimerApi : ITimerApi
{
    private readonly IRpcClient _client;

    public TimerApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Timer> Create(Create arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Create, arg, ctkn);

    public Task<SetRes<Timer>> Get(Get arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Get, arg, ctkn);

    public Task<Timer> Update(Update arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Update, arg, ctkn);

    public System.Threading.Tasks.Task Delete(Delete arg, CancellationToken ctkn = default) =>
        _client.Do(TimerRpcs.Delete, arg, ctkn);
}

public static class TimerRpcs
{
    public static readonly Rpc<Create, Timer> Create = new("/timer/create");
    public static readonly Rpc<Get, SetRes<Timer>> Get = new("/timer/get");
    public static readonly Rpc<Update, Timer> Update = new("/timer/update");
    public static readonly Rpc<Delete, Nothing> Delete = new("/timer/delete");
}

public record Timer(
    string Org,
    string Project,
    string Task,
    string User,
    DateTime CreatedOn,
    string Note,
    ulong Inc,
    DateTime LastStartedOn,
    bool IsRunning
);

public record Create(string Org, string Project, string Task, string User);

public record Get(string Org, string Project, string? Task, string? User, bool Asc = true);

public record Update(
    string Org,
    string Project,
    string Task,
    string User,
    string? Note,
    bool? IsRunning
);

public record Delete(string Org, string Project, string Task, string User, bool Log);
