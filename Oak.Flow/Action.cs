using System.Runtime.Serialization;
using Common.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Oak.Flow;

public record Action
{
    public Key Key { get; init; }
    public ActionType Type { get; init; }
    public IReadOnlyList<Conditional<Key>>? Next { get; init; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum ActionType
{
    [EnumMember(Value = "form")]
    Form,

    [EnumMember(Value = "auto")]
    Auto
}

internal static class ActionTypesMap
{
    public static readonly IReadOnlyDictionary<ActionType, Type> Types = new Dictionary<
        ActionType,
        Type
    >()
    {
        { ActionType.Form, typeof(Form) },
        { ActionType.Auto, typeof(Auto) }
    };
}

public class ActionConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Action);
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
            throw new InvalidDataException("json action object can not be null");
        }

        if (!o.TryGetValue("type", out var t))
        {
            throw new InvalidDataException("json action object missing type property");
        }

        var enumType = Enum.Parse<ActionType>(
            t.Value<string>() ?? throw new InvalidDataException("action type property missing"),
            true
        );
        var type = ActionTypesMap.Types[enumType];

        return o.ToObject(type, serializer);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
