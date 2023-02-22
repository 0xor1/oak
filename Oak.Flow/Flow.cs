using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Oak.Flow;

public record Flow
{
    public Key Key { get; init; }
    public IReadOnlyDictionary<string, IReadOnlyDictionary<Key, string>> Strings { get; init; } =
        new Dictionary<string, IReadOnlyDictionary<Key, string>>();
    public IReadOnlyList<Set> Sets { get; init; } = new List<Set>();
    public IReadOnlyList<Action> Actions { get; init; } = new List<Action>();
}
