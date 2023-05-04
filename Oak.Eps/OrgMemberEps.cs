using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Db;
using OrgMember = Oak.Api.OrgMember.OrgMember;

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
                            ctx.InsufficientPermissionsIf(sesOrgMem == null);
                            var sesRole = sesOrgMem.NotNull().Role;
                            ctx.InsufficientPermissionsIf(
                                sesRole is not (OrgMemberRole.Owner or OrgMemberRole.Admin)
                                    || (
                                        sesRole is OrgMemberRole.Admin
                                        && req.Role == OrgMemberRole.Owner
                                    )
                            );
                            // check new member is an active user
                            var newMemExists = await db.Auths.AnyAsync(
                                x => x.Id == req.Id && x.ActivatedOn != DateTimeExt.Zero()
                            );
                            ctx.NotFoundIf(!newMemExists, model: new { Name = "User" });
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
            new RpcEndpoint<Exact, OrgMember>(
                OrgMemberRpcs.GetOne,
                async (ctx, req) =>
                {
                    var ses = ctx.GetAuthedSession();
                    var db = ctx.Get<OakDb>();
                    await EpsUtil.MustBeActiveOrgMember(ctx, db, ses.Id, req.Org);
                    var mem = await db.OrgMembers.SingleOrDefaultAsync(
                        x => x.Org == req.Org && x.Id == req.Id
                    );
                    ctx.NotFoundIf(mem == null, model: new { Name = "Org Member" });
                    return mem.NotNull().ToApi();
                }
            ),
            new RpcEndpoint<Get, SetRes<OrgMember>>(
                OrgMemberRpcs.Get,
                async (ctx, req) =>
                {
                    var ses = ctx.GetAuthedSession();
                    var db = ctx.Get<OakDb>();
                    // check current member has sufficient permissions
                    var isActiveMember = await db.OrgMembers.AnyAsync(
                        x => x.Org == req.Org && x.IsActive && x.Id == ses.Id
                    );
                    ctx.InsufficientPermissionsIf(!isActiveMember);
                    var qry = db.OrgMembers.Where(x => x.Org == req.Org);
                    // filters
                    qry = qry.Where(x => x.IsActive == req.IsActive);
                    if (req.Role != null)
                    {
                        qry = qry.Where(x => x.Role == req.Role);
                    }

                    if (!req.NameStartsWith.IsNullOrWhiteSpace())
                    {
                        qry = qry.Where(x => x.Name.StartsWith(req.NameStartsWith));
                    }

                    if (req.After != null)
                    {
                        // implement cursor based pagination ... in a fashion
                        var after = await db.OrgMembers.SingleOrDefaultAsync(
                            x => x.Org == req.Org && x.Id == req.After
                        );
                        ctx.NotFoundIf(after == null, model: new { Name = "Org Member" });
                        after.NotNull();
                        qry = (req.OrderBy, req.Asc) switch
                        {
                            (OrgMemberOrderBy.Role, true)
                                => qry.Where(
                                    x =>
                                        x.Role > after.Role
                                        || (
                                            x.Role == after.Role && x.Name.CompareTo(after.Name) > 0
                                        )
                                ),
                            (OrgMemberOrderBy.Name, true)
                                => qry.Where(
                                    x =>
                                        x.Name.CompareTo(after.Name) > 0
                                        || (
                                            x.Name.CompareTo(after.Name) == 0 && x.Role > after.Role
                                        )
                                ),
                            (OrgMemberOrderBy.Role, false)
                                => qry.Where(
                                    x =>
                                        x.Role < after.Role
                                        || (
                                            x.Role == after.Role && x.Name.CompareTo(after.Name) > 0
                                        )
                                ),
                            (OrgMemberOrderBy.Name, false)
                                => qry.Where(
                                    x =>
                                        x.Name.CompareTo(after.Name) < 0
                                        || (
                                            x.Name.CompareTo(after.Name) == 0 && x.Role > after.Role
                                        )
                                ),
                        };
                    }

                    qry = (req.OrderBy, req.Asc) switch
                    {
                        (OrgMemberOrderBy.Role, true)
                            => qry.OrderBy(x => x.Role).ThenBy(x => x.Name),
                        (OrgMemberOrderBy.Name, true)
                            => qry.OrderBy(x => x.Name).ThenBy(x => x.Role),
                        (OrgMemberOrderBy.Role, false)
                            => qry.OrderByDescending(x => x.Role).ThenBy(x => x.Name),
                        (OrgMemberOrderBy.Name, false)
                            => qry.OrderByDescending(x => x.Name).ThenBy(x => x.Role),
                    };
                    var set = await qry.Take(101).Select(x => x.ToApi()).ToListAsync();
                    return SetRes<OrgMember>.FromLimit(set, 101);
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
                            ctx.InsufficientPermissionsIf(sesOrgMem == null);
                            var sesRole = sesOrgMem.NotNull().Role;
                            // must be an owner or admin, and admins cant make members owners
                            ctx.InsufficientPermissionsIf(
                                sesRole is not (OrgMemberRole.Owner or OrgMemberRole.Admin)
                                    || (
                                        sesRole is OrgMemberRole.Admin
                                        && req.Role == OrgMemberRole.Owner
                                    )
                            );
                            var updateMem = await db.OrgMembers.SingleOrDefaultAsync(
                                x => x.Org == req.Org && x.Id == req.Id
                            );
                            // update target must exist
                            ctx.InsufficientPermissionsIf(updateMem == null);
                            updateMem.NotNull();
                            // cant update a member with high permissions than you
                            ctx.InsufficientPermissionsIf(updateMem.Role < sesRole);
                            if (
                                updateMem is { IsActive: true, Role: OrgMemberRole.Owner }
                                && (
                                    (req.Role != null && req.Role != OrgMemberRole.Owner)
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
                                ctx.InsufficientPermissionsIf(ownerCount == 1);
                            }
                            var nameUpdated = req.Name != null && updateMem.Name != req.Name;
                            updateMem.IsActive = req.IsActive ?? updateMem.IsActive;
                            updateMem.Name = req.Name ?? updateMem.Name;
                            updateMem.Role = req.Role ?? updateMem.Role;
                            if (nameUpdated)
                            {
                                await db.ProjectMembers
                                    .Where(x => x.Org == req.Org && x.Id == req.Id)
                                    .ExecuteUpdateAsync(
                                        x => x.SetProperty(x => x.Name, _ => req.Name)
                                    );
                            }
                            return updateMem.NotNull().ToApi();
                        }
                    )
            )
        };
}
