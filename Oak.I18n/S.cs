﻿using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    public const string EN = "en";
    public const string ES = "es";
    public const string FR = "fr";
    public const string DE = "de";
    public const string IT = "it";
    public const string DefaultLang = EN;
    public const string DefaultDateFmt = "yyyy-MM-dd";
    public const string DefaultTimeFmt = "HH:mm";

    public static readonly IReadOnlyList<Lang> SupportedLangs = new List<Lang>()
    {
        new(EN, "English"),
        new(ES, "Español"),
        new(FR, "Français"),
        new(DE, "Deutsch"),
        new(IT, "Italiano")
    };

    public static readonly IReadOnlyList<DateTimeFmt> SupportedDateFmts = new List<DateTimeFmt>()
    {
        new(DefaultDateFmt),
        new("dd-MM-yyyy"),
        new("MM-dd-yyyy")
    };

    public static readonly IReadOnlyList<DateTimeFmt> SupportedTimeFmts = new List<DateTimeFmt>()
    {
        new(DefaultTimeFmt),
        new("h:mmtt")
    };

    public static readonly Common.Shared.S Inst;

    static S()
    {
        Inst = new Strings(
            DefaultLang,
            DefaultDateFmt,
            DefaultTimeFmt,
            SupportedLangs,
            SupportedDateFmts,
            SupportedTimeFmts,
            Library
        );
    }
}
