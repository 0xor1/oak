using Common.Server.Test;
using Common.Shared;
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

        // get all private projects
        var res = await ali.Project.Get(new(org.Id));
        Assert.Equal(1, res.Count);
        Assert.Equal(b, res[0]);

        // get all public projects
        res = await ali.Project.Get(new(org.Id, IsPublic: true));
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get public projects with filters
        res = await ali.Project.Get(
            new(
                org.Id,
                IsPublic: true,
                NameStartsWith: "a",
                CreatedOn: new(a.CreatedOn, a.CreatedOn),
                StartOn: new(a.StartOn.NotNull(), a.StartOn.NotNull()),
                EndOn: new(a.EndOn.NotNull(), a.EndOn.NotNull())
            )
        );
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get private projects as perProject permission user
        res = await dan.Project.Get(new(org.Id));
        Assert.Equal(0, res.Count);

        // get all public projects ordered By Name
        res = await ali.Project.Get(new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.Name));
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get all public projects ordered By Name after A
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, After: a.Id, OrderBy: ProjectOrderBy.Name)
        );
        Assert.Equal(1, res.Count);
        Assert.Equal(c, res[0]);

        // get all public projects ordered By CreatedOn
        res = await ali.Project.Get(new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.CreatedOn));
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get all public projects ordered By CreatedOn after a
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, After: a.Id, OrderBy: ProjectOrderBy.CreatedOn)
        );
        Assert.Equal(1, res.Count);
        Assert.Equal(c, res[0]);

        // get all public projects ordered By StartOn
        res = await ali.Project.Get(new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.StartOn));
        Assert.Equal(2, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(a, res[1]);

        // get all public projects ordered By StartOn after C
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, After: c.Id, OrderBy: ProjectOrderBy.StartOn)
        );
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get all public projects ordered By EndOn
        res = await ali.Project.Get(new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.EndOn));
        Assert.Equal(2, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(a, res[1]);

        // get all public projects ordered By EndOn after C
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, After: c.Id, OrderBy: ProjectOrderBy.EndOn)
        );
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get all public projects ordered By Name Desc
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.Name, Asc: false)
        );
        Assert.Equal(2, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(a, res[1]);

        // get all public projects ordered By Name Desc after C
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, After: c.Id, OrderBy: ProjectOrderBy.Name, Asc: false)
        );
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get all public projects ordered By CreatedOn Desc
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.CreatedOn, Asc: false)
        );
        Assert.Equal(2, res.Count);
        Assert.Equal(c, res[0]);
        Assert.Equal(a, res[1]);

        // get all public projects ordered By CreatedOn Desc after C
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, After: c.Id, OrderBy: ProjectOrderBy.CreatedOn, Asc: false)
        );
        Assert.Equal(1, res.Count);
        Assert.Equal(a, res[0]);

        // get all public projects ordered By StartOn Desc
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.StartOn, Asc: false)
        );
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get all public projects ordered By StartOn Desc after A
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, After: a.Id, OrderBy: ProjectOrderBy.StartOn, Asc: false)
        );
        Assert.Equal(1, res.Count);
        Assert.Equal(c, res[0]);

        // get all public projects ordered By EndOn Desc
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, OrderBy: ProjectOrderBy.EndOn, Asc: false)
        );
        Assert.Equal(2, res.Count);
        Assert.Equal(a, res[0]);
        Assert.Equal(c, res[1]);

        // get all public projects ordered By EndOn Desc after A
        res = await ali.Project.Get(
            new(org.Id, IsPublic: true, After: a.Id, OrderBy: ProjectOrderBy.EndOn, Asc: false)
        );
        Assert.Equal(1, res.Count);
        Assert.Equal(c, res[0]);
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
        var backToA = aUp with { Name = a.Name };
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
            Assert.Equal("Entity not found", ex.Rpc.Message);
        }
    }
}
