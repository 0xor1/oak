using Common.Server.Test;
using Common.Shared;
using Oak.Api;
using Oak.Api.Org;
using Oak.Db;
using S = Oak.I18n.S;

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
        var name = "a";
        var org = await ali.Org.Create(new(name, userName));
        Assert.Equal(name, org.Name);
        Assert.True(org.CreatedOn.AddSeconds(1) > DateTimeExt.UtcNowMilli());
    }

    [Fact]
    public async void Update_Success()
    {
        var userName = "ali";
        var (ali, _, _) = await _rpcTestRig.NewApi(userName);
        var org = await ali.Org.Create(new("Oak.Eps.Test", userName));
        var newName = "name changed";
        org = await ali.Org.Update(new(org.Id, newName));
        Assert.Equal(newName, org.Name);
    }

    [Fact]
    public async void Get_Success()
    {
        var userName = "ali";
        var (ali, _, _) = await _rpcTestRig.NewApi(userName);
        var c = await ali.Org.Create(new("c", userName));
        var b = await ali.Org.Create(new("b", userName));
        var a = await ali.Org.Create(new("a", userName));
        var res = await ali.Org.Get(new());
        Assert.Equal(3, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(b, res[1]);
        Assert.Equal(c, res[2]);
        res = await ali.Org.Get(new(Asc: false));
        Assert.Equal(3, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(b, res[1]);
        Assert.Equal(a, res[2]);
        res = await ali.Org.Get(new(OrgOrderBy.CreatedOn));
        Assert.Equal(3, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(b, res[1]);
        Assert.Equal(a, res[2]);
        res = await ali.Org.Get(new(OrgOrderBy.CreatedOn, false));
        Assert.Equal(3, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(b, res[1]);
        Assert.Equal(c, res[2]);
    }

    [Fact]
    public async void Delete_Success()
    {
        var userName = "ali";
        var (ali, _, _) = await _rpcTestRig.NewApi(userName);
        var a = await ali.Org.Create(new("a", userName));
        await ali.Org.Delete(new(a.Id));
        var res = await ali.Org.Get(new());
        Assert.Empty(res);
    }

    public void Dispose()
    {
        _rpcTestRig.Dispose();
    }
}
