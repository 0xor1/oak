using System.Net;
using Common.Server;
using Common.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Db;
using Org = Oak.Api.Org.Org;
using Get = Oak.Api.Org.Get;
using S = Oak.I18n.S;
using Update = Oak.Api.Org.Update;
using Task = System.Threading.Tasks.Task;

namespace Oak.Eps;

internal static class EpsUtil
{
    public static async Task MustHaveOrgAccess(HttpContext ctx, string org, OrgMemberRole role)
    {
        await MustHaveOrgAccess(ctx, ctx.Get<OakDb>(), ctx.GetAuthedSession(), org, role);
    }
    public static async Task MustHaveOrgAccess(HttpContext ctx, OakDb db, Session ses, string org, OrgMemberRole role)
    {
        var x = await db.OrgMembers.SingleOrDefaultAsync(x => x.Org == org && x.IsActive && x.Member == ses.Id);
        ctx.ErrorIf(x == null, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
        ctx.ErrorIf(x.NotNull().Role > role, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
    }

    public static async Task MustHaveProjectAccess(HttpContext ctx, string org, string project, ProjectMemberRole role)
    {
        await MustHaveProjectAccess(ctx, ctx.Get<OakDb>(), ctx.GetAuthedSession(), org, project, role);
    }
    
    public static async Task MustHaveProjectAccess(HttpContext ctx, OakDb db, Session ses, string org, string project, ProjectMemberRole role)
    {
        var projMem =
            await db.ProjectMembers.SingleOrDefaultAsync(
                x => x.Org == org && x.Project == project && x.Member == ses.Id);
        if (projMem != null && projMem.Role <= role)
        {
            // user has sufficient project permission
            return;
        }
        // they dont have specific project permission but their org role
        // may be sufficient so fall back to check that 
        var orgMem = await db.OrgMembers.SingleOrDefaultAsync(x => x.Org == org && x.IsActive && x.Member == ses.Id);
        ctx.ErrorIf(orgMem == null, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
        orgMem.NotNull();
        ctx.ErrorIf((role == ProjectMemberRole.Admin && orgMem.Role > OrgMemberRole.Admin) || (role == ProjectMemberRole.Writer && orgMem.Role > OrgMemberRole.WriteAllProjects) || (role == ProjectMemberRole.Reader && orgMem.Role > OrgMemberRole.ReadAllProjects), S.InsufficientPermission, null, HttpStatusCode.Forbidden);
    }
}