using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;

namespace Oak.Eps.Test;

public class OrgMemberTests : TestBase
{
    [Fact]
    public async void Invite_Success()
    {
        var (ali, _, _) = await Rig.NewApi("ali");
        var org = await ali.Org.Create(new("a", "ali"));
        var mem = await ali.OrgMember.Invite(
            new(org.Id, "bob@bob.bob", "bob", OrgMemberRole.Admin)
        );
        Assert.Equal(org.Id, mem.Org);
        Assert.True(mem.IsActive);
        Assert.Equal("bob", mem.Name);
        Assert.Equal(OrgMemberRole.Admin, mem.Role);
        // needs to be manually cleaned up since Rig isnt creating the user for us
        Rig.RunDb((db) => db.Auths.Where(x => x.Id == mem.Id).ExecuteDelete());
    }

    [Fact]
    public async void Update_Success()
    {
        var bobName = "bob";
        var (ali, _, _) = await Rig.NewApi("ali");
        var (bob, bobEmail, _) = await Rig.NewApi(bobName);
        var bobSes = await bob.Auth.GetSession();
        var org = await ali.Org.Create(new("a", "ali"));
        var mem = await ali.OrgMember.Invite(new(org.Id, bobEmail, bobName, OrgMemberRole.Owner));
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
        var (bob, bobEmail, _) = await Rig.NewApi(bobName);
        var (cat, catEmail, _) = await Rig.NewApi(catName);
        var bobSes = await bob.Auth.GetSession();
        var org = await ali.Org.Create(new("a", "ali"));
        var aliMem = (await ali.OrgMember.Get(new(org.Id, true))).Set.Single();
        var bobMem = await ali.OrgMember.Invite(
            new(org.Id, bobEmail, bobName, OrgMemberRole.Admin)
        );
        var catSes = await cat.Auth.GetSession();
        var catMem = await ali.OrgMember.Invite(
            new(org.Id, catEmail, catName, OrgMemberRole.WriteAllProjects)
        );
        // get one
        var one = await ali.OrgMember.GetOne(new(org.Id, aliMem.Id));
        Assert.Equal(aliMem, one.Item);
        // get all by default ordering
        var res = (await ali.OrgMember.Get(new(org.Id, true))).Set;
        Assert.Equal(3, res.Count);
        Assert.Equal(aliMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(catMem, res[2]);
        // get all by default ordering after ali
        res = (await ali.OrgMember.Get(new(org.Id, true, after: aliMem.Id))).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(catMem, res[1]);
        // get members with search filters nameStartsWith and role
        res = (
            await ali.OrgMember.Get(new(org.Id, true, "ca", OrgMemberRole.WriteAllProjects))
        ).Set;
        Assert.Equal(catMem, res.Single());
        // get members with name order
        res = (
            await ali.OrgMember.Get(new(org.Id, true, null, null, null, OrgMemberOrderBy.Name))
        ).Set;
        Assert.Equal(3, res.Count);
        Assert.Equal(aliMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(catMem, res[2]);
        // get members with name order after ali
        res = (
            await ali.OrgMember.Get(new(org.Id, true, null, null, aliMem.Id, OrgMemberOrderBy.Name))
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(catMem, res[1]);
        // get members with role order after ali
        res = (
            await ali.OrgMember.Get(new(org.Id, true, null, null, aliMem.Id, OrgMemberOrderBy.Role))
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(catMem, res[1]);
        // get members with desc name order
        res = (
            await ali.OrgMember.Get(
                new(org.Id, true, null, null, null, OrgMemberOrderBy.Name, false)
            )
        ).Set;
        Assert.Equal(3, res.Count);
        Assert.Equal(catMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(aliMem, res[2]);
        // get members with desc name order after cat
        res = (
            await ali.OrgMember.Get(
                new(org.Id, true, null, null, catMem.Id, OrgMemberOrderBy.Name, false)
            )
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(aliMem, res[1]);
        // get members with desc role order
        res = (
            await ali.OrgMember.Get(
                new(org.Id, true, null, null, null, OrgMemberOrderBy.Role, false)
            )
        ).Set;
        Assert.Equal(3, res.Count);
        Assert.Equal(catMem, res[0]);
        Assert.Equal(bobMem, res[1]);
        Assert.Equal(aliMem, res[2]);
        // get members with desc role order after cat
        res = (
            await ali.OrgMember.Get(
                new(org.Id, true, null, null, catMem.Id, OrgMemberOrderBy.Role, false)
            )
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(bobMem, res[0]);
        Assert.Equal(aliMem, res[1]);
    }
}
