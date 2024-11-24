using Common.Server;
using Common.Shared;
using Common.Shared.Auth;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using Oak.Api;
using Oak.Api.Comment;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Db;
using Comment = Oak.Api.Comment.Comment;
using Create = Oak.Api.Comment.Create;
using Exact = Oak.Api.Comment.Exact;
using Get = Oak.Api.Comment.Get;
using Update = Oak.Api.Comment.Update;

namespace Oak.Eps;

internal static class CommentEps
{
    private const int minBodyLen = 1;
    private const int maxBodyLen = 10000;

    public static IReadOnlyList<IEp> Eps { get; } =
        new List<IEp>()
        {
            Ep<Create, Comment>.DbTx<OakDb>(CommentRpcs.Create, Create),
            Ep<Update, Comment>.DbTx<OakDb>(CommentRpcs.Update, Update),
            Ep<Exact, Nothing>.DbTx<OakDb>(CommentRpcs.Delete, Delete),
            new Ep<Get, SetRes<Comment>>(CommentRpcs.Get, Get)
        };

    private static async Task<Comment> Create(IRpcCtx ctx, OakDb db, ISession ses, Create req)
    {
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Writer
        );
        req.Body = ctx.Get<IHtmlSanitizer>().Sanitize(req.Body);
        EpsUtil.ValidStr(ctx, req.Body, minBodyLen, maxBodyLen, "body");
        ctx.NotFoundIf(
            !await db.Tasks.AnyAsync(
                x => x.Org == req.Org && x.Project == req.Project && x.Id == req.Task,
                ctx.Ctkn
            ),
            model: new { Name = "Task" }
        );
        var c = new Db.Comment()
        {
            Org = req.Org,
            Project = req.Project,
            Task = req.Task,
            Id = Id.New(),
            CreatedBy = ses.Id,
            CreatedOn = DateTimeExt.UtcNowMilli(),
            Body = req.Body
        };
        await db.Comments.AddAsync(c, ctx.Ctkn);
        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            req.Org,
            req.Project,
            req.Task,
            c.Id,
            ActivityItemType.Comment,
            ActivityAction.Create,
            null,
            null,
            null
        );
        return c.ToApi();
    }

    private static async Task<Comment> Update(IRpcCtx ctx, OakDb db, ISession ses, Update req)
    {
        req.Body = ctx.Get<IHtmlSanitizer>().Sanitize(req.Body);
        EpsUtil.ValidStr(ctx, req.Body, minBodyLen, maxBodyLen, "body");
        await EpsUtil.MustHaveProjectAccess(
            ctx,
            db,
            ses.Id,
            req.Org,
            req.Project,
            ProjectMemberRole.Writer
        );
        var c = await db.Comments.SingleOrDefaultAsync(
            x =>
                x.Org == req.Org
                && x.Project == req.Project
                && x.Task == req.Task
                && x.Id == req.Id
                && x.CreatedBy == ses.Id,
            ctx.Ctkn
        );
        ctx.NotFoundIf(c == null, model: new { Name = "Comment" });
        c.NotNull();
        c.Body = req.Body;
        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            req.Org,
            req.Project,
            req.Task,
            c.Id,
            ActivityItemType.Comment,
            ActivityAction.Update,
            null,
            req,
            null
        );
        return c.ToApi();
    }

    private static async Task<Nothing> Delete(IRpcCtx ctx, OakDb db, ISession ses, Exact req)
    {
        var c = await db.Comments.SingleOrDefaultAsync(
            x =>
                x.Org == req.Org
                && x.Project == req.Project
                && x.Task == req.Task
                && x.Id == req.Id,
            ctx.Ctkn
        );
        ctx.NotFoundIf(c == null, model: new { Name = "Comment" });
        c.NotNull();
        var requiredRole = ProjectMemberRole.Admin;
        if (c.CreatedBy == ses.Id)
        {
            // I can delete my own comments
            requiredRole = ProjectMemberRole.Writer;
        }

        await EpsUtil.MustHaveProjectAccess(ctx, db, ses.Id, req.Org, req.Project, requiredRole);
        db.Comments.Remove(c);
        await EpsUtil.LogActivity(
            ctx,
            db,
            ses,
            req.Org,
            req.Project,
            req.Task,
            c.Id,
            ActivityItemType.Comment,
            ActivityAction.Delete,
            null,
            null,
            null
        );
        return Nothing.Inst;
    }

    private static async Task<SetRes<Comment>> Get(IRpcCtx ctx, Get req)
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
        var qry = db.Comments.Where(x => x.Org == req.Org && x.Project == req.Project);
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
            var after = await db.Comments.SingleOrDefaultAsync(
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
        return SetRes<Comment>.FromLimit(res, 101);
    }
}
