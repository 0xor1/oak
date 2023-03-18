using System.Runtime.Serialization;
using Common.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oak.Flow.Controls;

namespace Oak.Flow;

public record Control
{
    public Key Key { get; init; }
    public ControlType Type { get; init; }
    public IReadOnlyList<Conditional<Key>>? Next { get; init; }
}

public abstract record InputControl : Control
{
    public IReadOnlyList<Conditional<bool>>? Required { get; init; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum ControlType
{
    [EnumMember(Value = "static")]
    Static,

    [EnumMember(Value = "bool")]
    Bool,

    [EnumMember(Value = "int")]
    Int,

    [EnumMember(Value = "decimal")]
    Decimal,

    [EnumMember(Value = "string")]
    String,

    [EnumMember(Value = "select")]
    Select,

    [EnumMember(Value = "search")]
    Search
}

public static class ControlTypesMap
{
    public static readonly IReadOnlyDictionary<ControlType, Type> Types = new Dictionary<
        ControlType,
        Type
    >()
    {
        { ControlType.Static, typeof(Control) },
        { ControlType.Bool, typeof(BoolControl) },
        { ControlType.Int, typeof(IntControl) },
        { ControlType.Decimal, typeof(IntControl) },
        { ControlType.String, typeof(IntControl) },
        { ControlType.Select, typeof(IntControl) }
    };
}

public class ControlConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Control);
    }

    public override object? ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer
    )
    {
        var o = serializer.Deserialize<JObject>(reader);

        if (o == null)
        {
            throw new InvalidDataException("json control object can not be null");
        }

        if (!o.TryGetValue("type", out var t))
        {
            throw new InvalidDataException("json control object missing type property");
        }

        var enumType = Enum.Parse<ControlType>(
            t.Value<string>() ?? throw new InvalidDataException("control type property missing"),
            true
        );
        var type = ControlTypesMap.Types[enumType];

        return o.ToObject(type, serializer);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
