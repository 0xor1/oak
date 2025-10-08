using System.Net;
using Common.Shared;
using Newtonsoft.Json;

namespace Oak.Cli;

[Serializable]
public class State
{
    [JsonProperty]
    internal string BaseHref { get; set; } = "https://oak.dans-demos.com/";

    [JsonProperty]
    internal CookieCollection Cookies { get; set; } = new();

    [JsonProperty]
    internal CookieContainer CookieContainer { get; set; } = new();

    [JsonProperty]
    internal Dictionary<string, string> Values { get; set; } = new();

    public string? GetString(string key) => Values.GetValueOrDefault(key, null);

    public void SetString(string key, string? value)
    {
        if (value == null)
        {
            Values.Remove(key);
        }
        else
        {
            Values[key] = value;
        }
    }

    public int? GetInt(string key)
    {
        var val = Values.GetValueOrDefault(key, null);
        if (!val.IsNullOrEmpty() && int.TryParse(val, out int result))
        {
            return result;
        }

        return null;
    }

    public void SetInt(string key, int? value) =>
        SetString(key, value == null ? null : value.ToString());

    public bool? GetBool(string key)
    {
        var val = Values.GetValueOrDefault(key, null);
        if (!val.IsNullOrEmpty() && bool.TryParse(val, out bool result))
        {
            return result;
        }

        return null;
    }

    public void SetBool(string key, bool? value) =>
        SetString(key, value == null ? null : value.ToString());

    public float? GetFloat(string key)
    {
        var val = Values.GetValueOrDefault(key, null);
        if (!val.IsNullOrEmpty() && float.TryParse(val, out float result))
        {
            return result;
        }

        return null;
    }

    public void SetFloat(string key, float? value) =>
        SetString(key, value == null ? null : value.ToString());

    public double? GetDouble(string key)
    {
        var val = Values.GetValueOrDefault(key, null);
        if (!val.IsNullOrEmpty() && double.TryParse(val, out double result))
        {
            return result;
        }

        return null;
    }

    public void SetDouble(string key, double? value) =>
        SetString(key, value == null ? null : value.ToString());

    public decimal? GetDecimal(string key)
    {
        var val = Values.GetValueOrDefault(key, null);
        if (!val.IsNullOrEmpty() && decimal.TryParse(val, out decimal result))
        {
            return result;
        }

        return null;
    }

    public void SetDecimal(string key, decimal? value) =>
        SetString(key, value == null ? null : value.ToString());

    public DateTime? GetDateTime(string key)
    {
        var val = Values.GetValueOrDefault(key, null);
        if (!val.IsNullOrEmpty() && DateTime.TryParse(val, out DateTime result))
        {
            return result;
        }

        return null;
    }

    public void SetDateTime(string key, DateTime? value) =>
        SetString(key, value == null ? null : value.ToString());
}
