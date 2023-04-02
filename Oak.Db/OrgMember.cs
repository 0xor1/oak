using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oak.Db;

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
