﻿using Common.Server;
using Common.Shared;
using Oak.I18n;

namespace Oak.Db;

public class Auth : Pwd
{
    public string Id { get; set; }
    public string Email { get; set; }
    public DateTime LastSignedInOn { get; set; } = DateTimeExts.Zero();
    public DateTime LastSignInAttemptOn { get; set; } = DateTimeExts.Zero();
    public DateTime ActivatedOn { get; set; } = DateTimeExts.Zero();
    public string? NewEmail { get; set; }
    public DateTime VerifyEmailCodeCreatedOn { get; set; } = DateTime.UtcNow;
    public string VerifyEmailCode { get; set; } = "";
    public DateTime ResetPwdCodeCreatedOn { get; set; } = DateTimeExts.Zero();
    public string ResetPwdCode { get; set; } = "";
    public DateTime LoginCodeCreatedOn { get; set; } = DateTimeExts.Zero();
    public string LoginCode { get; set; } = "";
    public bool Use2FA { get; set; } = false;
    public string Lang { get; set; } = S.DefaultLang;
    public string DateFmt { get; set; } = S.DefaultDateFmt;
    public string TimeFmt { get; set; } = S.DefaultTimeFmt;
}
