// Generated Code File, Do Not Edit.
// This file is generated with Common.Cli.
// see https://github.com/0xor1/common/blob/main/Common.Cli/Api.cs
// executed with arguments: api <abs_file_path_to>/Oak.Api

using Common.Shared;
using Common.Shared.Auth;
using Oak.Api.Comment;
using Oak.Api.File;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Api.Task;
using Oak.Api.Timer;
using Oak.Api.VItem;


namespace Oak.Api;

public interface IApi : Common.Shared.Auth.IApi
{
    public ICommentApi Comment { get; }
    public IFileApi File { get; }
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
    public IProjectMemberApi ProjectMember { get; }
    public ITaskApi Task { get; }
    public ITimerApi Timer { get; }
    public IVItemApi VItem { get; }
    
}

public class Api : IApi
{
    public Api(IRpcClient client)
    {
        App = new AppApi(client);
        Auth = new AuthApi(client);
        Comment = new CommentApi(client);
        File = new FileApi(client);
        Org = new OrgApi(client);
        OrgMember = new OrgMemberApi(client);
        Project = new ProjectApi(client);
        ProjectMember = new ProjectMemberApi(client);
        Task = new TaskApi(client);
        Timer = new TimerApi(client);
        VItem = new VItemApi(client);
        
    }

    public IAppApi App { get; }
    public IAuthApi Auth { get; }
    public ICommentApi Comment { get; }
    public IFileApi File { get; }
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
    public IProjectMemberApi ProjectMember { get; }
    public ITaskApi Task { get; }
    public ITimerApi Timer { get; }
    public IVItemApi VItem { get; }
    
}