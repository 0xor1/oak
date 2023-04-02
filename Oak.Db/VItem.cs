namespace Oak.Db;

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
}

public enum VItemType
{
    Time,
    Cost
}
