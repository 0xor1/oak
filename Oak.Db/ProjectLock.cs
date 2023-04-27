using Microsoft.EntityFrameworkCore;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Id))]
public class ProjectLock
{
    public string Org { get; set; }
    public string Id { get; set; }
}
