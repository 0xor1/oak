using Microsoft.EntityFrameworkCore;
using ApiOrg = Oak.Api.Org.Org;

namespace Oak.Db;

[PrimaryKey(nameof(Id))]
public class Org
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }

    public ApiOrg ToApi(OrgMember? m) => new(Id, Name, CreatedOn, m?.ToApi());
}
