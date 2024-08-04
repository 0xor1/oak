using System.Text;
using Common.Shared;
using Oak.Api.File;

namespace Oak.Eps.Test;

public class FileTests : TestBase
{
    [Fact]
    public async void Upload_Download_Delete_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var aliSes = await ali.Auth.GetSession();
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
        Assert.Equal("test", f.Name);
        Assert.Equal(aliSes.Id, f.CreatedBy);
        Assert.InRange(
            f.CreatedOn,
            DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-1)),
            DateTimeExt.UtcNowMilli()
        );
        Assert.Equal("text/plain", f.Type);

        var url = ali.File.DownloadUrl(new(f.Org, f.Project, f.Task, f.Id, false));
        Assert.False(url.IsNullOrEmpty());
        var resp = await ali.File.Download(new(f.Org, f.Project, f.Task, f.Id, false));
        using var sr = new StreamReader(resp.Stream.Data);
        var res = await sr.ReadToEndAsync();
        Assert.Equal(test, res);

        var e = await ali.File.Delete(new(org.Id, tt.P.Id, tt.E.Id, f.Id));
        Assert.Equal(0ul, e.FileN);
        Assert.Equal(0ul, e.FileSize);
        await tt.Refresh();
        Assert.Equal(tt.E, e);
        Assert.Equal(0ul, tt.P.FileSubN);
        Assert.Equal(0ul, tt.P.FileSubSize);
    }

    [Fact]
    public async void Get_Success()
    {
        var (ali, bob, cat, dan, anon, org) = await Setup();
        var aliSes = await ali.Auth.GetSession();
        var tt = await CreateTaskTree(ali, org.Id);

        var upload = new Upload(org.Id, tt.P.Id, tt.E.Id);
        var test = "aaa yolo baby!";
        using var aus = new MemoryStream(Encoding.UTF8.GetBytes(test));
        upload.Stream = new RpcStream(aus, "a", "text/plain", false, (ulong)aus.Length);
        var fileRes = await ali.File.Upload(upload);
        var a = fileRes.File;

        upload = new Upload(org.Id, tt.P.Id, tt.E.Id);
        test = "bbb nolo baby!";
        using var bus = new MemoryStream(Encoding.UTF8.GetBytes(test));
        upload.Stream = new RpcStream(bus, "b", "text/plain", false, (ulong)bus.Length);
        fileRes = await ali.File.Upload(upload);
        var b = fileRes.File;

        var getRes = await ali.File.Get(
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

        getRes = await ali.File.Get(
            new(
                a.Org,
                a.Project,
                a.Task,
                new(
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(-2)),
                    DateTimeExt.UtcNowMilli().Add(TimeSpan.FromSeconds(2))
                ),
                aliSes.Id,
                asc: true
            )
        );
        @is = getRes.Set;
        Assert.Equal(2, @is.Count);
        Assert.Equal(a, @is[0]);
        Assert.Equal(b, @is[1]);

        getRes = await ali.File.Get(
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

        getRes = await ali.File.Get(
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
