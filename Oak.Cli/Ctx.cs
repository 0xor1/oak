using Common.Shared;
using IApi = Oak.Api.IApi;
using OrgExact = Oak.Api.Org.Exact;
using ProjectExact = Oak.Api.Project.Exact;
using Task = Oak.Api.Task.Task;
using TaskExact = Oak.Api.Task.Exact;

namespace Oak.Cli;

public class Ctx
{
    private readonly IApi _api;
    private readonly State _state;

    public Ctx(IApi api, State state)
    {
        _api = api;
        _state = state;
    }

    /// <summary>
    /// Get current app context
    /// </summary>
    public async System.Threading.Tasks.Task Get(CancellationToken ctkn = default)
    {
        var orgId = _state.GetString("org");
        var projectId = _state.GetString("project");
        var taskId = _state.GetString("task");
        Oak.Api.Org.Org? org = null;
        Oak.Api.Project.Project? project = null;
        Task? task = null;
        List<System.Threading.Tasks.Task> apiCalls = new List<System.Threading.Tasks.Task>();
        if (orgId is not null)
        {
            var orgGetter = async () =>
            {
                org = await _api.Org.GetOne(new OrgExact(orgId), ctkn);
            };
            apiCalls.Add(orgGetter());
            if (projectId is not null)
            {
                var projectGetter = async () =>
                {
                    project = await _api.Project.GetOne(new ProjectExact(orgId, projectId), ctkn);
                };
                apiCalls.Add(projectGetter());
                if (taskId is not null)
                {
                    var taskGetter = async () =>
                    {
                        task = await _api.Task.GetOne(
                            new TaskExact(orgId, projectId, taskId),
                            ctkn
                        );
                    };
                    apiCalls.Add(taskGetter());
                }
            }
            await TaskExt.WhenAll(apiCalls.ToArray());
            if (org is not null)
            {
                Console.WriteLine("org:");
                Console.WriteLine($"  name: {org.Name}");
                Console.WriteLine($"  id: {org.Id}");
                if (project is not null)
                {
                    Console.WriteLine("project:");
                    Console.WriteLine($"  name: {project.Name}");
                    Console.WriteLine($"  id: {project.Id}");

                    if (task is not null)
                    {
                        Console.WriteLine("task:");
                        Console.WriteLine($"  name: {task.Name}");
                        Console.WriteLine($"  id: {task.Id}");
                    }
                }
            }
        }
    }
}
