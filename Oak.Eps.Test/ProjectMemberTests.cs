using Common.Server.Auth;
using Common.Server.Test;
using Common.Shared;
using Oak.Api;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Db;
using Org = Oak.Api.Org.Org;
using S = Oak.I18n.S;

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
        var res = await ali.ProjectMember.Get(new(org.Id, p.Id, ProjectMemberRole.Admin));
        Assert.Equal(2, res.Count);
        Assert.Equal(aliPm, res[0]);
        Assert.Equal(bobPm, res[1]);
    }
}
