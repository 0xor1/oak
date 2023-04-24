using Microsoft.EntityFrameworkCore;
using Oak.Api.OrgMember;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Member))]
public class OrgMember
{
    public string Org { get; set; }
    public string Member { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public OrgMemberRole Role { get; set; }

    public Api.OrgMember.OrgMember ToApi() => new(Org, Member, IsActive, Name, Role);
}
