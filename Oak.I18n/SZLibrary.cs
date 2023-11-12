using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, Dictionary<string, TemplatableString>> Library =
        new()
        {
            { Common.Shared.I18n.S.EN, English },
            { Common.Shared.I18n.S.ES, Spanish },
            { Common.Shared.I18n.S.FR, French },
            { Common.Shared.I18n.S.DE, German },
            { Common.Shared.I18n.S.IT, Italian }
        };
}
