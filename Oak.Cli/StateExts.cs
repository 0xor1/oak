using System.Net;
using Common.Shared;
using Newtonsoft.Json;

namespace Oak.Cli;

public static class StateExts
{
    public static string GetOrg(this State state, string? org = null)
    {
        // set the id using the current state if it's null
        org ??= state.GetString("org") ?? throw new ArgumentNullException(nameof(org));
        // reset state incase the user did pass an id explicitly
        state.SetOrg(org);
        return org;
    }

    public static void SetOrg(this State state, string org) => state.SetString("org", org);

    public static string GetProject(this State state, string? project = null)
    {
        // set the id using the current state if it's null
        project ??= state.GetString("project") ?? throw new ArgumentNullException(nameof(project));
        // reset state incase the user did pass an id explicitly
        state.SetProject(project);
        return project;
    }

    public static void SetProject(this State state, string project) =>
        state.SetString("project", project);
}
