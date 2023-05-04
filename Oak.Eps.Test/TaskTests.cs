using Oak.Api.ProjectMember;
using Oak.Api.Task;

namespace Oak.Eps.Test;

public class TaskTests : TestBase
{
    [Fact]
    public async void Create_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var res = await ali.Task.Create(new(org.Id, p.Id, p.Id, null, "a"));
        Assert.Equal(p.Id, res.Parent.Id);
        Assert.Equal(1ul, res.Parent.ChildN);
        Assert.Equal(1ul, res.Parent.DescN);
        Assert.Equal("a", res.New.Name);
    }
}
