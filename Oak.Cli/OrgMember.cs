using Oak.Api.OrgMember;
using IApi = Oak.Api.IApi;

namespace Oak.Cli;

public class OrgMember
{
    private readonly IApi _api;
    private readonly State _state;

    public OrgMember(IApi api, State state)
    {
        _api = api;
        _state = state;
    }

    /// <summary>
    /// Invite a new org member
    /// </summary>
    /// <param name="email">-e, members email address</param>
    /// <param name="name">-n, members display name within the org</param>
    /// <param name="role">-r, members role for permissions</param>
    /// <param name="org">-o, the org id</param>
    /// <param name="ctkn"></param>
    public async Task Invite(
        string email,
        string name,
        OrgMemberRole role,
        string? org = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        var res = await _api.OrgMember.Invite(new Invite(org, email, name, role), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get an org member
    /// </summary>
    /// <param name="id">-i, members id</param>
    /// <param name="org">-o, the org id</param>
    /// <param name="ctkn"></param>
    public async Task GetOne(string id, string? org = null, CancellationToken ctkn = default)
    {
        org = _state.GetOrg(org);
        var res = await _api.OrgMember.GetOne(new Exact(org, id), ctkn);
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get org members
    /// </summary>
    /// <param name="isActive">-ia, is active</param>
    /// <param name="nameStartsWith">-nsw, name starts with</param>
    /// <param name="role">-r, role</param>
    /// <param name="after">-a, after member id</param>
    /// <param name="orderBy">-ob, order by</param>
    /// <param name="asc">-asc, order ascending</param>
    /// <param name="org">-o, org id</param>
    /// <param name="ctkn"></param>
    public async Task Get(
        bool? isActive = null,
        string? nameStartsWith = null,
        OrgMemberRole? role = null,
        string? after = null,
        OrgMemberOrderBy? orderBy = OrgMemberOrderBy.Role,
        bool asc = true,
        string? org = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        var res = await _api.OrgMember.Get(
            new Get(org, isActive, nameStartsWith, role, after, orderBy, asc),
            ctkn
        );
        Io.WriteYml(res);
    }

    /// <summary>
    /// Get org members
    /// </summary>
    /// <param name="isActive">-ia, is active</param>
    /// <param name="name">-n, name</param>
    /// <param name="role">-r, role</param>
    /// <param name="org">-o, org id</param>
    /// <param name="ctkn"></param>
    public async Task Update(
        string id,
        bool? isActive = null,
        string? name = null,
        OrgMemberRole? role = null,
        string? org = null,
        CancellationToken ctkn = default
    )
    {
        org = _state.GetOrg(org);
        var res = await _api.OrgMember.Update(new Update(org, id, isActive, name, role), ctkn);
        Io.WriteYml(res);
    }
}
