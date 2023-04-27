using Common.Server.Test;
using Common.Shared;
using Oak.Api;
using Oak.Api.Org;
using Oak.Db;
using Org = Oak.Api.Org.Org;
using S = Oak.I18n.S;

namespace Oak.Eps.Test;

public class ProjectTests : IDisposable
{
    private readonly RpcTestRig<OakDb, Api.Api> _rpcTestRig;

    public ProjectTests()
    {
        _rpcTestRig = new RpcTestRig<OakDb, Api.Api>(S.Inst, OakEps.Eps, c => new Api.Api(c));
    }

    [Fact]
    public async void Create_Success()
    {
        var (ali, org) = await Setup();
    }

    private async Task<(IApi, Org)> Setup()
    {
        var userName = "ali";
        var (ali, _, _) = await _rpcTestRig.NewApi(userName);
        var org = await ali.Org.Create(new("a", userName));
        return (ali, org);
    }

    public void Dispose()
    {
        _rpcTestRig.Dispose();
    }
}
