using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Oak.Flow;

public record Flow
{
    public Key Key { get; init; }
    public IReadOnlyDictionary<string, IReadOnlyDictionary<Key, string>> Strings { get; init; }
    public IReadOnlyList<Set> Sets { get; init; }
    public IReadOnlyList<Action> Actions { get; init; }
}