using Common.Server;
using Common.Server.Auth;
using Oak.Db;

namespace Oak.Eps;

public static class OakEps
{
    private static IReadOnlyList<IRpcEndpoint>? _eps;
    public static IReadOnlyList<IRpcEndpoint> Eps => _eps ??= AuthEps<OakDb>.Eps;
}