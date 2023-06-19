using Common.Client;
using Common.Shared;
using Oak.Api;
using Oak.Api.Comment;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Api.VItem;
using Task = Oak.Api.Task.Task;

namespace Oak.Client.Lib;

public class UiCtx
{
    private readonly SemaphoreSlim _ss = new(1, 1);
    private readonly IApi _api;
    private readonly IAuthService _auth;

    public UIDisplay Display { get; init; } = new();

    public string? OrgId { get; set; }
    public string? ProjectId { get; set; }
    public string? TaskId { get; set; }
    public Org? Org { get; set; }
    public OrgMember? OrgMember { get; set; }
    public Project? Project { get; set; }
    public ProjectMember? ProjectMember { get; set; }
    public Task? Task { get; set; }

    public bool HasOrgOwnerPerm => OrgMember is { Role: OrgMemberRole.Owner, IsActive: true };

    public bool HasOrgAdminPerm => OrgMember is { Role: <= OrgMemberRole.Admin, IsActive: true };

    public bool HasProjectAdminPerm =>
        OrgMember?.IsActive == true
        && (
            OrgMember.Role <= OrgMemberRole.Admin || ProjectMember?.Role <= ProjectMemberRole.Admin
        );

    public bool HasProjectWritePerm =>
        OrgMember?.IsActive == true
        && (
            OrgMember.Role <= OrgMemberRole.WriteAllProjects
            || ProjectMember?.Role <= ProjectMemberRole.Writer
        );

    public bool CanDeleteTask(Task t) =>
        Project.NotNull().Id != t.Id
        && t.DescN <= 20
        && (
            (
                HasProjectWritePerm
                && t.CreatedBy == ProjectMember?.Id
                && t.DescN == 0
                && t.CreatedOn.Add(TimeSpan.FromHours(1)) > DateTime.UtcNow
            ) || HasProjectAdminPerm
        );

    public bool CanDeleteVItemOrFile(ICreatable i) =>
        HasProjectAdminPerm
        || (
            HasProjectWritePerm
            && i.CreatedBy == OrgMember?.Id
            && i.CreatedOn.Add(TimeSpan.FromHours(1)) > DateTime.UtcNow
        );

    public bool CanDeleteOorUpdateComment(Comment c) =>
        HasProjectAdminPerm || (HasProjectWritePerm && c.CreatedBy == OrgMember?.Id);

    public UiCtx(IApi api, IAuthService auth)
    {
        _api = api;
        _auth = auth;
    }

    public System.Threading.Tasks.Task Set(Org org) => Set(org, null, null);

    public System.Threading.Tasks.Task Set(Project project) => Set(null, project, null);

    public System.Threading.Tasks.Task Set(Task task) => Set(null, null, task);

    private async System.Threading.Tasks.Task Set(Org? org, Project? project, Task? task)
    {
        await _ss.WaitAsync();
        try
        {
            // no white space shenanigans
            var orgId = org?.Id ?? project?.Org ?? task?.Org;
            var projectId = project?.Id ?? task?.Project;
            var taskId = task?.Id;

            var orgChanged = OrgId != orgId;
            var projectChanged = ProjectId != projectId;

            OrgId = orgId;
            ProjectId = projectId;
            TaskId = taskId;
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

            if (TaskId == null)
            {
                Task = null;
            }

            if (orgChanged && OrgId != null)
            {
                Org = org ?? await _api.Org.GetOne(new(OrgId));
                OrgMember = (await _api.OrgMember.GetOne(new(OrgId, sesId))).Item;
            }

            if (projectChanged && ProjectId != null)
            {
                Project = project ?? await _api.Project.GetOne(new(OrgId.NotNull(), ProjectId));
                ProjectMember = (
                    await _api.ProjectMember.GetOne(new(OrgId, ProjectId, sesId))
                ).Item;
            }

            Task = task;
        }
        finally
        {
            _ss.Release();
        }
    }

    // private (string? orgId, string? projectId, string? taskId) GetIdsFromUrl()
    // {
    //     var uri = _nav.Uri;
    //     // check for /org/{OrgId}/project/{ProjectId}/task/{TaskId}
    //     string? orgId = null;
    //     var orgMatch = OrgIdRx().Match(uri);
    //     if (orgMatch.Success && orgMatch.Groups.Count == 2)
    //     {
    //         orgId = orgMatch.Groups[1].Value;
    //     }
    //     string? projectId = null;
    //     var projectMatch = ProjectIdRx().Match(uri);
    //     if (projectMatch.Success && projectMatch.Groups.Count == 2)
    //     {
    //         projectId = projectMatch.Groups[1].Value;
    //     }
    //     string? taskId = null;
    //     var taskMatch = TaskIdRx().Match(uri);
    //     if (taskMatch.Success && taskMatch.Groups.Count == 2)
    //     {
    //         taskId = taskMatch.Groups[1].Value;
    //     }
    //
    //     return (orgId, projectId, taskId);
    // }
    //
    // [GeneratedRegex(@"/org/([^/\s]+)")]
    // private static partial Regex OrgIdRx();
    //
    // [GeneratedRegex(@"/project/([^/\s]+)")]
    // private static partial Regex ProjectIdRx();
    //
    // [GeneratedRegex(@"/task/([^/\s]+)")]
    // private static partial Regex TaskIdRx();
}
