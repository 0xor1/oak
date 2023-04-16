using Common.Shared;

namespace Oak.Api.Org;

public interface IOrgApi
{
    private static IOrgApi? _inst;
    static IOrgApi Init() => _inst ??= new OrgApi();
    
    public Rpc<Create, Org> Create { get; }
}
public class OrgApi: IOrgApi
{
    public Rpc<Create, Org> Create { get; } = new ("/org/create");
    public Rpc<Get, Org> Get { get; } = new ("/org/get");
    public Rpc<Update, Org> Update { get; } = new ("/org/update");
    public Rpc<Delete, Nothing> Delete { get; } = new ("/org/delete");
}

public record Org(string Id, string Name);
public record Create(string Name, string OwnerMemberName);
public record Get(string Name);
public record Update(string NewName);
public record Delete(string Id);