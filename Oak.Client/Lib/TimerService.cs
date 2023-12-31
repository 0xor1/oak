using Timer = Oak.Api.Timer.Timer;

namespace Oak.Client.Lib;

public interface ITimerService
{
    void OnTimersChanged(Action<List<Timer>> fn);
    void TimersChanged(List<Timer> ts);
}

public class TimerService : ITimerService
{
    private Action<List<Timer>>? _fn = null;

    public void OnTimersChanged(Action<List<Timer>> fn) => _fn = fn;

    public void TimersChanged(List<Timer> ts) => _fn?.Invoke(ts);
}
