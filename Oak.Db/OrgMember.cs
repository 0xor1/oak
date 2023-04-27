using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Id))]
public class OrgMember
{
    public string Org { get; set; }
    public string Id { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public OrgMemberRole Role { get; set; }

    public Api.OrgMember.OrgMember ToApi() => new(Org, Id, IsActive, Name, Role);
}
