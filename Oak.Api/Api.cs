using Common.Shared;
using Common.Shared.Auth;
using Oak.Api.App;
using Oak.Api.Comment;
using Oak.Api.File;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Api.Task;
using Oak.Api.VItem;

namespace Oak.Api;

public interface IApi : Common.Shared.Auth.IApi
{
    public IAppApi App { get; }
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
    public IProjectMemberApi ProjectMember { get; }
    public ITaskApi Task { get; }
    public IVItemApi VItem { get; }
    public IFileApi File { get; }
    public ICommentApi Comment { get; }
}

public class Api : IApi
{
    public Api(IRpcClient client)
    {
        Auth = new AuthApi(client);
        App = new AppApi(client);
        Org = new OrgApi(client);
        OrgMember = new OrgMemberApi(client);
        Project = new ProjectApi(client);
        ProjectMember = new ProjectMemberApi(client);
        Task = new TaskApi(client);
        VItem = new VItemApi(client);
        File = new FileApi(client);
        Comment = new CommentApi(client);
    }

    public IAuthApi Auth { get; }
    public IAppApi App { get; }
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
    public IProjectMemberApi ProjectMember { get; }
    public ITaskApi Task { get; }
    public IVItemApi VItem { get; }
    public IFileApi File { get; }
    public ICommentApi Comment { get; }
}

public enum ActivityItemType
{
    Org,
    Project,
    Member,
    Task,
    VItem,
    File,
    Comment
}

public enum ActivityAction
{
    Create,
    Update,
    Delete
}

public record FcmData(Activity Activity, IReadOnlyList<string> Ancestors);
