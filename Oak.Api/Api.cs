﻿using Common.Shared;
using Common.Shared.Auth;
using Oak.Api.Org;
using Oak.Api.OrgMember;
using Oak.Api.Project;
using Oak.Api.ProjectMember;
using Oak.Api.Task;

namespace Oak.Api;

public interface IApi : Common.Shared.Auth.IApi
{
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
    public IProjectMemberApi ProjectMember { get; }
    public ITaskApi Task { get; }
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
        Task = new TaskApi(client);
    }

    public IAuthApi Auth { get; }
    public IOrgApi Org { get; }
    public IOrgMemberApi OrgMember { get; }
    public IProjectApi Project { get; }
    public IProjectMemberApi ProjectMember { get; }
    public ITaskApi Task { get; }
}

public enum ActivityItemType
{
    Org,
    Project,
    Member,
    Task,
    Time,
    Cost,
    File,
    Note
}

public enum ActivityAction
{
    Create,
    Update,
    Delete
}
