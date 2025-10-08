using Oak.Api.Org;
using IApi = Oak.Api.IApi;

namespace Oak.Cli;

public class Org
{
    private readonly IApi _api;
    private readonly State _state;

    public Org(IApi api, State state)
    {
        _api = api;
        _state = state;
    }

    /// <summary>
    /// Create a new Org
    /// </summary>
    /// <param name="name">-n, org name</param>
    /// <param name="ownerMemberName">-o, your display name within this new org</param>
    public async Task Create(string name, string ownerMemberName, CancellationToken ctkn = default)
    {
        var res = await _api.Org.Create(new Create(name, ownerMemberName), ctkn);
        SetOrg(res.Id);
        Io.WriteYml(res);
    }

    /// <summary>
    /// GetOne Org
    /// </summary>
    /// <param name="org">-o, org id</param>
    public async Task GetOne(string? org = null, CancellationToken ctkn = default)
    {
        org ??= GetOrg() ?? throw new ArgumentNullException(nameof(org));
        var res = await _api.Org.GetOne(new Exact(org), ctkn);
        Io.WriteYml(res);
    }

    private void SetOrg(string org) => _state.SetString("org", org);

    private string? GetOrg() => _state.GetString("org");
}
