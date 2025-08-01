﻿using System.Net;
using Amazon.S3;
using Common.Server;
using Common.Shared;
using Common.Shared.Auth;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.ProjectMember;
using Oak.Db;
using Exact = Oak.Api.Org.Exact;
using Get = Oak.Api.Org.Get;
using Org = Oak.Api.Org.Org;
using S = Oak.I18n.S;
using Task = System.Threading.Tasks.Task;
using Update = Oak.Api.Org.Update;

namespace Oak.Eps;

public static class OrgEps
{
    private const int MaxActiveOrgs = 10;
    public const string FilesBucket = "oaktaskfiles";

    public static void AddServices(IServiceCollection sc)
    {
        sc.AddSingleton<IHtmlSanitizer, HtmlSanitizer>();
    }

    public static async Task InitApp(IServiceProvider sp)
    {
        using var sc = sp.GetRequiredService<IStoreClient>();
        await sc.CreateBucket(FilesBucket, S3CannedACL.Private);
    }

    public static IReadOnlyList<IEp> Eps { get; } =
        new List<IEp>()
        {
            Ep<Create, Org>.DbTx<OakDb>(OrgRpcs.Create, Create),
            new Ep<Exact, Org>(OrgRpcs.GetOne, GetOne),
            new Ep<Get, List<Org>>(OrgRpcs.Get, Get),
            Ep<Update, Org>.DbTx<OakDb>(OrgRpcs.Update, Update),
            Ep<Exact, Nothing>.DbTx<OakDb>(OrgRpcs.Delete, Delete),
        };

    private static async Task<Org> Create(IRpcCtx ctx, OakDb db, ISession ses, Create req)
    {
        var activeOrgs = await db.OrgMembers.CountAsync(
            x => x.IsActive && x.Id == ses.Id,
            ctx.Ctkn
        );
        ctx.ErrorIf(activeOrgs > MaxActiveOrgs, S.OrgTooMany, null, HttpStatusCode.BadRequest);
        var newOrg = new Db.Org()
        {
            Id = Id.New(),
            Name = req.Name,
            CreatedOn = DateTimeExt.UtcNowMilli(),
        };
        await db.Orgs.AddAsync(newOrg, ctx.Ctkn);
        var m = new Db.OrgMember()
        {
            Org = newOrg.Id,
            Id = ses.Id,
            IsActive = true,
            Name = req.OwnerMemberName,
            Role = OrgMemberRole.Owner,
        };
        await db.OrgMembers.AddAsync(m, ctx.Ctkn);
        return newOrg.ToApi(m);
    }

    private static async Task<Org> GetOne(IRpcCtx ctx, Exact req)
    {
        var ses = ctx.GetSession();
        var db = ctx.Get<OakDb>();
        var org = await db.Orgs.SingleOrDefaultAsync(x => x.Id == req.Id, ctx.Ctkn);
        ctx.NotFoundIf(org == null, model: new { Name = "Org" });
        var m = await db.OrgMembers.SingleOrDefaultAsync(
            x => x.Org == req.Id && x.Id == ses.Id,
            ctx.Ctkn
        );
        return org.NotNull().ToApi(m);
    }

    private static async Task<List<Org>> Get(IRpcCtx ctx, Get req)
    {
        var ses = ctx.GetAuthedSession();
        var db = ctx.Get<OakDb>();
        var ms = await db.OrgMembers.Where(x => x.IsActive && x.Id == ses.Id).ToListAsync(ctx.Ctkn);
        var oIds = ms.Select(x => x.Org);
        var qry = db.Orgs.Where(x => oIds.Contains(x.Id));
        qry = req switch
        {
            { OrderBy: OrgOrderBy.Name, Asc: true } => qry.OrderBy(x => x.Name),
            { OrderBy: OrgOrderBy.CreatedOn, Asc: true } => qry.OrderBy(x => x.CreatedOn),
            { OrderBy: OrgOrderBy.Name, Asc: false } => qry.OrderByDescending(x => x.Name),
            { OrderBy: OrgOrderBy.CreatedOn, Asc: false } => qry.OrderByDescending(x =>
                x.CreatedOn
            ),
        };
        var os = await qry.ToListAsync(ctx.Ctkn);
        return os.Select(x => x.ToApi(ms.Single(y => y.Org == x.Id))).ToList();
    }

    private static async Task<Org> Update(IRpcCtx ctx, OakDb db, ISession ses, Update req)
    {
        var m = await db.OrgMembers.SingleOrDefaultAsync(
            x => x.Org == req.Id && x.Id == ses.Id && x.IsActive,
            ctx.Ctkn
        );
        ctx.InsufficientPermissionsIf(m?.Role != OrgMemberRole.Owner);
        var org = await db.Orgs.SingleAsync(x => x.Id == req.Id, ctx.Ctkn);
        org.Name = req.Name;
        return org.ToApi(m);
    }

    private static async Task<Nothing> Delete(IRpcCtx ctx, OakDb db, ISession ses, Exact req)
    {
        await EpsUtil.MustHaveOrgAccess(ctx, db, ses.Id, req.Id, OrgMemberRole.Owner);
        await RawBatchDelete(ctx, db, new List<string>() { req.Id });
        return Nothing.Inst;
    }

    public static async Task AuthOnDelete(IRpcCtx ctx, OakDb db, ISession ses)
    {
        // when a user wants to delete their account entirely,
        var allOwnerOrgs = await db
            .OrgMembers.Where(x => x.Id == ses.Id && x.IsActive && x.Role == OrgMemberRole.Owner)
            .Select(x => x.Org)
            .Distinct()
            .ToListAsync(ctx.Ctkn);
        var activeOwnerCounts = await db
            .OrgMembers.Where(x =>
                allOwnerOrgs.Contains(x.Org) && x.IsActive && x.Role == OrgMemberRole.Owner
            )
            .GroupBy(x => x.Org)
            .Select(x => new { Org = x.Key, ActiveOwnerCount = x.Count() })
            .ToListAsync(ctx.Ctkn);
        var orgsWithSoleOwner = activeOwnerCounts
            .Where(x => x.ActiveOwnerCount == 1)
            .Select(x => x.Org)
            .ToList();
        if (orgsWithSoleOwner.Any())
        {
            // we can auto delete all their orgs for which they are the sole owner
            await RawBatchDelete(ctx, db, orgsWithSoleOwner);
        }
        // all remaining orgs user is not the sole owner so just deactivate them.
        await RawBatchDeactivate(ctx, db, ses);
    }

    public static async Task AuthValidateFcmTopic(
        IRpcCtx ctx,
        OakDb db,
        ISession ses,
        IReadOnlyList<string> topic
    )
    {
        ctx.BadRequestIf(topic.Count != 2);
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            topic[0],
            topic[1],
            ProjectMemberRole.Reader
        );
    }

    private static async Task RawBatchDelete(IRpcCtx ctx, OakDb db, List<string> orgs)
    {
        await db.Orgs.Where(x => orgs.Contains(x.Id)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.OrgMembers.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.ProjectLocks.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.Projects.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.ProjectMembers.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.Activities.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.Tasks.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.Timers.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.VItems.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.Files.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        await db.Comments.Where(x => orgs.Contains(x.Org)).ExecuteDeleteAsync(ctx.Ctkn);
        using var store = ctx.Get<IStoreClient>();
        foreach (var org in orgs)
        {
            await store.DeletePrefix(FilesBucket, org, ctx.Ctkn);
        }
    }

    private static async Task RawBatchDeactivate(IRpcCtx ctx, OakDb db, ISession ses)
    {
        await db
            .OrgMembers.Where(x => x.Id == ses.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.IsActive, x => false), ctx.Ctkn);
    }

    public static async Task AuthOnActivation(IRpcCtx ctx, OakDb db, string id, string email)
    {
        await Task.CompletedTask;
    }
}
