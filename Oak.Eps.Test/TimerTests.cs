using Common.Shared;
using Oak.Api.VItem;

namespace Oak.Eps.Test;

public class TimerTests : TestBase
{
    [Fact]
    public async void All_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var aliSes = await ali.Auth.GetSession();
        var tt = await CreateTaskTree(ali, org.Id);
        var ts = await ali.Timer.Create(new(org.Id, tt.P.Id, tt.P.Id));
        ts = await ali.Timer.Create(new(org.Id, tt.P.Id, tt.A.Id));
        ts = await ali.Timer.Create(new(org.Id, tt.P.Id, tt.B.Id));
        Assert.Equal(3, ts.Count);

        var getRes = await ali.Timer.Get(new(org.Id, tt.P.Id, tt.P.Id, aliSes.Id));
        Assert.False(getRes.More);
        Assert.Equal(1, getRes.Set.Count);
        Assert.Equal(ts.Single(x => x.Task == tt.P.Id), getRes.Set[0]);

        getRes = await ali.Timer.Get(new(org.Id, tt.P.Id, Asc: true));
        Assert.False(getRes.More);
        Assert.Equal(3, getRes.Set.Count);
        Assert.Equal(tt.B.Id, getRes.Set[0].Task);
        Assert.Equal(tt.P.Id, getRes.Set[1].Task);
        Assert.Equal(tt.A.Id, getRes.Set[2].Task);

        ts = await ali.Timer.Update(new(org.Id, tt.P.Id, tt.A.Id, true));
        ts = await ali.Timer.Update(new(org.Id, tt.P.Id, tt.A.Id, false));
        Assert.True(ts.All(x => x.IsRunning == false));

        ts = await ali.Timer.Update(new(org.Id, tt.P.Id, tt.B.Id, true));
        Assert.True(ts.SingleOrDefault(x => x.IsRunning) != null);

        ts = await ali.Timer.Delete(new(org.Id, tt.P.Id, tt.B.Id));
        Assert.Equal(2, ts.Count);
        Assert.True(ts.All(x => x.Task != tt.B.Id));
    }
}
