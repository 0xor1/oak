namespace Oak.Client.Lib;

public record UIDisplay
{
    public bool Time { get; set; } = true;
    public bool Cost { get; set; } = true;
    public bool File { get; set; } = true;
}
