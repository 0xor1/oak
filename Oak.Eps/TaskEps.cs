using Common.Server;
using Oak.Api.ProjectMember;
using Oak.Api.Task;
using Oak.Db;
using Create = Oak.Api.Task.Create;
using Task = Oak.Api.Task.Task;

namespace Oak.Eps;

internal static class TaskEps
{
    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Create, Task>(
                TaskRpcs.Create,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, Task>(
                        async (db, ses) =>
                        {
                            await EpsUtil.MustHaveProjectAccess(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                req.Project,
                                ProjectMemberRole.Writer
                            );
                            throw new NotImplementedException();
                        }
                    )
            ),
        };
}
