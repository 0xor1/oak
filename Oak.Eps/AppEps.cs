using Common.Server;
using Common.Shared;
using Oak.Api.App;
using Config = Oak.Api.App.Config;

namespace Oak.Eps;

internal static class AppEps
{
    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Nothing, Config>(
                AppRpcs.GetConfig,
                async (ctx, _) =>
                {
                    await Task.CompletedTask;
                    var conf = ctx.Get<IConfig>();
                    return new Config(conf.Client.DemoMode, conf.Client.RepoUrl);
                }
            ),
        };
}
