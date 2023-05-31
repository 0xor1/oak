using System.Text.RegularExpressions;
using Common.Client;
using Common.Shared;
using Microsoft.AspNetCore.Components;
using Oak.Api;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;

namespace Oak.Client.Lib;

public record UICtx(
    Org? Org = null,
    OrgMember? OrgMember = null,
    Project? Project = null,
    ProjectMember? ProjectMember = null
);

public interface IUICtxService
{
    Task<UICtx> Get();
}

public partial class UICtxService : IUICtxService
{
    private readonly SemaphoreSlim _ss = new(1, 1);
    private readonly IApi _api;
    private readonly IAuthService _auth;
    private readonly NavigationManager _nav;

    private string? OrgId { get; set; }
    private string? ProjectId { get; set; }
    private string? TaskId { get; set; }
    private Org? Org { get; set; }
    private OrgMember? OrgMember { get; set; }
    private Project? Project { get; set; }
    private ProjectMember? ProjectMember { get; set; }

    public UICtxService(IApi api, IAuthService auth, NavigationManager nav)
    {
        _api = api;
        _auth = auth;
        _nav = nav;
    }

    public async Task<UICtx> Get()
    {
        await _ss.WaitAsync();
        try
        {
            var (orgId, projectId) = GetIdsFromUrl();

            // no white space shenanigans
            orgId = orgId.IsNullOrWhiteSpace() ? null : orgId;
            projectId = projectId.IsNullOrWhiteSpace() ? null : projectId;

            Throw.DataIf(
                projectId != null && orgId == null,
                "if taskId is given the projectId must be given and if projectId is given the orgId must be given"
            );

            var orgChanged = OrgId != orgId;
            var projectChanged = TaskId != projectId;

            OrgId = orgId;
            ProjectId = projectId;
            var sesId = (await _auth.GetSession()).Id;

            if (OrgId == null)
            {
                Org = null;
                OrgMember = null;
            }

            if (ProjectId == null)
            {
                Project = null;
                ProjectMember = null;
            }

            if (orgChanged && OrgId != null)
            {
                Org = await _api.Org.GetOne(new(OrgId));
                OrgMember = (await _api.OrgMember.GetOne(new(OrgId, sesId))).Item;
            }

            if (projectChanged && ProjectId != null)
            {
                Project = await _api.Project.GetOne(new(OrgId.NotNull(), ProjectId));
                ProjectMember = (
                    await _api.ProjectMember.GetOne(new(OrgId, ProjectId, sesId))
                ).Item;
            }
        }
        finally
        {
            _ss.Release();
        }

        return new(Org, OrgMember, Project, ProjectMember);
    }

    private (string? orgId, string? projectId) GetIdsFromUrl()
    {
        var uri = _nav.Uri;
        // check for /org/{OrgId}/project/{ProjectId}/task/{TaskId}
        string? orgId = null;
        var orgMatch = OrgIdRx().Match(uri);
        if (orgMatch.Success && orgMatch.Groups.Count == 2)
        {
            orgId = orgMatch.Groups[1].Value;
        }
        string? projectId = null;
        var projectMatch = ProjectIdRx().Match(uri);
        if (projectMatch.Success && projectMatch.Groups.Count == 2)
        {
            projectId = projectMatch.Groups[1].Value;
        }

        return (orgId, projectId);
    }

    [GeneratedRegex(@"/org/([^/\s]+)")]
    private static partial Regex OrgIdRx();

    [GeneratedRegex(@"/project/([^/\s]+)")]
    private static partial Regex ProjectIdRx();
}
