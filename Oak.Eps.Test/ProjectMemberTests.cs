using Oak.Api.ProjectMember;

namespace Oak.Eps.Test;

public class ProjectMemberTests : TestBase
{
    [Fact]
    public async void Create_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var bobId = (await bob.Auth.GetSession()).Id;
        var bobPm = await ali.ProjectMember.Add(new(org.Id, p.Id, bobId, ProjectMemberRole.Admin));
        Assert.Equal(bobId, bobPm.Id);
        Assert.Equal("bob", bobPm.Name);
    }

    [Fact]
    public async void GetOne_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var aliId = (await ali.Auth.GetSession()).Id;
        var aliPm = await ali.ProjectMember.GetOne(new(org.Id, p.Id, aliId));
        Assert.Equal(aliId, aliPm.Id);
        Assert.Equal("ali", aliPm.Name);
    }

    [Fact]
    public async void Get_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var aliId = (await ali.Auth.GetSession()).Id;
        var bobId = (await bob.Auth.GetSession()).Id;
        var catId = (await cat.Auth.GetSession()).Id;
        var danId = (await dan.Auth.GetSession()).Id;
        var aliPm = await ali.ProjectMember.GetOne(new(org.Id, p.Id, aliId));
        var mems = await SetProjectMembers(
            ali,
            org.Id,
            p.Id,
            new()
            {
                (bobId, ProjectMemberRole.Admin),
                (catId, ProjectMemberRole.Writer),
                (danId, ProjectMemberRole.Reader)
            }
        );
        var bobPm = mems[0];
        var catPm = mems[1];
        var danPm = mems[2];

        // all filters
        var res = (
            await ali.ProjectMember.Get(new(org.Id, p.Id, ProjectMemberRole.Admin, "a"))
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(aliPm, res[0]);

        // order by role asc
        res = (await ali.ProjectMember.Get(new(org.Id, p.Id))).Set;
        Assert.Equal(4, res.Count);
        Assert.Equal(aliPm, res[0]);
        Assert.Equal(bobPm, res[1]);
        Assert.Equal(catPm, res[2]);
        Assert.Equal(danPm, res[3]);

        // order by role asc after ali
        res = (await ali.ProjectMember.Get(new(org.Id, p.Id, After: aliId))).Set;
        Assert.Equal(3, res.Count);
        Assert.Equal(bobPm, res[0]);
        Assert.Equal(catPm, res[1]);
        Assert.Equal(danPm, res[2]);

        // order by role desc
        res = (await ali.ProjectMember.Get(new(org.Id, p.Id, Asc: false))).Set;
        Assert.Equal(4, res.Count);
        Assert.Equal(danPm, res[0]);
        Assert.Equal(catPm, res[1]);
        Assert.Equal(aliPm, res[2]);
        Assert.Equal(bobPm, res[3]);

        // order by role desc after bob
        res = (await ali.ProjectMember.Get(new(org.Id, p.Id, After: aliId, Asc: false))).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(bobPm, res[0]);

        // order by name asc
        res = (
            await ali.ProjectMember.Get(new(org.Id, p.Id, OrderBy: ProjectMemberOrderBy.Name))
        ).Set;
        Assert.Equal(4, res.Count);
        Assert.Equal(aliPm, res[0]);
        Assert.Equal(bobPm, res[1]);
        Assert.Equal(catPm, res[2]);
        Assert.Equal(danPm, res[3]);

        // order by name asc after ali
        res = (
            await ali.ProjectMember.Get(
                new(org.Id, p.Id, After: aliId, OrderBy: ProjectMemberOrderBy.Name)
            )
        ).Set;
        Assert.Equal(3, res.Count);
        Assert.Equal(bobPm, res[0]);
        Assert.Equal(catPm, res[1]);
        Assert.Equal(danPm, res[2]);

        // order by name desc
        res = (
            await ali.ProjectMember.Get(
                new(org.Id, p.Id, OrderBy: ProjectMemberOrderBy.Name, Asc: false)
            )
        ).Set;
        Assert.Equal(4, res.Count);
        Assert.Equal(danPm, res[0]);
        Assert.Equal(catPm, res[1]);
        Assert.Equal(bobPm, res[2]);
        Assert.Equal(aliPm, res[3]);

        // order by name desc after bob
        res = (
            await ali.ProjectMember.Get(
                new(org.Id, p.Id, After: bobId, OrderBy: ProjectMemberOrderBy.Name, Asc: false)
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(aliPm, res[0]);
    }

    [Fact]
    public async void Update_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var aliId = (await ali.Auth.GetSession()).Id;
        var bobId = (await bob.Auth.GetSession()).Id;
        var catId = (await cat.Auth.GetSession()).Id;
        var mems = await SetProjectMembers(
            ali,
            org.Id,
            p.Id,
            new() { (bobId, ProjectMemberRole.Writer), (catId, ProjectMemberRole.Writer), }
        );
        var bobPm = mems[0];
        var catPm = mems[1];

        var updatedBob = await ali.ProjectMember.Update(
            new(org.Id, p.Id, bobPm.Id, ProjectMemberRole.Reader)
        );
        // bob is an org admin so cant be made anything less than a project admin
        Assert.Equal(ProjectMemberRole.Admin, updatedBob.Role);

        var updatedCat = await ali.ProjectMember.Update(
            new(org.Id, p.Id, catPm.Id, ProjectMemberRole.Reader)
        );
        Assert.Equal(ProjectMemberRole.Reader, updatedCat.Role);
        Assert.Equal(catPm, updatedCat with { Role = ProjectMemberRole.Writer });
    }

    [Fact]
    public async void Remove_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var aliId = (await ali.Auth.GetSession()).Id;
        var bobId = (await bob.Auth.GetSession()).Id;
        var mems = await SetProjectMembers(
            ali,
            org.Id,
            p.Id,
            new() { (bobId, ProjectMemberRole.Admin), }
        );
        var aliPm = await ali.ProjectMember.GetOne(new(org.Id, p.Id, aliId));
        var bobPm = mems[0];

        await ali.ProjectMember.Remove(new(org.Id, p.Id, bobPm.Id));
        var res = (await ali.ProjectMember.Get(new(org.Id, p.Id))).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(aliPm, res[0]);
    }
}
