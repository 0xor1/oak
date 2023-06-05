namespace Oak.Client.Lib;

public record UIDisplay
{
    public bool User { get; set; } = true;
    public bool Time { get; set; } = true;
    public bool Cost { get; set; } = true;
    public bool File { get; set; } = true;
    public bool SubCounts { get; set; } = true;
    public bool CreatedOn { get; set; } = true;
}
