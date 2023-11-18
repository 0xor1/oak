using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, Dictionary<string, TemplatableString>> Library =
        new()
        {
            { Common.Shared.I18n.S.EN, EN_Strings },
            { Common.Shared.I18n.S.ES, ES_Strings },
            { Common.Shared.I18n.S.FR, FR_Strings },
            { Common.Shared.I18n.S.DE, DE_Strings },
            { Common.Shared.I18n.S.IT, IT_Strings }
        };
}
