using Common.Shared;

namespace Oak.Flow;

public record Lang
{
    public IReadOnlyDictionary<Key, IReadOnlyDictionary<Key, string>> Sets { get; init; } =
        new Dictionary<Key, IReadOnlyDictionary<Key, string>>();

    public IReadOnlyDictionary<Key, ActionStrings> Actions { get; init; } =
        new Dictionary<Key, ActionStrings>();

    public IReadOnlyDictionary<Key, string> Strings { get; init; } = new Dictionary<Key, string>();
}

public record ActionStrings
{
    public string Name { get; init; }
    public string Title { get; init; }
    public IReadOnlyDictionary<Key, ControlStrings> Controls { get; init; } =
        new Dictionary<Key, ControlStrings>();
}

public record ControlStrings
{
    public string Main { get; init; }
    public string? Invalid { get; init; } = null;
};
