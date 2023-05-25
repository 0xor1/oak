using System.Text.RegularExpressions;

namespace Oak.Client.Shared.MainLayout;

public partial class MainLayout
{
    [GeneratedRegex(@"/org/([^/\s]+)")]
    private static partial Regex OrgId();

    [GeneratedRegex(@"/project/([^/\s]+)")]
    private static partial Regex ProjectId();

    [GeneratedRegex(@"/task/([^/\s]+)")]
    private static partial Regex TaskId();
}
