using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.I18n;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Id))]
public class Project
{
    public string Org { get; set; }
    public string Id { get; set; }
    public bool IsArchived { get; set; }
    public bool IsPublic { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CurrencySymbol { get; set; }
    public string CurrencyCode { get; set; }
    public uint? HoursPerDay { get; set; }
    public uint? DaysPerWeek { get; set; }
    public DateTime? StartOn { get; set; }
    public DateTime? EndOn { get; set; }
    public ulong FileLimit { get; set; }
}
