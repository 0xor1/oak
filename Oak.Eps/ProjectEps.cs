using System.Net;
using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Db;
using Project = Oak.Api.Project.Project;
using S = Oak.I18n.S;

namespace Oak.Eps;

internal static class ProjectEps
{
    private static readonly IProjectApi Api = IProjectApi.Init();

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } = new List<IRpcEndpoint>()
    {
        new RpcEndpoint<Create, Project>(Api.Create, async (ctx, req) =>
            await ctx.DbTx<OakDb, Project>(async (db, ses) =>
            {
                // check current member has sufficient permissions
                var sesOrgMem = await db.OrgMembers.Where(x => x.Org == req.Org && x.IsActive && x.Member == ses.Id).SingleOrDefaultAsync();
                ctx.ErrorIf(sesOrgMem == null, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
                var sesRole = sesOrgMem.NotNull().Role;
                ctx.ErrorIf(sesRole is not (OrgMemberRole.Owner or OrgMemberRole.Admin), S.InsufficientPermission, null, HttpStatusCode.Forbidden);
                // TODO
                return new Project("", "", "");
            }))
    };
}