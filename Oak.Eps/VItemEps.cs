using Common.Server;
using Common.Shared;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Oak.Api;
using Oak.Api.ProjectMember;
using Oak.Api.VItem;
using Oak.Db;
using S = Oak.I18n.S;
using Create = Oak.Api.VItem.Create;
using Exact = Oak.Api.VItem.Exact;
using Get = Oak.Api.VItem.Get;
using Task = Oak.Api.Task.Task;
using VItem = Oak.Api.VItem.VItem;
using Update = Oak.Api.VItem.Update;

namespace Oak.Eps;

internal static class VItemEps
{
    private const ulong maxTimeInc = 1440; // 24hrs in mins
    private const int noteMaxLen = 250;
    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Create, VItemRes>(
                VItemRpcs.Create,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, VItemRes>(
                        async (db, ses) =>
                        {
                            await EpsUtil.MustHaveProjectAccess(
                                ctx,
                                db,
                                ses.Id,
                                req.Org,
                                req.Project,
                                ProjectMemberRole.Writer
                            );
                            ValidateInc(ctx, req.Type, req.Inc);
                            EpsUtil.ValidStr(ctx, req.Note, 0, noteMaxLen, "note");
                            await db.LockProject(req.Org, req.Project);
                            var t = await db.Tasks.SingleOrDefaultAsync(
                                x =>
                                    x.Org == req.Org && x.Project == req.Project && x.Id == req.Task
                            );
                            ctx.NotFoundIf(t == null, model: new { Name = "Task" });
                            t.NotNull();
                            setTaskVal(t, req.Est, req.Type, true);
                            setTaskVal(t, req.Inc, req.Type, false);
                            var vi = new Db.VItem()
                            {
                                Org = req.Org,
                                Project = req.Project,
                                Task = req.Task,
                                Type = req.Type,
                                Id = Id.New(),
                                CreatedBy = ses.Id,
                                CreatedOn = DateTimeExt.UtcNowMilli(),
                                Inc = req.Inc,
                                Note = req.Note
                            };
                            await db.VItems.AddAsync(vi);
                            await db.SaveChangesAsync();
                            List<string>? ancestors = null;
                            if (t.Parent != null)
                            {
                                ancestors = await db.SetAncestralChainAggregateValuesFromTask(
                                    req.Org,
                                    req.Project,
                                    t.Parent
                                );
                            }

                            req = req with { Note = req.Note.Ellipsis(50).NotNull() };
                            await EpsUtil.LogActivity(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                req.Project,
                                req.Task,
                                vi.Id,
                                ActivityItemType.VItem,
                                ActivityAction.Create,
                                null,
                                req,
                                ancestors
                            );
                            return new VItemRes(t.ToApi(), vi.ToApi());
                        }
                    )
            ),
            new RpcEndpoint<Update, VItemRes>(
                VItemRpcs.Update,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, VItemRes>(
                        async (db, ses) =>
                        {
                            ValidateInc(ctx, req.Type, req.Inc);
                            EpsUtil.ValidStr(ctx, req.Note, 0, noteMaxLen, "note");
                            await db.LockProject(req.Org, req.Project);
                            var vi = await db.VItems.SingleOrDefaultAsync(
                                x =>
                                    x.Org == req.Org
                                    && x.Project == req.Project
                                    && x.Task == req.Task
                                    && x.Type == req.Type
                                    && x.Id == req.Id
                            );
                            ctx.NotFoundIf(vi == null, model: new { Name = req.Type.Humanize() });
                            vi.NotNull();
                            var requiredRole = ProjectMemberRole.Admin;
                            if (
                                vi.CreatedBy == ses.Id
                                && vi.CreatedOn.Add(TimeSpan.FromHours(1)) > DateTime.UtcNow
                            )
                            {
                                // if i created it in the last hour I only need to be a writer
                                requiredRole = ProjectMemberRole.Writer;
                            }
                            await EpsUtil.MustHaveProjectAccess(
                                ctx,
                                db,
                                ses.Id,
                                req.Org,
                                req.Project,
                                requiredRole
                            );
                            var t = await db.Tasks.SingleOrDefaultAsync(
                                x =>
                                    x.Org == req.Org && x.Project == req.Project && x.Id == req.Task
                            );
                            ctx.NotFoundIf(t == null, model: new { Name = "Task" });
                            t.NotNull();
                            var oldInc = (int)vi.Inc;
                            var newInc = (int)req.Inc;
                            var change = newInc - oldInc;
                            List<string>? ancestors = null;
                            vi.Note = req.Note;
                            if (change != 0)
                            {
                                vi.Inc = req.Inc;
                                switch (vi.Type)
                                {
                                    case VItemType.Time:
                                        if (change > 0)
                                        {
                                            t.TimeInc += (ulong)change;
                                        }
                                        else
                                        {
                                            t.TimeInc -= (ulong)-change;
                                        }
                                        break;
                                    case VItemType.Cost:
                                        if (change > 0)
                                        {
                                            t.CostInc += (ulong)change;
                                        }
                                        else
                                        {
                                            t.CostInc -= (ulong)-change;
                                        }
                                        break;
                                }
                            }
                            await db.SaveChangesAsync();
                            if (change != 0 && t.Parent != null)
                            {
                                ancestors = await db.SetAncestralChainAggregateValuesFromTask(
                                    req.Org,
                                    req.Project,
                                    t.Parent
                                );
                            }
                            req = req with { Note = req.Note.Ellipsis(50).NotNull() };
                            await EpsUtil.LogActivity(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                req.Project,
                                req.Task,
                                vi.Id,
                                ActivityItemType.VItem,
                                ActivityAction.Update,
                                null,
                                req,
                                ancestors
                            );
                            return new VItemRes(t.ToApi(), vi.ToApi());
                        }
                    )
            ),
            new RpcEndpoint<Exact, Task>(
                VItemRpcs.Delete,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, Task>(
                        async (db, ses) =>
                        {
                            await db.LockProject(req.Org, req.Project);
                            var vi = await db.VItems.SingleOrDefaultAsync(
                                x =>
                                    x.Org == req.Org
                                    && x.Project == req.Project
                                    && x.Task == req.Task
                                    && x.Type == req.Type
                                    && x.Id == req.Id
                            );
                            ctx.NotFoundIf(vi == null, model: new { Name = req.Type.Humanize() });
                            vi.NotNull();
                            var requiredRole = ProjectMemberRole.Admin;
                            if (
                                vi.CreatedBy == ses.Id
                                && vi.CreatedOn.Add(TimeSpan.FromHours(1)) > DateTime.UtcNow
                            )
                            {
                                // if i created it in the last hour I only need to be a writer
                                requiredRole = ProjectMemberRole.Writer;
                            }
                            await EpsUtil.MustHaveProjectAccess(
                                ctx,
                                db,
                                ses.Id,
                                req.Org,
                                req.Project,
                                requiredRole
                            );
                            var t = await db.Tasks.SingleOrDefaultAsync(
                                x =>
                                    x.Org == req.Org && x.Project == req.Project && x.Id == req.Task
                            );
                            ctx.NotFoundIf(t == null, model: new { Name = "Task" });
                            t.NotNull();
                            switch (vi.Type)
                            {
                                case VItemType.Time:
                                    t.TimeInc -= vi.Inc;
                                    break;
                                case VItemType.Cost:
                                    t.CostInc -= vi.Inc;
                                    break;
                            }
                            await db.SaveChangesAsync();
                            List<string>? ancestors = null;
                            if (t.Parent != null)
                            {
                                ancestors = await db.SetAncestralChainAggregateValuesFromTask(
                                    req.Org,
                                    req.Project,
                                    t.Parent
                                );
                            }
                            await EpsUtil.LogActivity(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                req.Project,
                                req.Task,
                                vi.Id,
                                ActivityItemType.VItem,
                                ActivityAction.Delete,
                                null,
                                req,
                                ancestors
                            );
                            return t.ToApi();
                        }
                    )
            ),
            new RpcEndpoint<Get, SetRes<VItem>>(
                VItemRpcs.Get,
                async (ctx, req) =>
                {
                    var ses = ctx.GetSession();
                    var db = ctx.Get<OakDb>();
                    await EpsUtil.MustHaveProjectAccess(
                        ctx,
                        db,
                        ses.Id,
                        req.Org,
                        req.Project,
                        ProjectMemberRole.Reader
                    );
                    var qry = db.VItems.Where(
                        x => x.Org == req.Org && x.Project == req.Project && x.Type == req.Type
                    );
                    if (req.Task != null)
                    {
                        qry = qry.Where(x => x.Task == req.Task);
                    }

                    if (req.CreatedOn != null)
                    {
                        if (req.CreatedOn.Min != null)
                        {
                            qry = qry.Where(x => x.CreatedOn >= req.CreatedOn.Min);
                        }
                        if (req.CreatedOn.Max != null)
                        {
                            qry = qry.Where(x => x.CreatedOn <= req.CreatedOn.Max);
                        }
                    }

                    if (req.CreatedBy != null)
                    {
                        qry = qry.Where(x => x.CreatedBy == req.CreatedBy);
                    }

                    if (req.After != null)
                    {
                        // implement cursor based pagination ... in a fashion
                        var after = await db.VItems.SingleOrDefaultAsync(
                            x =>
                                x.Org == req.Org
                                && x.Project == req.Project
                                && x.Type == req.Type
                                && x.Id == req.After
                        );
                        ctx.NotFoundIf(after == null, model: new { Name = "After" });
                        after.NotNull();
                        if (req.Asc)
                        {
                            qry = qry.Where(x => x.CreatedOn > after.CreatedOn);
                        }
                        else
                        {
                            qry = qry.Where(x => x.CreatedOn < after.CreatedOn);
                        }
                    }
                    if (req.Asc)
                    {
                        qry = qry.OrderBy(x => x.CreatedOn);
                    }
                    else
                    {
                        qry = qry.OrderByDescending(x => x.CreatedOn);
                    }

                    var res = await qry.Take(101).Select(x => x.ToApi()).ToListAsync();
                    return SetRes<VItem>.FromLimit(res, 101);
                }
            )
        };

    private static void ValidateInc(IRpcCtx ctx, VItemType type, ulong inc)
    {
        switch (type)
        {
            case VItemType.Time:
                ctx.BadRequestIf(inc == 0 || inc > maxTimeInc, S.VItemInvalidTimeInc);
                break;
            case VItemType.Cost:
                ctx.BadRequestIf(inc == 0, S.VItemInvalidCostInc);
                break;
        }
    }

    private static void setTaskVal(Db.Task t, ulong? val, VItemType type, bool isEst)
    {
        if (val == null)
        {
            return;
        }
        switch (type)
        {
            case VItemType.Time:
                if (isEst)
                {
                    t.TimeEst = val.NotNull();
                }
                else
                {
                    t.TimeInc += val.NotNull();
                }
                break;
            case VItemType.Cost:
                if (isEst)
                {
                    t.CostEst = val.NotNull();
                }
                else
                {
                    t.CostInc += val.NotNull();
                }
                break;
        }
    }
}
