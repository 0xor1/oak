using System.Net;
using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Db;
using S = Oak.I18n.S;
using Task = System.Threading.Tasks.Task;

namespace Oak.Eps;

internal static class EpsUtil
{
    public static async Task MustHaveOrgAccess(
        IRpcCtx ctx,
        OakDb db,
        Session ses,
        string org,
        OrgMemberRole role
    )
    {
        var x = await db.OrgMembers.SingleOrDefaultAsync(
            x => x.Org == org && x.IsActive && x.Member == ses.Id
        );
        ctx.ErrorIf(x == null, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
        ctx.ErrorIf(
            x.NotNull().Role > role,
            S.InsufficientPermission,
            null,
            HttpStatusCode.Forbidden
        );
    }

    public static async Task MustHaveProjectAccess(
        IRpcCtx ctx,
        OakDb db,
        Session ses,
        string org,
        string project,
        ProjectMemberRole role
    )
    {
        // 1. ensure active org member
        var orgMem = await db.OrgMembers.SingleOrDefaultAsync(
            x => x.Org == org && x.IsActive && x.Member == ses.Id
        );
        ctx.ErrorIf(orgMem == null, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
        orgMem.NotNull();

        var projMem = await db.ProjectMembers.SingleOrDefaultAsync(
            x => x.Org == org && x.Project == project && x.Member == ses.Id
        );
        if (projMem != null && projMem.Role <= role)
        {
            // user has sufficient project permission
            return;
        }

        // they dont have specific project permission but their org role
        // may be sufficient so fall back to check that
        ctx.ErrorIf(
            (role == ProjectMemberRole.Admin && orgMem.Role > OrgMemberRole.Admin)
                || (
                    role == ProjectMemberRole.Writer && orgMem.Role > OrgMemberRole.WriteAllProjects
                )
                || (
                    role == ProjectMemberRole.Reader && orgMem.Role > OrgMemberRole.ReadAllProjects
                ),
            S.InsufficientPermission,
            null,
            HttpStatusCode.Forbidden
        );
    }
}
