using Common;

namespace Oak.Flow;

public record Branding
{
    public IReadOnlyDictionary<Key, string> Fonts { get; init; }
}