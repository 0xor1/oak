using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.ProjectMember;
using Oak.Db;
using ProjectMember = Oak.Api.ProjectMember.ProjectMember;
using Create = Oak.Api.ProjectMember.Create;

namespace Oak.Eps;

internal static class ProjectMemberEps
{
    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Create, ProjectMember>(
                ProjectMemberRpcs.Create,
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
                    var stats = await db.Tasks
                        .Where(
                            x => x.Org == req.Org && x.Project == req.Project && x.User == mem.Id
                        )
                        .GroupBy(x => x.Id)
                        .Select(
                            x =>
                                new ProjectMemberStats()
                                {
                                    Id = x.Key,
                                    TimeEst = (ulong)x.Sum(x => (decimal)x.TimeEst),
                                    TimeInc = (ulong)x.Sum(x => (decimal)x.TimeInc),
                                    CostEst = (ulong)x.Sum(x => (decimal)x.CostEst),
                                    CostInc = (ulong)x.Sum(x => (decimal)x.CostInc),
                                    FileN = (ulong)x.Sum(x => (decimal)x.CostEst),
                                    FileSize = (ulong)x.Sum(x => (decimal)x.CostInc),
                                    TaskN = (ulong)x.Sum(x => (decimal)x.CostEst)
                                }
                        )
                        .SingleAsync();
                    return mem.ToApi(stats);
                }
            )
        };
}
