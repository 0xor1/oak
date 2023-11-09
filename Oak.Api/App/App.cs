using Common.Shared;

namespace Oak.Api.App;

public interface IAppApi
{
    public Task<Config> GetConfig(CancellationToken ctkn = default);
}

public class AppApi : IAppApi
{
    private readonly IRpcClient _client;

    public AppApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Config> GetConfig(CancellationToken ctkn = default) =>
        _client.Do(AppRpcs.GetConfig, Nothing.Inst, ctkn);
}

public static class AppRpcs
{
    public static readonly Rpc<Nothing, Config> GetConfig = new("/app/get_config");
}

public record Config(bool IsDemo, string? RepoUrl);
