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
using Task = System.Threading.Tasks.Task;
using Update = Oak.Api.Org.Update;

namespace Oak.Eps;

internal static class OrgEps
{
    private const int MaxActiveOrgs = 10;

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } = new List<IRpcEndpoint>()
    {
        new RpcEndpoint<Create, Org>(OrgRpcs.Create, async (ctx, req) =>
            await ctx.DbTx<OakDb, Org>(async (db, ses) =>
            {
                var activeOrgs = await db.OrgMembers.CountAsync(x => x.IsActive && x.Member == ses.Id);
                ctx.ErrorIf(activeOrgs > MaxActiveOrgs, S.OrgTooMany, null, HttpStatusCode.BadRequest);
                var newOrg = new Db.Org() 
                {
                    Id = Id.New(),
                    Name = req.Name,
                    CreatedOn = DateTimeExt.UtcNowMilli()
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
        
        new RpcEndpoint<Get, IReadOnlyList<Org>>(OrgRpcs.Get, async (ctx, req) =>
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
        
        new RpcEndpoint<Update, Org>(OrgRpcs.Update, async (ctx, req) =>
        await ctx.DbTx<OakDb, Org>(async (db, ses) =>
        {
            await EpsUtil.MustHaveOrgAccess(ctx, db, ses, req.Id, OrgMemberRole.Owner);
            var org = await db.Orgs.SingleAsync(x => x.Id == req.Id);
            org.Name = req.NewName;
            return org.ToApi();
        })),
        
        new RpcEndpoint<Delete, Nothing>(OrgRpcs.Delete, async (ctx, req) =>
        await ctx.DbTx<OakDb, Nothing>(async (db, ses) =>
        {
            await EpsUtil.MustHaveOrgAccess(ctx, db, ses, req.Id, OrgMemberRole.Owner);
            await RawBatchDelete(db, new List<string>(){req.Id});
            return Nothing.Inst;
        }))
            
    };

    public static async Task AuthOnDelete(OakDb db, Session ses)
    {
        // when a user wants to delete their account entirely,
        var allOwnerOrgs = await db.OrgMembers.Where(x => x.Member == ses.Id && x.IsActive && x.Role == OrgMemberRole.Owner).Select(x => x.Org).Distinct().ToListAsync();
        var activeOwnerCounts = await db.OrgMembers.Where(x => allOwnerOrgs.Contains(x.Org) && x.IsActive && x.Role == OrgMemberRole.Owner).GroupBy(x => x.Org).Select(x => new {Org = x.Key, ActiveOwnerCount = x.Count()}).ToListAsync();
        var orgsWithSoleOwner = activeOwnerCounts.Where(x => x.ActiveOwnerCount == 1).Select(x => x.Org).ToList();
        if (orgsWithSoleOwner.Any())
        {
            // we can auto delete all their orgs for which they are the sole owner
            await RawBatchDelete(db, orgsWithSoleOwner);
        }
        // all remaining orgs user is not the sole owner so just deactivate them.
        await RawBatchDeactivate(db, ses);
    }

    private static async Task RawBatchDelete(OakDb db, List<string> orgs)
    {
        await db.Orgs.Where(x => orgs.Contains(x.Id)).ExecuteDeleteAsync();
        await db.OrgMembers.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync();
        await db.ProjectLocks.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync();
        await db.Projects.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync();
        await db.ProjectMembers.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync();
        await db.Activities.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync();
        await db.Tasks.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync();
        await db.VItems.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync();
        await db.Files.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync();
        await db.Comments.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync();
        // TODO delete all files from S3 using IStoreClient.DeletePrefix(bucket, orgId);
    }

    private static async Task RawBatchDeactivate(OakDb db, Session ses)
    {
        await db.OrgMembers.Where(x => x.Member == ses.Id).ExecuteUpdateAsync(x => x.SetProperty(x => x.IsActive, x => false));
    }
}