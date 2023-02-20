using Common;
using Newtonsoft.Json;

namespace Oak.Flow;

public record Action
{
    public Key Key { get; init; }
    public ActionType Type { get; init; }
    public IReadOnlyList<Cond<Key>>? Next { get; init; }
}

[JsonConverter(typeof(ActionTypeConverter))]
public record ActionType(Key Key);
public static class ActionTypes
{
    public static readonly ActionType Form = new (new("form"));
    public static readonly ActionType Auto = new (new ("auto"));

    public static readonly IReadOnlyDictionary<Key, ActionType> Types = new List<ActionType>()
    {
        Form,
        Auto
    }.ToDictionary(x => x.Key);
}

public class ActionTypeConverter : JsonConverter
{
    public static readonly ActionTypeConverter Singleton = new();

    private ActionTypeConverter()
    {
    }
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ActionType);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var s = serializer.Deserialize<string>(reader);
        if (s == null || !Key.IsValid(s) || !ActionTypes.Types.TryGetValue(new (s), out var ct))
        {
            throw new InvalidDataException("json action type must be a valid key string value that maps to a supported action type");
        }
        return ct;
    }
    
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, ((ActionType)value.NotNull()).Key);
    }
}