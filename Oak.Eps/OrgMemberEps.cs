using Common.Server;
using Common.Server.Auth;
using Common.Shared;
using Common.Shared.Auth;
using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Db;
using OrgMember = Oak.Api.OrgMember.OrgMember;
using S = Oak.I18n.S;

namespace Oak.Eps;

internal static class OrgMemberEps
{
    public static IReadOnlyList<IEp> Eps { get; } =
        new List<IEp>()
        {
            Ep<Invite, OrgMember>.DbTx<OakDb>(OrgMemberRpcs.Invite, Invite),
            new Ep<Exact, Maybe<OrgMember>>(OrgMemberRpcs.GetOne, GetOne),
            new Ep<Get, SetRes<OrgMember>>(OrgMemberRpcs.Get, Get),
            Ep<Update, OrgMember>.DbTx<OakDb>(OrgMemberRpcs.Update, Update)
        };

    private static async Task<OrgMember> Invite(IRpcCtx ctx, OakDb db, ISession ses, Invite req)
    {
        req.Email = req.Email.ToLower();
        // check current member has sufficient permissions
        var sesOrgMem = await db.OrgMembers
            .Where(x => x.Org == req.Org && x.IsActive && x.Id == ses.Id)
            .SingleOrDefaultAsync(ctx.Ctkn);
        ctx.InsufficientPermissionsIf(sesOrgMem == null);
        var sesRole = sesOrgMem.NotNull().Role;
        ctx.InsufficientPermissionsIf(
            sesRole is not (OrgMemberRole.Owner or OrgMemberRole.Admin)
                || (sesRole is OrgMemberRole.Admin && req.Role == OrgMemberRole.Owner)
        );
        var (auth, created) = await AuthEps<OakDb>.CreateAuth(
            ctx,
            db,
            new Register(req.Email, $"{Crypto.String(16)}a1@"),
            Id.New(),
            ses.Lang,
            ses.DateFmt,
            ses.TimeFmt,
            ses.DateSeparator,
            ses.ThousandsSeparator,
            ses.DecimalSeparator
        );

        var mem = new Db.OrgMember()
        {
            Org = req.Org,
            Id = auth.Id,
            IsActive = true,
            Name = req.Name,
            Role = req.Role
        };
        await db.OrgMembers.AddAsync(mem, ctx.Ctkn);
        if (created)
        {
            var org = await db.Orgs.SingleAsync(x => x.Id == req.Org, ctx.Ctkn);
            var config = ctx.Get<IConfig>();
            var emailClient = ctx.Get<IEmailClient>();
            var model = new
            {
                BaseHref = config.Server.Listen,
                Email = auth.Email,
                Code = auth.VerifyEmailCode,
                OrgName = org.Name,
                InviteeName = req.Name,
                InvitedByName = sesOrgMem.Name
            };
            await emailClient.SendEmailAsync(
                ctx.String(S.OrgMemberInviteEmailSubject, model),
                ctx.String(S.OrgMemberInviteEmailHtml, model),
                ctx.String(S.OrgMemberInviteEmailText, model),
                config.Email.NoReplyAddress,
                new List<string>() { auth.Email }
            );
        }

        return mem.ToApi();
    }

    private static async Task<Maybe<OrgMember>> GetOne(IRpcCtx ctx, Exact req)
    {
        var db = ctx.Get<OakDb>();
        var mem = await db.OrgMembers.SingleOrDefaultAsync(
            x => x.Org == req.Org && x.Id == req.Id,
            ctx.Ctkn
        );
        return new(mem?.ToApi());
    }

    private static async Task<SetRes<OrgMember>> Get(IRpcCtx ctx, Get req)
    {
        var db = ctx.Get<OakDb>();
        var qry = db.OrgMembers.Where(x => x.Org == req.Org);
        // filters
        if (req.IsActive != null)
        {
            qry = qry.Where(x => x.IsActive == req.IsActive);
        }

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
            ctx.NotFoundIf(after == null, model: new { Name = "After" });
            after.NotNull();
            qry = (req.OrderBy, req.Asc) switch
            {
                (OrgMemberOrderBy.Role, true)
                    => qry.Where(
                        x =>
                            x.Role > after.Role
                            || (x.Role == after.Role && x.Name.CompareTo(after.Name) > 0)
                    ),
                (OrgMemberOrderBy.Name, true)
                    => qry.Where(
                        x =>
                            x.Name.CompareTo(after.Name) > 0
                            || (x.Name.CompareTo(after.Name) == 0 && x.Role > after.Role)
                    ),
                (OrgMemberOrderBy.Role, false)
                    => qry.Where(
                        x =>
                            x.Role < after.Role
                            || (x.Role == after.Role && x.Name.CompareTo(after.Name) > 0)
                    ),
                (OrgMemberOrderBy.Name, false)
                    => qry.Where(
                        x =>
                            x.Name.CompareTo(after.Name) < 0
                            || (x.Name.CompareTo(after.Name) == 0 && x.Role > after.Role)
                    ),
            };
        }

        qry = (req.OrderBy, req.Asc) switch
        {
            (OrgMemberOrderBy.Role, true) => qry.OrderBy(x => x.Role).ThenBy(x => x.Name),
            (OrgMemberOrderBy.Name, true) => qry.OrderBy(x => x.Name).ThenBy(x => x.Role),
            (OrgMemberOrderBy.Role, false)
                => qry.OrderByDescending(x => x.Role).ThenBy(x => x.Name),
            (OrgMemberOrderBy.Name, false)
                => qry.OrderByDescending(x => x.Name).ThenBy(x => x.Role),
        };
        var set = await qry.Take(101).Select(x => x.ToApi()).ToListAsync();
        return SetRes<OrgMember>.FromLimit(set, 101);
    }

    private static async Task<OrgMember> Update(IRpcCtx ctx, OakDb db, ISession ses, Update req)
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
                || (sesRole is OrgMemberRole.Admin && req.Role == OrgMemberRole.Owner)
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
            && ((req.Role != null && req.Role != OrgMemberRole.Owner) || req.IsActive is false)
        )
        {
            // a live org owner is being downgraded permissions or being deactivated completely,
            // need to ensure that org is not left without any owners
            var ownerCount = await db.OrgMembers.CountAsync(
                x => x.Org == req.Org && x.IsActive && x.Role == OrgMemberRole.Owner
            );
            ctx.InsufficientPermissionsIf(ownerCount == 1);
        }

        var nameUpdated = req.Name != null && updateMem.Name != req.Name;
        var isActiveUpdated = req.IsActive != null && updateMem.IsActive != req.IsActive;
        var roleUpdated = req.Role != null && updateMem.Role != req.Role;
        updateMem.IsActive = req.IsActive ?? updateMem.IsActive;
        updateMem.Name = req.Name ?? updateMem.Name;
        updateMem.Role = req.Role ?? updateMem.Role;
        if (nameUpdated || isActiveUpdated || roleUpdated)
        {
            // copy values over to denormalized duplicates in projectMembers
            await db.ProjectMembers
                .Where(x => x.Org == req.Org && x.Id == req.Id)
                .ExecuteUpdateAsync(
                    x =>
                        x.SetProperty(x => x.Name, _ => updateMem.Name)
                            .SetProperty(x => x.OrgRole, x => updateMem.Role)
                            .SetProperty(x => x.IsActive, x => updateMem.IsActive)
                );
        }

        return updateMem.NotNull().ToApi();
    }
}
