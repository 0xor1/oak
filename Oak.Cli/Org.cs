using Oak.Api.Org;
using IApi = Oak.Api.IApi;

namespace Oak.Cli;

public class Org
{
    private readonly IApi _api;

    public Org(IApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Create a new Org
    /// </summary>
    /// <param name="name">-n, org name</param>
    /// <param name="ownerMemberName">-o, your display name within this new org</param>
    public async Task Create(string name, string ownerMemberName) =>
        Io.WriteYml(await _api.Org.Create(new Create(name, ownerMemberName)));
}
