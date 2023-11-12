namespace Oak.Eps.Test;

public class AppTests : TestBase
{
    [Fact]
    public async void GetConfig_Success()
    {
        var (ali, _, _) = await Rig.NewApi("ali");
        var c = await ali.App.GetConfig();
        Assert.True(c.DemoMode);
        Assert.Equal("https://github.com/0xor1/oak", c.RepoUrl);
    }
}
