using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Api.ProjectMember;
using Oak.Db;
using Add = Oak.Api.ProjectMember.Add;
using ProjectMember = Oak.Api.ProjectMember.ProjectMember;
using Exact = Oak.Api.ProjectMember.Exact;
using Get = Oak.Api.ProjectMember.Get;
using Update = Oak.Api.ProjectMember.Update;

namespace Oak.Eps;

internal static class ProjectMemberEps
{
    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Add, ProjectMember>(
                ProjectMemberRpcs.Add,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, ProjectMember>(
                        async (db, ses) =>
                        {
                            await EpsUtil.MustHaveProjectAccess(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                req.Project,
                                ProjectMemberRole.Admin
                            );
                            var orgMem = await db.OrgMembers.SingleOrDefaultAsync(
                                x => x.Org == req.Org && x.Id == req.Id
                            );
                            ctx.NotFoundIf(orgMem == null);
                            orgMem.NotNull();
                            if (orgMem.Role is OrgMemberRole.Owner or OrgMemberRole.Admin)
                            {
                                // org level owners and admins cant be less than project admins
                                req = req with
                                {
                                    Role = ProjectMemberRole.Admin
                                };
                            }
                            var mem = new Db.ProjectMember()
                            {
                                Org = req.Org,
                                Project = req.Project,
                                Id = req.Id,
                                Name = orgMem.Name,
                                Role = req.Role
                            };
                            await db.ProjectMembers.AddAsync(mem);
                            return mem.ToApi(new ProjectMemberStats());
                        }
                    )
            ),
            new RpcEndpoint<Exact, ProjectMember>(
                ProjectMemberRpcs.GetOne,
                async (ctx, req) =>
                {
                    var ses = ctx.GetAuthedSession();
                    var db = ctx.Get<OakDb>();
                    await EpsUtil.MustHaveProjectAccess(
                        ctx,
                        db,
                        ses,
                        req.Org,
                        req.Project,
                        ProjectMemberRole.Reader
                    );
                    var mem = await db.ProjectMembers.SingleOrDefaultAsync(
                        x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id
                    );
                    ctx.NotFoundIf(mem == null);
                    mem.NotNull();
                    var stats = await GetStats(
                        db,
                        req.Org,
                        req.Project,
                        new List<string>() { mem.Id }
                    );
                    return mem.ToApi(stats.SingleOrDefault() ?? new ProjectMemberStats());
                }
            ),
            new RpcEndpoint<Get, IReadOnlyList<ProjectMember>>(
                ProjectMemberRpcs.Get,
                async (ctx, req) =>
                {
                    var ses = ctx.GetAuthedSession();
                    var db = ctx.Get<OakDb>();
                    await EpsUtil.MustHaveProjectAccess(
                        ctx,
                        db,
                        ses,
                        req.Org,
                        req.Project,
                        ProjectMemberRole.Reader
                    );
                    var qry = db.ProjectMembers.Where(
                        x => x.Org == req.Org && x.Project == req.Project
                    );
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
                            x => x.Org == req.Org && x.Project == req.Project && x.Id == req.After
                        );
                        ctx.NotFoundIf(after == null);
                        after.NotNull();
                        qry = (req.OrderBy, req.Asc) switch
                        {
                            (ProjectMemberOrderBy.Role, true)
                                => qry.Where(
                                    x =>
                                        x.Role > after.Role
                                        || (
                                            x.Role == after.Role && x.Name.CompareTo(after.Name) > 0
                                        )
                                ),
                            (ProjectMemberOrderBy.Name, true)
                                => qry.Where(
                                    x =>
                                        x.Name.CompareTo(after.Name) > 0
                                        || (
                                            x.Name.CompareTo(after.Name) == 0 && x.Role > after.Role
                                        )
                                ),
                            (ProjectMemberOrderBy.Role, false)
                                => qry.Where(
                                    x =>
                                        x.Role < after.Role
                                        || (
                                            x.Role == after.Role && x.Name.CompareTo(after.Name) > 0
                                        )
                                ),
                            (ProjectMemberOrderBy.Name, false)
                                => qry.Where(
                                    x =>
                                        x.Name.CompareTo(after.Name) < 0
                                        || (
                                            x.Name.CompareTo(after.Name) == 0 && x.Role > after.Role
                                        )
                                ),
                        };
                    }

                    qry = (req.OrderBy, req.Asc) switch
                    {
                        (ProjectMemberOrderBy.Role, true)
                            => qry.OrderBy(x => x.Role).ThenBy(x => x.Name),
                        (ProjectMemberOrderBy.Name, true)
                            => qry.OrderBy(x => x.Name).ThenBy(x => x.Role),
                        (ProjectMemberOrderBy.Role, false)
                            => qry.OrderByDescending(x => x.Role).ThenBy(x => x.Name),
                        (ProjectMemberOrderBy.Name, false)
                            => qry.OrderByDescending(x => x.Name).ThenBy(x => x.Role),
                    };
                    qry = qry.Take(100);
                    var mems = await qry.ToListAsync();
                    var ids = mems.Select(x => x.Id).ToList();
                    var stats = await GetStats(db, req.Org, req.Project, ids);
                    return mems.Select(
                            x =>
                                x.ToApi(
                                    stats.SingleOrDefault(y => y.Id == x.Id)
                                        ?? new ProjectMemberStats()
                                )
                        )
                        .ToList();
                }
            ),
            new RpcEndpoint<Update, ProjectMember>(
                ProjectMemberRpcs.Update,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, ProjectMember>(
                        async (db, ses) =>
                        {
                            await EpsUtil.MustHaveProjectAccess(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                req.Project,
                                ProjectMemberRole.Admin
                            );
                            var mem = await db.ProjectMembers.SingleOrDefaultAsync(
                                x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id
                            );
                            ctx.NotFoundIf(mem == null);
                            mem.NotNull();
                            var orgMem = await db.OrgMembers.SingleAsync(
                                x => x.Org == req.Org && x.Id == req.Id
                            );
                            if (orgMem.Role is OrgMemberRole.Owner or OrgMemberRole.Admin)
                            {
                                // org level owners and admins cant be less than project admins
                                req = req with
                                {
                                    Role = ProjectMemberRole.Admin
                                };
                            }
                            mem.Role = req.Role;
                            var stats = await GetStats(
                                db,
                                req.Org,
                                req.Project,
                                new List<string>() { mem.Id }
                            );
                            return mem.ToApi(stats.SingleOrDefault() ?? new ProjectMemberStats());
                        }
                    )
            ),
            new RpcEndpoint<Exact, Nothing>(
                ProjectMemberRpcs.Remove,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, Nothing>(
                        async (db, ses) =>
                        {
                            await EpsUtil.MustHaveProjectAccess(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                req.Project,
                                ProjectMemberRole.Admin
                            );
                            await db.ProjectMembers
                                .Where(
                                    x =>
                                        x.Org == req.Org
                                        && x.Project == req.Project
                                        && x.Id == req.Id
                                )
                                .ExecuteDeleteAsync();
                            // dont do anything clever like mass unassigning their currently assigned tasks
                            return Nothing.Inst;
                        }
                    )
            )
        };

    private static async Task<List<ProjectMemberStats>> GetStats(
        OakDb db,
        string org,
        string project,
        List<string> ids
    ) =>
        await db.Tasks
            .Where(x => x.Org == org && x.Project == project && ids.Contains(x.User))
            .GroupBy(x => x.User)
            .Select(
                x =>
                    new ProjectMemberStats()
                    {
                        Id = x.Key,
                        TimeEst = (ulong)x.Sum(x => (decimal)x.TimeEst),
                        TimeInc = (ulong)x.Sum(x => (decimal)x.TimeInc),
                        CostEst = (ulong)x.Sum(x => (decimal)x.CostEst),
                        CostInc = (ulong)x.Sum(x => (decimal)x.CostInc),
                        FileN = (ulong)x.Sum(x => (decimal)x.FileN),
                        FileSize = (ulong)x.Sum(x => (decimal)x.FileSize),
                        TaskN = (ulong)x.Count()
                    }
            )
            .ToListAsync();
}
