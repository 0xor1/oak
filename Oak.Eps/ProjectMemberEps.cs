using Common.Server;
using Common.Shared;
using Common.Shared.Auth;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Db;
using Add = Oak.Api.ProjectMember.Add;
using Exact = Oak.Api.ProjectMember.Exact;
using Get = Oak.Api.ProjectMember.Get;
using ProjectMember = Oak.Api.ProjectMember.ProjectMember;
using Update = Oak.Api.ProjectMember.Update;

namespace Oak.Eps;

internal static class ProjectMemberEps
{
    public static IReadOnlyList<IEp> Eps { get; } =
        new List<IEp>()
        {
            Ep<Add, ProjectMember>.DbTx<OakDb>(ProjectMemberRpcs.Add, Add),
            new Ep<Exact, Maybe<ProjectMember>>(ProjectMemberRpcs.GetOne, GetOne),
            new Ep<Get, SetRes<ProjectMember>>(ProjectMemberRpcs.Get, Get),
            Ep<Update, ProjectMember>.DbTx<OakDb>(ProjectMemberRpcs.Update, Update),
            Ep<Exact, Nothing>.DbTx<OakDb>(ProjectMemberRpcs.Remove, Remove),
        };

    private static async Task<ProjectMember> Add(IRpcCtx ctx, OakDb db, ISession ses, Add req)
    {
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Admin
        );
        var orgMem = await db.OrgMembers.SingleOrDefaultAsync(
            x => x.Org == req.Org && x.Id == req.Id && x.IsActive,
            ctx.Ctkn
        );
        ctx.NotFoundIf(orgMem == null, model: new { Name = "Org Member" });
        orgMem.NotNull();
        if (orgMem.Role is OrgMemberRole.Owner or OrgMemberRole.Admin)
        {
            // org level owners and admins cant be less than project admins
            req.Role = ProjectMemberRole.Admin;
        }

        var mem = new Db.ProjectMember()
        {
            Org = req.Org,
            Project = req.Project,
            Id = req.Id,
            IsActive = orgMem.IsActive,
            OrgRole = orgMem.Role,
            Name = orgMem.Name,
            Role = req.Role,
        };
        await db.ProjectMembers.AddAsync(mem, ctx.Ctkn);
        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            req.Org,
            req.Project,
            req.Project,
            mem.Id,
            ActivityItemType.Member,
            ActivityAction.Create,
            mem.Name,
            null,
            null
        );
        return mem.ToApi(new ProjectMemberStats());
    }

    private static async Task<Maybe<ProjectMember>> GetOne(IRpcCtx ctx, Exact req)
    {
        var ses = ctx.GetSession();
        var db = ctx.Get<OakDb>();
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Reader
        );
        var mem = await db.ProjectMembers.SingleOrDefaultAsync(
            x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id,
            ctx.Ctkn
        );
        if (mem == null)
        {
            return new Maybe<ProjectMember>(null);
        }

        var stats = await GetStats(ctx, db, req.Org, req.Project, new List<string>() { mem.Id });
        return new Maybe<ProjectMember>(
            mem.ToApi(stats.SingleOrDefault() ?? new ProjectMemberStats())
        );
    }

    private static async Task<SetRes<ProjectMember>> Get(IRpcCtx ctx, Get req)
    {
        var ses = ctx.GetSession();
        var db = ctx.Get<OakDb>();
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Reader
        );
        var qry = db.ProjectMembers.Where(x => x.Org == req.Org && x.Project == req.Project);
        if (req.IsActive != null)
        {
            qry = qry.Where(x => x.IsActive == req.IsActive);
        }

        if (req.Role != null)
        {
            qry = qry.Where(x => x.Role == req.Role);
        }

        if (!req.NameStartsWith.IsNullOrWhiteSpace())
        {
            qry = qry.Where(x => x.Name.StartsWith(req.NameStartsWith));
        }

        if (req.After != null)
        {
            // implement cursor based pagination ... in a fashion
            var after = await db.ProjectMembers.SingleOrDefaultAsync(
                x => x.Org == req.Org && x.Project == req.Project && x.Id == req.After,
                ctx.Ctkn
            );
            ctx.NotFoundIf(after == null, model: new { Name = "After" });
            after.NotNull();
            qry = (req.OrderBy, req.Asc) switch
            {
                (ProjectMemberOrderBy.Role, true) => qry.Where(x =>
                    x.Role > after.Role
                    || (x.Role == after.Role && x.Name.CompareTo(after.Name) > 0)
                ),
                (ProjectMemberOrderBy.Name, true) => qry.Where(x =>
                    x.Name.CompareTo(after.Name) > 0
                    || (x.Name.CompareTo(after.Name) == 0 && x.Role > after.Role)
                ),
                (ProjectMemberOrderBy.Role, false) => qry.Where(x =>
                    x.Role < after.Role
                    || (x.Role == after.Role && x.Name.CompareTo(after.Name) > 0)
                ),
                (ProjectMemberOrderBy.Name, false) => qry.Where(x =>
                    x.Name.CompareTo(after.Name) < 0
                    || (x.Name.CompareTo(after.Name) == 0 && x.Role > after.Role)
                ),
            };
        }

        qry = (req.OrderBy, req.Asc) switch
        {
            (ProjectMemberOrderBy.Role, true) => qry.OrderBy(x => x.Role).ThenBy(x => x.Name),
            (ProjectMemberOrderBy.Name, true) => qry.OrderBy(x => x.Name).ThenBy(x => x.Role),
            (ProjectMemberOrderBy.Role, false) => qry.OrderByDescending(x => x.Role)
                .ThenBy(x => x.Name),
            (ProjectMemberOrderBy.Name, false) => qry.OrderByDescending(x => x.Name)
                .ThenBy(x => x.Role),
        };
        qry = qry.Take(101);
        var mems = await qry.ToListAsync(ctx.Ctkn);
        var ids = mems.Select(x => x.Id).ToList();
        var stats = await GetStats(ctx, db, req.Org, req.Project, ids);
        var set = mems.Select(x =>
                x.ToApi(stats.SingleOrDefault(y => y.Id == x.Id) ?? new ProjectMemberStats())
            )
            .ToList();
        return SetRes<ProjectMember>.FromLimit(set, 101);
    }

    private static async Task<ProjectMember> Update(IRpcCtx ctx, OakDb db, ISession ses, Update req)
    {
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Admin
        );
        var mem = await db.ProjectMembers.SingleOrDefaultAsync(
            x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id,
            ctx.Ctkn
        );
        ctx.NotFoundIf(mem == null, model: new { Name = "Project Member" });
        mem.NotNull();
        var orgMem = await db.OrgMembers.SingleAsync(
            x => x.Org == req.Org && x.Id == req.Id,
            ctx.Ctkn
        );
        if (orgMem.Role is OrgMemberRole.Owner or OrgMemberRole.Admin)
        {
            // org level owners and admins cant be less than project admins
            req.Role = ProjectMemberRole.Admin;
        }

        mem.Role = req.Role;
        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            req.Org,
            req.Project,
            req.Project,
            mem.Id,
            ActivityItemType.Member,
            ActivityAction.Update,
            mem.Name,
            null,
            null
        );

        var stats = await GetStats(ctx, db, req.Org, req.Project, new List<string>() { mem.Id });
        return mem.ToApi(stats.SingleOrDefault() ?? new ProjectMemberStats());
    }

    private static async Task<Nothing> Remove(IRpcCtx ctx, OakDb db, ISession ses, Exact req)
    {
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Admin
        );
        var mem = await db.ProjectMembers.SingleOrDefaultAsync(
            x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id,
            ctx.Ctkn
        );
        ctx.NotFoundIf(mem == null, model: new { Name = "Project Member" });
        await db
            .ProjectMembers.Where(x =>
                x.Org == req.Org && x.Project == req.Project && x.Id == req.Id
            )
            .ExecuteDeleteAsync(ctx.Ctkn);
        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            req.Org,
            req.Project,
            req.Project,
            mem.NotNull().Id,
            ActivityItemType.Member,
            ActivityAction.Delete,
            mem.Name,
            null,
            null
        );

        // dont do anything clever like mass unassigning their currently assigned tasks
        return Nothing.Inst;
    }

    private static async Task<List<ProjectMemberStats>> GetStats(
        IRpcCtx ctx,
        OakDb db,
        string org,
        string project,
        List<string> ids
    ) =>
        await db
            .Tasks.Where(x => x.Org == org && x.Project == project && ids.Contains(x.User))
            .GroupBy(x => x.User)
            .Select(x => new ProjectMemberStats()
            {
                Id = x.Key,
                TimeEst = (ulong)x.Sum(x => (decimal)x.TimeEst),
                TimeInc = (ulong)x.Sum(x => (decimal)x.TimeInc),
                CostEst = (ulong)x.Sum(x => (decimal)x.CostEst),
                CostInc = (ulong)x.Sum(x => (decimal)x.CostInc),
                FileN = (ulong)x.Sum(x => (decimal)x.FileN),
                FileSize = (ulong)x.Sum(x => (decimal)x.FileSize),
                TaskN = (ulong)x.Count(),
            })
            .ToListAsync(ctx.Ctkn);
}
