using Newtonsoft.Json;

namespace Oak.Flow;

public record Cond<T>
{
    public IReadOnlyList<IReadOnlyList<Cond>> If { get; init; }
    public T Then { get; init; }
    public T? Else { get; init; }
}

public record Cond
{
    // TODO
}