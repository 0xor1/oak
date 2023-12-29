namespace Oak.Client.Test;

public class HomeTest : TestBase
{
    [Fact]
    public async Task Load_Success()
    {
        var ali = await NewTestPack("ali");
        var aliSes = await ali.Api.Auth.GetSession();
        var aliUi = ali.Ctx.RenderComponent<Oak.Client.Shared.Pages.Home>(
            ps => ps.Add(p => p.Session, aliSes)
        );
        ali.Ctx.DisposeComponents();
    }
}
