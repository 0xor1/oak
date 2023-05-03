using Oak.Api.OrgMember;

namespace Oak.Eps.Test;

public class OrgMemberTests : TestBase
{
    [Fact]
    public async void Add_Success()
    {
        var bobName = "bob";
        var (ali, _, _) = await Rig.NewApi("ali");
        var (bob, _, _) = await Rig.NewApi(bobName);
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
        var (ali, _, _) = await Rig.NewApi("ali");
        var (bob, _, _) = await Rig.NewApi(bobName);
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
        var (ali, _, _) = await Rig.NewApi("ali");
        var (bob, _, _) = await Rig.NewApi(bobName);
        var (cat, _, _) = await Rig.NewApi(catName);
        var bobSes = await bob.Auth.GetSession();
        var org = await ali.Org.Create(new("a", "ali"));
        var aliMem = (await ali.OrgMember.Get(new(org.Id, true))).Single();
        var bobMem = await ali.OrgMember.Add(new(org.Id, bobSes.Id, bobName, OrgMemberRole.Admin));
        var catSes = await cat.Auth.GetSession();
        var catMem = await ali.OrgMember.Add(
            new(org.Id, catSes.Id, catName, OrgMemberRole.WriteAllProjects)
        );
        // get one
        var one = await ali.OrgMember.GetOne(new(org.Id, aliMem.Id));
        Assert.Equal(aliMem, one);
        // get all by default ordering
        var res = await ali.OrgMember.Get(new(org.Id, true));
        Assert.Equal(3, res.Count);
        Assert.Equal(aliMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(catMem, res[2]);
        // get all by default ordering after ali
        res = await ali.OrgMember.Get(new(org.Id, true, After: aliMem.Id));
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(catMem, res[1]);
        // get members with search filters nameStartsWith and role
        res = await ali.OrgMember.Get(new(org.Id, true, "ca", OrgMemberRole.WriteAllProjects));
        Assert.Equal(catMem, res.Single());
        // get members with name order
        res = await ali.OrgMember.Get(new(org.Id, true, null, null, null, OrgMemberOrderBy.Name));
        Assert.Equal(3, res.Count);
        Assert.Equal(aliMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(catMem, res[2]);
        // get members with name order after ali
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, aliMem.Id, OrgMemberOrderBy.Name)
        );
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(catMem, res[1]);
        // get members with role order after ali
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, aliMem.Id, OrgMemberOrderBy.Role)
        );
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(catMem, res[1]);
        // get members with desc name order
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, null, OrgMemberOrderBy.Name, false)
        );
        Assert.Equal(3, res.Count);
        Assert.Equal(catMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(aliMem, res[2]);
        // get members with desc name order after cat
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, catMem.Id, OrgMemberOrderBy.Name, false)
        );
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(aliMem, res[1]);
        // get members with desc role order
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, null, OrgMemberOrderBy.Role, false)
        );
        Assert.Equal(3, res.Count);
        Assert.Equal(catMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(aliMem, res[2]);
        // get members with desc role order after cat
        res = await ali.OrgMember.Get(
            new(org.Id, true, null, null, catMem.Id, OrgMemberOrderBy.Role, false)
        );
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(aliMem, res[1]);
    }
}
