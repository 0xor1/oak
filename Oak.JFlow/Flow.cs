using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Oak.JFlow;

public record Flow
{
    public Branding Branding { get; init; }
    public IReadOnlyDictionary<string, IReadOnlyDictionary<Key, string>> Strings { get; init; }
    public IReadOnlyList<Set> Sets { get; init; }
    public IReadOnlyList<Form> Forms { get; init; }
    public IReadOnlyList<Auto> Autos { get; init; }
}

public record Branding
{
    public IReadOnlyDictionary<Key, string> Fonts { get; init; }
}

public record Set
{
    public Key Key { get; init; }
    public IReadOnlyList<Key> Options { get; init; }
}

public record Form
{
    public Key Key { get; init; }
    public IReadOnlyList<Control> Controls { get; init; }
}

public record Auto
{
    public Key Key { get; init; }
    public IReadOnlyList<Control> Controls { get; init; }
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


[JsonConverter(typeof(ControlConverter))]
public record Control
{
    public Key Key { get; init; }

    public ControlType Type { get; init; }

    public If<Key>? Next { get; init; }
}

public abstract record InputControl : Control
{
    public If<bool> Required { get; init; }
}


public record BoolType(Key Key);
public static class BoolTypes
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
public record BoolControl : InputControl
{
    public BoolType SubType { get; init; } = BoolType.Checkbox;
}

public record IntControl : InputControl
{
    public BoolType SubType { get; init; } = BoolType.Checkbox;
}

public record StringControl : InputControl
{
    public BoolType SubType { get; init; } = BoolType.Checkbox;
}

public record If<T>
{
    public IReadOnlyList<IReadOnlyList<Cond>> Conditions { get; init; }
    public T Then { get; init; }
    public T? Else { get; init; }
}

public record Cond
{
    
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
        Throw.DataIf(value == null || value is not ControlType, "serializing control type got unexpected type");
        serializer.Serialize(writer, ((ControlType)value.NotNull()).Key);
    }
}
public class IfConverter : JsonConverter
{
    public static readonly IfConverter Singleton = new();

    private IfConverter()
    {
    }
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(If<>);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var t = serializer.Deserialize<JToken>(reader);
        if (t == null)
        {
            return null;
        }

        var genType = objectType.GenericTypeArguments.First();
        if (genType == typeof(Key))
        {
            
        }
        else if (genType == typeof(bool))
        {
            
        }
        else if (genType == typeof(IReadOnlyList<Key>) )
        {
            
        }
        return null;
    }
    
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        Throw.DataIf(value == null || value is not ControlType, "serializing control type got unexpected type");
        serializer.Serialize(writer, ((ControlType)value.NotNull()).Key);
    }
}