using Common;
using Newtonsoft.Json;

namespace Oak.Flow;

public record ActionUnit
{
    public Key Key { get; init; }
    public ActionUnitType Type { get; init; }
    public IReadOnlyList<Cond<Key>>? Next { get; init; }
}

[JsonConverter(typeof(ActionUnitTypeConverter))]
public record ActionUnitType(Key Key);
public static class ActionUnitTypes
{
    public static readonly ActionUnitType Form = new (new("form"));
    public static readonly ActionUnitType Auto = new (new ("auto"));

    public static readonly IReadOnlyDictionary<Key, ActionUnitType> Types = new List<ActionUnitType>()
    {
        Form,
        Auto
    }.ToDictionary(x => x.Key);
}

public class ActionUnitTypeConverter : JsonConverter
{
    public static readonly ActionUnitTypeConverter Singleton = new();

    private ActionUnitTypeConverter()
    {
    }
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ActionUnitType);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var s = serializer.Deserialize<string>(reader);
        if (s == null || !Key.IsValid(s) || !ActionUnitTypes.Types.TryGetValue(new (s), out var ct))
        {
            throw new InvalidDataException("json action unit type must be a valid key string value that maps to a supported action unit type");
        }
        return ct;
    }
    
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, ((ActionUnitType)value.NotNull()).Key);
    }
}