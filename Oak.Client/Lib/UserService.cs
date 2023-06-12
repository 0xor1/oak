using Common.Shared;
using Oak.Api;
using Oak.Api.OrgMember;

namespace Oak.Client.Lib;

public interface IUserService
{
    Task<OrgMember?> Get(string orgId, string? userId);
    Task<List<OrgMember>> Search(string orgId, bool? isActive, string? nameStartsWith);
}

public class UserService : IUserService
{
    private readonly SemaphoreSlim _ss = new(1, 1);
    private readonly IApi _api;

    private Dictionary<string, Dictionary<string, OrgMember>>? OrgMembers { get; set; } = new();

    public UserService(IApi api)
    {
        _api = api;
    }

    public async Task<OrgMember?> Get(string orgId, string? userId)
    {
        if (userId.IsNullOrWhiteSpace())
        {
            return null;
        }
        await Init(orgId);
        if (!OrgMembers[orgId].ContainsKey(userId))
        {
            await _ss.WaitAsync();
            try
            {
                if (!OrgMembers[orgId].ContainsKey(userId))
                {
                    var mo = await _api.OrgMember.GetOne(new(orgId, userId));
                    OrgMembers[orgId].Add(userId, mo.Item.NotNull());
                }
            }
            finally
            {
                _ss.Release();
            }
        }

        return OrgMembers[orgId][userId];
    }

    public async Task<List<OrgMember>> Search(string orgId, bool? isActive, string? nameStartsWith)
    {
        await Init(orgId);
        var qry = OrgMembers[orgId].Select(x => x.Value).AsQueryable();
        if (isActive.HasValue)
        {
            qry = qry.Where(x => x.IsActive == isActive);
        }

        if (!nameStartsWith.IsNullOrWhiteSpace())
        {
            qry = qry.Where(
                x => x.Name.StartsWith(nameStartsWith, StringComparison.InvariantCultureIgnoreCase)
            );
        }
        return qry.OrderBy(x => x.Name).ToList();
    }

    private async Task Init(string orgId)
    {
        if (!OrgMembers.ContainsKey(orgId))
        {
            await _ss.WaitAsync();
            try
            {
                if (!OrgMembers.ContainsKey(orgId))
                {
                    OrgMembers.Add(orgId, new Dictionary<string, OrgMember>());
                    var active = true;
                    string? after = null;
                    // for now just init the local user cache with every orgMember
                    while (true)
                    {
                        var res = await _api.OrgMember.Get(new(orgId, active, After: after));
                        foreach (var om in res.Set)
                        {
                            OrgMembers[orgId].Add(om.Id, om);
                        }

                        if (res.More)
                        {
                            after = res.Set.Last().Id;
                        }
                        else if (active)
                        {
                            active = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            finally
            {
                _ss.Release();
            }
        }
    }
}
