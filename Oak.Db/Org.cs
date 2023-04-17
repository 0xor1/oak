using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.I18n;
using ApiOrg = Oak.Api.Org.Org;

namespace Oak.Db;

[PrimaryKey(nameof(Id))]
public class Org
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }

    public ApiOrg ToApi() => new(Id, Name, CreatedOn);
}
