using Microsoft.EntityFrameworkCore;
using Oak.Api.ProjectMember;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project), nameof(Id))]
public class ProjectMember
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public ProjectMemberRole Role { get; set; }

    public Api.ProjectMember.ProjectMember ToApi()
    {
        throw new NotImplementedException();
    }
}
