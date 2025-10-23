using Oak.Api.ProjectMember;
using IApi = Oak.Api.IApi;

namespace Oak.Cli;

public class ProjectMember
{
    private readonly IApi _api;
    private readonly State _state;

    public ProjectMember(IApi api, State state)
    {
        _api = api;
        _state = state;
    }

    /// <summary>
    /// Add a new project member
    /// </summary>
    /// <param name="id">-i, user id</param>
    /// <param name="role">-r, members role for permissions</param>
    /// <param name="org">-o, the org id</param>
    /// <param name="project">-p, the project id</param>
    /// <param name="ctkn"></param>
    public async Task Add(
        string id,
        ProjectMemberRole role,
        string? org = null,
        string? project = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        var res = await _api.ProjectMember.Add(new Add(org, project, id, role), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get a project member
    /// </summary>
    /// <param name="id">-i, user id</param>
    /// <param name="org">-o, the org id</param>
    /// <param name="project">-p, the project id</param>
    /// <param name="ctkn"></param>
    public async Task GetOne(
        string id,
        string? org = null,
        string? project = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        var res = await _api.ProjectMember.GetOne(new Exact(org, project, id), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get project members
    /// </summary>
    /// <param name="isActive">-ia, is active</param>
    /// <param name="role">-r, role</param>
    /// <param name="nameStartsWith">-nsw, name starts with</param>
    /// <param name="after">-a, after</param>
    /// <param name="orderBy">-ob, order by</param>
    /// <param name="asc">-asc, order ascending</param>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="ctkn"></param>
    public async Task Get(
        bool? isActive,
        ProjectMemberRole? role = null,
        string? nameStartsWith = null,
        string? after = null,
        ProjectMemberOrderBy orderBy = ProjectMemberOrderBy.Role,
        bool asc = true,
        string? org = null,
        string? project = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        var res = await _api.ProjectMember.Get(
            new Get(org, project, isActive, role, nameStartsWith, after, orderBy, asc),
            ctkn
        );
        Io.WriteYml(res);
    }

    /// <summary>
    /// Update a project member
    /// </summary>
    /// <param name="id">-i, member id</param>
    /// <param name="role">-r, role</param>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="ctkn"></param>
    public async Task Update(
        string id,
        ProjectMemberRole role,
        string? org = null,
        string? project = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        var res = await _api.ProjectMember.Update(new Update(org, project, id, role), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Remove a project member
    /// </summary>
    /// <param name="id">-i, member id</param>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="ctkn"></param>
    public async Task Remove(
        string id,
        string? org = null,
        string? project = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        await _api.ProjectMember.Remove(new Exact(org, project, id), ctkn);
        Io.WriteSuccess();
    }
}
