namespace Oak.Flow;

public record Form: Action
{
    public IReadOnlyList<Control> Controls { get; init; }
}