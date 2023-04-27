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
    public static async Task<bool> IsActiveOrgMember(
        OakDb db,
        Session ses,
        string org
    )
        => await OrgRole(db, ses, org) != null;
    
    public static async Task<OrgMemberRole?> OrgRole(
        OakDb db,
        Session ses,
        string org
    )
    {
        var orgMem = await db.OrgMembers.SingleOrDefaultAsync(
            x => x.Org == org && x.IsActive && x.Id == ses.Id
        );
        return orgMem?.Role;
    }
    public static async Task<bool> HasOrgAccess(
        OakDb db,
        Session ses,
        string org,
        OrgMemberRole role
    )
    {
        var memRole = await OrgRole(db, ses, org);
        return memRole != null && memRole <= role;
    }
    public static async Task MustHaveOrgAccess(
        IRpcCtx ctx,
        OakDb db,
        Session ses,
        string org,
        OrgMemberRole role
    )
        => ctx.ErrorIf(!await HasOrgAccess(db, ses, org, role), S.InsufficientPermission, null, HttpStatusCode.Forbidden);

    
    public static async Task<ProjectMemberRole?> ProjectRole(
        OakDb db,
        Session ses,
        string org,
        string project
    )
    {
        var projMem = await db.ProjectMembers.SingleOrDefaultAsync(
            x => x.Org == org && x.Project == project && x.Id == ses.Id
        );
        return projMem?.Role;
    }
    
    public static async Task<bool> HasProjectAccess(
        OakDb db,
        Session ses,
        string org,
        string project,
        ProjectMemberRole role
    )
    {
        var isPublicList = await db.Projects.Where(x => x.Org == org && x.Id == project).Select(x => x.IsPublic).ToListAsync();
        if (!isPublicList.Any())
        {
            // there is no such project
            return false;
        }

        var isPublic = isPublicList.Single();
        if (isPublic && role == ProjectMemberRole.Reader)
        {
            // project is public and only asking for read access
            return true;
        }
        
        // at this point explicit access permission is required
        // 1. ensure active org member
        var orgRole = await OrgRole(db, ses, org);
        if (orgRole == null)
        {
            return false;
        }

        if (orgRole is (OrgMemberRole.Owner or OrgMemberRole.Admin))
        {
            // org owners and admins have full access to all permissions on all projects
            return true;
        }

        var projRole = await ProjectRole(db, ses, org, project);
        if (projRole != null && projRole <= role)
        {
            // user has sufficient project permission
            return true;
        }

        // they dont have specific project permission but their org role
        // may be sufficient so fall back to check that
        return 
            (
                role == ProjectMemberRole.Writer && orgRole <= OrgMemberRole.WriteAllProjects
            )
            || (
                role == ProjectMemberRole.Reader && orgRole <= OrgMemberRole.ReadAllProjects
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
        => ctx.ErrorIf(!await HasProjectAccess(db, ses, org, project, role), S.InsufficientPermission, null, HttpStatusCode.Forbidden);

}
