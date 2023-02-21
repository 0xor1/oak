using System.Runtime.Serialization;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oak.Flow;

public record Conditional<T>
{
    public IReadOnlyList<IReadOnlyList<Condition>>? If { get; init; }
    public T Then { get; init; }
}

public record Condition
{
    public string This { get; init; }
    public Is Is { get; init; }
    public object? To { get; init; }
    public string? ToRef { get; init; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum Is
{
    [EnumMember(Value = "eq")]
    EQ,
    [EnumMember(Value = "neq")]
    NEQ,
    [EnumMember(Value = "gt")]
    GT,
    [EnumMember(Value = "gte")]
    GTE,
    [EnumMember(Value = "lt")]
    LT,
    [EnumMember(Value = "lte")]
    LTE,
    [EnumMember(Value = "contains")]
    Contains,
    [EnumMember(Value = "excludes")]
    Excludes
}