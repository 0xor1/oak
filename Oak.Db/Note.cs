﻿using Microsoft.EntityFrameworkCore;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project), nameof(Task), nameof(CreatedOn), nameof(CreatedBy))]
public class Note
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Task { get; set; }
    public string Id { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string Body { get; set; }
}
