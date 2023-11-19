using Common.Server.Test;
using Common.Shared;
using Oak.Api;
using Oak.Api.Project;

namespace Oak.Eps.Test;

public class ProjectTests : TestBase
{
    [Fact]
    public async void Create_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await ali.Project.Create(
            new(
                org.Id,
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
        Assert.Equal("a", p.Name);
        Assert.False(p.IsArchived);
        Assert.True(p.IsPublic);
        Assert.Equal("£", p.CurrencySymbol);
        Assert.Equal("GBP", p.CurrencyCode);
        Assert.Equal((uint)8, p.HoursPerDay);
        Assert.Equal((uint)5, p.DaysPerWeek);
        Assert.Equal((ulong)10, p.FileLimit);
    }

    [Fact]
    public async void Get_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var a = await ali.Project.Create(
            new(
                org.Id,
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
        Assert.Equal("a", a.Name);
        var b = await ali.Project.Create(
            new(
                org.Id,
                false,
                "b",
                "$",
                "USD",
                8,
                5,
                DateTimeExt.UtcNowMilli().Add(TimeSpan.FromDays(-1)),
                DateTimeExt.UtcNowMilli().Add(TimeSpan.FromDays(-1)).Add(TimeSpan.FromDays(5)),
                10
            )
        );
        var c = await ali.Project.Create(
            new(
                org.Id,
                true,
                "c",
                "€",
                "EUR",
                8,
                5,
                DateTimeExt.UtcNowMilli().Add(TimeSpan.FromDays(-2)),
                DateTimeExt.UtcNowMilli().Add(TimeSpan.FromDays(-2)).Add(TimeSpan.FromDays(5)),
                10
            )
        );

        // get a specifc project by id
        var one = await ali.Project.GetOne(new(org.Id, b.Id));
        Assert.Equal(b, one);

        // get all projects
        var res = (await ali.Project.Get(new(org.Id))).Set;
        Assert.Equal(3, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(b, res[1]);
        Assert.Equal(c, res[2]);

        // get all public projects
        res = (await ali.Project.Get(new(org.Id, IsPublic: true))).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get public projects with filters
        res = (
            await ali.Project.Get(
                new(
                    org.Id,
                    IsPublic: true,
                    NameStartsWith: "a",
                    CreatedOn: new(a.CreatedOn, a.CreatedOn),
                    StartOn: new(a.StartOn.NotNull(), a.StartOn.NotNull()),
                    EndOn: new(a.EndOn.NotNull(), a.EndOn.NotNull())
                )
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get private projects as perProject permission user
        res = (await dan.Project.Get(new(org.Id))).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get all public projects ordered By Name
        res = (
            await ali.Project.Get(new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.Name))
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get all public projects ordered By Name after A
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, After: a.Id, OrderBy: ProjectOrderBy.Name)
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(c, res[0]);

        // get all public projects ordered By CreatedOn
        res = (
            await ali.Project.Get(new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.CreatedOn))
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get all public projects ordered By CreatedOn after a
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, After: a.Id, OrderBy: ProjectOrderBy.CreatedOn)
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(c, res[0]);

        // get all public projects ordered By StartOn
        res = (
            await ali.Project.Get(new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.StartOn))
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(a, res[1]);

        // get all public projects ordered By StartOn after C
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, After: c.Id, OrderBy: ProjectOrderBy.StartOn)
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get all public projects ordered By EndOn
        res = (
            await ali.Project.Get(new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.EndOn))
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(a, res[1]);

        // get all public projects ordered By EndOn after C
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, After: c.Id, OrderBy: ProjectOrderBy.EndOn)
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get all public projects ordered By Name Desc
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.Name, Asc: false)
            )
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(a, res[1]);

        // get all public projects ordered By Name Desc after C
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, After: c.Id, OrderBy: ProjectOrderBy.Name, Asc: false)
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get all public projects ordered By CreatedOn Desc
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.CreatedOn, Asc: false)
            )
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(a, res[1]);

        // get all public projects ordered By CreatedOn Desc after C
        res = (
            await ali.Project.Get(
                new(
                    org.Id,
                    IsPublic: true,
                    After: c.Id,
                    OrderBy: ProjectOrderBy.CreatedOn,
                    Asc: false
                )
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get all public projects ordered By StartOn Desc
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.StartOn, Asc: false)
            )
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get all public projects ordered By StartOn Desc after A
        res = (
            await ali.Project.Get(
                new(
                    org.Id,
                    IsPublic: true,
                    After: a.Id,
                    OrderBy: ProjectOrderBy.StartOn,
                    Asc: false
                )
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(c, res[0]);

        // get all public projects ordered By EndOn Desc
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.EndOn, Asc: false)
            )
        ).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get all public projects ordered By EndOn Desc after A
        res = (
            await ali.Project.Get(
                new(org.Id, IsPublic: true, After: a.Id, OrderBy: ProjectOrderBy.EndOn, Asc: false)
            )
        ).Set;
        Assert.Equal(1, res.Count);
        Assert.Equal(c, res[0]);

        // get as non org member
        var (edd, _, _) = await Rig.NewApi("edd");
        res = (await edd.Project.Get(new(org.Id))).Set;
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);
    }

    [Fact]
    public async void Update_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var a = await ali.Project.Create(
            new(
                org.Id,
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
        var aUp = await ali.Project.Update(new(a.Org, a.Id, Name: "aUp"));
        Assert.Equal("aUp", aUp.Name);
        var backToA = aUp with { Name = a.Name, Task = a.Task with { Name = a.Task.Name } };
        Assert.Equal(a, backToA);
    }

    [Fact]
    public async void Delete_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var a = await ali.Project.Create(
            new(
                org.Id,
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
        await ali.Project.Delete(new(a.Org, a.Id));
        try
        {
            await ali.Project.GetOne(new(a.Org, a.Id));
        }
        catch (RpcTestException ex)
        {
            Assert.Equal("Project not found", ex.Rpc.Message);
        }
    }

    [Fact]
    public async void GetActivities_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var a = await ali.Project.Create(
            new(
                org.Id,
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
        var aliSes = await ali.Auth.GetSession();
        a = await ali.Project.Update(new(a.Org, a.Id, Name: "yolo"));
        var res = await ali.Project.GetActivities(new(a.Org, a.Id, Task: a.Id));
        Assert.Equal(2, res.Set.Count);
        Assert.Equal("yolo", res.Set[0].ItemName);
        Assert.Equal(org.Id, res.Set[0].Org);
        Assert.Equal(a.Id, res.Set[0].Project);
        Assert.Equal(a.Id, res.Set[0].Task);
        Assert.InRange(
            res.Set[0].OccurredOn,
            DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-1)),
            DateTimeExt.UtcNowMilli()
        );
        Assert.Equal(a.Id, res.Set[0].Project);
        Assert.Equal(aliSes.Id, res.Set[0].User);
        Assert.Equal(a.Id, res.Set[0].Item);
        Assert.Equal(ActivityItemType.Project, res.Set[0].ItemType);
        Assert.False(res.Set[0].TaskDeleted);
        Assert.False(res.Set[0].ItemDeleted);
        Assert.Equal(ActivityAction.Update, res.Set[0].Action);
        Assert.Equal("yolo", res.Set[0].TaskName);
        Assert.NotNull(res.Set[0].ExtraInfo);
        Assert.Equal("a", res.Set[1].ItemName);
        res = await ali.Project.GetActivities(
            new(
                a.Org,
                a.Id,
                Task: a.Id,
                Item: a.Id,
                OccuredOn: new MinMax<DateTime>(
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-1)),
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(1))
                ),
                User: aliSes.Id,
                Asc: true
            )
        );
        Assert.Equal(2, res.Set.Count);
        Assert.Equal("a", res.Set[0].ItemName);
        Assert.Equal("yolo", res.Set[1].ItemName);
    }
}
