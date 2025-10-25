using Common.Shared;
using Oak.Api.VItem;
using IApi = Oak.Api.IApi;

namespace Oak.Cli;

public class VItem
{
    private readonly IApi _api;
    private readonly State _state;

    public VItem(IApi api, State state)
    {
        _api = api;
        _state = state;
    }

    /// <summary>
    /// Create a new VItem
    /// </summary>
    /// <param name="type">-ty, type of vitem</param>
    /// <param name="est">-e, estimated remaining value, mins for time or cents for cost</param>
    /// <param name="inc">-i, incurred value, mins for time or cents for cost</param>
    /// <param name="note">-n, note</param>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="task">-t, task id</param>
    /// <param name="ctkn"></param>
    public async Task Create(
        VItemType type,
        ulong? est,
        ulong inc,
        string note,
        string? org = null,
        string? project = null,
        string? task = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        task = _state.GetTask(task);
        var res = await _api.VItem.Create(
            new Create(org, project, task, type, est, inc, note),
            ctkn
        );
        Io.WriteYml(res);
    }

    /// <summary>
    /// Update a new VItem
    /// </summary>
    /// <param name="type">-ty, type of vitem</param>
    /// <param name="id">-i, vitem id</param>
    /// <param name="inc">-in, incurred value, mins for time or cents for cost</param>
    /// <param name="note">-n, note</param>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="task">-t, task id</param>
    /// <param name="ctkn"></param>
    public async Task Update(
        VItemType type,
        string id,
        ulong inc,
        string note,
        string? org = null,
        string? project = null,
        string? task = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        task = _state.GetTask(task);
        var res = await _api.VItem.Update(
            new Update(org, project, task, type, id, inc, note),
            ctkn
        );
        Io.WriteYml(res);
    }

    /// <summary>
    /// Delete a new VItem
    /// </summary>
    /// <param name="type">-ty, type of vitem</param>
    /// <param name="id">-i, vitem id</param>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="task">-t, task id</param>
    /// <param name="ctkn"></param>
    public async Task Delete(
        VItemType type,
        string id,
        string? org = null,
        string? project = null,
        string? task = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        task = _state.GetTask(task);
        var res = await _api.VItem.Delete(new Exact(org, project, task, type, id), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get VItems
    /// </summary>
    /// <param name="type">-ty, type of vitem</param>
    /// <param name="minCreatedOn">-mco, min created on</param>
    /// <param name="maxCreatedOn">-xco, max created on</param>
    /// <param name="createdBy">-cb, created by</param>
    /// <param name="after">-a, after vitem id</param>
    /// <param name="asc">-asc, ascending order</param>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="task">-t, task id</param>
    /// <param name="ctkn"></param>
    public async Task Get(
        VItemType type,
        DateTime? minCreatedOn = null,
        DateTime? maxCreatedOn = null,
        string? createdBy = null,
        string? after = null,
        bool asc = false,
        string? org = null,
        string? project = null,
        string? task = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        task = _state.GetTask(task);
        var res = await _api.VItem.Get(
            new Get(
                org,
                project,
                type,
                task,
                MinMax<DateTime>.Create(minCreatedOn, maxCreatedOn),
                createdBy,
                after,
                asc
            ),
            ctkn
        );
        Io.WriteYml(res);
    }
}
