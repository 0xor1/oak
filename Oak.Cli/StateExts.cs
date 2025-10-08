using System.Net;
using Common.Shared;
using Newtonsoft.Json;

namespace Oak.Cli;

public static class StateExts
{
    public static string? GetOrg(this State state) => state.GetString("org");

    public static void SetOrg(this State state, string org) => state.SetString("org", org);
}
