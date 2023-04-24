using Common.Server;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Db;
using Project = Oak.Api.Project.Project;

namespace Oak.Eps;

internal static class ProjectEps
{
    public static IReadOnlyList<IRpcEndpoint> Eps { get; } = new List<IRpcEndpoint>()
    {
        new RpcEndpoint<Create, Project>(ProjectRpcs.Create, async (ctx, req) =>
            await ctx.DbTx<OakDb, Project>(async (db, ses) =>
            {
                await EpsUtil.MustHaveOrgAccess(ctx, db, ses, req.Org, OrgMemberRole.Admin);
                // TODO
                return new Project("", "", "");
            }))
    };
}