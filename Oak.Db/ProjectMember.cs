namespace Oak.Db;

public class ProjectMember
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Member { get; set; }
    public ProjectMemberRole Role { get; set; }
}

public enum ProjectMemberRole
{
    Admin,
    Writer,
    Reader
}
