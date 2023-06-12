using Common.Shared;
using Oak.Api;
using Oak.Api.OrgMember;
using Oak.Api.ProjectMember;

namespace Oak.Client.Lib;

public interface IUserService
{
    Task<OrgMember?> Get(string orgId, string? userId);
    Task<List<OrgMember>> Search(string orgId, bool? isActive, string? nameStartsWith);
    Task<ProjectMember?> Get(string orgId, string projectId, string? userId);
    Task<List<ProjectMember>> Search(
        string orgId,
        string projectId,
        bool? isActive,
        string? nameStartsWith
    );
}

public class UserService : IUserService
{
    private readonly SemaphoreSlim _ss = new(1, 1);
    private readonly IApi _api;

    private Dictionary<string, Dictionary<string, OrgMember>> OrgMembers { get; set; } = new();
    private Dictionary<
        string,
        Dictionary<string, Dictionary<string, ProjectMember>>
    > ProjectMembers { get; set; } = new();

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
                    var m = await _api.OrgMember.GetOne(new(orgId, userId));
                    OrgMembers[orgId].Add(userId, m.Item.NotNull());
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

    public async Task<ProjectMember?> Get(string orgId, string projectId, string? userId)
    {
        if (userId.IsNullOrWhiteSpace())
        {
            return null;
        }
        await Init(orgId, projectId);
        if (!ProjectMembers[orgId][projectId].ContainsKey(userId))
        {
            await _ss.WaitAsync();
            try
            {
                if (!ProjectMembers[orgId][projectId].ContainsKey(userId))
                {
                    var m = await _api.ProjectMember.GetOne(new(orgId, projectId, userId));
                    ProjectMembers[orgId][projectId].Add(userId, m.Item.NotNull());
                }
            }
            finally
            {
                _ss.Release();
            }
        }

        return ProjectMembers[orgId][projectId][userId];
    }

    public async Task<List<ProjectMember>> Search(
        string orgId,
        string projectId,
        bool? isActive,
        string? nameStartsWith
    )
    {
        await Init(orgId, projectId);
        var qry = ProjectMembers[orgId][projectId].Select(x => x.Value).AsQueryable();
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
                    var oms = OrgMembers[orgId];
                    string? after = null;
                    // for now just init the local user cache with every orgMember
                    while (true)
                    {
                        var res = await _api.OrgMember.Get(new(orgId, After: after));
                        foreach (var om in res.Set)
                        {
                            oms.Add(om.Id, om);
                        }

                        if (res.More)
                        {
                            after = res.Set[^1].Id;
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

    private async Task Init(string orgId, string projectId)
    {
        await Init(orgId);
        if (!ProjectMembers.ContainsKey(orgId) || !ProjectMembers[orgId].ContainsKey(projectId))
        {
            await _ss.WaitAsync();
            try
            {
                if (
                    !ProjectMembers.ContainsKey(orgId)
                    || !ProjectMembers[orgId].ContainsKey(projectId)
                )
                {
                    if (!ProjectMembers.ContainsKey(orgId))
                    {
                        ProjectMembers.Add(
                            orgId,
                            new Dictionary<string, Dictionary<string, ProjectMember>>()
                        );
                    }

                    if (!ProjectMembers[orgId].ContainsKey(projectId))
                    {
                        ProjectMembers[orgId].Add(
                            projectId,
                            new Dictionary<string, ProjectMember>()
                        );
                    }

                    var pms = ProjectMembers[orgId][projectId];
                    string? after = null;
                    // for now just init the local user cache with every projectMember
                    while (true)
                    {
                        var res = await _api.ProjectMember.Get(new(orgId, projectId, After: after));
                        foreach (var pm in res.Set)
                        {
                            pms.Add(pm.Id, pm);
                        }

                        if (res.More)
                        {
                            after = res.Set[^1].Id;
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
