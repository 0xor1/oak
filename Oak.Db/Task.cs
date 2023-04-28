using Microsoft.EntityFrameworkCore;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project), nameof(Id))]
public class Task
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Id { get; set; }
    public string? Parent { get; set; }
    public string? FirstChild { get; set; }
    public string? NextSib { get; set; }
    public string? User { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public ulong TimeEst { get; set; }
    public ulong TimeInc { get; set; }
    public ulong TimeSubMin { get; set; }
    public ulong TimeSubEst { get; set; }
    public ulong TimeSubInc { get; set; }
    public ulong CostEst { get; set; }
    public ulong CostInc { get; set; }
    public ulong CostSubEst { get; set; }
    public ulong CostSubInc { get; set; }
    public ulong FileN { get; set; }
    public ulong FileSize { get; set; }
    public ulong FileSubN { get; set; }
    public ulong FileSubSize { get; set; }
    public ulong ChildN { get; set; }
    public ulong DescN { get; set; }
    public bool IsParallel { get; set; }

    public Api.Task.Task ToApi() =>
        new(
            Org,
            Project,
            Id,
            Parent,
            FirstChild,
            NextSib,
            User,
            Name,
            Description,
            CreatedBy,
            CreatedOn,
            TimeEst,
            TimeInc,
            TimeSubMin,
            TimeSubEst,
            TimeSubInc,
            CostEst,
            CostInc,
            CostSubEst,
            CostSubInc,
            FileN,
            FileSize,
            FileSubN,
            FileSubSize,
            ChildN,
            DescN,
            IsParallel
        );
}
