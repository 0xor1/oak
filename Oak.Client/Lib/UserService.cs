using System.Text.RegularExpressions;
using Common.Client;
using Common.Shared;
using Microsoft.AspNetCore.Components;
using Oak.Api;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Task = Oak.Api.Task.Task;

namespace Oak.Client.Lib;

public interface IUserService
{
    Task<OrgMember> Get(string orgId, string userId);
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

    public async Task<OrgMember> Get(string orgId, string userId)
    {
        if (!OrgMembers.ContainsKey(orgId) && !OrgMembers[orgId].ContainsKey(userId))
        {
            await _ss.WaitAsync();
            try
            {
                if (!OrgMembers.ContainsKey(orgId) && !OrgMembers[orgId].ContainsKey(userId))
                {
                    var mo = await _api.OrgMember.GetOne(new(orgId, userId));
                    if (!OrgMembers.ContainsKey(orgId))
                    {
                        OrgMembers[orgId] = new Dictionary<string, OrgMember>();
                    }
                    OrgMembers[orgId][userId] = mo.Item.NotNull();
                }
            }
            finally
            {
                _ss.Release();
            }
        }

        return OrgMembers[orgId][userId];
    }
}
