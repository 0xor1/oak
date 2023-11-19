using Common.Shared;

namespace Oak.Eps.Test;

public class CommentTests : TestBase
{
    [Fact]
    public async void Create_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var aliSes = await ali.Auth.GetSession();
        var p = await CreateProject(ali, org.Id);
        var c = await ali.Comment.Create(new(org.Id, p.Id, p.Id, "a"));
        Assert.Equal("a", c.Body);
        Assert.Equal(aliSes.Id, c.CreatedBy);
        Assert.InRange(
            c.CreatedOn,
            DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-1)),
            DateTimeExt.UtcNowMilli()
        );
    }

    [Fact]
    public async void Update_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var aliSes = await ali.Auth.GetSession();
        var p = await CreateProject(ali, org.Id);
        var c = await ali.Comment.Create(new(org.Id, p.Id, p.Id, "a"));
        Assert.Equal("a", c.Body);
        c = await ali.Comment.Update(new(org.Id, p.Id, p.Id, c.Id, "b"));
        Assert.Equal("b", c.Body);
    }

    [Fact]
    public async void Delete_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var aliSes = await ali.Auth.GetSession();
        var p = await CreateProject(ali, org.Id);
        var c = await ali.Comment.Create(new(org.Id, p.Id, p.Id, "a"));
        await ali.Comment.Delete(new(org.Id, p.Id, p.Id, c.Id));
    }

    [Fact]
    public async void Get_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var aliSes = await ali.Auth.GetSession();
        var p = await CreateProject(ali, org.Id);
        var a = await ali.Comment.Create(new(org.Id, p.Id, p.Id, "a"));
        var b = await ali.Comment.Create(new(org.Id, p.Id, p.Id, "b"));
        var c = await ali.Comment.Create(new(org.Id, p.Id, p.Id, "c"));
        // delete c to ensure deleted items dont show up in search results
        await ali.Comment.Delete(new(org.Id, p.Id, p.Id, c.Id));

        var getRes = await ali.Comment.Get(
            new(
                a.Org,
                a.Project,
                a.Task,
                new(
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-2)),
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(2))
                ),
                aliSes.Id
            )
        );
        var @is = getRes.Set;
        Assert.Equal(2, @is.Count);
        Assert.Equal(b, @is[0]);
        Assert.Equal(a, @is[1]);

        getRes = await ali.Comment.Get(
            new(
                a.Org,
                a.Project,
                a.Task,
                new(
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-2)),
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(2))
                ),
                aliSes.Id,
                Asc: true
            )
        );
        @is = getRes.Set;
        Assert.Equal(2, @is.Count);
        Assert.Equal(a, @is[0]);
        Assert.Equal(b, @is[1]);

        getRes = await ali.Comment.Get(
            new(
                a.Org,
                a.Project,
                a.Task,
                new(
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-2)),
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(2))
                ),
                aliSes.Id,
                b.Id
            )
        );
        @is = getRes.Set;
        Assert.Equal(1, @is.Count);
        Assert.Equal(a, @is[0]);

        getRes = await ali.Comment.Get(
            new(
                a.Org,
                a.Project,
                a.Task,
                new(
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-2)),
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(2))
                ),
                aliSes.Id,
                a.Id,
                true
            )
        );
        @is = getRes.Set;
        Assert.Equal(1, @is.Count);
        Assert.Equal(b, @is[0]);
    }
}
