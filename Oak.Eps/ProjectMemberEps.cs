using Common.Server;
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
                            return new Db.ProjectMember().ToApi();
                        }
                    )
            )
        };
}
