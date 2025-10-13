using Common.Shared;
using Oak.Api.Task;
using IApi = Oak.Api.IApi;

namespace Oak.Cli;

public class TaskCli
{
    private readonly IApi _api;
    private readonly State _state;

    public TaskCli(IApi api, State state)
    {
        _api = api;
        _state = state;
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    /// <param name="prevSib">-ps, previous sibling task id</param>
    /// <param name="name">-n, name</param>
    /// <param name="description">-d, description</param>
    /// <param name="isParallel">-ip, is parallel</param>
    /// <param name="user">-u, user</param>
    /// <param name="timeEst">-te, time estiamte</param>
    /// <param name="costEst">-ce, cost estimate</param>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="id">-i, parent task id</param>
    public async System.Threading.Tasks.Task Create(
        string? prevSib,
        string name,
        string description = "",
        bool isParallel = false,
        string? user = null,
        ulong timeEst = 0,
        ulong costEst = 0,
        string? org = null,
        string? project = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        id = _state.GetTask(id);
        var res = await _api.Task.Create(
            new Create(
                org,
                project,
                id,
                prevSib,
                name,
                description,
                isParallel,
                user,
                timeEst,
                costEst
            ),
            ctkn
        );
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get a task
    /// </summary>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="id">-i, task id</param>
    public async System.Threading.Tasks.Task GetOne(
        string? org = null,
        string? project = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        id = _state.GetTask(id);
        var res = await _api.Task.GetOne(new Exact(org, project, id), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get the ancestor tasks
    /// </summary>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="id">-i, task id</param>
    public async System.Threading.Tasks.Task GetAncestors(
        string? org = null,
        string? project = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        id = _state.GetTask(id);
        var res = await _api.Task.GetAncestors(new Exact(org, project, id), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get the child tasks
    /// </summary>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="id">-i, task id</param>
    /// <param name="after">-a, after task id</param>
    public async System.Threading.Tasks.Task GetChildren(
        string? org = null,
        string? project = null,
        string? id = null,
        string? after = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        id = _state.GetTask(id);
        var res = await _api.Task.GetChildren(new GetChildren(org, project, id, after), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get the initial task view used in the web app
    /// </summary>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="id">-i, task id</param>
    public async System.Threading.Tasks.Task GetInitView(
        string? org = null,
        string? project = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        id = _state.GetTask(id);
        var res = await _api.Task.GetInitView(new Exact(org, project, id), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get all the descendants of the task
    /// </summary>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="id">-i, task id</param>
    public async System.Threading.Tasks.Task GetAllDescendants(
        string? org = null,
        string? project = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        id = _state.GetTask(id);
        var res = await _api.Task.GetAllDescendants(new Exact(org, project, id), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Update a task
    /// </summary>
    /// <param name="parent">-pr, parent task id</param>
    /// <param name="prevSib">-ps, previous sibling task id</param>
    /// <param name="name">-n, name</param>
    /// <param name="description">-d, description</param>
    /// <param name="isParallel">-ip, is parallel</param>
    /// <param name="user">-u, user</param>
    /// <param name="timeEst">-te, time estiamte</param>
    /// <param name="costEst">-ce, cost estimate</param>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="id">-i, task id</param>
    public async System.Threading.Tasks.Task Update(
        string? parent = null,
        [NSetStringParser] NSet<string>? prevSib = null,
        string? name = null,
        string? description = null,
        bool? isParallel = null,
        [NSetStringParser] NSet<string>? user = null,
        ulong? timeEst = null,
        ulong? costEst = null,
        string? org = null,
        string? project = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        id = _state.GetTask(id);
        var res = await _api.Task.Update(
            new Update(
                org,
                project,
                id,
                parent,
                prevSib,
                name,
                description,
                isParallel,
                user,
                timeEst,
                costEst
            ),
            ctkn
        );
        Io.WriteYml(res);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    /// <param name="org">-o, org id</param>
    /// <param name="project">-p, project id</param>
    /// <param name="id">-i, task id</param>
    public async System.Threading.Tasks.Task Delete(
        string? org = null,
        string? project = null,
        string? id = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        project = _state.GetProject(project);
        id = _state.GetTask(id);
        var res = await _api.Task.Delete(new Exact(org, project, id), ctkn);
        Io.WriteYml(res);
    }
}
