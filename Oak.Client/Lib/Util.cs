using Common.Shared;
using Oak.Api.Project;
using Oak.Client.Shared.Components;

namespace Oak.Client.Lib;

// TODO i18n/l10n this content
public static class Util
{
    const ulong MinsPerHour = 60ul;
    const ulong WeeksPerYear = 52ul;

    const ulong T = 1000000000000ul;
    const ulong B = 1000000000ul;
    const ulong M = 1000000ul;
    const ulong K = 1000ul;
    const ulong KB = 1024ul;

    const ulong MB = KB * KB;
    const ulong GB = KB * MB;
    const ulong TB = KB * GB;
    const ulong PB = KB * TB;

    public static string Time(ulong val, uint hoursPerDay, uint daysPerWeek)
    {
        Throw.OpIf(hoursPerDay == 0 || daysPerWeek == 0, "hoursPerDay and daysPerWeek must be > 0");

        var minsPerDay = MinsPerHour * hoursPerDay;
        var minsPerWeek = minsPerDay * daysPerWeek;
        var minsPerYear = minsPerWeek * WeeksPerYear;

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
            return $"{((decimal)val / minsPerDay).ToString("F1")}d";
        }
        if (val >= MinsPerHour)
        {
            return $"{((decimal)val / MinsPerHour).ToString("F1")}h";
        }
        return $"{val}m";
    }

    public static string Cost(Project p, ulong val, string symbol)
    {
        var divs =
            CurrencyPicker.Currencies.SingleOrDefault(x => x.Code == p.CurrencyCode)?.Divisions
            ?? 100;
        var dVal = (decimal)val / divs;
        if (dVal >= T)
        {
            return $"{symbol}{(dVal / T).ToString("N1")}T";
        }
        if (dVal >= B)
        {
            return $"{symbol}{(dVal / B).ToString("N1")}B";
        }
        if (dVal >= M)
        {
            return $"{symbol}{(dVal / M).ToString("N1")}M";
        }
        if (dVal >= K)
        {
            return $"{symbol}{(dVal / K).ToString("N1")}K";
        }
        return $"{symbol}{dVal.ToString(divs == 100 ? "N2" : "N0")}";
    }

    public static string Size(ulong val)
    {
        if (val >= PB)
        {
            return $"{((decimal)val / PB).ToString("F")}PB";
        }
        if (val >= TB)
        {
            return $"{((decimal)val / TB).ToString("F")}TB";
        }
        if (val >= GB)
        {
            return $"{((decimal)val / GB).ToString("F")}GB";
        }
        if (val >= MB)
        {
            return $"{((decimal)val / MB).ToString("F")}MB";
        }
        if (val >= KB)
        {
            return $"{((decimal)val / MB).ToString("F")}KB";
        }
        return $"{val}B";
    }
}
