namespace Oak.Db;

public class File
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Task { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public ulong Size { get; set; }
    public string Type { get; set; }
}
