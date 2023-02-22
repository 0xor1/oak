using System.Runtime.Serialization;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oak.Flow.Controls;

public record BoolControl : InputControl
{
    public BoolType DisplayAs { get; init; } = BoolType.Checkbox;
}

[JsonConverter(typeof(StringEnumConverter))]
public enum BoolType
{
    [EnumMember(Value = "yes_no")]
    YesNo,

    [EnumMember(Value = "checkbox")]
    Checkbox
}

public record IntControl : InputControl
{
    public BoolType DisplayAs { get; init; } = BoolType.Checkbox;
}

public record StringControl : InputControl
{
    public BoolType DisplayAs { get; init; } = BoolType.Checkbox;
}
