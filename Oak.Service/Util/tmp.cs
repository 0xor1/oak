// using System.ComponentModel;
// using System.Globalization;
// using Common;
// using Humanizer;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Converters;
// using Newtonsoft.Json.Linq;
//
// namespace Dnsk.JFlow;
//
// public record Form
// {
//     public Key Key { get; init; }
//     public IReadOnlyList<Control> Controls { get; init; }
//
// }
//
// public record ControlType(Key Key, Type Control);
//
// public static class ControlTypes
// {
//     public static readonly ControlType Static = new (new("static"), typeof(StaticControl));
//     public static readonly ControlType Bool = new (new ("bool"), typeof(BoolControl));
//     public static readonly ControlType Int = new (new ("int"), typeof(IntControl));
//     public static readonly ControlType String = new (new ("string"), typeof(StringControl));
//
//     public static readonly IReadOnlyDictionary<Key, ControlType> Types = new List<ControlType>()
//     {
//         Static,
//         Bool,
//         Int,
//         String
//     }.ToDictionary(x => x.Key);
// }
//
// public record Control
// {
//     public Key Key { get; init; }
//
//     public ControlType Type { get; init; }
//
//     public If<Key>? Next { get; init; }
// }
// public record StaticControl : Control
// {
//     public bool Markdown { get; init; } = false;
// }
//
// public record InputControl : Control
// {
//     public If<bool> Required { get; init; }
// }
//
//
// [JsonConverter(typeof(StringEnumConverter))]
// public enum BoolType
// {
//     YesNo,
//     Checkbox
// }
//
// public record BoolControl : InputControl
// {
//     public BoolType SubType { get; init; } = BoolType.Checkbox;
// }
//
// public record IntControl : InputControl
// {
//     public BoolType SubType { get; init; } = BoolType.Checkbox;
// }
//
// public record StringControl : InputControl
// {
//     public BoolType SubType { get; init; } = BoolType.Checkbox;
// }
//
// public record If<T>
// {
//     public IReadOnlyList<IReadOnlyList<Cond>> Conditions { get; init; }
//     public T Then { get; init; }
//     public T? Else { get; init; }
// }
//
// public record Cond
// {
//     
// }
//
// public class ControlConverter : JsonConverter
// {
//     public static readonly ControlConverter Singleton = new();
//
//     private ControlConverter()
//     {
//     }
//     
//     public override bool CanConvert(Type objectType)
//     {
//         return objectType == typeof(Control);
//     }
//
//     public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
//     {
//         var o = serializer.Deserialize<JObject>(reader);
//         if (o == null)
//         {
//             throw new InvalidDataException("json control object can not be null");
//         }
//         if (!o.TryGetValue("type", out var t))
//         {
//             throw new InvalidDataException("json control object missing type propertry");
//         }
//         var kStr = t.Value<string>();
//         if (kStr == null || !Key.IsValid(kStr) || !ControlTypes.Types.TryGetValue(new (kStr), out var ct))
//         {
//             throw new InvalidDataException("json control object must contain type property with valid key string value that maps to a supported control type");
//         }
//         return o.ToObject(ct.Control, serializer);
//     }
//     
//     public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
//     {
//         serializer.Serialize(writer, value);
//     }
// }
// public class ControlTypeConverter : JsonConverter
// {
//     public static readonly ControlTypeConverter Singleton = new();
//
//     private ControlTypeConverter()
//     {
//     }
//     
//     public override bool CanConvert(Type objectType)
//     {
//         return objectType == typeof(ControlType);
//     }
//
//     public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
//     {
//         var s = serializer.Deserialize<string>(reader);
//         if (s == null || !Key.IsValid(s) || !ControlTypes.Types.TryGetValue(new (s), out var ct))
//         {
//             throw new InvalidDataException("json control type must be a valid key string value that maps to a supported control type");
//         }
//         return ct;
//     }
//     
//     public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
//     {
//         Throw.DataIf(value == null || value is not ControlType, "serializing control type got unexpected type");
//         serializer.Serialize(writer, ((ControlType)value.NotNull()).Key);
//     }
// }
// public class IfConverter : JsonConverter
// {
//     public static readonly IfConverter Singleton = new();
//
//     private IfConverter()
//     {
//     }
//     
//     public override bool CanConvert(Type objectType)
//     {
//         return objectType == typeof(If<>);
//     }
//
//     public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
//     {
//         var t = serializer.Deserialize<JToken>(reader);
//         if (t == null)
//         {
//             return null;
//         }
//
//         var genType = objectType.GenericTypeArguments.First();
//         if (genType == typeof(Key))
//         {
//             
//         }
//         else if (genType == typeof(bool))
//         {
//             
//         }
//         else if (genType == typeof(IReadOnlyList<Key>) )
//         {
//             
//         }
//         return null;
//     }
//     
//     public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
//     {
//         Throw.DataIf(value == null || value is not ControlType, "serializing control type got unexpected type");
//         serializer.Serialize(writer, ((ControlType)value.NotNull()).Key);
//     }
// }
//
// public record ContentItem(string Main, string? Invalid = null);
//
// public class ContentItemConverter : JsonConverter
// {
//     public static readonly ContentItemConverter Singleton = new();
//
//     private ContentItemConverter()
//     {
//     }
//
//     public override bool CanConvert(Type objectType) => objectType == typeof(ControlStaticContent);
//
//     public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
//     {
//         var t = serializer.Deserialize<JToken>(reader);
//         if (t == null)
//         {
//             return null;
//         }
//
//         switch (t.Type)
//         {
//             case JTokenType.Object:
//                 return serializer.Deserialize<ContentItem>(reader);
//             case JTokenType.String:
//                 return new ContentItem(serializer.Deserialize<string>(reader).NotNull());
//             default:
//                 throw new InvalidDataException($"invalid json token type {t.Type.Humanize()} for ContentItem, it should be either a string or an object {{\"main\": \"<control_label_text>\", \"invalid\": \"<control_invalid_text>\"}}");
//         }
//     }
//     
//     public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
//     {
//         var v = (ContentItem)value.NotNull();
//         if (v.Invalid == null)
//         {
//             // if there's no invalid value just serialize the main value to a simple string 
//             serializer.Serialize(writer, ((ContentItem)value.NotNull()).Main);
//         }
//         else
//         {
//             serializer.Serialize(writer, value);
//         }
//     }
// }