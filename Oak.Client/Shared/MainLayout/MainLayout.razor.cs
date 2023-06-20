using System.Text.RegularExpressions;

namespace Oak.Client.Shared.MainLayout;

public partial class MainLayout
{
    [GeneratedRegex(@"/org/([^/\s]+)")]
    private static partial Regex OrgIdRx();

    [GeneratedRegex(@"/project/([^/\s]+)")]
    private static partial Regex ProjectIdRx();

    [GeneratedRegex(@"/task/([^/\s]+)")]
    private static partial Regex TaskIdRx();
}
