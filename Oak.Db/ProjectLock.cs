using Microsoft.EntityFrameworkCore;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project))]
public class ProjectLock
{
    public string Org { get; set; }
    public string Project { get; set; }
}
