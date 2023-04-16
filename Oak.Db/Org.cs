using Common.Server;
using Common.Shared;
using Microsoft.EntityFrameworkCore;
using Oak.I18n;

namespace Oak.Db;

[PrimaryKey(nameof(Id))]
public class Org
{
    public string Id { get; set; }
    public string Name { get; set; }
}
