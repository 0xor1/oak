using System.Text;
using Common.Server.Test;
using Common.Shared;
using Oak.Api.File;
using Oak.Api.ProjectMember;
using Oak.Api.VItem;

namespace Oak.Eps.Test;

public class FileTests : TestBase
{
    [Fact]
    public async void Upload_Download_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var tt = await CreateTaskTree(ali, org.Id);
        var upload = new Upload(org.Id, tt.P.Id, tt.E.Id);
        var test = "yolo baby!";
        using var us = new MemoryStream(Encoding.UTF8.GetBytes(test));
        upload.Stream = new RpcStream(us, "test", "text/plain", false, (ulong)us.Length);
        var fileRes = await ali.File.Upload(upload);
        var f = fileRes.File;
        Assert.Equal(tt.E.Id, fileRes.Task.Id);
        await tt.Refresh();
        Assert.Equal(1ul, tt.P.FileSubN);
        Assert.Equal(f.Size, tt.P.FileSubSize);

        var resp = await ali.File.Download(new(f.Org, f.Project, f.Task, f.Id, false));
        using var sr = new StreamReader(resp.Stream.Data);
        var res = await sr.ReadToEndAsync();
        Assert.Equal(test, res);
    }
}
