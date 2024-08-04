using Common.Server.Test;
using Common.Shared;
using Oak.Api.Org;

namespace Oak.Eps.Test;

public class OrgTests : TestBase
{
    [Fact]
    public async void Create_Success()
    {
        var userName = "ali";
        var (ali, _, _) = await Rig.NewApi(userName);
        var name = "a";
        var org = await ali.Org.Create(new(name, userName));
        Assert.Equal(name, org.Name);
        Assert.True(org.CreatedOn.AddSeconds(1) > DateTimeExt.UtcNowMilli());
    }

    [Fact]
    public async void Update_Success()
    {
        var userName = "ali";
        var (ali, _, _) = await Rig.NewApi(userName);
        var org = await ali.Org.Create(new("Oak.Eps.Test", userName));
        var newName = "name changed";
        org = await ali.Org.Update(new(org.Id, newName));
        Assert.Equal(newName, org.Name);
    }

    [Fact]
    public async void Get_Success()
    {
        var userName = "ali";
        var (ali, _, _) = await Rig.NewApi(userName);
        var c = await ali.Org.Create(new("c", userName));
        var b = await ali.Org.Create(new("b", userName));
        var a = await ali.Org.Create(new("a", userName));
        var one = await ali.Org.GetOne(new(a.Id));
        Assert.Equal(a, one);
        var res = await ali.Org.Get(new());
        Assert.Equal(3, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(b, res[1]);
        Assert.Equal(c, res[2]);
        res = await ali.Org.Get(new(asc: false));
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
        var (ali, _, _) = await Rig.NewApi(userName);
        var a = await ali.Org.Create(new("a", userName));
        await ali.Org.Delete(new(a.Id));
        var res = await ali.Org.Get(new());
        Assert.Empty(res);
    }

    [Fact]
    public async void ValidateFcmTopic_Success()
    {
        var (ali, _, _, _, _, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        await ali.Auth.FcmEnabled(new(true));
        var res = await ali.Auth.FcmRegister(
            new(new List<string>() { org.Id, p.Id }, "abc:123", null)
        );
        Assert.NotEmpty(res.Client);
        RpcTestException? ex = null;
        try
        {
            res = await ali.Auth.FcmRegister(
                new(new List<string>() { org.Id }, "abc:123", res.Client)
            );
        }
        catch (RpcTestException x)
        {
            ex = x;
        }
        Assert.Equal("Bad request", ex.Rpc.Message);
        try
        {
            res = await ali.Auth.FcmRegister(
                new(new List<string>() { p.Id, org.Id }, "abc:123", res.Client)
            );
        }
        catch (RpcTestException x)
        {
            ex = x;
        }
        Assert.Equal("Project not found", ex.Rpc.Message);
    }
}
