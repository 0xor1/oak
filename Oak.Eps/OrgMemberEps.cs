using System.Net;
using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Db;
using OrgMember = Oak.Api.OrgMember.OrgMember;
using S = Oak.I18n.S;

namespace Oak.Eps;

internal static class OrgMemberEps
{
    public static IReadOnlyList<IRpcEndpoint> Eps { get; } =
        new List<IRpcEndpoint>()
        {
            new RpcEndpoint<Add, OrgMember>(
                OrgMemberRpcs.Add,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, OrgMember>(
                        async (db, ses) =>
                        {
                            // check current member has sufficient permissions
                            var sesOrgMem = await db.OrgMembers
                                .Where(x => x.Org == req.Org && x.IsActive && x.Id == ses.Id)
                                .SingleOrDefaultAsync();
                            ctx.ErrorIf(
                                sesOrgMem == null,
                                S.InsufficientPermission,
                                null,
                                HttpStatusCode.Forbidden
                            );
                            var sesRole = sesOrgMem.NotNull().Role;
                            ctx.ErrorIf(
                                sesRole is not (OrgMemberRole.Owner or OrgMemberRole.Admin)
                                    || (
                                        sesRole is OrgMemberRole.Admin
                                        && req.Role == OrgMemberRole.Owner
                                    ),
                                S.InsufficientPermission,
                                null,
                                HttpStatusCode.Forbidden
                            );
                            // check new member is an active user
                            var newMemExists = await db.Auths.AnyAsync(
                                x => x.Id == req.Id && x.ActivatedOn != DateTimeExt.Zero()
                            );
                            ctx.ErrorIf(
                                !newMemExists,
                                S.NoMatchingRecord,
                                null,
                                HttpStatusCode.NotFound
                            );
                            var newMem = new Db.OrgMember()
                            {
                                Org = req.Org,
                                Id = req.Id,
                                IsActive = true,
                                Name = req.Name,
                                Role = req.Role
                            };
                            await db.OrgMembers.AddAsync(newMem);
                            return newMem.ToApi();
                        }
                    )
            ),
            new RpcEndpoint<Get, IReadOnlyList<OrgMember>>(
                OrgMemberRpcs.Get,
                async (ctx, req) =>
                {
                    var ses = ctx.GetAuthedSession();
                    var db = ctx.Get<OakDb>();
                    // check current member has sufficient permissions
                    var isActiveMember = await db.OrgMembers.AnyAsync(
                        x => x.Org == req.Org && x.IsActive && x.Id == ses.Id
                    );
                    ctx.ErrorIf(
                        !isActiveMember,
                        S.InsufficientPermission,
                        null,
                        HttpStatusCode.Forbidden
                    );
                    var qry = db.OrgMembers.Where(x => x.Org == req.Org);
                    // filters
                    if (req.Member != null)
                    {
                        qry = qry.Where(x => x.Id == req.Member);
                    }
                    else
                    {
                        qry = qry.Where(x => x.IsActive == req.IsActive);
                        if (req.Role != null)
                        {
                            qry = qry.Where(x => x.Role == req.Role);
                        }

                        if (!req.NameStartsWith.IsNullOrWhiteSpace())
                        {
                            qry = qry.Where(x => x.Name.StartsWith(req.NameStartsWith));
                        }

                        qry = (req.OrderBy, req.Asc) switch
                        {
                            (OrgMemberOrderBy.Name, true)
                                => qry.OrderBy(x => x.Name),
                            (OrgMemberOrderBy.IsActive, true)
                                => qry.OrderBy(x => x.IsActive).ThenBy(x => x.Name),
                            (OrgMemberOrderBy.Role, true)
                                => qry.OrderBy(x => x.Role).ThenBy(x => x.Name),
                            (OrgMemberOrderBy.Name, false)
                                => qry.OrderByDescending(x => x.Name),
                            (OrgMemberOrderBy.IsActive, false)
                                => qry.OrderByDescending(x => x.IsActive)
                                    .ThenByDescending(x => x.Name),
                            (OrgMemberOrderBy.Role, false)
                                => qry.OrderByDescending(x => x.Role).ThenByDescending(x => x.Name),
                        };
                    }
                    return await qry.Select(x => x.ToApi()).ToListAsync();
                }
            ),
            new RpcEndpoint<Update, OrgMember>(
                OrgMemberRpcs.Update,
                async (ctx, req) =>
                    await ctx.DbTx<OakDb, OrgMember>(
                        async (db, ses) =>
                        {
                            // check current member has sufficient permissions
                            var sesOrgMem = await db.OrgMembers
                                .Where(x => x.Org == req.Org && x.IsActive && x.Id == ses.Id)
                                .SingleOrDefaultAsync();
                            // msut be a member
                            ctx.ErrorIf(
                                sesOrgMem == null,
                                S.InsufficientPermission,
                                null,
                                HttpStatusCode.Forbidden
                            );
                            var sesRole = sesOrgMem.NotNull().Role;
                            // must be an owner or admin, and admins cant make members owners
                            ctx.ErrorIf(
                                sesRole is not (OrgMemberRole.Owner or OrgMemberRole.Admin)
                                    || (
                                        sesRole is OrgMemberRole.Admin
                                        && req.NewRole == OrgMemberRole.Owner
                                    ),
                                S.InsufficientPermission,
                                null,
                                HttpStatusCode.Forbidden
                            );
                            var updateMem = await db.OrgMembers.SingleOrDefaultAsync(
                                x => x.Org == req.Org && x.Id == req.Id
                            );
                            // update target must exist
                            ctx.ErrorIf(
                                updateMem == null,
                                S.NoMatchingRecord,
                                null,
                                HttpStatusCode.NotFound
                            );
                            updateMem.NotNull();
                            // cant update a member with high permissions than you
                            ctx.ErrorIf(
                                updateMem.Role < sesRole,
                                S.InsufficientPermission,
                                null,
                                HttpStatusCode.Forbidden
                            );
                            if (
                                updateMem is { IsActive: true, Role: OrgMemberRole.Owner }
                                && (
                                    (req.NewRole != null && req.NewRole != OrgMemberRole.Owner)
                                    || req.IsActive is false
                                )
                            )
                            {
                                // a live org owner is being downgraded permissions or being deactivated completely,
                                // need to ensure that org is not left without any owners
                                var ownerCount = await db.OrgMembers.CountAsync(
                                    x =>
                                        x.Org == req.Org
                                        && x.IsActive
                                        && x.Role == OrgMemberRole.Owner
                                );
                                ctx.ErrorIf(
                                    ownerCount == 1,
                                    S.InsufficientPermission,
                                    null,
                                    HttpStatusCode.Forbidden
                                );
                            }
                            updateMem.IsActive = req.IsActive ?? updateMem.IsActive;
                            updateMem.Name = req.NewName ?? updateMem.Name;
                            updateMem.Role = req.NewRole ?? updateMem.Role;
                            return updateMem.NotNull().ToApi();
                        }
                    )
            )
        };
}
