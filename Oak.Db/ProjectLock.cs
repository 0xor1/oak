using Common.Server;
using Common.Shared;
using Oak.I18n;

namespace Oak.Db;

public class ProjectLock
{
    public string Org { get; set; }
    public string Project { get; set; }
}
