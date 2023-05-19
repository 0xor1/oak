using System.Text.RegularExpressions;
using Common.Server.Test;
using Common.Shared;
using Oak.Api.ProjectMember;
using S = Oak.I18n.S;

namespace Oak.Eps.Test;

public class EpsUtilTests : TestBase
{
    [Fact]
    public void ValidStr_Success()
    {
        EpsUtil.ValidStr(
            new RpcTestCtx(null, null, S.Inst, new Dictionary<string, string>(), null),
            "yolo",
            1,
            5,
            "test",
            new() { new Regex("^yolo$") }
        );
    }

    [Fact]
    public async void Indirect_EpsUtil_Permissions_Probing_Giggidy()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var bobId = (await bob.Auth.GetSession()).Id;
        var catId = (await cat.Auth.GetSession()).Id;
        var danId = (await dan.Auth.GetSession()).Id;
        var anonId = (await anon.Auth.GetSession()).Id;
        await SetProjectMembers(
            ali,
            org.Id,
            p.Id,
            new()
            {
                new(bobId, ProjectMemberRole.Admin),
                new(catId, ProjectMemberRole.Writer),
                new(danId, ProjectMemberRole.Reader),
            }
        );
        await ali.Project.Update(new(org.Id, p.Id, IsPublic: false));
        RpcTestException? ex = null;
        try
        {
            var res = await dan.Task.Create(new(org.Id, p.Id, p.Id, null, "a"));
        }
        catch (RpcTestException x)
        {
            ex = x;
        }
        Assert.Equal("Insufficient permission", ex.NotNull().Rpc.Message);
        ex = null;
        try
        {
            var res = await anon.Task.GetOne(new(org.Id, p.Id, p.Id));
        }
        catch (RpcTestException x)
        {
            ex = x;
        }
        Assert.Equal("Insufficient permission", ex.NotNull().Rpc.Message);

        var pt = await dan.Task.GetOne(new(org.Id, p.Id, p.Id));
        Assert.Equal(p.Task, pt);
    }
}
