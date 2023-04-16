using Common.Shared;

namespace Oak.Api;

public interface IOrgApi
{
    public Rpc<Create, Org> Create { get; }
}
public class OrgApi: IOrgApi
{
    public Rpc<Create, Org> Create { get; } = new Rpc<Create, Org>("/org/create");
}

public record Org(string Id, string Name);
public record Create(string Name);
public record Update(string NewName);