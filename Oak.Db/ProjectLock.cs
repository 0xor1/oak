using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.I18n;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project))]
public class ProjectLock
{
    public string Org { get; set; }
    public string Project { get; set; }
}
