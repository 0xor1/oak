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
        var bobPm = await ali.ProjectMember.Create(
            new(org.Id, p.Id, bobId, ProjectMemberRole.Admin)
        );
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
}
