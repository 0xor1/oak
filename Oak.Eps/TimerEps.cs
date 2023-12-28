using System.Net;
using Amazon.S3;
using Common.Server;
using Common.Shared;
using Common.Shared.Auth;
using Microsoft.EntityFrameworkCore;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.ProjectMember;
using Oak.Api.Timer;
using Oak.Db;
using Create = Oak.Api.Timer.Create;
using Delete = Oak.Api.Timer.Delete;
using Get = Oak.Api.Timer.Get;
using S = Oak.I18n.S;
using Task = Oak.Api.Task.Task;
using Timer = Oak.Api.Timer.Timer;
using Update = Oak.Api.Timer.Update;

namespace Oak.Eps;

public static class TimerEps
{
    private const int MaxTimers = 5;

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Create, Timer>(
                TimerRpcs.Create,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, Timer>(
                        async (db, ses) =>
                        {
                            throw new NotImplementedException("YOLO");
                        }
                    )
            )
        };
}
