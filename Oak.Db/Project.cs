﻿using Microsoft.EntityFrameworkCore;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Id))]
public class Project
{
    public string Org { get; set; }
    public string Id { get; set; }
    public bool IsArchived { get; set; }
    public bool IsPublic { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedOn { get; set; }
    public string CurrencySymbol { get; set; } = "$";
    public string CurrencyCode { get; set; } = "USD";
    public uint HoursPerDay { get; set; } = 8;
    public uint DaysPerWeek { get; set; } = 5;
    public DateTime? StartOn { get; set; }
    public DateTime? EndOn { get; set; }
    public ulong FileLimit { get; set; }

    public Api.Project.Project ToApi(Task t) =>
        new(
            Org,
            Id,
            IsArchived,
            IsPublic,
            Name,
            CreatedOn,
            CurrencySymbol,
            CurrencyCode,
            HoursPerDay,
            DaysPerWeek,
            StartOn,
            EndOn,
            FileLimit,
            t.ToApi()
        );
}
