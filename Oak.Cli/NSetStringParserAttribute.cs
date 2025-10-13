using Common.Shared;
using ConsoleAppFramework;

namespace Oak.Cli;

[AttributeUsage(AttributeTargets.Parameter)]
public class NSetStringParserAttribute : Attribute, IArgumentParser<NSet<string>>
{
    public static bool TryParse(ReadOnlySpan<char> v, out NSet<string> result)
    {
        var s = v.ToString();
        if (s.IsNullOrWhiteSpace() || s.ToLower() is "null" or "nil")
        {
            result = new NSet<string>(null);
        }
        else
        {
            result = new NSet<string>(s);
        }
        return true;
    }
}
