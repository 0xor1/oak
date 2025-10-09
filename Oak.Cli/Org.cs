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
    /// Create a new org
    /// </summary>
    /// <param name="name">-n, org name</param>
    /// <param name="ownerMemberName">-o, your display name within this new org</param>
    public async Task Create(string name, string ownerMemberName, CancellationToken ctkn = default)
    {
        var res = await _api.Org.Create(new Create(name, ownerMemberName), ctkn);
        _state.SetOrg(res.Id);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get one org
    /// </summary>
    /// <param name="id">-i, org id</param>
    public async Task GetOne(string? id = null, CancellationToken ctkn = default)
    {
        id = _state.GetOrg(id);
        var res = await _api.Org.GetOne(new Exact(id), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get orgs
    /// </summary>
    /// <param name="orderBy">-ob, order by</param>
    /// <param name="asc">-a, ascending</param>
    public async Task Get(
        OrgOrderBy orderBy = OrgOrderBy.Name,
        bool asc = true,
        CancellationToken ctkn = default
    )
    {
        var res = await _api.Org.Get(new Get(orderBy, asc), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Update org
    /// </summary>
    /// <param name="name">-n, new name</param>
    /// <param name="id">-i, org id</param>
    public async Task Update(string name, string? id = null, CancellationToken ctkn = default)
    {
        id = _state.GetOrg(id);
        var res = await _api.Org.Update(new Update(id, name), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Delete org
    /// </summary>
    /// <param name="id">-i, org id</param>
    /// <param name="ctkn"></param>
    public async Task Delete(string id, CancellationToken ctkn = default)
    {
        await _api.Org.Delete(new Exact(id), ctkn);
        var stateOrg = _state.GetOrg();
        if (stateOrg == id)
        {
            // if we just deleted the current ctx org clear the ctx value
            _state.SetOrg(null);
        }

        Io.WriteSuccess();
    }
}
