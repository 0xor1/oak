using Common.Server.Test;
using Common.Shared;
using Oak.Api;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Db;
using Org = Oak.Api.Org.Org;
using Project = Oak.Api.Project.Project;
using ProjectMember = Oak.Api.ProjectMember.ProjectMember;
using S = Oak.I18n.S;
using Task = System.Threading.Tasks.Task;

namespace Oak.Eps.Test;

public class TestBase : IDisposable
{
    protected readonly RpcTestRig<OakDb, Api.Api> Rig;

    public TestBase()
    {
        Rig = new RpcTestRig<OakDb, Api.Api>(S.Inst, OakEps.Eps, c => new Api.Api(c));
    }

    protected async Task<(IApi Ali, IApi Bob, IApi Cat, IApi Dan, IApi Anon, Org)> Setup()
    {
        var userName = "ali";
        var (ali, _, _) = await Rig.NewApi(userName);
        var org = await ali.Org.Create(new("a", userName));
        var (bob, _, _) = await Rig.NewApi("bob");
        var bobId = (await bob.Auth.GetSession()).Id;
        var (cat, _, _) = await Rig.NewApi("cat");
        var catId = (await cat.Auth.GetSession()).Id;
        var (dan, _, _) = await Rig.NewApi("dan");
        var danId = (await dan.Auth.GetSession()).Id;
        var (anon, _, _) = await Rig.NewApi();

        await ali.OrgMember.Add(new(org.Id, bobId, "bob", OrgMemberRole.Admin));
        await ali.OrgMember.Add(new(org.Id, catId, "cat", OrgMemberRole.WriteAllProjects));
        await ali.OrgMember.Add(new(org.Id, danId, "dan", OrgMemberRole.PerProject));

        return (ali, bob, cat, dan, anon, org);
    }

    protected async Task<Project> CreateProject(IApi api, string org) =>
        await api.Project.Create(
            new(
                org,
                true,
                "a",
                "£",
                "GBP",
                8,
                5,
                DateTimeExt.UtcNowMilli(),
                DateTimeExt.UtcNowMilli().Add(TimeSpan.FromDays(5)),
                10
            )
        );

    protected async Task<List<ProjectMember>> SetProjectMembers(
        IApi api,
        string org,
        string project,
        List<(string Id, ProjectMemberRole Role)> users
    )
    {
        var res = new List<ProjectMember>();
        foreach (var user in users)
        {
            res.Add(await api.ProjectMember.Add(new(org, project, user.Id, user.Role)));
        }
        return res;
    }

    public void Dispose()
    {
        Rig.Dispose();
    }
}
