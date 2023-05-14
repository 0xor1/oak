using Amazon.S3;
using Common.Server;
using Common.Server.Test;
using Common.Shared;
using Microsoft.Extensions.DependencyInjection;
using Oak.Api;
using Oak.Api.OrgMember;
using Oak.Api.ProjectMember;
using Oak.Db;
using Org = Oak.Api.Org.Org;
using Project = Oak.Api.Project.Project;
using ProjectMember = Oak.Api.ProjectMember.ProjectMember;
using S = Oak.I18n.S;
using ApiTask = Oak.Api.Task.Task;
using Task = System.Threading.Tasks.Task;

namespace Oak.Eps.Test;

public class TestBase : IDisposable
{
    protected readonly RpcTestRig<OakDb, Api.Api> Rig;

    public TestBase()
    {
        Rig = new RpcTestRig<OakDb, Api.Api>(
            S.Inst,
            OakEps.Eps,
            c => new Api.Api(c),
            OrgEps.InitApp
        );
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

    protected async Task<TestTree> CreateTaskTree(IApi api, string org)
    {
        var p = await api.Project.Create(
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

        var tt = await TestTree.Init(api, org, p.Id);

        // validate the values are correct
        Assert.Equal(8ul, tt.P.DescN);
        Assert.Equal(4ul, tt.P.ChildN);
        Assert.Equal(18ul, tt.P.TimeSubMin);
        Assert.Equal(36ul, tt.P.TimeSubEst);
        Assert.Equal(36ul, tt.P.CostSubEst);
        Assert.Equal(4ul, tt.A.DescN);
        Assert.Equal(4ul, tt.A.ChildN);
        Assert.Equal(8ul, tt.A.TimeSubMin);
        Assert.Equal(26ul, tt.A.TimeSubEst);
        Assert.Equal(26ul, tt.A.CostSubEst);
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
        Assert.Equal(tt.B.Id, tt.A.NextSib);
        Assert.Equal(tt.C.Id, tt.B.NextSib);
        Assert.Equal(tt.D.Id, tt.C.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);

        return tt;
    }

    public void Dispose()
    {
        Rig.Dispose();
    }
}

public record TestTree
{
    private IApi api { get; init; }
    private string org { get; init; }
    private string project { get; init; }

    public ApiTask? MaybeP { get; private set; }
    public ApiTask? MaybeA { get; private set; }
    public ApiTask? MaybeB { get; private set; }
    public ApiTask? MaybeC { get; private set; }
    public ApiTask? MaybeD { get; private set; }
    public ApiTask? MaybeE { get; private set; }
    public ApiTask? MaybeF { get; private set; }
    public ApiTask? MaybeG { get; private set; }
    public ApiTask? MaybeH { get; private set; }
    public ApiTask P => MaybeP.NotNull();
    public ApiTask A => MaybeA.NotNull();
    public ApiTask B => MaybeB.NotNull();
    public ApiTask C => MaybeC.NotNull();
    public ApiTask D => MaybeD.NotNull();
    public ApiTask E => MaybeE.NotNull();
    public ApiTask F => MaybeF.NotNull();
    public ApiTask G => MaybeG.NotNull();
    public ApiTask H => MaybeH.NotNull();

    private TestTree() { }

    public static async Task<TestTree> Init(IApi api, string org, string project)
    {
        // make 4 tasks a-d that are all children of the root node, in order
        var a = (
            await api.Task.Create(
                new(org, project, project, null, "a", TimeEst: 1, CostEst: 1, IsParallel: true)
            )
        ).New;
        var b = (
            await api.Task.Create(new(org, project, project, a.Id, "b", TimeEst: 2, CostEst: 2))
        ).New;
        var c = (
            await api.Task.Create(new(org, project, project, b.Id, "c", TimeEst: 3, CostEst: 3))
        ).New;
        var d = (
            await api.Task.Create(new(org, project, project, c.Id, "d", TimeEst: 4, CostEst: 4))
        ).New;

        // make 4 tasks e-h that are all children of a, created in reverse order
        var h = (
            await api.Task.Create(new(org, project, a.Id, null, "h", TimeEst: 8, CostEst: 8))
        ).New;
        var g = (
            await api.Task.Create(new(org, project, a.Id, null, "g", TimeEst: 7, CostEst: 7))
        ).New;
        var f = (
            await api.Task.Create(new(org, project, a.Id, null, "f", TimeEst: 6, CostEst: 6))
        ).New;
        var e = (
            await api.Task.Create(new(org, project, a.Id, null, "e", TimeEst: 5, CostEst: 5))
        ).New;
        var p = await api.Task.GetOne(new(org, project, project));

        var all = (await api.Task.GetAllDescendants(new(org, project, project)))
            .OrderBy(x => x.Name)
            .ToList();

        var tt = new TestTree()
        {
            api = api,
            org = org,
            project = project
        };
        await tt.Refresh();
        return tt;
    }

    public async Task Refresh()
    {
        var p = await api.Task.GetOne(new(org, project, project));
        var all = (await api.Task.GetAllDescendants(new(org, project, project)))
            .OrderBy(x => x.Name)
            .ToList();
        MaybeP = p;
        MaybeA = all.SingleOrDefault(x => x.Name == "a");
        MaybeB = all.SingleOrDefault(x => x.Name == "b");
        MaybeC = all.SingleOrDefault(x => x.Name == "c");
        MaybeD = all.SingleOrDefault(x => x.Name == "d");
        MaybeE = all.SingleOrDefault(x => x.Name == "e");
        MaybeF = all.SingleOrDefault(x => x.Name == "f");
        MaybeG = all.SingleOrDefault(x => x.Name == "g");
        MaybeH = all.SingleOrDefault(x => x.Name == "h");
    }
}
