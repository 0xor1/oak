using System.Net;
using Common.Server;
using Common.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Db;
using Project = Oak.Api.Project.Project; 
using S = Oak.I18n.S;
using Task = System.Threading.Tasks.Task;

namespace Oak.Eps;

internal static class ProjectEps
{
    private static readonly IProjectApi Api = IProjectApi.Init();

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } = new List<IRpcEndpoint>()
    {
        new RpcEndpoint<Create, Project>(Api.Create, async (ctx, req) =>
            await ctx.DbTx<OakDb, Project>(async (db, ses) =>
            {
                await EpsUtil.MustHaveOrgAccess(ctx, db, ses, req.Org, OrgMemberRole.Admin);
                // TODO
                return new Project("", "", "");
            }))
    };
}