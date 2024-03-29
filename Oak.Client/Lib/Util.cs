﻿using Common.Shared;
using Oak.Api.Project;
using Oak.Client.Shared.Components;

namespace Oak.Client.Lib;

// TODO i18n/l10n this content
public static class Util
{
    public const ulong MinsPerHour = 60ul;
    public const ulong WeeksPerYear = 52ul;

    public const ulong T = 1000000000000ul;
    public const ulong B = 1000000000ul;
    public const ulong M = 1000000ul;
    public const ulong K = 1000ul;

    public const ulong KB = 1024ul;
    public const ulong MB = KB * KB;
    public const ulong GB = KB * MB;
    public const ulong TB = KB * GB;
    public const ulong PB = KB * TB;

    public static string TimerDuration(ulong val)
    {
        var hrs = val / 3600;
        val -= hrs * 3600;
        var mins = val / 60;
        val -= mins * 60;
        return $"{hrs:D2}:{mins:D2}:{val:D2}";
    }

    public static string Duration(Project? p, ulong val)
    {
        if (p == null || p.HoursPerDay == 0 || p.DaysPerWeek == 0)
        {
            return $"{val}m";
        }
        Throw.OpIf(
            p.HoursPerDay == 0 || p.DaysPerWeek == 0,
            "hoursPerDay and daysPerWeek must be > 0"
        );

        var minsPerDay = MinsPerHour * p.HoursPerDay;
        var minsPerWeek = minsPerDay * p.DaysPerWeek;
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

    public static string Cost(Project? p, ulong val)
    {
        if (p == null || p.CurrencyCode.IsNullOrEmpty())
        {
            return $"{p?.CurrencySymbol}{val}";
        }

        var divs =
            CurrencyPicker.Currencies.SingleOrDefault(x => x.Code == p.CurrencyCode)?.Divisions
            ?? 100;
        var dVal = (decimal)val / divs;
        if (dVal >= T)
        {
            return $"{p.CurrencySymbol}{(dVal / T).ToString("N1")}T";
        }
        if (dVal >= B)
        {
            return $"{p.CurrencySymbol}{(dVal / B).ToString("N1")}B";
        }
        if (dVal >= M)
        {
            return $"{p.CurrencySymbol}{(dVal / M).ToString("N1")}M";
        }
        if (dVal >= K)
        {
            return $"{p.CurrencySymbol}{(dVal / K).ToString("N1")}K";
        }
        return $"{p.CurrencySymbol}{dVal.ToString(divs == 100 ? "N2" : "N0")}";
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
            return $"{((decimal)val / KB).ToString("F")}KB";
        }
        return $"{val}B";
    }
}
