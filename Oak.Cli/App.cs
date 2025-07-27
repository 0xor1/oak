using IApi = Oak.Api.IApi;

namespace Oak.Cli;

public class App
{
    private readonly IApi _api;

    public App(IApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Get the app configuration
    /// </summary>
    public async Task GetConfig() => Io.WriteYml(await _api.App.GetConfig());
}
