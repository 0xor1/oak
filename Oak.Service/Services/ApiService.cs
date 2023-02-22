using Common;
using Oak.Db;
using Oak.Proto;
using Oak.Service.Util;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Oak.I18n;

namespace Oak.Service.Services;

public class ApiService : Api.ApiBase
{
    private readonly OakDb _db;
    private readonly IEmailClient _emailClient;

    public ApiService(OakDb db, IEmailClient emailClient)
    {
        _db = db;
        _emailClient = emailClient;
    }

    public override Task<Auth_Session> Auth_GetSession(Nothing _, ServerCallContext stx) =>
        AuthSession(stx.GetSession()).Task();

    public override async Task<Nothing> Auth_Register(Auth_RegisterReq req, ServerCallContext stx)
    {
        // basic validation
        var ses = stx.GetSession();
        stx.ErrorIf(ses.IsAuthed, S.AlreadyAuthenticated);
        // !!! ToLower all emails in all Auth_ api endpoints
        req.Email = req.Email.ToLower();
        stx.ErrorFromValidationResult(AuthValidator.Email(req.Email));
        stx.ErrorFromValidationResult(AuthValidator.Pwd(req.Pwd));

        // start db tx
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var existing = await _db.Auths.SingleOrDefaultAsync(
                x => x.Email.Equals(req.Email) || x.NewEmail.Equals(req.Email)
            );
            var newCreated = existing == null;
            if (existing == null)
            {
                var verifyEmailCode = Crypto.String(32);
                var pwd = Crypto.HashPwd(req.Pwd);
                existing = new Auth()
                {
                    Id = ses.Id,
                    Email = req.Email,
                    VerifyEmailCodeCreatedOn = DateTime.UtcNow,
                    VerifyEmailCode = verifyEmailCode,
                    Lang = ses.Lang,
                    DateFmt = ses.DateFmt,
                    TimeFmt = ses.TimeFmt,
                    PwdVersion = pwd.PwdVersion,
                    PwdSalt = pwd.PwdSalt,
                    PwdHash = pwd.PwdHash,
                    PwdIters = pwd.PwdIters
                };
                await _db.Auths.AddAsync(existing, stx.CancellationToken);
                await _db.SaveChangesAsync();
            }

            if (
                !existing.VerifyEmailCode.IsNullOrEmpty()
                && (
                    newCreated
                    || (
                        existing.VerifyEmailCodeCreatedOn.MinutesSince() > 10
                        && existing.ActivatedOn.IsZero()
                    )
                )
            )
            {
                // if there is a verify email code and
                // we've just registered a new account
                // or the verify email was sent over 10 mins ago
                // and the account is not yet activated
                var model = new
                {
                    BaseHref = Config.Server.Listen,
                    Email = existing.Email,
                    Code = existing.VerifyEmailCode
                };
                await _emailClient.SendEmailAsync(
                    stx.String(S.AuthConfirmEmailSubject),
                    stx.String(S.AuthConfirmEmailHtml, model),
                    stx.String(S.AuthConfirmEmailText, model),
                    Config.Email.NoReplyAddress,
                    new List<string>() { req.Email }
                );
            }
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return new Nothing();
    }

    public override async Task<Nothing> Auth_VerifyEmail(
        Auth_VerifyEmailReq req,
        ServerCallContext stx
    )
    {
        // !!! ToLower all emails in all Auth_ api endpoints
        req.Email = req.Email.ToLower();
        stx.ErrorFromValidationResult(AuthValidator.Email(req.Email));

        // start db tx
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var auth = await _db.Auths.SingleOrDefaultAsync(
                x => x.Email.Equals(req.Email) || x.NewEmail.Equals(req.Email)
            );
            stx.ErrorIf(auth == null, S.NoMatchingRecord);
            stx.ErrorIf(auth.NotNull().VerifyEmailCode != req.Code, S.InvalidEmailCode);
            if (!auth.NewEmail.IsNullOrEmpty() && auth.NewEmail == req.Email)
            {
                // verifying new email
                auth.Email = auth.NewEmail;
                auth.NewEmail = string.Empty;
            }
            else
            {
                // first account activation
                auth.ActivatedOn = DateTime.UtcNow;
            }

            auth.VerifyEmailCodeCreatedOn = DateTimeExts.Zero();
            auth.VerifyEmailCode = string.Empty;
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return new Nothing();
    }

    public override async Task<Nothing> Auth_SendResetPwdEmail(
        Auth_SendResetPwdEmailReq req,
        ServerCallContext stx
    )
    {
        // basic validation
        var ses = stx.GetSession();
        stx.ErrorIf(ses.IsAuthed, S.AlreadyAuthenticated);
        // !!! ToLower all emails in all Auth_ api endpoints
        req.Email = req.Email.ToLower();
        stx.ErrorFromValidationResult(AuthValidator.Email(req.Email));

        // start db tx
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var existing = await _db.Auths.SingleOrDefaultAsync(x => x.Email.Equals(req.Email));
            if (existing == null || existing.ResetPwdCodeCreatedOn.MinutesSince() < 10)
            {
                // if email is not associated with an account or
                // a reset pwd was sent within the last 10 minutes
                // dont do anything
                return new Nothing();
            }

            existing.ResetPwdCodeCreatedOn = DateTime.UtcNow;
            existing.ResetPwdCode = Crypto.String(32);
            await _db.SaveChangesAsync();
            var model = new
            {
                BaseHref = Config.Server.Listen,
                Email = existing.Email,
                Code = existing.ResetPwdCode
            };
            await _emailClient.SendEmailAsync(
                stx.String(S.AuthResetPwdSubject),
                stx.String(S.AuthResetPwdHtml, model),
                stx.String(S.AuthResetPwdText, model),
                Config.Email.NoReplyAddress,
                new List<string>() { req.Email }
            );
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return new Nothing();
    }

    public override async Task<Nothing> Auth_ResetPwd(Auth_ResetPwdReq req, ServerCallContext stx)
    {
        // !!! ToLower all emails in all Auth_ api endpoints
        req.Email = req.Email.ToLower();
        stx.ErrorFromValidationResult(AuthValidator.Email(req.Email));
        stx.ErrorFromValidationResult(AuthValidator.Pwd(req.NewPwd));

        // start db tx
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var auth = await _db.Auths.SingleOrDefaultAsync(x => x.Email.Equals(req.Email));
            stx.ErrorIf(auth == null, S.NoMatchingRecord);
            stx.ErrorIf(auth.NotNull().ResetPwdCode != req.Code, S.InvalidResetPwdCode);
            var pwd = Crypto.HashPwd(req.NewPwd);
            auth.ResetPwdCodeCreatedOn = DateTimeExts.Zero();
            auth.ResetPwdCode = string.Empty;
            auth.PwdVersion = pwd.PwdVersion;
            auth.PwdSalt = pwd.PwdSalt;
            auth.PwdHash = pwd.PwdHash;
            auth.PwdIters = pwd.PwdIters;
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return new Nothing();
    }

    public override async Task<Auth_Session> Auth_SignIn(Auth_SignInReq req, ServerCallContext stx)
    {
        // basic validation
        var ses = stx.GetSession();
        stx.ErrorIf(ses.IsAuthed, S.AlreadyAuthenticated);
        // !!! ToLower all emails in all Auth_ api endpoints
        req.Email = req.Email.ToLower();
        stx.ErrorFromValidationResult(AuthValidator.Email(req.Email));

        // start db tx
        await using var tx = await _db.Database.BeginTransactionAsync();
        var auth = await _db.Auths.SingleOrDefaultAsync(x => x.Email.Equals(req.Email));
        stx.ErrorIf(auth == null, S.NoMatchingRecord);
        stx.ErrorIf(auth.NotNull().ActivatedOn.IsZero(), S.AccountNotVerified);
        RateLimitAuthAttempts(stx, auth.NotNull());
        auth.LastSignInAttemptOn = DateTime.UtcNow;
        var pwdIsValid = Crypto.PwdIsValid(req.Pwd, auth);
        if (pwdIsValid)
        {
            auth.LastSignedInOn = DateTime.UtcNow;
            ses = stx.CreateSession(
                auth.Id,
                true,
                req.RememberMe,
                auth.Lang,
                auth.DateFmt,
                auth.TimeFmt
            );
        }
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        stx.ErrorIf(!pwdIsValid, S.NoMatchingRecord);
        return AuthSession(ses);
    }

    public override Task<Auth_Session> Auth_SignOut(Nothing _, ServerCallContext stx)
    {
        // basic validation
        var ses = stx.GetSession();
        if (ses.IsAuthed)
        {
            ses = stx.ClearSession();
        }
        return AuthSession(ses).Task();
    }

    public override async Task<Auth_Session> Auth_SetL10n(
        Auth_SetL10nReq req,
        ServerCallContext stx
    )
    {
        var ses = stx.GetSession();
        if (
            (req.Lang.IsNullOrWhiteSpace() || ses.Lang == req.Lang)
            && (req.DateFmt.IsNullOrWhiteSpace() || ses.DateFmt == req.DateFmt)
            && (req.TimeFmt.IsNullOrWhiteSpace() || ses.TimeFmt == req.TimeFmt)
        )
        {
            return AuthSession(ses);
        }
        ses = stx.CreateSession(
            ses.Id,
            ses.IsAuthed,
            ses.RememberMe,
            S.BestLang(req.Lang ?? ses.Lang),
            req.DateFmt ?? ses.DateFmt,
            req.TimeFmt ?? ses.TimeFmt
        );
        if (ses.IsAuthed)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            var auth = await _db.Auths.SingleOrDefaultAsync(x => x.Id.Equals(ses.Id));
            stx.ErrorIf(auth == null, S.NoMatchingRecord);
            stx.ErrorIf(auth.NotNull().ActivatedOn.IsZero(), S.AccountNotVerified);
            auth.Lang = ses.Lang;
            auth.DateFmt = ses.DateFmt;
            auth.TimeFmt = ses.TimeFmt;
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        return AuthSession(ses);
    }

    private const int AuthAttemptsRateLimit = 5;

    private static void RateLimitAuthAttempts(ServerCallContext stx, Auth auth)
    {
        stx.ErrorIf(
            auth.LastSignInAttemptOn.SecondsSince() < AuthAttemptsRateLimit,
            S.AuthAttemptRateLimit
        );
    }

    private static Auth_Session AuthSession(Session s)
    {
        return new Auth_Session()
        {
            Id = s.Id,
            IsAuthed = s.IsAuthed,
            Lang = s.Lang,
            DateFmt = s.DateFmt,
            TimeFmt = s.TimeFmt
        };
    }
}
