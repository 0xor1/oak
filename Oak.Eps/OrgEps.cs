using System.Net;
using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Db;
using Org = Oak.Api.Org.Org;
using Get = Oak.Api.Org.Get;
using S = Oak.I18n.S;
using Update = Oak.Api.Org.Update;

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
        
        new RpcEndpoint<Get, IReadOnlyList<Org>>(Api.Get, async (ctx, req) =>
        {
            var ses = ctx.GetAuthedSession();
            var db = ctx.Get<OakDb>();
            var orgs = await db.OrgMembers.Where(x => x.IsActive && x.Member == ses.Id).Select(x => x.Org).ToListAsync();
            var qry = db.Orgs.Where(x => orgs.Contains(x.Id));
            qry = req switch
            {
                (OrgOrderBy.Name, true) => qry.OrderBy(x => x.Name),
                (OrgOrderBy.CreatedOn, true) => qry.OrderBy(x => x.CreatedOn),
                (OrgOrderBy.Name, false) => qry.OrderByDescending(x => x.Name),
                (OrgOrderBy.CreatedOn, false) => qry.OrderByDescending(x => x.CreatedOn),
            };
            return await qry.Select(x => x.ToApi()).ToListAsync();
        }),
        
        new RpcEndpoint<Update, Org>(Api.Update, async (ctx, req) =>
        await ctx.DbTx<OakDb, Org>(async (db, ses) =>
        {
            await EpsUtil.MustHaveOrgAccess(ctx, db, ses, req.Id, OrgMemberRole.Owner);
            var org = await db.Orgs.SingleAsync(x => x.Id == req.Id);
            org.Name = req.NewName;
            return org.ToApi();
        })),
        
        new RpcEndpoint<Delete, Nothing>(Api.Delete, async (ctx, req) =>
        await ctx.DbTx<OakDb, Nothing>(async (db, ses) =>
        {
            await EpsUtil.MustHaveOrgAccess(ctx, db, ses, req.Id, OrgMemberRole.Owner);
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