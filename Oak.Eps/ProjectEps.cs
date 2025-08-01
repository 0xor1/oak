﻿using Common.Server;
using Common.Shared;
using Common.Shared.Auth;
using Microsoft.EntityFrameworkCore;
using Oak.Api;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Db;
using Activity = Oak.Api.Project.Activity;
using Create = Oak.Api.Project.Create;
using Exact = Oak.Api.Project.Exact;
using Get = Oak.Api.Project.Get;
using Project = Oak.Api.Project.Project;
using S = Oak.I18n.S;
using Update = Oak.Api.Project.Update;

namespace Oak.Eps;

internal static class ProjectEps
{
    public static IReadOnlyList<IEp> Eps { get; } =
        new List<IEp>()
        {
            Ep<Create, Project>.DbTx<OakDb>(ProjectRpcs.Create, Create),
            new Ep<Exact, Project>(ProjectRpcs.GetOne, GetOne),
            new Ep<Get, SetRes<Project>>(ProjectRpcs.Get, Get),
            Ep<Update, Project>.DbTx<OakDb>(ProjectRpcs.Update, Update),
            Ep<Exact, Nothing>.DbTx<OakDb>(ProjectRpcs.Delete, Delete),
            new Ep<GetActivities, SetRes<Activity>>(ProjectRpcs.GetActivities, GetActivities),
        };

    private static async Task<Project> Create(IRpcCtx ctx, OakDb db, ISession ses, Create req)
    {
        await EpsUtil.MustHaveOrgAccess(ctx, db, ses.Id, req.Org, OrgMemberRole.Admin);
        ctx.BadRequestIf(req.HoursPerDay < 1 || req.HoursPerDay > 24, S.ProjectInvalidHoursPerDay);
        ctx.BadRequestIf(req.DaysPerWeek < 1 || req.DaysPerWeek > 7, S.ProjectInvalidDaysPerWeek);
        var p = new Db.Project()
        {
            Org = req.Org,
            Id = Id.New(),
            IsArchived = false,
            IsPublic = req.IsPublic,
            Name = req.Name,
            CreatedOn = DateTimeExt.UtcNowMilli(),
            CurrencySymbol = req.CurrencySymbol,
            CurrencyCode = req.CurrencyCode,
            HoursPerDay = req.HoursPerDay,
            DaysPerWeek = req.DaysPerWeek,
            StartOn = req.StartOn,
            EndOn = req.EndOn,
            FileLimit = req.FileLimit,
        };
        await db.Projects.AddAsync(p, ctx.Ctkn);
        await db.ProjectLocks.AddAsync(new() { Org = req.Org, Id = p.Id }, ctx.Ctkn);
        var t = new Db.Task()
        {
            Org = req.Org,
            Project = p.Id,
            Id = p.Id,
            User = ses.Id,
            Name = p.Name,
            CreatedBy = ses.Id,
            CreatedOn = DateTimeExt.UtcNowMilli(),
        };
        await db.Tasks.AddAsync(t, ctx.Ctkn);
        var mem = await db.OrgMembers.SingleAsync(
            x => x.Org == req.Org && x.Id == ses.Id,
            ctx.Ctkn
        );
        await db.ProjectMembers.AddAsync(
            new()
            {
                Org = req.Org,
                Project = p.Id,
                Id = ses.Id,
                IsActive = mem.IsActive,
                OrgRole = mem.Role,
                Name = mem.Name,
                Role = ProjectMemberRole.Admin,
            },
            ctx.Ctkn
        );
        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            req.Org,
            p.Id,
            p.Id,
            p.Id,
            ActivityItemType.Project,
            ActivityAction.Create,
            p.Name,
            null,
            null
        );
        return p.ToApi(t);
    }

    private static async Task<Project> GetOne(IRpcCtx ctx, Exact req)
    {
        var ses = ctx.GetSession();
        var db = ctx.Get<OakDb>();
        // requesting a specific project
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Id,
            ProjectMemberRole.Reader
        );
        var res = await db.Projects.SingleOrDefaultAsync(
            x => x.Org == req.Org && x.Id == req.Id,
            ctx.Ctkn
        );
        ctx.NotFoundIf(res == null, model: new { Name = "Project" });
        res.NotNull();
        var t = await db.Tasks.SingleAsync(
            x => x.Org == req.Org && x.Project == req.Id && x.Id == req.Id,
            ctx.Ctkn
        );
        return res.ToApi(t);
    }

    private static async Task<SetRes<Project>> Get(IRpcCtx ctx, Get req)
    {
        var ses = ctx.GetSession();
        var db = ctx.Get<OakDb>();

        var orgMemRole = await EpsUtil.OrgRole(ctx, db, ses.Id, req.Org);
        if (orgMemRole == null && req.IsPublic != true)
        {
            req.IsPublic = true;
        }

        var qry = db.Projects.Where(x => x.Org == req.Org && x.IsArchived == req.IsArchived);
        if (req.IsPublic != null)
        {
            qry = qry.Where(x => x.IsPublic == req.IsPublic);
        }

        if (!req.NameStartsWith.IsNullOrWhiteSpace())
        {
            qry = qry.Where(x => x.Name.StartsWith(req.NameStartsWith));
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

        if (req.StartOn != null)
        {
            if (req.StartOn.Min != null)
            {
                qry = qry.Where(x => x.StartOn >= req.StartOn.Min);
            }

            if (req.StartOn.Max != null)
            {
                qry = qry.Where(x => x.StartOn <= req.StartOn.Max);
            }
        }

        if (req.EndOn != null)
        {
            if (req.EndOn.Min != null)
            {
                qry = qry.Where(x => x.EndOn >= req.EndOn.Min);
            }

            if (req.EndOn.Max != null)
            {
                qry = qry.Where(x => x.EndOn <= req.EndOn.Max);
            }
        }

        if (req.IsPublic != true && orgMemRole > OrgMemberRole.ReadAllProjects)
        {
            // req is for private projects and the user has per project permissions access
            var projectIds = await db
                .ProjectMembers.Where(x => x.Org == req.Org && x.Id == ses.Id)
                .Select(x => x.Project)
                .Distinct()
                .ToListAsync(ctx.Ctkn);
            qry = qry.Where(x => projectIds.Contains(x.Id) || x.IsPublic);
        }

        if (req.After != null)
        {
            // implement cursor based pagination ... in a fashion
            var after = await db.Projects.SingleOrDefaultAsync(
                x => x.Org == req.Org && x.Id == req.After,
                ctx.Ctkn
            );
            ctx.NotFoundIf(after == null, model: new { Name = "After" });
            after.NotNull();
            qry = (req.OrderBy, req.Asc) switch
            {
                (ProjectOrderBy.Name, true) => qry.Where(x =>
                    x.Name.CompareTo(after.Name) > 0
                    || (x.Name.CompareTo(after.Name) == 0 && x.CreatedOn > after.CreatedOn)
                ),
                (ProjectOrderBy.CreatedOn, true) => qry.Where(x =>
                    x.CreatedOn > after.CreatedOn
                    || (x.CreatedOn == after.CreatedOn && x.Name.CompareTo(after.Name) > 0)
                ),
                (ProjectOrderBy.StartOn, true) => qry.Where(x =>
                    x.StartOn > after.StartOn
                    || (x.StartOn == after.StartOn && x.Name.CompareTo(after.Name) > 0)
                ),
                (ProjectOrderBy.EndOn, true) => qry.Where(x =>
                    x.EndOn > after.EndOn
                    || (x.EndOn == after.EndOn && x.Name.CompareTo(after.Name) > 0)
                ),
                (ProjectOrderBy.Name, false) => qry.Where(x =>
                    x.Name.CompareTo(after.Name) < 0
                    || (x.Name.CompareTo(after.Name) == 0 && x.CreatedOn > after.CreatedOn)
                ),
                (ProjectOrderBy.CreatedOn, false) => qry.Where(x =>
                    x.CreatedOn < after.CreatedOn
                    || (x.CreatedOn == after.CreatedOn && x.Name.CompareTo(after.Name) > 0)
                ),
                (ProjectOrderBy.StartOn, false) => qry.Where(x =>
                    x.StartOn < after.StartOn
                    || (x.StartOn == after.StartOn && x.Name.CompareTo(after.Name) > 0)
                ),
                (ProjectOrderBy.EndOn, false) => qry.Where(x =>
                    x.EndOn < after.EndOn
                    || (x.EndOn == after.EndOn && x.Name.CompareTo(after.Name) > 0)
                ),
            };
        }

        qry = (req.OrderBy, req.Asc) switch
        {
            (ProjectOrderBy.Name, true) => qry.OrderBy(x => x.Name).ThenBy(x => x.CreatedOn),
            (ProjectOrderBy.CreatedOn, true) => qry.OrderBy(x => x.CreatedOn).ThenBy(x => x.Name),
            (ProjectOrderBy.StartOn, true) => qry.OrderBy(x => x.StartOn).ThenBy(x => x.Name),
            (ProjectOrderBy.EndOn, true) => qry.OrderBy(x => x.EndOn).ThenBy(x => x.Name),
            (ProjectOrderBy.Name, false) => qry.OrderByDescending(x => x.Name)
                .ThenBy(x => x.CreatedOn),
            (ProjectOrderBy.CreatedOn, false) => qry.OrderByDescending(x => x.CreatedOn)
                .ThenBy(x => x.Name),
            (ProjectOrderBy.StartOn, false) => qry.OrderByDescending(x => x.StartOn)
                .ThenBy(x => x.Name),
            (ProjectOrderBy.EndOn, false) => qry.OrderByDescending(x => x.EndOn)
                .ThenBy(x => x.Name),
        };
        qry = qry.Take(101);
        var ps = await qry.ToListAsync(ctx.Ctkn);
        var ids = ps.Select(x => x.Id).ToList();
        var ts = await db
            .Tasks.Where(x => x.Org == req.Org && x.Project == x.Id && ids.Contains(x.Id))
            .ToListAsync(ctx.Ctkn);
        var set = ps.Select(x => x.ToApi(ts.Single(y => y.Id == x.Id))).ToList();
        return SetRes<Project>.FromLimit(set, 101);
    }

    private static async Task<Project> Update(IRpcCtx ctx, OakDb db, ISession ses, Update req)
    {
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Id,
            ProjectMemberRole.Admin
        );
        ctx.BadRequestIf(req.HoursPerDay < 1 || req.HoursPerDay > 24, S.ProjectInvalidHoursPerDay);
        ctx.BadRequestIf(req.DaysPerWeek < 1 || req.DaysPerWeek > 7, S.ProjectInvalidDaysPerWeek);
        var p = await db.Projects.SingleOrDefaultAsync(
            x => x.Org == req.Org && x.Id == req.Id,
            ctx.Ctkn
        );
        ctx.NotFoundIf(p == null, model: new { Name = "Project" });
        p.NotNull();
        p.Name = req.Name ?? p.Name;
        p.IsPublic = req.IsPublic ?? p.IsPublic;
        p.CurrencySymbol = req.CurrencySymbol ?? p.CurrencySymbol;
        p.CurrencyCode = req.CurrencyCode ?? p.CurrencyCode;
        p.HoursPerDay = req.HoursPerDay ?? p.HoursPerDay;
        p.DaysPerWeek = req.DaysPerWeek ?? p.DaysPerWeek;
        p.StartOn = req.StartOn ?? p.StartOn;
        p.EndOn = req.EndOn ?? p.EndOn;
        p.FileLimit = req.FileLimit ?? p.FileLimit;
        var t = await db.Tasks.SingleAsync(
            x => x.Org == req.Org && x.Project == req.Id && x.Id == req.Id,
            ctx.Ctkn
        );
        t.Name = p.Name;
        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            p.Org,
            p.Id,
            p.Id,
            p.Id,
            ActivityItemType.Project,
            ActivityAction.Update,
            p.Name,
            req,
            null
        );
        return p.ToApi(t);
    }

    private static async Task<Nothing> Delete(IRpcCtx ctx, OakDb db, ISession ses, Exact req)
    {
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Id,
            ProjectMemberRole.Admin
        );
        await db
            .Projects.Where(x => x.Org == req.Org && x.Id == req.Id)
            .ExecuteDeleteAsync(ctx.Ctkn);
        await db
            .ProjectLocks.Where(x => x.Org == req.Org && x.Id == req.Id)
            .ExecuteDeleteAsync(ctx.Ctkn);
        await db
            .Activities.Where(x => x.Org == req.Org && x.Project == req.Id)
            .ExecuteDeleteAsync(ctx.Ctkn);
        await db
            .ProjectMembers.Where(x => x.Org == req.Org && x.Project == req.Id)
            .ExecuteDeleteAsync(ctx.Ctkn);
        await db
            .Tasks.Where(x => x.Org == req.Org && x.Project == req.Id)
            .ExecuteDeleteAsync(ctx.Ctkn);
        await db
            .Timers.Where(x => x.Org == req.Org && x.Project == req.Id)
            .ExecuteDeleteAsync(ctx.Ctkn);
        await db
            .VItems.Where(x => x.Org == req.Org && x.Project == req.Id)
            .ExecuteDeleteAsync(ctx.Ctkn);
        await db
            .Files.Where(x => x.Org == req.Org && x.Project == req.Id)
            .ExecuteDeleteAsync(ctx.Ctkn);
        await db
            .Comments.Where(x => x.Org == req.Org && x.Project == req.Id)
            .ExecuteDeleteAsync(ctx.Ctkn);
        using var store = ctx.Get<IStoreClient>();
        await store.DeletePrefix(OrgEps.FilesBucket, string.Join("/", req.Org, req.Id), ctx.Ctkn);
        return Nothing.Inst;
    }

    private static async Task<SetRes<Activity>> GetActivities(IRpcCtx ctx, GetActivities req)
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
        var qry = db.Activities.Where(x => x.Org == req.Org && x.Project == req.Project);
        if (req.ExcludeDeletedItem)
        {
            qry = qry.Where(x => x.ItemDeleted == false);
        }

        if (req.Task != null)
        {
            qry = qry.Where(x => x.Task == req.Task);
        }

        if (req.Item != null)
        {
            qry = qry.Where(x => x.Item == req.Item);
        }

        if (req.User != null)
        {
            qry = qry.Where(x => x.User == req.User);
        }

        if (req.OccurredOn?.Min != null)
        {
            qry = qry.Where(x => x.OccurredOn >= req.OccurredOn.Min);
        }

        if (req.OccurredOn?.Max != null)
        {
            qry = qry.Where(x => x.OccurredOn <= req.OccurredOn.Max);
        }

        qry = req.Asc ? qry.OrderBy(x => x.OccurredOn) : qry.OrderByDescending(x => x.OccurredOn);
        var set = await qry.Take(101).Select(x => x.ToApi()).ToListAsync();
        return SetRes<Activity>.FromLimit(set, 101);
    }
}
