using System.Net;
using Common.Shared;
using Newtonsoft.Json;

namespace Oak.Cli;

public static class StateExts
{
    public static string GetOrg(this State state, string? org = null)
    {
        // set the org using the current state if it's null
        org ??= state.GetString("org") ?? throw new ArgumentNullException(nameof(org));
        // reset state incase the user did pass a org id explicitly
        state.SetOrg(org);
        return org;
    }

    public static void SetOrg(this State state, string org) => state.SetString("org", org);
}
