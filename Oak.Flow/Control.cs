using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oak.Flow.Controls;

namespace Oak.Flow;

[JsonConverter(typeof(ControlConverter))]
public record Control
{
    public Key Key { get; init; }
    public ControlType Type { get; init; }
    public IReadOnlyList<Cond<Key>>? Next { get; init; }
}

public abstract record InputControl : Control
{
    public IReadOnlyList<Cond<bool>>? Required { get; init; }
}


[JsonConverter(typeof(ControlTypeConverter))]
public record ControlType(Key Key, Type Control);
public static class ControlTypes
{
    public static readonly ControlType Static = new (new("static"), typeof(Control));
    public static readonly ControlType Bool = new (new ("bool"), typeof(BoolControl));
    public static readonly ControlType Int = new (new ("int"), typeof(IntControl));
    public static readonly ControlType Decimal = new (new ("decimal"), typeof(IntControl));
    public static readonly ControlType String = new (new ("string"), typeof(StringControl));

    public static readonly IReadOnlyDictionary<Key, ControlType> Types = new List<ControlType>()
    {
        Static,
        Bool,
        Int,
        Decimal,
        String
    }.ToDictionary(x => x.Key);
}

public class ControlConverter : JsonConverter
{
    public static readonly ControlConverter Singleton = new();

    private ControlConverter()
    {
    }
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Control);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var o = serializer.Deserialize<JObject>(reader);
        if (o == null)
        {
            throw new InvalidDataException("json control object can not be null");
        }
        if (!o.TryGetValue("type", out var t))
        {
            throw new InvalidDataException("json control object missing type propertry");
        }
        var kStr = t.Value<string>();
        if (kStr == null || !Key.IsValid(kStr) || !ControlTypes.Types.TryGetValue(new (kStr), out var ct))
        {
            throw new InvalidDataException($"unknown control type \"{kStr}\" json control object must contain type property with valid key string value that maps to a supported control type");
        }
        return o.ToObject(ct.Control, serializer);
    }
    
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

public class ControlTypeConverter : JsonConverter
{
    public static readonly ControlTypeConverter Singleton = new();

    private ControlTypeConverter()
    {
    }
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ControlType);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var s = serializer.Deserialize<string>(reader);
        if (s == null || !Key.IsValid(s) || !ControlTypes.Types.TryGetValue(new (s), out var ct))
        {
            throw new InvalidDataException("json control type must be a valid key string value that maps to a supported control type");
        }
        return ct;
    }
    
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, ((ControlType)value.NotNull()).Key);
    }
}