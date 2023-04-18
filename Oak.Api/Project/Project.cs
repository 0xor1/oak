using Common.Shared;

namespace Oak.Api.Project;

public interface IProjectApi
{
    private static IProjectApi? _inst;
    static IProjectApi Init() => _inst ??= new ProjectApi();
    
    public Rpc<Create, Project> Create { get; }
    public Rpc<Get, IReadOnlyList<Project>> Get { get; }
    public Rpc<Update, Project> Update { get; }
}
public class ProjectApi: IProjectApi
{
    public Rpc<Create, Project> Create { get; } = new ("/project/create");
    public Rpc<Get, IReadOnlyList<Project>> Get { get; } = new ("/project/get");
    public Rpc<Update, Project> Update { get; } = new ("/project/update");
    public Rpc<Delete, Project> Delete { get; } = new ("/project/delete");
}

public record Project(string Org, string Id, string Name);
public record Create(string Org, string Member, string Name);
public record Get();
public record Update();
public record Delete();