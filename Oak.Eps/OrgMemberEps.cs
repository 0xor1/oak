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
    private static readonly IOrgMemberApi Api = IOrgMemberApi.Init();

    public static IReadOnlyList<IRpcEndpoint> Eps { get; } = new List<IRpcEndpoint>()
    {
        new RpcEndpoint<Add, OrgMember>(Api.Add, async (ctx, req) =>
            await ctx.DbTx<OakDb, OrgMember>(async (db, ses) =>
            {
                throw new NotImplementedException();
            })),
        
        new RpcEndpoint<Get, IReadOnlyList<OrgMember>>(Api.Get, async (ctx, req) =>
            await ctx.DbTx<OakDb, IReadOnlyList<OrgMember>>(async (db, ses) =>
            {
                throw new NotImplementedException();
            })),
        
        new RpcEndpoint<Update, OrgMember>(Api.Update, async (ctx, req) =>
            await ctx.DbTx<OakDb, OrgMember>(async (db, ses) =>
            {
                throw new NotImplementedException();
            }))
    };
}