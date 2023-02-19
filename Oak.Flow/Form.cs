namespace Oak.Flow;

public record Form: ActionUnit
{
    public IReadOnlyList<Control> Controls { get; init; }
}