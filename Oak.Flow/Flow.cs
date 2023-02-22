using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Oak.Flow;

public record Flow
{
    public Key Key { get; init; }
    public IReadOnlyDictionary<string, Lang> Strings { get; init; } =
        new Dictionary<string, Lang>();
    public IReadOnlyList<Set> Sets { get; init; } = new List<Set>();
    public IReadOnlyList<Action> Actions { get; init; } = new List<Action>();

    private static readonly JsonSerializerSettings _jss =
        new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>()
            {
                new ControlConverter(),
                new ActionConverter(),
            }
        };

    public static Flow FromJson(string json)
    {
        return JsonConvert.DeserializeObject<Flow>(json, _jss);
    }

    public string ToJson(bool indent = false)
    {
        return JsonConvert.SerializeObject(
            this,
            indent ? Formatting.Indented : Formatting.None,
            _jss
        );
    }
}

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
