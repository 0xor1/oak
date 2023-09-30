using System.Text.RegularExpressions;
using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api;
using Oak.Api.OrgMember;
using Oak.Api.ProjectMember;
using Oak.Db;
using S = Oak.I18n.S;
using Task = System.Threading.Tasks.Task;

namespace Oak.Eps;

public static class EpsUtil
{
    public static void ValidStr(
        IRpcCtx ctx,
        string val,
        int min,
        int max,
        string name,
        List<Regex>? regexes = null
    )
    {
        var m = new
        {
            Name = name,
            Min = min,
            Max = max,
            Regexes = regexes?.Select(x => x.ToString()).ToList()
        };
        ctx.BadRequestIf(val.Length < min || val.Length > max, S.StringValidation, m);
        if (regexes != null)
        {
            foreach (var r in regexes)
            {
                ctx.BadRequestIf(!r.IsMatch(val), S.StringValidation, m);
            }
        }
    }

    public static async Task<OrgMemberRole?> OrgRole(IRpcCtx ctx, OakDb db, string user, string org)
    {
        var orgMem = await db.OrgMembers.SingleOrDefaultAsync(
            x => x.Org == org && x.IsActive && x.Id == user,
            ctx.Ctkn
        );
        return orgMem?.Role;
    }

    public static async Task<bool> HasOrgAccess(
        IRpcCtx ctx,
        OakDb db,
        string user,
        string org,
        OrgMemberRole role
    )
    {
        var memRole = await OrgRole(ctx, db, user, org);
        return memRole != null && memRole <= role;
    }

    public static async Task MustHaveOrgAccess(
        IRpcCtx ctx,
        OakDb db,
        string user,
        string org,
        OrgMemberRole role
    ) => ctx.InsufficientPermissionsIf(!await HasOrgAccess(ctx, db, user, org, role));

    public static async Task<ProjectMemberRole?> ProjectRole(
        IRpcCtx ctx,
        OakDb db,
        string user,
        string org,
        string project
    )
    {
        var projMem = await db.ProjectMembers.SingleOrDefaultAsync(
            x => x.Org == org && x.Project == project && x.Id == user,
            ctx.Ctkn
        );
        return projMem?.Role;
    }

    public static async Task<bool> HasProjectAccess(
        IRpcCtx ctx,
        OakDb db,
        string user,
        string org,
        string project,
        ProjectMemberRole role
    )
    {
        var p = await db.Projects.SingleOrDefaultAsync(
            x => x.Org == org && x.Id == project,
            ctx.Ctkn
        );

        ctx.NotFoundIf(p == null, model: new { Name = "Project" });
        p.NotNull();

        var isPublic = p.IsPublic;
        if (isPublic && role == ProjectMemberRole.Reader)
        {
            // project is public and only asking for read access
            return true;
        }

        // at this point explicit access permission is required
        // 1. ensure active org member
        var orgRole = await OrgRole(ctx, db, user, org);
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

        var projRole = await ProjectRole(ctx, db, user, org, project);
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
        string user,
        string org,
        string project,
        ProjectMemberRole role
    ) => ctx.InsufficientPermissionsIf(!await HasProjectAccess(ctx, db, user, org, project, role));

    public static async Task LogActivity(
        IRpcCtx ctx,
        OakDb db,
        Session ses,
        string org,
        string project,
        string task,
        string item,
        ActivityItemType type,
        ActivityAction action,
        string? itemName,
        object? extraInfo,
        List<string>? ancestors
    )
    {
        Throw.OpIf(
            type == ActivityItemType.Task && task != item,
            "item type is task but task id and item id are not the same"
        );
        string? exInStr = null;
        if (extraInfo != null)
        {
            exInStr = Json.From(extraInfo);
            Throw.DataIf(exInStr.Length > 10000, "extraInfo string is too long");
        }

        var taskDeleted = type == ActivityItemType.Task && action == ActivityAction.Delete;
        var itemDeleted = action == ActivityAction.Delete;
        var occuredOn = DateTimeExt.UtcNowMilli();
        string? taskName = null;
        if (type != ActivityItemType.Task)
        {
            taskName = (
                await db.Tasks.SingleOrDefaultAsync(
                    x => x.Org == org && x.Project == project && x.Id == task,
                    ctx.Ctkn
                )
            )?.Name;
        }
        taskName ??= itemName;

        var a = new Activity()
        {
            Org = org,
            Project = project,
            Task = task,
            OccurredOn = occuredOn,
            User = ses.Id,
            Item = item,
            ItemType = type,
            TaskDeleted = taskDeleted,
            ItemDeleted = itemDeleted,
            Action = action,
            TaskName = taskName,
            ItemName = itemName,
            ExtraInfo = exInStr
        };
        await db.Activities.AddAsync(a, ctx.Ctkn);

        if (itemDeleted)
        {
            if (type == ActivityItemType.Task)
            {
                await db.Activities
                    .Where(x => x.Org == org && x.Project == project && x.Task == task)
                    .ExecuteUpdateAsync(
                        x =>
                            x.SetProperty(x => x.TaskDeleted, _ => true)
                                .SetProperty(x => x.ItemDeleted, _ => true),
                        ctx.Ctkn
                    );
            }
            else
            {
                await db.Activities
                    .Where(x => x.Org == org && x.Project == project && x.Item == item)
                    .ExecuteUpdateAsync(
                        x => x.SetProperty(x => x.ItemDeleted, _ => true),
                        ctx.Ctkn
                    );
            }
        }

        var sendAct = a.ToApi();
        ancestors ??= new List<string>();
        var fcm = ctx.Get<IFcmClient>();
        await fcm.SendTopic(
            ctx,
            db,
            ses,
            new List<string>() { org, project },
            new FcmData(sendAct, ancestors)
        );
    }
}
