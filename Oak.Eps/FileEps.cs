using Common.Server;
using Common.Shared;
using Common.Shared.Auth;
using Microsoft.EntityFrameworkCore;
using Oak.Api.File;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Db;
using Download = Oak.Api.File.Download;
using Exact = Oak.Api.File.Exact;
using File = Oak.Api.File.File;
using FileRes = Oak.Api.File.FileRes;
using Get = Oak.Api.File.Get;
using S = Oak.I18n.S;
using Task = Oak.Api.Task.Task;
using Upload = Oak.Api.File.Upload;

namespace Oak.Eps;

internal static class FileEps
{
    private const int nameMinLen = 1;
    private const int nameMaxLen = 250;
    public static IReadOnlyList<IEp> Eps { get; } =
        new List<IEp>()
        {
            Ep<Upload, FileRes>.DbTx<OakDb>(FileRpcs.Upload, Upload),
            new Ep<Download, HasStream>(FileRpcs.Download, Download),
            Ep<Exact, Api.Task.Task>.DbTx<OakDb>(FileRpcs.Delete, Delete),
            new Ep<Get, SetRes<File>>(FileRpcs.Get, Get),
        };

    private static async Task<FileRes> Upload(IRpcCtx ctx, OakDb db, ISession ses, Upload req)
    {
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Writer
        );
        EpsUtil.ValidStr(ctx, req.Stream.Name, nameMinLen, nameMaxLen, "Name");
        await db.LockProject(req.Org, req.Project, ctx.Ctkn);
        var p = await db.Projects.SingleAsync(
            x => x.Org == req.Org && x.Id == req.Project,
            ctx.Ctkn
        );
        var pt = await db.Tasks.SingleAsync(
            x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Project,
            ctx.Ctkn
        );
        ctx.BadRequestIf(
            pt.FileSize + pt.FileSubSize + req.Stream.Size > p.FileLimit,
            S.ProjectFileLimitExceeded,
            new { p.FileLimit }
        );
        var t = await db.Tasks.SingleOrDefaultAsync(
            x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Task,
            ctx.Ctkn
        );
        ctx.NotFoundIf(t == null, model: new { Name = "Task" });
        t.NotNull();
        t.FileN++;
        t.FileSize += req.Stream.Size;
        var f = new Db.File()
        {
            Org = req.Org,
            Project = req.Project,
            Task = req.Task,
            Id = Id.New(),
            Name = req.Stream.Name,
            CreatedBy = ses.Id,
            CreatedOn = DateTimeExt.UtcNowMilli(),
            Size = req.Stream.Size,
            Type = req.Stream.Type,
        };
        await db.Files.AddAsync(f, ctx.Ctkn);
        await db.SaveChangesAsync(ctx.Ctkn);
        List<string>? ancestors = null;
        if (t.Parent != null)
        {
            ancestors = await db.SetAncestralChainAggregateValuesFromTask(
                req.Org,
                req.Project,
                t.Parent,
                ctx.Ctkn
            );
        }

        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            req.Org,
            req.Project,
            req.Task,
            f.Id,
            ActivityItemType.File,
            ActivityAction.Create,
            f.Name,
            new { f.Size, f.Type },
            ancestors
        );

        var store = ctx.Get<IStoreClient>();
        await store.Upload(
            OrgEps.FilesBucket,
            string.Join("/", req.Org, req.Project, req.Task, f.Id),
            req.Stream.Type,
            req.Stream.Size,
            req.Stream.Data,
            ctx.Ctkn
        );

        return new FileRes(t.ToApi(), f.ToApi());
    }

    private static async Task<HasStream> Download(IRpcCtx ctx, Download req)
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
        var f = await db.Files.SingleOrDefaultAsync(
            x =>
                x.Org == req.Org
                && x.Project == req.Project
                && x.Task == req.Task
                && x.Id == req.Id,
            ctx.Ctkn
        );
        ctx.NotFoundIf(f == null, model: new { Name = "File" });
        f.NotNull();
        var store = ctx.Get<IStoreClient>();
        var data = await store.Download(
            OrgEps.FilesBucket,
            string.Join("/", f.Org, f.Project, f.Task, f.Id),
            ctx.Ctkn
        );

        return new HasStream()
        {
            Stream = new RpcStream(data, f.Name, f.Type, req.IsDownload, f.Size),
        };
    }

    private static async Task<Task> Delete(IRpcCtx ctx, OakDb db, ISession ses, Exact req)
    {
        await db.LockProject(req.Org, req.Project, ctx.Ctkn);
        var f = await db.Files.SingleOrDefaultAsync(
            x =>
                x.Org == req.Org
                && x.Project == req.Project
                && x.Task == req.Task
                && x.Id == req.Id,
            ctx.Ctkn
        );
        ctx.NotFoundIf(f == null, model: new { Name = "File" });
        f.NotNull();
        var requiredRole = ProjectMemberRole.Admin;
        if (f.CreatedBy == ses.Id && f.CreatedOn.Add(TimeSpan.FromHours(1)) > DateTime.UtcNow)
        {
            // if i created it in the last hour I only need to be a writer
            requiredRole = ProjectMemberRole.Writer;
        }

        await EpsUtil.MustHaveProjectAccess(ctx, db, ses.Id, req.Org, req.Project, requiredRole);
        db.Files.Remove(f);
        var t = await db.Tasks.SingleOrDefaultAsync(
            x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Task,
            ctx.Ctkn
        );
        ctx.NotFoundIf(t == null, model: new { Name = "Task" });
        t.NotNull();
        t.FileN--;
        t.FileSize -= f.Size;
        await db.SaveChangesAsync(ctx.Ctkn);
        List<string>? ancestors = null;
        if (t.Parent != null)
        {
            ancestors = await db.SetAncestralChainAggregateValuesFromTask(
                req.Org,
                req.Project,
                t.Parent,
                ctx.Ctkn
            );
        }

        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            req.Org,
            req.Project,
            req.Task,
            f.Id,
            ActivityItemType.File,
            ActivityAction.Delete,
            f.Name,
            new { f.Size, f.Type },
            ancestors
        );

        return t.ToApi();
    }

    private static async Task<SetRes<File>> Get(IRpcCtx ctx, Get req)
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
        var qry = db.Files.Where(x => x.Org == req.Org && x.Project == req.Project);
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
            var after = await db.Files.SingleOrDefaultAsync(
                x => x.Org == req.Org && x.Project == req.Project && x.Id == req.After,
                ctx.Ctkn
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

        var res = await qry.Take(101).Select(x => x.ToApi()).ToListAsync(ctx.Ctkn);
        return SetRes<File>.FromLimit(res, 101);
    }
}
