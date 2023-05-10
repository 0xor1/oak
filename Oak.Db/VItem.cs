using Microsoft.EntityFrameworkCore;
using Oak.Api.VItem;

namespace Oak.Db;

[PrimaryKey(
    nameof(Org),
    nameof(Project),
    nameof(Task),
    nameof(Type),
    nameof(CreatedOn),
    nameof(CreatedBy)
)]
public class VItem
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Task { get; set; }
    public VItemType Type { get; set; }
    public string Id { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public ulong Inc { get; set; }
    public string Note { get; set; }

    public Api.VItem.VItem ToApi()
    {
        return new(Org, Project, Task, Type, Id, CreatedBy, CreatedOn, Inc, Note);
    }
}
