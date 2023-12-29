using Common.Server;
using Common.Shared;
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

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Create, Timer>(
                TimerRpcs.Create,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, Timer>(
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
                            var ts = await db.Timers
                                .Where(
                                    x =>
                                        x.Org == req.Org
                                        && x.Project == req.Project
                                        && x.User == ses.Id
                                )
                                .ToListAsync(ctx.Ctkn);
                            // not allowed to have more than 5 timers at any time
                            ctx.BadRequestIf(ts.Count > 4, S.TimerMaxTimers, new { MaxTimers });
                            // only 1 timer per task per user
                            ctx.BadRequestIf(ts.Any(x => x.Task == req.Task), S.TimerAlreadyExists);
                            // only one running timer at a time
                            var hadRunningTimer = false;
                            ts.ForEach(x =>
                            {
                                if (!x.IsRunning)
                                    return;
                                hadRunningTimer = true;
                                x.Inc += (ulong)
                                    DateTimeExt.UtcNowMilli().Subtract(x.LastStartedOn).Seconds;
                            });
                            if (hadRunningTimer)
                            {
                                await db.SaveChangesAsync(ctx.Ctkn);
                            }

                            var t = new Db.Timer
                            {
                                Org = req.Org,
                                Project = req.Project,
                                Task = req.Task,
                                User = ses.Id,
                                Note = "",
                                Inc = 0,
                                LastStartedOn = DateTimeExt.UtcNowMilli(),
                                IsRunning = true
                            };
                            await db.Timers.AddAsync(t, ctx.Ctkn);
                            return t.ToApi();
                        }
                    )
            ),
            new RpcEndpoint<Get, SetRes<Timer>>(
                TimerRpcs.Get,
                async (ctx, req) =>
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

                    qry = qry.OrderByDescending(x => x.IsRunning);
                    qry = req.Asc switch
                    {
                        true => qry.OrderBy(x => x.LastStartedOn),
                        false => qry.OrderByDescending(x => x.LastStartedOn)
                    };
                    var res = await qry.Take(101).ToListAsync(ctx.Ctkn);
                    return SetRes<Timer>.FromLimit(res.Select(x => x.ToApi()).ToList(), 101);
                }
            ),
            new RpcEndpoint<Update, Timer>(
                TimerRpcs.Update,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, Timer>(
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
                            var ts = await db.Timers
                                .Where(
                                    x =>
                                        x.Org == req.Org
                                        && x.Project == req.Project
                                        && x.User == ses.Id
                                )
                                .ToListAsync(ctx.Ctkn);
                            // only one running timer at a time
                            var t = ts.SingleOrDefault(x => x.Task == req.Task);
                            ctx.NotFoundIf(t == null, model: new { Name = "Timer" });
                            t.NotNull();
                            t.Note = req.Note ?? t.Note;
                            if (req.IsRunning == true && t.IsRunning == false)
                            {
                                // starting the timer, must stop any other running timer
                                ts.ForEach(x =>
                                {
                                    if (!x.IsRunning)
                                        return;
                                    x.IsRunning = false;
                                    x.Inc += (ulong)
                                        DateTimeExt.UtcNowMilli().Subtract(x.LastStartedOn).Seconds;
                                });
                                t.LastStartedOn = DateTimeExt.UtcNowMilli();
                                t.IsRunning = true;
                            }
                            if (req.IsRunning == false && t.IsRunning == true)
                            {
                                // pausing timer
                                t.IsRunning = false;
                                t.Inc += (ulong)
                                    DateTimeExt.UtcNowMilli().Subtract(t.LastStartedOn).Seconds;
                            }
                            await db.SaveChangesAsync(ctx.Ctkn);
                            return t.ToApi();
                        }
                    )
            )
        };
}
