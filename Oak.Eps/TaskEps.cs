using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api;
using Oak.Api.ProjectMember;
using Oak.Api.Task;
using Oak.Db;
using Create = Oak.Api.Task.Create;
using Task = Oak.Api.Task.Task;

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
        };
}
