using Microsoft.EntityFrameworkCore;
using Oak.Api;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project), nameof(OccurredOn), nameof(Item), nameof(User))]
public class Activity
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string? Task { get; set; }
    public DateTime OccurredOn { get; set; }
    public string User { get; set; }
    public string Item { get; set; }
    public ActivityItemType ItemType { get; set; }
    public bool TaskDeleted { get; set; }
    public bool ItemDeleted { get; set; }
    public ActivityAction Action { get; set; }
    public string? TaskName { get; set; }
    public string? ItemName { get; set; }
    public string? ExtraInfo { get; set; }

    public Api.Project.Activity ToApi() =>
        new Api.Project.Activity(
            Org,
            Project,
            Task,
            OccurredOn,
            User,
            Item,
            ItemType,
            TaskDeleted,
            ItemDeleted,
            Action,
            TaskName,
            ItemName,
            ExtraInfo
        );
}
