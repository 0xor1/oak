using Microsoft.EntityFrameworkCore;

namespace Oak.Db;

public class OakDb : DbContext
{
    public OakDb(DbContextOptions<OakDb> opts)
        : base(opts) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // all enum to string conversions
        modelBuilder
            .Entity<OrgMember>()
            .Property(e => e.Role)
            .HasConversion(
                v => v.ToString(),
                v => (OrgMemberRole)Enum.Parse(typeof(OrgMemberRole), v)
            );
        modelBuilder
            .Entity<ProjectMember>()
            .Property(e => e.Role)
            .HasConversion(
                v => v.ToString(),
                v => (ProjectMemberRole)Enum.Parse(typeof(ProjectMemberRole), v)
            );
        modelBuilder
            .Entity<Activity>()
            .Property(e => e.ItemType)
            .HasConversion(
                v => v.ToString(),
                v => (ActivityItemType)Enum.Parse(typeof(ActivityItemType), v)
            );
        modelBuilder
            .Entity<Activity>()
            .Property(e => e.Action)
            .HasConversion(
                v => v.ToString(),
                v => (ActivityAction)Enum.Parse(typeof(ActivityAction), v)
            );
        modelBuilder
            .Entity<VItem>()
            .Property(e => e.Type)
            .HasConversion(v => v.ToString(), v => (VItemType)Enum.Parse(typeof(VItemType), v));
    }

    public List<string> SetAncestralChainAggregateValuesFromTask(
        string org,
        string project,
        string task
    )
    {
        return Database
            .SqlQueryRaw<string>(
                "CALL SetAncestralChainAggregateValuesFromTask({0}, {1}, {2})",
                org,
                project,
                task
            )
            .ToList();
    }

    public DbSet<Auth> Auths { get; set; } = null!;
    public DbSet<Org> Orgs { get; set; } = null!;
    public DbSet<OrgMember> OrgMembers { get; set; } = null!;
    public DbSet<ProjectLock> ProjectLocks { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    public DbSet<Activity> Activities { get; set; } = null!;
    public DbSet<Task> Tasks { get; set; } = null!;
    public DbSet<VItem> VItems { get; set; } = null!;
    public DbSet<File> Files { get; set; } = null!;
    public DbSet<Note> Notes { get; set; } = null!;
}
