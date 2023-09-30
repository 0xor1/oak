using Common.Shared;
using Common.Server.Auth;
using Microsoft.EntityFrameworkCore;

namespace Oak.Db;

public class OakDb : DbContext, IAuthDb
{
    public OakDb(DbContextOptions<OakDb> opts)
        : base(opts) { }

    public async System.Threading.Tasks.Task LockProject(
        string org,
        string id,
        CancellationToken ctkn = default
    )
    {
        var pl = await ProjectLocks
            .FromSql($"SELECT * FROM ProjectLocks WHERE Org={org} AND id={id} FOR UPDATE")
            .SingleOrDefaultAsync(ctkn);
        Throw.DataIf(pl == null, $"project doesnt exists, org: {org}, id: {id}");
    }

    public async Task<List<string>> SetAncestralChainAggregateValuesFromTask(
        string org,
        string project,
        string task,
        CancellationToken ctkn = default
    )
    {
        return await Database
            .SqlQuery<string>(
                $"CALL SetAncestralChainAggregateValuesFromTask({org}, {project}, {task})"
            )
            .ToListAsync(ctkn);
    }

    public DbSet<Auth> Auths { get; set; } = null!;
    public DbSet<FcmReg> FcmRegs { get; set; } = null!;
    public DbSet<Org> Orgs { get; set; } = null!;
    public DbSet<OrgMember> OrgMembers { get; set; } = null!;
    public DbSet<ProjectLock> ProjectLocks { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    public DbSet<Activity> Activities { get; set; } = null!;
    public DbSet<Task> Tasks { get; set; } = null!;
    public DbSet<VItem> VItems { get; set; } = null!;
    public DbSet<File> Files { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
}
