﻿using Microsoft.EntityFrameworkCore;
using ApiTimer = Oak.Api.Timer.Timer;

namespace Oak.Db;

[PrimaryKey(nameof(Org), nameof(Project), nameof(Task), nameof(User))]
public class Timer
{
    public string Org { get; set; }
    public string Project { get; set; }
    public string Task { get; set; }
    public string User { get; set; }
    public ulong Inc { get; set; }
    public DateTime LastStartedOn { get; set; }
    public bool IsRunning { get; set; }

    public ApiTimer ToApi(string taskName) =>
        new(Org, Project, Task, User, taskName, Inc, LastStartedOn, IsRunning);
}
