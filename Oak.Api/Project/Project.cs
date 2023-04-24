using Common.Shared;

namespace Oak.Api.Project;

public interface IProjectApi
{
    public Task<Project> Create(Create arg);
    public Task<IReadOnlyList<Project>> Get(Get arg);
    public Task<Project> Update(Update arg);
}

public class ProjectApi:IProjectApi
{
    private readonly IRpcClient _client;

    public ProjectApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Project> Create(Create arg) => _client.Do(ProjectRpcs.Create, arg);
    public Task<IReadOnlyList<Project>> Get(Get arg) => _client.Do(ProjectRpcs.Get, arg);
    public Task<Project> Update(Update arg) => _client.Do(ProjectRpcs.Update, arg);
}
public static class ProjectRpcs
{
    public static readonly Rpc<Create, Project> Create = new ("/project/create");
    public static readonly Rpc<Get, IReadOnlyList<Project>> Get = new ("/project/get");
    public static readonly Rpc<Update, Project> Update = new ("/project/update");
    public static readonly Rpc<Delete, Project> Delete = new ("/project/delete");
}

public record Project(string Org, string Id, string Name);
public record Create(string Org, string Member, string Name);
public record Get();
public record Update();
public record Delete();