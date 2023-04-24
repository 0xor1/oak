using Common.Server.Test;
using Oak.Api;
using Oak.Db;
using Oak.I18n;

namespace Oak.Eps.Test;

public class OrgTests : IDisposable
{
    private readonly RpcTestRig<OakDb, Api.Api> _rpcTestRig;

    public OrgTests()
    {
        _rpcTestRig = new RpcTestRig<OakDb, Api.Api>(S.Inst, OakEps.Eps, c => new Api.Api(c));
    }
    
    [Fact]
    public async void Create_Success()
    {
        var userName = "ali";
        var (ali, _, _) = await _rpcTestRig.NewApi(userName);
        var name = $"Oak.Eps.Test {userName}";
        var org = await ali.Org.Create(new (name, userName));
        Assert.Equal(name, org.Name);
        Assert.True(org.CreatedOn.AddSeconds(1) > DateTime.UtcNow);
    }

    public void Dispose()
    {
        _rpcTestRig.Dispose();
    }
}