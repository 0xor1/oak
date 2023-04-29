namespace Oak.Api.ProjectMember;

public class ProjectMember
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Id { get; set; }
    public ProjectMemberRole Role { get; set; }

    // TimeEst  uint64     `json:"timeEst"`
    // TimeInc  uint64     `json:"timeInc"`
    // CostEst  uint64     `json:"costEst"`
    // CostInc  uint64     `json:"costInc"`
    // FileN    uint64     `json:"fileN"`
    // FileSize uint64     `json:"fileSize"`
    // TaskN    uint64     `json:"taskN"`
}

public enum ProjectMemberRole
{
    Admin,
    Writer,
    Reader
}
