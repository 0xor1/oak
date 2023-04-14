using Microsoft.EntityFrameworkCore;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project), nameof(OccurredOn), nameof(Item), nameof(Member))]
public class Activity
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string? Task { get; set; }
    public DateTime OccurredOn { get; set; }
    public string Member { get; set; }
    public string Item { get; set; }
    public ActivityItemType ItemType { get; set; }
    public bool TaskDeleted { get; set; }
    public bool ItemDeleted { get; set; }
    public ActivityAction Action { get; set; }
    public string? TaskName { get; set; }
    public string? ItemName { get; set; }
    public string? ExtraInfo { get; set; }
}

public enum ActivityItemType
{
    Org,
    Project,
    Member,
    Task,
    Time,
    Cost,
    File,
    Note
}

public enum ActivityAction
{
    Create,
    Update,
    Delete
}
