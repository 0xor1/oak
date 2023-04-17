using System.Net;
using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.Org;
using Oak.Db;
using Org = Oak.Api.Org.Org;
using S = Oak.I18n.S;

namespace Oak.Eps;

internal static class OrgEps
{
    private static readonly IOrgApi Api = IOrgApi.Init();
    private const int MaxActiveOrgs = 10;

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } = new List<IRpcEndpoint>()
    {
        new RpcEndpoint<Create, Org>(Api.Create, async (ctx, req) =>
            await ctx.DbTx<OakDb, Org>(async (db, ses) =>
            {
                var activeOrgs = await db.OrgMembers.CountAsync(x => x.IsActive && x.Member == ses.Id);
                ctx.ErrorIf(activeOrgs > MaxActiveOrgs, S.OrgTooMany, null, HttpStatusCode.BadRequest);
                var newOrg = new Db.Org() 
                {
                    Id = Id.New(),
                    Name = req.Name
                };
                await db.Orgs.AddAsync(newOrg);
                await db.OrgMembers.AddAsync(new ()
                {
                    Org = newOrg.Id,
                    Member = ses.Id,
                    IsActive = true,
                    Name = req.OwnerMemberName,
                    Role = OrgMemberRole.Owner
                });
                return newOrg.ToApi();
            })),
        
        new RpcEndpoint<Nothing, IReadOnlyList<Org>>(Api.Get, async (ctx, _) =>
        {
            var ses = ctx.GetAuthedSession();
            var db = ctx.Get<OakDb>();
            var orgs = await db.OrgMembers.Where(x => x.IsActive && x.Member == ses.Id).Select(x => x.Org).ToListAsync();
            return await db.Orgs.Where(x => orgs.Contains(x.Id)).Select(x => new Org(x.Id, x.Name)).ToListAsync();
        }),
        
        new RpcEndpoint<Update, Org>(Api.Update, async (ctx, req) =>
        await ctx.DbTx<OakDb, Org>(async (db, ses) =>
        {
            var x = await db.OrgMembers.SingleOrDefaultAsync(x => x.Org == req.Id && x.IsActive && x.Member == ses.Id && x.Role == OrgMemberRole.Owner);
            ctx.ErrorIf(x == null, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
            var org = await db.Orgs.SingleAsync(x => x.Id == req.Id);
            org.Name = req.NewName;
            return org.ToApi();
        })),
        
        new RpcEndpoint<Delete, Nothing>(Api.Delete, async (ctx, req) =>
        await ctx.DbTx<OakDb, Nothing>(async (db, ses) =>
        {
            var x = await db.OrgMembers.SingleOrDefaultAsync(x => x.Org == req.Id && x.IsActive && x.Member == ses.Id && x.Role == OrgMemberRole.Owner);
            ctx.ErrorIf(x == null, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
            await TaskExt.WhenAll(
                db.Orgs.Where(x => x.Id == req.Id).ExecuteDeleteAsync(),
                db.OrgMembers.Where(x => x.Org == req.Id).ExecuteDeleteAsync(),
                db.ProjectLocks.Where(x => x.Org == req.Id).ExecuteDeleteAsync(),
                db.Projects.Where(x => x.Org == req.Id).ExecuteDeleteAsync(),
                db.ProjectMembers.Where(x => x.Org == req.Id).ExecuteDeleteAsync(),
                db.Activities.Where(x => x.Org == req.Id).ExecuteDeleteAsync(),
                db.Tasks.Where(x => x.Org == req.Id).ExecuteDeleteAsync(),
                db.VItems.Where(x => x.Org == req.Id).ExecuteDeleteAsync(),
                db.Files.Where(x => x.Org == req.Id).ExecuteDeleteAsync(),
                db.Comments.Where(x => x.Org == req.Id).ExecuteDeleteAsync());
            return new Nothing();
        }))
            
    };
}