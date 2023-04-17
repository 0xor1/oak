﻿using System.Net;
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
    private static readonly IOrgMemberApi Api = IOrgMemberApi.Init();

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } = new List<IRpcEndpoint>()
    {
        new RpcEndpoint<Add, OrgMember>(Api.Add, async (ctx, req) =>
            await ctx.DbTx<OakDb, OrgMember>(async (db, ses) =>
            {
                // check current member has sufficient permissions
                var sesOrgMem = await db.OrgMembers.Where(x => x.Org == req.Org && x.IsActive && x.Member == ses.Id).SingleOrDefaultAsync();
                ctx.ErrorIf(sesOrgMem == null, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
                var sesRole = sesOrgMem.NotNull().Role;
                ctx.ErrorIf(sesRole is not (OrgMemberRole.Owner or OrgMemberRole.Admin) || (sesRole is OrgMemberRole.Admin && req.Role == OrgMemberRole.Owner), S.InsufficientPermission, null, HttpStatusCode.Forbidden);
                // check new member is an active user
                var newMemExists =  await db.Auths.AnyAsync(x => x.Id == req.Member && x.ActivatedOn != DateTimeExts.Zero());
                ctx.ErrorIf(!newMemExists, S.NoMatchingRecord, null, HttpStatusCode.NotFound);
                var newMem = new Db.OrgMember()
                {
                    Org = req.Org,
                    Member = req.Member,
                    IsActive = true,
                    Name = req.Name,
                    Role = req.Role
                };
                await db.OrgMembers.AddAsync(newMem);
                return newMem.ToApi();
            })),
        
        new RpcEndpoint<Get, IReadOnlyList<OrgMember>>(Api.Get, async (ctx, req) =>
        {
            var ses = ctx.GetAuthedSession();
            var db = ctx.Get<OakDb>();
            // check current member has sufficient permissions
            var isActiveMember =
                await db.OrgMembers.AnyAsync(x => x.Org == req.Org && x.IsActive && x.Member == ses.Id);
            ctx.ErrorIf(!isActiveMember, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
            var qry = db.OrgMembers.Where(x => x.Org == req.Org);
            // filters
            if (req.Member != null)
            {
                qry = qry.Where(x => x.Member == req.Member);
            }
            else
            {
                qry = qry.Where(x => x.IsActive == req.IsActive);
                if (req.Role != null)
                {
                    qry = qry.Where(x => x.Role == req.Role);
                }

                if (req.NameStartsWith != null)
                {
                    qry = qry.Where(x => x.Name.StartsWith(req.NameStartsWith));
                }

                qry = req switch
                {
                    (_, _, _, _, _, OrgMemberOrderBy.Name, true) => qry.OrderBy(x => x.Name),
                    (_, _, _, _, _, OrgMemberOrderBy.IsActive, true) => qry.OrderBy(x => x.IsActive)
                        .ThenBy(x => x.Name),
                    // TODO should enums be switched to int sql storage so they have more
                    // meaningful ordering???, alphabetical ordering is meaningless!!!
                    (_, _, _, _, _, OrgMemberOrderBy.Role, true) => qry.OrderBy(x => x.Role).ThenBy(x => x.Name),
                    (_, _, _, _, _, OrgMemberOrderBy.Name, false) => qry.OrderByDescending(x => x.Name),
                    (_, _, _, _, _, OrgMemberOrderBy.IsActive, false) => qry.OrderByDescending(x => x.IsActive)
                        .ThenByDescending(x => x.Name),
                    (_, _, _, _, _, OrgMemberOrderBy.Role, false) => qry.OrderByDescending(x => x.Role)
                        .ThenByDescending(x => x.Name),
                };
            }
            return await qry.Select(x => x.ToApi()).ToListAsync();
        }),
        
        new RpcEndpoint<Update, OrgMember>(Api.Update, async (ctx, req) =>
            await ctx.DbTx<OakDb, OrgMember>(async (db, ses) =>
            {
                // check current member has sufficient permissions
                var sesOrgMem = await db.OrgMembers.Where(x => x.Org == req.Org && x.IsActive && x.Member == ses.Id).SingleOrDefaultAsync();
                ctx.ErrorIf(sesOrgMem == null, S.InsufficientPermission, null, HttpStatusCode.Forbidden);
                var sesRole = sesOrgMem.NotNull().Role;
                ctx.ErrorIf(sesRole is not (OrgMemberRole.Owner or OrgMemberRole.Admin) || (sesRole is OrgMemberRole.Admin && req.NewRole == OrgMemberRole.Owner), S.InsufficientPermission, null, HttpStatusCode.Forbidden);
                var updateMem = await db.OrgMembers.SingleOrDefaultAsync(x => x.Org == req.Org && x.Member == req.Member);
                ctx.ErrorIf(updateMem == null, S.NoMatchingRecord, null, HttpStatusCode.NotFound);
                updateMem.NotNull();
                updateMem.IsActive = req.IsActive ?? updateMem.IsActive;
                updateMem.Name = req.NewName ?? updateMem.Name;
                updateMem.Role = req.NewRole ?? updateMem.Role;
                return updateMem.NotNull().ToApi();
            }))
    };
}