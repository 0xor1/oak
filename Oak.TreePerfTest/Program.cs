// See https://aka.ms/new-console-template for more information

using Common.Shared;
using Oak.Api;

const string baseHref = "https://localhost:9500/";
const string email = "asd@asd.asd";
const string pwd = "asdASD123@";
const string orgId = "AYj4i_tbWPvz-m8lOmkh9A";
const string projectId = "AYj-OyGqe8NEsPPrbot5zw";
const int k = 3; // must be > 1
const int h = 5;

var api = new Api(new RpcHttpClient(baseHref, new HttpClient(), Console.WriteLine));
await api.Auth.SignIn(new (email, pwd, true));
var p = await api.Project.GetOne(new(orgId, projectId));

var start = DateTime.UtcNow;

await CreateTree(p.Id, 0, 0, k, h);

var dur = DateTime.UtcNow.Subtract(start);
Console.WriteLine();
Console.WriteLine($"time: {dur.ToString()}");

async Task<int> CreateTree(string parentId, int lastIdx, int currentDepth, int k, int h)
{
    if(currentDepth >= h)
    {
        return lastIdx;
    }
    string? prevSib = null;
    for(var i = 0; i < k; i++ )
    {
        lastIdx++;
        Console.Write($"\rcreating node {lastIdx}");
        var isParallel = true;
        var timeEst = (ulong)lastIdx*10;
        if (currentDepth == h - 1 || i % 2 == 0)
        {
            isParallel = false;
        }

        var res = await api.Task.Create(new (orgId, projectId, parentId, prevSib, lastIdx.ToString(), "", isParallel, null, timeEst, timeEst));
        prevSib = res.New.Id;
        lastIdx = await CreateTree(prevSib, lastIdx, currentDepth + 1, k, h);
    }

    return lastIdx;
}
