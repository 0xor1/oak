using Common.Server;
using Common.Shared;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Oak.Api;
using Oak.Api.File;
using Oak.Api.ProjectMember;
using Oak.Api.VItem;
using Oak.Db;
using S = Oak.I18n.S;
using Upload = Oak.Api.File.Upload;
using Download = Oak.Api.File.Download;
using FileRes = Oak.Api.File.FileRes;
using Exact = Oak.Api.File.Exact;
using Get = Oak.Api.File.Get;

namespace Oak.Eps;

internal static class FileEps
{
    private const int nameMinLen = 1;
    private const int nameMaxLen = 250;
    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Upload, FileRes>(
                FileRpcs.Upload,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, FileRes>(
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
                            EpsUtil.ValidStr(ctx, req.Stream.Name, nameMinLen, nameMaxLen, "Name");
                            var t = await db.Tasks.SingleOrDefaultAsync(
                                x =>
                                    x.Org == req.Org && x.Project == req.Project && x.Id == req.Task
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
                                Type = req.Stream.Type
                            };
                            await db.Files.AddAsync(f);
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
                                f.Id,
                                ActivityItemType.File,
                                ActivityAction.Create,
                                f.Name,
                                new { Size = f.Size, Type = f.Type },
                                null,
                                ancestors
                            );

                            var store = ctx.Get<IStoreClient>();
                            await store.Upload(
                                OrgEps.FilesBucket,
                                string.Join("/", req.Org, req.Project, req.Task, f.Id),
                                req.Stream.Data
                            );

                            return new FileRes(t.ToApi(), f.ToApi());
                        }
                    )
            ),
            new RpcEndpoint<Download, HasStream>(
                FileRpcs.Download,
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
                    var f = await db.Files.SingleOrDefaultAsync(
                        x =>
                            x.Org == req.Org
                            && x.Project == req.Project
                            && x.Task == req.Task
                            && x.Id == req.Id
                    );
                    ctx.NotFoundIf(f == null, model: new { Name = "File" });
                    f.NotNull();
                    var store = ctx.Get<IStoreClient>();
                    var data = await store.Download(
                        OrgEps.FilesBucket,
                        string.Join("/", f.Org, f.Project, f.Task, f.Id)
                    );

                    return new HasStream()
                    {
                        Stream = new RpcStream(data, f.Name, f.Type, req.IsDownload, f.Size)
                    };
                }
            )
        };
}
