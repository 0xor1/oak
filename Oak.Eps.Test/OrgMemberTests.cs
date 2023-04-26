using Common.Server.Test;
using Oak.Api.OrgMember;
using Oak.Db;
using S = Oak.I18n.S;

namespace Oak.Eps.Test;

public class OrgMemberTests : IDisposable
{
    private readonly RpcTestRig<OakDb, Api.Api> _rpcTestRig;

    public OrgMemberTests()
    {
        _rpcTestRig = new RpcTestRig<OakDb, Api.Api>(S.Inst, OakEps.Eps, c => new Api.Api(c));
    }
    
    [Fact]
    public async void Add_Success()
    {
        var bobName = "bob";
        var (ali, _, _) = await _rpcTestRig.NewApi("ali");
        var (bob, _, _) = await _rpcTestRig.NewApi(bobName);
        var bobSes = await bob.Auth.GetSession();
        var org = await ali.Org.Create(new ("a", "ali"));
        var mem = await ali.OrgMember.Add(new(org.Id, bobSes.Id, bobName, OrgMemberRole.Admin));
        Assert.Equal(org.Id, mem.Org);
        Assert.Equal(bobSes.Id, mem.Member);
        Assert.True(mem.IsActive);
        Assert.Equal(bobName, mem.Name);
        Assert.Equal(OrgMemberRole.Admin, mem.Role);
    }
    
    [Fact]
    public async void Update_Success()
    {
        var bobName = "bob";
        var (ali, _, _) = await _rpcTestRig.NewApi("ali");
        var (bob, _, _) = await _rpcTestRig.NewApi(bobName);
        var bobSes = await bob.Auth.GetSession();
        var org = await ali.Org.Create(new ("a", "ali"));
        var mem = await ali.OrgMember.Add(new(org.Id, bobSes.Id, bobName, OrgMemberRole.Owner));
        var newName = "yolo";
        mem = await ali.OrgMember.Update(new(org.Id, mem.Member, false, newName, OrgMemberRole.PerProject));
        Assert.Equal(org.Id, mem.Org);
        Assert.Equal(bobSes.Id, mem.Member);
        Assert.False(mem.IsActive);
        Assert.Equal(newName, mem.Name);
        Assert.Equal(OrgMemberRole.PerProject, mem.Role);
    }

    public void Dispose()
    {
        _rpcTestRig.Dispose();
    }
}