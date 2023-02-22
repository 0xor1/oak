using Oak.I18n;

namespace Oak.Client.Lib;

// L for Localizer
public static class L
{
    private static string _lang = I18n.S.DefaultLang;
    private static string _date = I18n.S.DefaultDateFmt;
    private static string _time = I18n.S.DefaultTimeFmt;

    public static void Config(string lang, string date, string time)
    {
        _lang = lang;
        _date = date;
        _time = time;
    }

    // S for String
    public static string S(string key, object? model = null) =>
        I18n.S.GetOrAddress(_lang, key, model);

    // D for Date
    public static string D(DateTime dt) => dt.ToLocalTime().ToString(_date);

    // T for Time
    public static string T(DateTime dt) => dt.ToLocalTime().ToString(_time);
}
