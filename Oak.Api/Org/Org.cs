using Common.Shared;

namespace Oak.Api.Org;

public interface IOrgApi
{
    public Task<Org> Create(Create arg);
    public Task<Org> GetOne(Exact arg);
    public Task<IReadOnlyList<Org>> Get(Get arg);
    public Task<Org> Update(Update arg);
    public System.Threading.Tasks.Task Delete(Exact arg);
}

public class OrgApi : IOrgApi
{
    private readonly IRpcClient _client;

    public OrgApi(IRpcClient client)
    {
        _client = client;
    }

    public Task<Org> Create(Create arg) => _client.Do(OrgRpcs.Create, arg);

    public Task<Org> GetOne(Exact arg) => _client.Do(OrgRpcs.GetOne, arg);

    public Task<IReadOnlyList<Org>> Get(Get arg) => _client.Do(OrgRpcs.Get, arg);

    public Task<Org> Update(Update arg) => _client.Do(OrgRpcs.Update, arg);

    public System.Threading.Tasks.Task Delete(Exact arg) => _client.Do(OrgRpcs.Delete, arg);
}

public static class OrgRpcs
{
    public static readonly Rpc<Create, Org> Create = new("/org/create");
    public static readonly Rpc<Exact, Org> GetOne = new("/org/get_one");
    public static readonly Rpc<Get, IReadOnlyList<Org>> Get = new("/org/get");
    public static readonly Rpc<Update, Org> Update = new("/org/update");
    public static readonly Rpc<Exact, Nothing> Delete = new("/org/delete");
}

public record Org(string Id, string Name, DateTime CreatedOn, OrgMember.OrgMember? Member);

public record Create(string Name, string OwnerMemberName);

public record Get(OrgOrderBy OrderBy = OrgOrderBy.Name, bool Asc = true);

public record Update(string Id, string Name);

public record Exact(string Id);

public enum OrgOrderBy
{
    Name,
    CreatedOn
}
