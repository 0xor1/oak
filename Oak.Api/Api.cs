using Common.Shared;
using Common.Shared.Auth;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;

namespace Oak.Api;

public interface IApi : Common.Shared.Auth.IApi
{
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
    public IProjectMemberApi ProjectMember { get; }
}

public class Api : IApi
{
    public Api(IRpcClient client)
    {
        Auth = new AuthApi(client);
        Org = new OrgApi(client);
        OrgMember = new OrgMemberApi(client);
        Project = new ProjectApi(client);
        ProjectMember = new ProjectMemberApi(client);
    }

    public IAuthApi Auth { get; }
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
    public IProjectMemberApi ProjectMember { get; }
}
