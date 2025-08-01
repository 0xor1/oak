﻿using Common.Server;
using Common.Shared;
using Common.Shared.Auth;
using Microsoft.EntityFrameworkCore;
using Oak.Api.ProjectMember;
using Oak.Api.Timer;
using Oak.Db;
using Create = Oak.Api.Timer.Create;
using Get = Oak.Api.Timer.Get;
using S = Oak.I18n.S;
using Timer = Oak.Api.Timer.Timer;
using Update = Oak.Api.Timer.Update;

namespace Oak.Eps;

public static class TimerEps
{
    private const int MaxTimers = 5;

    public static List<IEp> Eps { get; } =
        new List<IEp>()
        {
            Ep<Create, List<Timer>>.DbTx<OakDb>(TimerRpcs.Create, Create),
            new Ep<Get, SetRes<Timer>>(TimerRpcs.Get, Get),
            Ep<Update, List<Timer>>.DbTx<OakDb>(TimerRpcs.Update, Update),
            Ep<Delete, List<Timer>>.DbTx<OakDb>(TimerRpcs.Delete, Delete),
        };

    private static async Task<List<Timer>> Create(IRpcCtx ctx, OakDb db, ISession ses, Create req)
    {
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Writer
        );
        var ts = await db
            .Timers.Where(x => x.Org == req.Org && x.Project == req.Project && x.User == ses.Id)
            .ToListAsync(ctx.Ctkn);
        // not allowed to have more than 5 timers at any time
        ctx.BadRequestIf(ts.Count > 4, S.TimerMaxTimers, new { MaxTimers });
        // only 1 timer per task per user
        ctx.BadRequestIf(ts.Any(x => x.Task == req.Task), S.TimerAlreadyExists);
        // only one running timer at a time
        ts.ForEach(x =>
        {
            if (!x.IsRunning)
                return;
            x.IsRunning = false;
            x.Inc = (ulong)DateTimeExt.UtcNowMilli().Subtract(x.LastStartedOn).TotalSeconds;
        });

        var t = new Db.Timer
        {
            Org = req.Org,
            Project = req.Project,
            Task = req.Task,
            User = ses.Id,
            Inc = 0,
            LastStartedOn = DateTimeExt.UtcNowMilli(),
            IsRunning = true,
        };
        await db.Timers.AddAsync(t, ctx.Ctkn);
        ts.Add(t);
        return await ReturnResult(ctx, db, ts);
    }

    private static async Task<SetRes<Timer>> Get(IRpcCtx ctx, Get req)
    {
        var ses = ctx.GetAuthedSession();
        var db = ctx.Get<OakDb>();
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Reader
        );
        var qry = db.Timers.Where(x => x.Org == req.Org && x.Project == req.Project);
        if (req.Task != null)
        {
            qry = qry.Where(x => x.Task == req.Task);
        }

        if (req.User != null)
        {
            qry = qry.Where(x => x.User == req.User);
        }

        var oqry = qry.OrderByDescending(x => x.IsRunning);
        oqry = req.Asc switch
        {
            true => oqry.ThenBy(x => x.LastStartedOn),
            false => oqry.ThenByDescending(x => x.LastStartedOn),
        };
        var initRes = await oqry.Take(101).ToListAsync(ctx.Ctkn);
        var tmpRes = await ReturnResult(ctx, db, initRes, req.User != null);
        return SetRes<Timer>.FromLimit(tmpRes, 101);
    }

    private static async Task<List<Timer>> Update(IRpcCtx ctx, OakDb db, ISession ses, Update req)
    {
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Writer
        );
        var ts = await db
            .Timers.Where(x => x.Org == req.Org && x.Project == req.Project && x.User == ses.Id)
            .ToListAsync(ctx.Ctkn);
        // only one running timer at a time
        var t = ts.SingleOrDefault(x => x.Task == req.Task);
        ctx.NotFoundIf(t == null, model: new { Name = "Timer" });
        t.NotNull();
        if (req.IsRunning && t.IsRunning == false)
        {
            // starting the timer, must stop any other running timer
            ts.ForEach(x =>
            {
                if (!x.IsRunning)
                    return;
                x.IsRunning = false;
                x.Inc += (ulong)DateTimeExt.UtcNowMilli().Subtract(x.LastStartedOn).TotalSeconds;
            });
            t.LastStartedOn = DateTimeExt.UtcNowMilli();
            t.IsRunning = true;
        }

        if (req.IsRunning == false && t.IsRunning)
        {
            // pausing timer
            t.IsRunning = false;
            t.Inc += (ulong)DateTimeExt.UtcNowMilli().Subtract(t.LastStartedOn).TotalSeconds;
        }

        return await ReturnResult(ctx, db, ts);
    }

    private static async Task<List<Timer>> Delete(IRpcCtx ctx, OakDb db, ISession ses, Delete req)
    {
        var ts = await db
            .Timers.Where(x => x.Org == req.Org && x.Project == req.Project && x.User == ses.Id)
            .ToListAsync(ctx.Ctkn);
        var t = ts.SingleOrDefault(x => x.Task == req.Task);
        ctx.NotFoundIf(t == null, model: new { Name = "Timer" });
        t.NotNull();
        db.Timers.Remove(t);
        ts.Remove(t);
        return await ReturnResult(ctx, db, ts);
    }

    private static async Task<List<Timer>> ReturnResult(
        IRpcCtx ctx,
        OakDb db,
        IList<Db.Timer> ts,
        bool defaultSort = true
    )
    {
        if (!ts.Any())
            return new List<Timer>();
        var taskIds = ts.Select(x => x.Task).ToList();
        var tasks = await db
            .Tasks.Where(x =>
                x.Org == ts[0].Org && x.Project == ts[0].Project && taskIds.Contains(x.Id)
            )
            .ToListAsync(ctx.Ctkn);
        var qry = ts.Select(x =>
            x.ToApi(tasks.SingleOrDefault(y => y.Id == x.Task)?.Name ?? "unknown")
        );

        if (defaultSort)
        {
            qry = qry.OrderByDescending(x => x.IsRunning).ThenByDescending(x => x.LastStartedOn);
        }

        return qry.ToList();
    }
}
