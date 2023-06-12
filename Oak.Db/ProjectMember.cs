using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;
using Oak.Api.ProjectMember;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project), nameof(Id))]
public class ProjectMember
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Id { get; set; }
    public bool IsActive { get; set; }
    public OrgMemberRole OrgRole { get; set; }
    public string Name { get; set; }
    public ProjectMemberRole Role { get; set; }

    public Api.ProjectMember.ProjectMember ToApi(ProjectMemberStats stats)
    {
        return new(
            Org,
            Project,
            Id,
            IsActive,
            OrgRole,
            Name,
            Role,
            stats.TimeEst,
            stats.TimeInc,
            stats.CostEst,
            stats.CostInc,
            stats.FileN,
            stats.FileSize,
            stats.TaskN
        );
    }
}

public class ProjectMemberStats
{
    public string Id { get; set; }
    public ulong TimeEst { get; set; }
    public ulong TimeInc { get; set; }
    public ulong CostEst { get; set; }
    public ulong CostInc { get; set; }
    public ulong FileN { get; set; }
    public ulong FileSize { get; set; }
    public ulong TaskN { get; set; }
}
