namespace Oak.Client.Lib;

public interface IMainLayout
{
    public Task AuthStateChanged();
}
public interface IMainLayoutService: IMainLayout
{
    public void Init(IMainLayout main);
}

public class MainLayoutService: IMainLayoutService
{
    private IMainLayout? _inst;
    
    public void Init(IMainLayout inst)
    {
        _inst = inst;
    }

    public async Task AuthStateChanged()
    {
        await (_inst?.AuthStateChanged() ?? Task.CompletedTask);
    }
}