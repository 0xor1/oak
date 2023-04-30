using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Api.ProjectMember;
using Oak.Db;
using Task = System.Threading.Tasks.Task;

namespace Oak.Eps;

internal static class EpsUtil
{
    public static async Task<bool> IsActiveOrgMember(OakDb db, Session ses, string org) =>
        await OrgRole(db, ses, org) != null;

    public static async Task MustBeActiveOrgMember(
        IRpcCtx ctx,
        OakDb db,
        Session ses,
        string org
    ) => ctx.InsufficientPermissionsIf(!await IsActiveOrgMember(db, ses, org));

    public static async Task<OrgMemberRole?> OrgRole(OakDb db, Session ses, string org)
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
    ) => ctx.InsufficientPermissionsIf(!await HasOrgAccess(db, ses, org, role));

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
        IRpcCtx ctx,
        OakDb db,
        Session ses,
        string org,
        string project,
        ProjectMemberRole role
    )
    {
        var p = await db.Projects.SingleOrDefaultAsync(x => x.Org == org && x.Id == project);

        ctx.NotFoundIf(p == null);
        p.NotNull();

        var isPublic = p.IsPublic;
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

        if (
            orgRole is (OrgMemberRole.Owner or OrgMemberRole.Admin)
            || (orgRole is OrgMemberRole.WriteAllProjects && role >= ProjectMemberRole.Writer)
            || (orgRole is OrgMemberRole.ReadAllProjects && role >= ProjectMemberRole.Reader)
        )
        {
            // org owners and admins have full access to all permissions on all projects
            // and if a user has write all or read all access and that is all that is required, return true now.
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
        return (role == ProjectMemberRole.Writer && orgRole <= OrgMemberRole.WriteAllProjects)
            || (role == ProjectMemberRole.Reader && orgRole <= OrgMemberRole.ReadAllProjects);
    }

    public static async Task MustHaveProjectAccess(
        IRpcCtx ctx,
        OakDb db,
        Session ses,
        string org,
        string project,
        ProjectMemberRole role
    ) => ctx.InsufficientPermissionsIf(!await HasProjectAccess(ctx, db, ses, org, project, role));
}
