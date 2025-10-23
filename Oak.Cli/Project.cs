using Common.Shared;
using Oak.Api.Project;
using IApi = Oak.Api.IApi;

namespace Oak.Cli;

public class Project
{
    private readonly IApi _api;
    private readonly State _state;

    public Project(IApi api, State state)
    {
        _api = api;
        _state = state;
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    /// <param name="name">-n, name</param>
    /// <param name="isPublic">-ip, is the project public</param>
    /// <param name="currencySymbol">-cs, currency symbol</param>
    /// <param name="currencyCode">-cc, currency code</param>
    /// <param name="hoursPerDay">-hpd, hours per day</param>
    /// <param name="daysPerWeek">-dpw, days per week</param>
    /// <param name="startOn">-so, start on</param>
    /// <param name="endOn">-eo, end on</param>
    /// <param name="fileLimit">-fl, total upload limit in bytes</param>
    /// <param name="org">-o, org id</param>
    public async Task Create(
        string name,
        bool isPublic = false,
        string currencySymbol = "$",
        string currencyCode = "USD",
        uint hoursPerDay = 8,
        uint daysPerWeek = 5,
        DateTime? startOn = null,
        DateTime? endOn = null,
        ulong fileLimit = 0,
        string? org = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        var res = await _api.Project.Create(
            new Create(
                org,
                isPublic,
                name,
                currencySymbol,
                currencyCode,
                hoursPerDay,
                daysPerWeek,
                startOn,
                endOn,
                fileLimit
            ),
            ctkn
        );
        _state.SetProject(res.Id);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get one project
    /// </summary>
    /// <param name="org">-o, org id</param>
    /// <param name="id">-i, project id</param>
    public async Task GetOne(
        string? org = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        id = _state.GetProject(id);
        var res = await _api.Project.GetOne(new Exact(org, id), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get projects
    /// </summary>
    /// <param name="isPublic">-ip, is public</param>
    /// <param name="nameStartsWith">-nsw, name starts with</param>
    /// <param name="minCreatedOn">-mco, min created on</param>
    /// <param name="maxCreatedOn">-xco, max created on</param>
    /// <param name="minStartOn">-mso, min started on</param>
    /// <param name="maxStartOn">-xso, max started on</param>
    /// <param name="minEndOn">-meo, min end on</param>
    /// <param name="maxEndOn">-xeo, max end on</param>
    /// <param name="isArchived">-ia, is archived</param>
    /// <param name="after">-a, after project id</param>
    /// <param name="orderBy">-ob, order by</param>
    /// <param name="asc">-asc, order ascending</param>
    /// <param name="org">-o, org id</param>
    public async Task Get(
        bool? isPublic = null,
        string? nameStartsWith = null,
        DateTime? minCreatedOn = null,
        DateTime? maxCreatedOn = null,
        DateTime? minStartOn = null,
        DateTime? maxStartOn = null,
        DateTime? minEndOn = null,
        DateTime? maxEndOn = null,
        bool isArchived = false,
        string? after = null,
        ProjectOrderBy orderBy = ProjectOrderBy.Name,
        bool asc = true,
        string? org = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        var res = await _api.Project.Get(
            new Get(
                org,
                isPublic,
                nameStartsWith,
                MinMax<DateTime>.Create(minCreatedOn, maxCreatedOn),
                MinMax<DateTime>.Create(minStartOn, maxStartOn),
                MinMax<DateTime>.Create(minEndOn, maxEndOn),
                isArchived,
                after,
                orderBy,
                asc
            ),
            ctkn
        );
        Io.WriteYml(res);
    }

    /// <summary>
    /// Update project
    /// </summary>
    /// <param name="name">-n, name</param>
    /// <param name="isPublic">-ip, is the project public</param>
    /// <param name="currencySymbol">-cs, currency symbol</param>
    /// <param name="currencyCode">-cc, currency code</param>
    /// <param name="hoursPerDay">-hpd, hours per day</param>
    /// <param name="daysPerWeek">-dpw, days per week</param>
    /// <param name="startOn">-so, start on</param>
    /// <param name="endOn">-eo, end on</param>
    /// <param name="fileLimit">-fl, total upload limit in bytes</param>
    /// <param name="org">-o, org id</param>
    /// <param name="id">-i, project id</param>
    public async Task Update(
        string? name = null,
        bool? isPublic = null,
        string? currencySymbol = null,
        string? currencyCode = null,
        uint? hoursPerDay = null,
        uint? daysPerWeek = null,
        DateTime? startOn = null,
        DateTime? endOn = null,
        ulong? fileLimit = null,
        string? org = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        id = _state.GetProject(id);
        var res = await _api.Project.Update(
            new Update(
                org,
                id,
                isPublic,
                name,
                currencySymbol,
                currencyCode,
                hoursPerDay,
                daysPerWeek,
                startOn,
                endOn,
                fileLimit
            ),
            ctkn
        );
        _state.SetProject(res.Id);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Delete project
    /// </summary>
    /// <param name="id">-i, project id</param>
    /// <param name="org">-o, org id</param>
    public async Task Delete(string id, string? org = null, CancellationToken ctkn = default)
    {
        org = _state.GetOrg(org);
        await _api.Project.Delete(new Exact(org, id), ctkn);
        var stateProject = _state.GetProject();
        if (stateProject == id)
        {
            // if we just deleted the current ctx project, clear the ctx value
            _state.SetProject(null);
        }
        Io.WriteSuccess();
    }

    /// <summary>
    /// Get project activities
    /// </summary>
    /// <param name="org">-o, org id</param>
    /// <param name="id">-i, project id</param>
    public async Task GetActivities(
        string? org = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        id = _state.GetProject(id);
        var res = await _api.Project.GetActivities(new GetActivities(org, id), ctkn);
        Io.WriteYml(res);
    }
}
