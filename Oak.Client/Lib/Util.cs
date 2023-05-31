using Common.Shared;

namespace Oak.Client.Lib;

// TODO i18n/l10n this content
public static class Util
{
    public static string Time(ulong val, uint hoursPerDay, uint daysPerWeek)
    {
        Throw.OpIf(hoursPerDay == 0 || daysPerWeek == 0, "hoursPerDay and daysPerWeek must be > 0");
        const ulong minsPerHour = 60ul;
        const ulong weeksPerYear = 52ul;
        var minsPerDay = minsPerHour * hoursPerDay;
        var minsPerWeek = minsPerDay * daysPerWeek;
        var minsPerYear = minsPerWeek * weeksPerYear;

        if (val >= minsPerYear)
        {
            return $"{((decimal)val / minsPerYear).ToString("F1")}y";
        }
        if (val >= minsPerWeek)
        {
            return $"{((decimal)val / minsPerWeek).ToString("F1")}w";
        }
        if (val >= minsPerDay)
        {
            return $"{((decimal)val / minsPerWeek).ToString("F1")}d";
        }
        if (val >= minsPerHour)
        {
            return $"{((decimal)val / minsPerHour).ToString("F1")}h";
        }
        return $"{val}m";
    }

    public static string Cost(ulong val, string symbol)
    {
        const ulong t = 1000000000000ul;
        const ulong b = 1000000000ul;
        const ulong m = 1000000ul;
        const ulong k = 1000ul;
        if (val >= t)
        {
            return $"{symbol}{((decimal)val / t).ToString("F")}T";
        }
        if (val >= b)
        {
            return $"{symbol}{((decimal)val / b).ToString("F")}B";
        }
        if (val >= m)
        {
            return $"{symbol}{((decimal)val / m).ToString("F")}M";
        }
        if (val >= k)
        {
            return $"{symbol}{((decimal)val / k).ToString("F")}K";
        }
        return $"{symbol}{val}";
    }

    public static string Size(ulong val)
    {
        const ulong k = 1024ul;
        const ulong m = k * k;
        const ulong g = k * m;
        const ulong t = k * g;
        const ulong p = k * t;

        if (val >= p)
        {
            return $"{((decimal)val / p).ToString("F")}PB";
        }
        if (val >= t)
        {
            return $"{((decimal)val / t).ToString("F")}TB";
        }
        if (val >= g)
        {
            return $"{((decimal)val / g).ToString("F")}GB";
        }
        if (val >= m)
        {
            return $"{((decimal)val / m).ToString("F")}MB";
        }
        if (val >= k)
        {
            return $"{((decimal)val / m).ToString("F")}KB";
        }
        return $"{val}B";
    }
}
