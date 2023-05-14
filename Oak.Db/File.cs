using Microsoft.EntityFrameworkCore;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project), nameof(Task), nameof(CreatedOn), nameof(CreatedBy))]
public class File
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Task { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public ulong Size { get; set; }
    public string Type { get; set; }

    public Api.File.File ToApi() =>
        new(Org, Project, Task, Id, Name, CreatedBy, CreatedOn, Size, Type);
}
