using System.Net;
using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Db;
using S = Oak.I18n.S;
using Get = Oak.Api.Project.Get;
using Project = Oak.Api.Project.Project;

namespace Oak.Eps;

internal static class ProjectEps
{
    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Create, Project>(
                ProjectRpcs.Create,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, Project>(
                        async (db, ses) =>
                        {
                            await EpsUtil.MustHaveOrgAccess(
                                ctx,
                                db,
                                ses,
                                req.Org,
                                OrgMemberRole.Admin
                            );
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
                                FileLimit = req.FileLimit
                            };
                            await db.Projects.AddAsync(p);
                            await db.ProjectLocks.AddAsync(new() { Org = req.Org, Id = p.Id });
                            var t = new Db.Task()
                            {
                                Org = req.Org,
                                Project = p.Id,
                                Id = p.Id,
                                User = ses.Id,
                                Name = p.Name,
                                CreatedBy = ses.Id,
                                CreatedOn = DateTimeExt.UtcNowMilli()
                            };
                            await db.Tasks.AddAsync(t);
                            await db.ProjectMembers.AddAsync(
                                new()
                                {
                                    Org = req.Org,
                                    Project = p.Id,
                                    Id = ses.Id,
                                    Role = ProjectMemberRole.Admin
                                }
                            );
                            return p.ToApi(t);
                        }
                    )
            ),
            new RpcEndpoint<Get, IReadOnlyList<Project>>(
                ProjectRpcs.Get,
                async (ctx, req) =>
                {
                    var ses = ctx.GetSession();
                    var db = ctx.Get<OakDb>();
                    if (req.Id != null)
                    {
                        // requesting a specific project
                        await EpsUtil.MustHaveProjectAccess(
                            ctx,
                            db,
                            ses,
                            req.Org,
                            req.Id,
                            ProjectMemberRole.Reader
                        );
                        var res = await db.Projects
                            .Where(x => x.Org == req.Org && x.Id == req.Id)
                            .ToListAsync();
                        ctx.ErrorIf(!res.Any(), S.NoMatchingRecord, null, HttpStatusCode.NotFound);
                        var t = await db.Tasks.SingleAsync(
                            x => x.Org == req.Org && x.Project == req.Id && x.Id == req.Id
                        );
                        return res.Select(x => x.ToApi(t)).ToList();
                    }

                    var orgMemRole = await EpsUtil.OrgRole(db, ses, req.Org);
                    ctx.ErrorIf(
                        !req.IsPublic && orgMemRole == null,
                        S.InsufficientPermission,
                        null,
                        HttpStatusCode.Forbidden
                    );

                    var qry = db.Projects.Where(
                        x =>
                            x.Org == req.Org
                            && x.IsArchived == req.IsArchived
                            && x.IsPublic == req.IsPublic
                    );
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

                    if (!req.IsPublic && orgMemRole > OrgMemberRole.ReadAllProjects)
                    {
                        // req is for private projects and the user has per project permissions access
                        var projectIds = await db.ProjectMembers
                            .Where(x => x.Org == req.Org && x.Id == ses.Id)
                            .Select(x => x.Project)
                            .Distinct()
                            .ToListAsync();
                        qry = qry.Where(x => projectIds.Contains(x.Id));
                    }

                    qry = (req.OrderBy, req.Asc) switch
                    {
                        (ProjectOrderBy.Name, true)
                            => qry.OrderBy(x => x.Name).ThenBy(x => x.CreatedOn),
                        (ProjectOrderBy.CreatedOn, true)
                            => qry.OrderBy(x => x.CreatedOn).ThenBy(x => x.Name),
                        (ProjectOrderBy.StartOn, true)
                            => qry.OrderBy(x => x.StartOn).ThenBy(x => x.Name),
                        (ProjectOrderBy.EndOn, true)
                            => qry.OrderBy(x => x.EndOn).ThenBy(x => x.Name),
                        (ProjectOrderBy.Name, false)
                            => qry.OrderByDescending(x => x.Name).ThenBy(x => x.CreatedOn),
                        (ProjectOrderBy.CreatedOn, false)
                            => qry.OrderByDescending(x => x.CreatedOn).ThenBy(x => x.Name),
                        (ProjectOrderBy.StartOn, false)
                            => qry.OrderByDescending(x => x.StartOn).ThenBy(x => x.Name),
                        (ProjectOrderBy.EndOn, false)
                            => qry.OrderByDescending(x => x.EndOn).ThenBy(x => x.Name),
                    };
                    var ps = await qry.ToListAsync();
                    var ids = ps.Select(x => x.Id).ToList();
                    var ts = await db.Tasks
                        .Where(x => x.Org == req.Org && x.Project == x.Id && ids.Contains(x.Id))
                        .ToListAsync();
                    return ps.Select(x => x.ToApi(ts.Single(y => y.Id == x.Id))).ToList();
                }
            )
        };
}
