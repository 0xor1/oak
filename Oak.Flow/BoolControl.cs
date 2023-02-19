using Common;

namespace Oak.Flow;

public record BoolControl : InputControl
{
    public BoolType DisplayAs { get; init; } = BoolTypes.Checkbox;
}

public record BoolType(Key Key);
public static class BoolTypes
{
    public static readonly BoolType YesNo = new (new("yes_no"));
    public static readonly BoolType Checkbox = new (new ("checkbox"));

    public static readonly IReadOnlyDictionary<Key, BoolType> Types = new List<BoolType>()
    {
        YesNo,
        Checkbox
    }.ToDictionary(x => x.Key);
}