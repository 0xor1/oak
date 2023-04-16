using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Member))]
public class OrgMember
{
    public string Org { get; set; }
    public string Member { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public OrgMemberRole Role { get; set; }
}

public enum OrgMemberRole
{
    Owner,
    Admin,
    WriteAllProjects,
    ReadAllProjects,
    PerProject
}
