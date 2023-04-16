using Common.Server;
using Common.Shared;
using Oak.Api.Org;
using Oak.Db;
using Org = Oak.Api.Org.Org;

namespace Oak.Eps;

internal static class OrgEps
{
    private static readonly IOrgApi Api = IOrgApi.Init();

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } = new List<IRpcEndpoint>()
    {
        new RpcEndpoint<Create, Org>(Api.Create, async (ctx, req) =>
             await ctx.DbTx<OakDb, Org>((db, ses) =>
            {
                var newOrg = new Db.Org() 
                {
                    Id = Id.New(),
                    Name = req.Name
                };
                db.Orgs.Add(newOrg);
                db.OrgMembers.Add(new ()
                {
                    IsActive = true,
                    Member = ses.Id,
                    Name = req.OwnerMemberName,
                    Role = OrgMemberRole.Owner
                });
                return newOrg.ToApi().Task();
            }))
    };
}