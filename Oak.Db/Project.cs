using Common.Server;
using Common.Shared;
using Oak.I18n;

namespace Oak.Db;

public class Project
{
    public string Org { get; set; }
    public string Id { get; set; }
    public bool IsArchived { get; set; }
    public bool IsPublic { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CurrencyCode { get; set; }
    public ushort? HoursPerDay { get; set; }
    public ushort? DaysPerWeek { get; set; }
    public DateTime? StartOn { get; set; }
    public DateTime? EndOn { get; set; }
    public ulong FileLimit { get; set; }
}
