using Common.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Oak.Flow;

public record Flow
{
    public Key Key { get; init; }
    public Key? Start { get; init; }
    public IReadOnlyDictionary<string, Lang> Langs { get; init; } = new Dictionary<string, Lang>();
    public IReadOnlyList<Set> Sets { get; init; } = new List<Set>();
    public IReadOnlyList<Action> Actions { get; init; } = new List<Action>();

    private static readonly JsonSerializerSettings Jss =
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

    public static Flow FromJson(string json) =>
        JsonConvert.DeserializeObject<Flow>(json, Jss).NotNull();

    public string ToJson(bool indent = false) =>
        JsonConvert.SerializeObject(this, indent ? Formatting.Indented : Formatting.None, Jss);
}
