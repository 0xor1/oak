using Common;

namespace Oak.Flow;

public record Set
{
    public Key Key { get; init; }
    public IReadOnlyList<Key> Options { get; init; }
}