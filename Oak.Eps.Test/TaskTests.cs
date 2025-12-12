using System.Text;
using Common.Server.Test;
using Common.Shared;
using Oak.Api.File;
using Oak.Api.ProjectMember;

namespace Oak.Eps.Test;

public class TaskTests : TestBase
{
    [Fact]
    public async Task Create_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var res = await ali.Task.Create(new(org.Id, p.Id, p.Id, null, "a"));
        Assert.Equal(p.Id, res.Parent.Id);
        Assert.Equal(1ul, res.Parent.ChildN);
        Assert.Equal(1ul, res.Parent.DescN);
        Assert.Equal("a", res.Created.Name);
    }

    [Fact]
    public async Task Create_ForOtherUser_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var p = await CreateProject(ali, org.Id);
        var bobSes = await bob.Auth.GetSession();
        await SetProjectMembers(ali, org.Id, p.Id, new() { (bobSes.Id, ProjectMemberRole.Writer) });
        var res = await ali.Task.Create(new(org.Id, p.Id, p.Id, null, "a", user: bobSes.Id));
        Assert.Equal(p.Id, res.Parent.Id);
        Assert.Equal(1ul, res.Parent.ChildN);
        Assert.Equal(1ul, res.Parent.DescN);
        Assert.Equal("a", res.Created.Name);
    }

    [Fact]
    public async Task Update_CantMoveProject()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        RpcTestException? ex = null;
        try
        {
            await ali.Task.Update(new(org.Id, tt.P.Id, tt.P.Id, prevSib: new(tt.B.Id)));
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
    public async Task Update_NoMove_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(
            new(org.Id, tt.P.Id, tt.B.Id, parent: tt.P.Id, prevSib: new(tt.A.Id))
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
    public async Task Update_IsParallel_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, isParallel: false));
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
    public async Task Update_TimeEst_CostEst_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.F.Id, timeEst: 10, costEst: 10));
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
    public async Task Update_IsParallel_AndMove_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.B.Id, parent: tt.A.Id, isParallel: true));
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
    public async Task Update_Project_NoMove_Success()
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
                name: "new name",
                description: "a meaningful description",
                user: new(bobSes.Id)
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
    public async Task Update_CantMakeVerticalRecursiveLoops()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        RpcTestException? ex = null;
        try
        {
            await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, parent: tt.E.Id));
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
    public async Task Update_CantMakeHorizontalRecursiveLoops()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        RpcTestException? ex = null;
        try
        {
            await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, prevSib: new(tt.A.Id)));
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
    public async Task Update_A_After_B_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, prevSib: new(tt.B.Id)));
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
    public async Task Update_A_After_C_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, prevSib: new(tt.C.Id)));
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
    public async Task Update_A_After_D_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.A.Id, prevSib: new(tt.D.Id)));
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
    public async Task Update_D_Before_C_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.D.Id, prevSib: new(tt.B.Id)));
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
    public async Task Update_D_Before_B_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.D.Id, prevSib: new(tt.A.Id)));
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
    public async Task Update_D_Before_A_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.D.Id, prevSib: new(null)));
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
    public async Task Update_B_After_C_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.B.Id, prevSib: new(tt.C.Id)));
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
    public async Task Update_B_Under_A_Success()
    {
        // oldPrevSib is newParent
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.B.Id, parent: tt.A.Id));
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
    public async Task Update_B_Under_A_After_E_Success()
    {
        // oldPrevSib is newParent
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(
            new(org.Id, tt.P.Id, tt.B.Id, parent: tt.A.Id, prevSib: new(tt.E.Id))
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
    public async Task Update_C_Under_A_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(
            new(org.Id, tt.P.Id, tt.C.Id, parent: tt.A.Id, prevSib: new(tt.E.Id))
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
    public async Task Update_E_Under_P_Success()
    {
        // oldParent is newNextSib
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(new(org.Id, tt.P.Id, tt.E.Id, parent: tt.P.Id));
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
    public async Task Update_E_Under_P_After_A_Success()
    {
        // oldParent is newPrevSib
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        await ali.Task.Update(
            new(org.Id, tt.P.Id, tt.E.Id, parent: tt.P.Id, prevSib: new(tt.A.Id))
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
    public async Task GetAncestors_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        var ancestors = await ali.Task.GetAncestors(new(org.Id, tt.P.Id, tt.F.Id));
        Assert.Equal(2, ancestors.Count);
        Assert.Equal(tt.A, ancestors[0]);
        Assert.Equal(tt.P, ancestors[1]);
    }

    [Fact]
    public async Task GetChildren_Success()
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
    public async Task GetDescendants_Empty()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        var descendants = await ali.Task.GetAllDescendants(new(org.Id, tt.P.Id, tt.F.Id));
        Assert.Equal(0, descendants.Count);
    }

    [Fact]
    public async Task GetInitView_Success()
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

    [Fact]
    public async Task Delete_B()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        var p = await ali.Task.Delete(new(org.Id, tt.P.Id, tt.B.Id));
        Assert.Equal(tt.P.Id, p.Id);
        Assert.Equal(3ul, p.ChildN);
        Assert.Equal(7ul, p.DescN);
        Assert.Equal(34ul, p.TimeSubEst);
        Assert.Equal(34ul, p.CostSubEst);
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.MaybeB);
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.A.Id, tt.P.FirstChild);
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
    public async Task Delete_A()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        // upload a file so we know it has been deleted
        var upload = new Upload(org.Id, tt.P.Id, tt.E.Id);
        var test = "yolo baby!";
        using var us = new MemoryStream(Encoding.UTF8.GetBytes(test));
        upload.Stream = new RpcStream(us, "test", "text/plain", false, (ulong)us.Length);
        var fileRes = await ali.File.Upload(upload);
        Assert.Equal(tt.E.Id, fileRes.Task.Id);
        await tt.Refresh();
        Assert.Equal(1ul, tt.P.FileSubN);
        Assert.Equal(fileRes.File.Size, tt.P.FileSubSize);
        var p = await ali.Task.Delete(new(org.Id, tt.P.Id, tt.A.Id));
        Assert.Equal(tt.P.Id, p.Id);
        Assert.Equal(3ul, p.ChildN);
        Assert.Equal(3ul, p.DescN);
        Assert.Equal(9ul, p.TimeSubEst);
        Assert.Equal(9ul, p.CostSubEst);
        await tt.Refresh();
        // validate the structure
        Assert.Null(tt.P.NextSib);
        Assert.Equal(tt.B.Id, tt.P.FirstChild);
        Assert.Equal(tt.C.Id, tt.B.NextSib);
        Assert.Equal(tt.D.Id, tt.C.NextSib);
        Assert.Null(tt.D.NextSib);
        Assert.Null(tt.MaybeA);
        Assert.Null(tt.MaybeE);
        Assert.Null(tt.MaybeF);
        Assert.Null(tt.MaybeG);
        Assert.Null(tt.MaybeH);
        Assert.Equal(0ul, tt.P.FileSubN);
        Assert.Equal(0ul, tt.P.FileSubSize);
    }
}
