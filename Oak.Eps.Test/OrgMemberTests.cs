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
        var org = await ali.Org.Create(new("a", "ali"));
        var mem = await ali.OrgMember.Add(new(org.Id, bobSes.Id, bobName, OrgMemberRole.Admin));
        Assert.Equal(org.Id, mem.Org);
        Assert.Equal(bobSes.Id, mem.Id);
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
        var org = await ali.Org.Create(new("a", "ali"));
        var mem = await ali.OrgMember.Add(new(org.Id, bobSes.Id, bobName, OrgMemberRole.Owner));
        var newName = "yolo";
        mem = await ali.OrgMember.Update(
            new(org.Id, mem.Id, false, newName, OrgMemberRole.PerProject)
        );
        Assert.Equal(org.Id, mem.Org);
        Assert.Equal(bobSes.Id, mem.Id);
        Assert.False(mem.IsActive);
        Assert.Equal(newName, mem.Name);
        Assert.Equal(OrgMemberRole.PerProject, mem.Role);
    }

    [Fact]
    public async void Get_Success()
    {
        var bobName = "bob";
        var catName = "cat";
        var (ali, _, _) = await _rpcTestRig.NewApi("ali");
        var (bob, _, _) = await _rpcTestRig.NewApi(bobName);
        var (cat, _, _) = await _rpcTestRig.NewApi(catName);
        var bobSes = await bob.Auth.GetSession();
        var org = await ali.Org.Create(new("a", "ali"));
        var aliMem = (await ali.OrgMember.Get(new(org.Id, true))).Single();
        var bobMem = await ali.OrgMember.Add(new(org.Id, bobSes.Id, bobName, OrgMemberRole.Admin));
        var catSes = await cat.Auth.GetSession();
        var catMem = await ali.OrgMember.Add(
            new(org.Id, catSes.Id, catName, OrgMemberRole.WriteAllProjects)
        );
        // get all by default ordering
        var res = await ali.OrgMember.Get(new(org.Id, true));
        Assert.Equal(3, res.Count);
        Assert.Equal(aliMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(catMem, res[2]);
        // get a specific single member
        res = await ali.OrgMember.Get(new(org.Id, true, bobSes.Id));
        Assert.Equal(bobMem, res.Single());
        // get members with search filters nameStartsWith and role
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, "ca", OrgMemberRole.WriteAllProjects)
        );
        Assert.Equal(catMem, res.Single());
        // get members with isActive order
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, null, OrgMemberOrderBy.IsActive)
        );
        Assert.Equal(3, res.Count);
        Assert.Equal(aliMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(catMem, res[2]);
        // get members with role order
        res = await ali.OrgMember.Get(new(org.Id, true, null, null, null, OrgMemberOrderBy.Role));
        Assert.Equal(3, res.Count);
        Assert.Equal(aliMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(catMem, res[2]);
        // get members with desc name order
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, null, OrgMemberOrderBy.Name, false)
        );
        Assert.Equal(3, res.Count);
        Assert.Equal(catMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(aliMem, res[2]);
        // get members with desc isActive order
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, null, OrgMemberOrderBy.IsActive, false)
        );
        Assert.Equal(3, res.Count);
        Assert.Equal(catMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(aliMem, res[2]);
        // get members with role order
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, null, OrgMemberOrderBy.Role, false)
        );
        Assert.Equal(3, res.Count);
        Assert.Equal(catMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(aliMem, res[2]);
    }

    public void Dispose()
    {
        _rpcTestRig.Dispose();
    }
}
