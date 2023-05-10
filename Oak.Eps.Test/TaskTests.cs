using Common.Server.Test;
using Common.Shared;
using Oak.Api.ProjectMember;

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

    [Fact]
    public async void Create_ForOtherUSer_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var bobSes = await bob.Auth.GetSession();
        await SetProjectMembers(ali, org.Id, p.Id, new() { (bobSes.Id, ProjectMemberRole.Writer) });
        var res = await ali.Task.Create(new(org.Id, p.Id, p.Id, null, "a", User: bobSes.Id));
        Assert.Equal(p.Id, res.Parent.Id);
        Assert.Equal(1ul, res.Parent.ChildN);
        Assert.Equal(1ul, res.Parent.DescN);
        Assert.Equal("a", res.New.Name);
    }

    [Fact]
    public async void Update_CantMoveProject()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        RpcTestException? ex = null;
        try
        {
            await ali.Task.Update(new(org.Id, tt.P.Id, tt.P.Id, PrevSib: new(tt.B.Id)));
        }
        catch (RpcTestException x)
        {
            ex = x;
        }
        Assert.Equal("Can't move root project node", ex.Rpc.Message);
        await tt.Refresh();
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
    }

    [Fact]
    public async void Update_NoMove_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(
            new(org.Id, tt.P.Id, tt.B.Id, Parent: tt.P.Id, PrevSib: new(tt.A.Id))
        );
        await tt.Refresh();
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
    }

    [Fact]
    public async void Update_IsParallel_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, IsParallel: false));
        await tt.Refresh();
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

        Assert.Equal(26ul, tt.A.TimeSubMin);
        Assert.Equal(36ul, tt.P.TimeSubMin);
    }

    [Fact]
    public async void Update_TimeEst_CostEst_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.F.Id, TimeEst: 10, CostEst: 10));
        await tt.Refresh();
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

        Assert.Equal(10ul, tt.A.TimeSubMin);
        Assert.Equal(30ul, tt.A.TimeSubEst);
        Assert.Equal(20ul, tt.P.TimeSubMin);
        Assert.Equal(40ul, tt.P.TimeSubEst);
    }

    [Fact]
    public async void Update_IsParallel_AndMove_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.B.Id, Parent: tt.A.Id, IsParallel: true));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
        Assert.Equal(tt.C.Id, tt.A.NextSib);
        Assert.Equal(tt.D.Id, tt.C.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.B.Id, tt.A.FirstChild);
        Assert.Equal(tt.E.Id, tt.B.NextSib);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_Project_NoMove_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        var bobSes = await bob.Auth.GetSession();
        await SetProjectMembers(
            ali,
            org.Id,
            tt.P.Id,
            new() { (bobSes.Id, ProjectMemberRole.Writer) }
        );
        await ali.Task.Update(
            new(
                org.Id,
                tt.P.Id,
                tt.P.Id,
                Name: "new name",
                Description: "a meaningful description",
                User: new(bobSes.Id)
            )
        );

        await tt.Refresh();
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
    }

    [Fact]
    public async void Update_CantMakeVerticalRecursiveLoops()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        RpcTestException? ex = null;
        try
        {
            await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, Parent: tt.E.Id));
        }
        catch (RpcTestException x)
        {
            ex = x;
        }
        Assert.Equal("Move operation would result in recursive loop", ex.Rpc.Message);
        await tt.Refresh();
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
    }

    [Fact]
    public async void Update_CantMakeHorizontalRecursiveLoops()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        RpcTestException? ex = null;
        try
        {
            await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, PrevSib: new(tt.A.Id)));
        }
        catch (RpcTestException x)
        {
            ex = x;
        }
        Assert.Equal("Move operation would result in recursive loop", ex.Rpc.Message);
        await tt.Refresh();
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
    }

    // horizontal moves
    [Fact]
    public async void Update_A_After_B_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, PrevSib: new(tt.B.Id)));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.B.Id, tt.P.FirstChild);
        Assert.Equal(tt.A.Id, tt.B.NextSib);
        Assert.Equal(tt.C.Id, tt.A.NextSib);
        Assert.Equal(tt.D.Id, tt.C.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_A_After_C_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, PrevSib: new(tt.C.Id)));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.B.Id, tt.P.FirstChild);
        Assert.Equal(tt.C.Id, tt.B.NextSib);
        Assert.Equal(tt.A.Id, tt.C.NextSib);
        Assert.Equal(tt.D.Id, tt.A.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_A_After_D_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, PrevSib: new(tt.D.Id)));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.B.Id, tt.P.FirstChild);
        Assert.Equal(tt.C.Id, tt.B.NextSib);
        Assert.Equal(tt.D.Id, tt.C.NextSib);
        Assert.Equal(tt.A.Id, tt.D.NextSib);
        Assert.Null(tt.A.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_D_Before_C_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.D.Id, PrevSib: new(tt.B.Id)));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
        Assert.Equal(tt.B.Id, tt.A.NextSib);
        Assert.Equal(tt.D.Id, tt.B.NextSib);
        Assert.Equal(tt.C.Id, tt.D.NextSib);
        Assert.Null(tt.C.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_D_Before_B_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.D.Id, PrevSib: new(tt.A.Id)));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
        Assert.Equal(tt.D.Id, tt.A.NextSib);
        Assert.Equal(tt.B.Id, tt.D.NextSib);
        Assert.Equal(tt.C.Id, tt.B.NextSib);
        Assert.Null(tt.C.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_D_Before_A_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.D.Id, PrevSib: new(null)));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.D.Id, tt.P.FirstChild);
        Assert.Equal(tt.A.Id, tt.D.NextSib);
        Assert.Equal(tt.B.Id, tt.A.NextSib);
        Assert.Equal(tt.C.Id, tt.B.NextSib);
        Assert.Null(tt.C.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_B_After_C_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.B.Id, PrevSib: new(tt.C.Id)));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
        Assert.Equal(tt.C.Id, tt.A.NextSib);
        Assert.Equal(tt.B.Id, tt.C.NextSib);
        Assert.Equal(tt.D.Id, tt.B.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    // vertical moves

    [Fact]
    public async void Update_B_Under_A_Success()
    {
        // oldPrevSib is newParent
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.B.Id, Parent: tt.A.Id));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
        Assert.Equal(tt.C.Id, tt.A.NextSib);
        Assert.Equal(tt.D.Id, tt.C.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.B.Id, tt.A.FirstChild);
        Assert.Equal(tt.E.Id, tt.B.NextSib);
        Assert.Equal(tt.F.Id, tt.E.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_B_Under_A_After_E_Success()
    {
        // oldPrevSib is newParent
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(
            new(org.Id, tt.P.Id, tt.B.Id, Parent: tt.A.Id, PrevSib: new(tt.E.Id))
        );
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
        Assert.Equal(tt.C.Id, tt.A.NextSib);
        Assert.Equal(tt.D.Id, tt.C.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.B.Id, tt.E.NextSib);
        Assert.Equal(tt.F.Id, tt.B.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_C_Under_A_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(
            new(org.Id, tt.P.Id, tt.C.Id, Parent: tt.A.Id, PrevSib: new(tt.E.Id))
        );
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
        Assert.Equal(tt.B.Id, tt.A.NextSib);
        Assert.Equal(tt.D.Id, tt.B.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.E.Id, tt.A.FirstChild);
        Assert.Equal(tt.C.Id, tt.E.NextSib);
        Assert.Equal(tt.F.Id, tt.C.NextSib);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_E_Under_P_Success()
    {
        // oldParent is newNextSib
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.E.Id, Parent: tt.P.Id));
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.E.Id, tt.P.FirstChild);
        Assert.Equal(tt.A.Id, tt.E.NextSib);
        Assert.Equal(tt.B.Id, tt.A.NextSib);
        Assert.Equal(tt.C.Id, tt.B.NextSib);
        Assert.Equal(tt.D.Id, tt.C.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.F.Id, tt.A.FirstChild);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void Update_E_Under_P_After_A_Success()
    {
        // oldParent is newPrevSib
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(
            new(org.Id, tt.P.Id, tt.E.Id, Parent: tt.P.Id, PrevSib: new(tt.A.Id))
        );
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
        Assert.Equal(tt.E.Id, tt.A.NextSib);
        Assert.Equal(tt.B.Id, tt.E.NextSib);
        Assert.Equal(tt.C.Id, tt.B.NextSib);
        Assert.Equal(tt.D.Id, tt.C.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Equal(tt.F.Id, tt.A.FirstChild);
        Assert.Equal(tt.G.Id, tt.F.NextSib);
        Assert.Equal(tt.H.Id, tt.G.NextSib);
        Assert.Null(tt.H.NextSib);
    }

    [Fact]
    public async void GetAncestors_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        var ancestors = await ali.Task.GetAncestors(new(org.Id, tt.P.Id, tt.F.Id));
        Assert.Equal(2, ancestors.Count);
        Assert.Equal(tt.A, ancestors[0]);
        Assert.Equal(tt.P, ancestors[1]);
    }

    [Fact]
    public async void GetChildren_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        var children = await ali.Task.GetChildren(new(org.Id, tt.P.Id, tt.A.Id, null));
        Assert.Equal(4, children.Count);
        Assert.Equal(tt.E, children[0]);
        Assert.Equal(tt.F, children[1]);
        Assert.Equal(tt.G, children[2]);
        Assert.Equal(tt.H, children[3]);
        children = await ali.Task.GetChildren(new(org.Id, tt.P.Id, tt.A.Id, tt.F.Id));
        Assert.Equal(2, children.Count);
        Assert.Equal(tt.G, children[0]);
        Assert.Equal(tt.H, children[1]);
    }

    [Fact]
    public async void GetDescendants_Empty()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        var descendants = await ali.Task.GetAllDescendants(new(org.Id, tt.P.Id, tt.F.Id));
        Assert.Equal(0, descendants.Count);
    }

    [Fact]
    public async void GetInitView_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        var init = await ali.Task.GetInitView(new(org.Id, tt.P.Id, tt.A.Id));
        Assert.Equal(tt.A, init.Task);
        Assert.Equal(1, init.Ancestors.Count);
        Assert.Equal(tt.P, init.Ancestors[0]);
        Assert.Equal(4, init.Children.Count);
        Assert.Equal(tt.E, init.Children[0]);
        Assert.Equal(tt.F, init.Children[1]);
        Assert.Equal(tt.G, init.Children[2]);
        Assert.Equal(tt.H, init.Children[3]);
    }
}
