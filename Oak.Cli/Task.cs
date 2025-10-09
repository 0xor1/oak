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
}
