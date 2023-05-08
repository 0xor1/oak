﻿using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api;
using Oak.Api.ProjectMember;
using Oak.Api.Task;
using Oak.Db;
using Create = Oak.Api.Task.Create;
using Task = Oak.Api.Task.Task;
using Exact = Oak.Api.Task.Exact;
using Update = Oak.Api.Task.Update;
using S = Oak.I18n.S;

namespace Oak.Eps;

internal static class TaskEps
{
    private const int NameMinLen = 1;
    private const int NameMaxLen = 250;
    private const int DescMinLen = 0;
    private const int DescMaxLen = 1250;

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Create, CreateRes>(
                TaskRpcs.Create,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, CreateRes>(
                        async (db, ses) =>
                        {
                            EpsUtil.ValidStr(
                                ctx,
                                req.Name,
                                NameMinLen,
                                NameMaxLen,
                                nameof(req.Name)
                            );
                            EpsUtil.ValidStr(
                                ctx,
                                req.Description,
                                DescMinLen,
                                DescMaxLen,
                                nameof(req.Description)
                            );
                            await EpsUtil.MustHaveProjectAccess(
                                ctx,
                                db,
                                ses.Id,
                                req.Org,
                                req.Project,
                                ProjectMemberRole.Writer
                            );
                            if (req.User != null && req.User != ses.Id)
                            {
                                // if Im assigning to someone that isnt me,
                                // validate that user has write access to this
                                // project
                                await EpsUtil.MustHaveProjectAccess(
                                    ctx,
                                    db,
                                    req.User,
                                    req.Org,
                                    req.Project,
                                    ProjectMemberRole.Writer
                                );
                            }
                            var t = new Db.Task()
                            {
                                Org = req.Org,
                                Project = req.Project,
                                Id = Id.New(),
                                Parent = req.Parent,
                                User = req.User,
                                Name = req.Name,
                                Description = req.Description,
                                CreatedBy = ses.Id,
                                CreatedOn = DateTimeExt.UtcNowMilli(),
                                TimeEst = req.TimeEst,
                                CostEst = req.CostEst,
                                IsParallel = req.IsParallel
                            };
                            await db.LockProject(req.Org, req.Project);
                            // get correct next sib value from either prevSib if
                            // specified or parent.FirstChild otherwise. Then update prevSibs nextSib value
                            // or parents firstChild value depending on the scenario.
                            Db.Task? prevSib = null;
                            Db.Task? parent = await db.Tasks.SingleOrDefaultAsync(
                                x =>
                                    x.Org == req.Org
                                    && x.Project == req.Project
                                    && x.Id == req.Parent
                            );
                            ctx.NotFoundIf(parent == null, model: new { Name = "Parent Task" });
                            if (req.PrevSib != null)
                            {
                                prevSib = await db.Tasks.SingleOrDefaultAsync(
                                    x =>
                                        x.Org == req.Org
                                        && x.Project == req.Project
                                        && x.Id == req.PrevSib
                                );
                                ctx.NotFoundIf(
                                    prevSib == null,
                                    model: new { Name = "PrevSib Task" }
                                );
                                ctx.BadRequestIf(prevSib.NotNull().Parent != req.Parent);
                                t.NextSib = prevSib.NextSib;
                                prevSib.NextSib = t.Id;
                            }
                            else
                            {
                                // else newTask is being inserted as firstChild, so set any current firstChild
                                // as newTask's NextSib
                                // get parent for updating child/descendant counts and firstChild if required
                                t.NextSib = parent.NotNull().FirstChild;
                                parent.FirstChild = t.Id;
                            }
                            // insert new task
                            await db.Tasks.AddAsync(t);
                            await db.SaveChangesAsync();
                            // at this point the tree structure has been updated so all tasks are pointing to the correct new positions
                            // all that remains to do is update aggregate values
                            var ancestors = await db.SetAncestralChainAggregateValuesFromTask(
                                req.Org,
                                req.Project,
                                req.Parent
                            );
                            await db.Entry(parent.NotNull()).ReloadAsync();
                            await EpsUtil.LogActivity(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                req.Project,
                                t.Id,
                                t.Id,
                                ActivityItemType.Task,
                                ActivityAction.Create,
                                t.Name,
                                null,
                                null,
                                ancestors
                            );
                            var p = await db.Tasks.SingleAsync(
                                x =>
                                    x.Org == req.Org
                                    && x.Project == req.Project
                                    && x.Id == req.Parent
                            );
                            return new CreateRes(p.ToApi(), t.ToApi());
                        }
                    )
            ),
            new RpcEndpoint<Update, UpdateRes>(
                TaskRpcs.Update,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, UpdateRes>(
                        async (db, ses) =>
                        {
                            if (req.Name != null)
                            {
                                EpsUtil.ValidStr(
                                    ctx,
                                    req.Name,
                                    NameMinLen,
                                    NameMaxLen,
                                    nameof(req.Name)
                                );
                            }
                            if (req.Description != null)
                            {
                                EpsUtil.ValidStr(
                                    ctx,
                                    req.Description,
                                    DescMinLen,
                                    DescMaxLen,
                                    nameof(req.Description)
                                );
                            }
                            var requiredPerm = ProjectMemberRole.Writer;
                            if (req.Project == req.Id)
                            {
                                // updating root node project task
                                requiredPerm = ProjectMemberRole.Admin;
                                ctx.BadRequestIf(req.Parent != null || req.PrevSib != null, S.TaskCantSetParentOrPrevSibOnRootProjectNode);
                            }
                            await EpsUtil.MustHaveProjectAccess(
                                ctx,
                                db,
                                ses.Id,
                                req.Org,
                                req.Project,
                                requiredPerm
                            );
                            if (req.User != null && req.User.V != null && req.User.V != ses.Id)
                            {
                                // if Im assigning to someone that isnt me,
                                // validate that user has write access to this
                                // project
                                await EpsUtil.MustHaveProjectAccess(
                                    ctx,
                                    db,
                                    req.User.V, 
                                    req.Org,
                                    req.Project,
                                    ProjectMemberRole.Writer
                                );
                            }

                            var treeUpdateRequired = false;
                            var simpleUpdateRequired = false;
                            if (req.Parent != null ||
                                req.PrevSib != null ||
                                req.CostEst != null ||
                                req.TimeEst != null ||
                                req.IsParallel != null)
                            {
                                // if moving the task or setting an aggregate value effecting property
                                // we must lock
                                await db.LockProject(req.Org, req.Project);
                            }
                            var t = await db.Tasks.SingleOrDefaultAsync(
                                x =>
                                    x.Org == req.Org
                                    && x.Project == req.Project
                                    && x.Id == req.Id
                            );
                            ctx.NotFoundIf(t == null, model: new { Name = "Task" });
                            t.NotNull();
                            Db.Task? newParent = null;
                            Db.Task? newPrevSib = null;
                            Db.Task? oldParent = null;
                            Db.Task? oldPrevSib = null;
                            if (req.Parent == t.Parent)
                            {
                                // if move parent is specified but its to the existing parent,
                                // just null it out as it effects nothing
                                req = req with { Parent = null };
                            }
                            if (req.Parent != null)
                            {
                                string? newNextSib = null;
                                // changing parent
                                newParent = await db.Tasks.SingleOrDefaultAsync(x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Parent);
                                ctx.NotFoundIf(newParent == null, model: new { Name = "Parent Task" });
                                newParent.NotNull();
                                var tFromAncestors = await db.Tasks.FromSql(RecursiveLoopDetectionQry(req.Org, req.Project, req.Id)).SingleOrDefaultAsync();
                                ctx.BadRequestIf(tFromAncestors != null || t.Id == req.Parent, S.TaskRecursiveLoopDetected);
                                if (req.PrevSib != null && req.PrevSib.V != null)
                                {
                                    ctx.BadRequestIf(req.PrevSib.V == t.Id, S.TaskRecursiveLoopDetected);
                                    newPrevSib = await db.Tasks.SingleOrDefaultAsync(x =>
                                        x.Org == req.Org && x.Project == req.Project && x.Id == req.PrevSib.V);
                                    ctx.NotFoundIf(newPrevSib == null, model: new {Name = "Previous Sibling"});
                                    ctx.BadRequestIf(newPrevSib.NotNull().Parent != newParent.Id, S.TaskMovePrevSibParentMismatch);
                                    newNextSib = newPrevSib.NextSib;
                                    newPrevSib.NextSib = t.Id;
                                }
                                else
                                {
                                    newNextSib = newParent.FirstChild;
                                    newParent.FirstChild = t.Id;
                                }
                                // need to reconnect oldPrevSib with oldNextSib
                                if (newParent.NextSib != null && newParent.NextSib == t.Id)
                                {
                                    // !!!SPECIAL CASE!!! oldPrevSib may be newParent
                                    oldPrevSib = newParent;
                                }
                                else
                                {
                                    oldPrevSib = await db.Tasks.SingleOrDefaultAsync(x => x.Org == req.Org && x.Project == req.Project && x.NextSib == t.Id);
                                }
                                // need to get old parent for ancestor value updates
                                if (newPrevSib != null && newPrevSib.Id == t.Parent)
                                {
                                    // !!!SPECIAL CASE!!! oldParent may be the newPrevSib
                                    oldParent = newPrevSib;
                                }
                                else
                                {
                                    oldParent = await db.Tasks.SingleOrDefaultAsync(x =>
                                        x.Org == req.Org && x.Project == req.Project && x.Id == t.Parent);
                                }
                                ctx.NotFoundIf(oldParent == null, model: new {Name = "Old Parent Task"});
                                if (oldPrevSib != null)
                                {
                                    oldPrevSib.NextSib = t.NextSib;
                                }
                                else
                                {
                                    // need to update oldParent firstChild as t is it
                                    oldParent.FirstChild = t.NextSib;
                                }

                                t.Parent = newParent.Id;
                                t.NextSib = newNextSib;
                                treeUpdateRequired = true;
                            }

                            if (req.Parent == null && req.PrevSib != null)
                            {
                                // we now know we are doing a purely horizontal move, i.e. not changing parent node
                                // get oldPrevSib
                                oldPrevSib = await db.Tasks.SingleOrDefaultAsync(x =>
                                    x.Org == req.Org && x.Project == req.Project && x.NextSib == t.Id);
                                if (!((oldPrevSib == null && req.PrevSib.V == null) ||
                                      (oldPrevSib != null && req.PrevSib.V != null &&
                                       oldPrevSib.Id == req.PrevSib.V)))
                                {
                                    string? newNextSib = null;
                                    // here we know that an actual change is being attempted
                                    oldParent = await db.Tasks.SingleOrDefaultAsync(x => x.Org == req.Org && x.Project == req.Project && x.Id == t.Parent);
                                    ctx.NotFoundIf(oldParent == null, model: new {Name = "Old Parent"});
                                    oldParent.NotNull();
                                    if (req.PrevSib.V != null)
                                    {
                                        // moving to a non first child position
                                        if (oldParent.FirstChild == t.Id)
                                        {
                                            //moving the old first child away so need to repoint oldParent.firstChild
                                            oldParent.FirstChild = t.NextSib;
                                        }
                                        else
                                        {
                                            // not moving first child therefore nil out oldParent to
                                            // save an update query
                                            oldParent = null;
                                        }
                                        ctx.BadRequestIf(req.PrevSib.V == t.Id, S.TaskRecursiveLoopDetected);
                                        newPrevSib = await db.Tasks.SingleOrDefaultAsync(x =>
                                            x.Org == req.Org && x.Project == req.Project && x.Id == req.PrevSib.V);
                                        ctx.NotFoundIf(newPrevSib == null, model: new {Name = "Previous Sibling"});
                                        newPrevSib.NotNull();
                                        ctx.BadRequestIf(newPrevSib.Parent != t.Parent, S.TaskMovePrevSibParentMismatch);
                                        newNextSib = newPrevSib.NextSib;
                                        newPrevSib.NextSib = t.Id;
                                    }
                                    else
                                    {
                                        // moving to first child position
                                        newNextSib = oldParent.FirstChild;
                                        oldParent.FirstChild = t.Id;
                                    }
                                    // need to reconnect oldPrevSib with oldNextSib
                                    if (oldPrevSib != null)
                                    {
                                        oldPrevSib.NextSib = t.NextSib;
                                    }

                                    t.NextSib = newNextSib;
                                    treeUpdateRequired = true;
                                }
                                else
                                {
                                    // here we know no change is being made so ensure everything that should be null is
                                    oldPrevSib = null;
                                    req = req with {PrevSib = null};
                                }
                                // at this point all the moving has been done
                                var nameUpdated = false;
                            }
                            
                            await db.SaveChangesAsync();
                            // at this point the tree structure has been updated so all tasks are pointing to the correct new positions
                            // all that remains to do is update aggregate values
                            var ancestors = await db.SetAncestralChainAggregateValuesFromTask(
                                req.Org,
                                req.Project,
                                req.Parent
                            );
                            await db.Entry(parent.NotNull()).ReloadAsync();
                            await EpsUtil.LogActivity(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                req.Project,
                                t.Id,
                                t.Id,
                                ActivityItemType.Task,
                                ActivityAction.Create,
                                t.Name,
                                req,
                                null,
                                ancestors
                            );
                            var p = await db.Tasks.SingleAsync(
                                x =>
                                    x.Org == req.Org
                                    && x.Project == req.Project
                                    && x.Id == req.Parent
                            );
                            return new UpdateRes(t.ToApi(), t.ToApi(), t.ToApi());
                        }
                    )
            ),
            new RpcEndpoint<Exact, Task>(
                TaskRpcs.GetOne,
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
                    var t = await db.Tasks.SingleOrDefaultAsync(
                        x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id
                    );
                    ctx.NotFoundIf(t == null, model: new { Name = "Task" });
                    return t.NotNull().ToApi();
                }
            ),
            new RpcEndpoint<Exact, SetRes<Task>>(
                TaskRpcs.GetAncestors,
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
                    var t = await db.Tasks.SingleOrDefaultAsync(
                        x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id
                    );
                    ctx.NotFoundIf(t == null, model: new { Name = "Task" });
                    var res = await db.Tasks
                        .FromSql(AncestorsQry(req.Org, req.Project, req.Id, 101))
                        .ToListAsync();
                    return SetRes<Task>.FromLimit(res.Select(x => x.ToApi()).ToList(), 101);
                }
            ),
            new RpcEndpoint<GetChildren, SetRes<Task>>(
                TaskRpcs.GetChildren,
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
                    var t = await db.Tasks.SingleOrDefaultAsync(
                        x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id
                    );
                    ctx.NotFoundIf(t == null, model: new { Name = "Task" });
                    FormattableString? qry = null;
                    if (req.After == null)
                    {
                        qry = ChildrenQry(req.Org, req.Project, req.Id, 101);
                    }
                    else
                    {
                        qry = SiblingsQry(req.Org, req.Project, req.After.NotNull(), 101);
                    }

                    var res = await db.Tasks.FromSql(qry).ToListAsync();
                    return SetRes<Task>.FromLimit(res.Select(x => x.ToApi()).ToList(), 101);
                }
            ),
            new RpcEndpoint<Exact, InitView>(
                TaskRpcs.GetInitView,
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
                    var t = await db.Tasks.SingleOrDefaultAsync(
                        x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id
                    );
                    ctx.NotFoundIf(t == null, model: new { Name = "Task" });
                    var children = await db.Tasks.FromSql(ChildrenQry(req.Org, req.Project, req.Id, 101)).Select(x => x.ToApi()).ToListAsync();
                    var ancestors = await db.Tasks.FromSql(AncestorsQry(req.Org, req.Project, req.Id, 101)).Select(x => x.ToApi()).ToListAsync();
                    return new InitView(t.NotNull().ToApi(), SetRes<Task>.FromLimit(children, 101), SetRes<Task>.FromLimit(ancestors, 101));
                }
            ),
            new RpcEndpoint<Exact, IReadOnlyList<Task>>(
                TaskRpcs.GetAllDescendants,
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
                    var t = await db.Tasks.SingleOrDefaultAsync(
                        x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Id
                    );
                    ctx.NotFoundIf(t == null, model: new { Name = "Task" });
                    ctx.BadRequestIf(t.NotNull().DescN > 1000, S.TaskTooManyDescN);
                    if (t.DescN == 0)
                    {
                        return new List<Task>();
                    }
                    return await db.Tasks.FromSql(DescendantsQry(req.Org, req.Project, req.Id)).Select(x => x.ToApi()).ToListAsync();
                }
            ),
        };

    private static FormattableString AncestorsQry(
        string org,
        string project,
        string id,
        int limit
    ) =>
        $"""
WITH RECURSIVE Ancestors (N, Id) 
AS (
    SELECT 0,
        Parent 
    FROM Tasks 
    WHERE Org={org} 
    AND Project={project} 
    AND Id={id} 
    UNION 
    SELECT a.N + 1, 
        t.Parent 
    FROM Tasks t, Ancestors a 
    WHERE t.Org={org} 
    AND t.Project={project} 
    AND t.Id = a.Id
) CYCLE Id RESTRICT
SELECT * 
FROM Tasks t 
JOIN Ancestors a ON t.Id = a.Id 
WHERE t.Org={org} 
AND t.Project={project} 
ORDER BY a.N ASC 
LIMIT {limit}
""";

    private static FormattableString ChildrenQry(
        string org,
        string project,
        string id,
        int limit
    ) =>
        $"""
WITH RECURSIVE Sibs (N, Id) 
AS (
    SELECT 0 AS N,
        FirstChild AS Id 
    FROM Tasks 
    WHERE Org={org} 
    AND Project={project} 
    AND Id={id} 
    UNION
    SELECT s.N + 1 AS N,
        t.NextSib AS Id 
    FROM Tasks t, Sibs s 
    WHERE t.Org={org} 
    AND t.Project={project} 
    AND t.Id = s.Id) CYCLE Id RESTRICT
SELECT * 
FROM Tasks t 
JOIN Sibs s ON t.Id = s.Id 
WHERE t.Org={org} AND t.Project={project} 
ORDER BY s.N ASC
LIMIT {limit}
""";

    private static FormattableString SiblingsQry(
        string org,
        string project,
        string id,
        int limit
    ) =>
        $"""
WITH RECURSIVE Sibs (N, Id) 
AS (
    SELECT 0 AS N,
        NextSib AS Id 
    FROM Tasks 
    WHERE Org={org} 
    AND Project={project} 
    AND Id={id} 
    UNION
    SELECT s.N + 1 AS N,
        t.NextSib AS Id 
    FROM Tasks t, Sibs s 
    WHERE t.Org={org} 
    AND t.Project={project} 
    AND t.Id = s.Id) CYCLE Id RESTRICT
SELECT * 
FROM Tasks t 
JOIN Sibs s ON t.Id = s.Id 
WHERE t.Org={org} AND t.Project={project} 
ORDER BY s.N ASC
LIMIT {limit}
""";

    private static FormattableString DescendantsQry(
        string org,
        string project,
        string id
    ) =>
        $"""
WITH RECURSIVE Nodes (Id) 
AS (
    SELECT Id 
    FROM Tasks 
    WHERE Org={org} 
    AND Project={project}
    AND Parent={id} 
    UNION 
    SELECT t.Id 
    FROM Tasks t 
    JOIN Nodes n ON t.Parent = n.Id 
    WHERE t.Org={org}
    AND t.Project={project}
) CYCLE id RESTRICT 
SELECT * 
FROM Tasks t 
JOIN Nodes n ON t.Id = n.Id 
WHERE t.Org={org} 
AND t.Project={project}
""";

    private static FormattableString RecursiveLoopDetectionQry(
        string org,
        string project,
        string id
    ) =>
        $"""
WITH RECURSIVE Ancestors (N, Id) 
AS (
    SELECT 0,
        Parent 
    FROM Tasks 
    WHERE Org={org} 
    AND Project={project} 
    AND Id={id} 
    UNION 
    SELECT a.N + 1, 
        t.Parent 
    FROM Tasks t, Ancestors a 
    WHERE t.Org={org} 
    AND t.Project={project} 
    AND t.Id = a.Id
) CYCLE Id RESTRICT
SELECT * FROM Tasks t 
JOIN Ancestors a ON t.Id = a.Id 
WHERE t.Org={org} 
AND t.Project={project}
AND t.Id={id}
""";
}
