using Common;
using Oak.Db;
using Oak.Proto;
using Oak.Service.Util;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Oak.Service.Services;

public class ApiService : Api.ApiBase
{
    private readonly OakDb _db;
    private readonly ISessionManager _session;
    private readonly IEmailClient _emailClient;
    
    public ApiService(OakDb db, ISessionManager session, IEmailClient emailClient)
    {
        _db = db;
        _session = session;
        _emailClient = emailClient;
    }

    public override Task<Auth_Session> Auth_GetSession(Nothing _, ServerCallContext stx)
    {
        var ses = _session.Get(stx);
        return new Auth_Session()
        {
            Id = ses.Id,
            IsAuthed = ses.IsAuthed
        }.Task();
    }

    public override async Task<Nothing> Auth_Register(Auth_RegisterReq req, ServerCallContext stx)
    {
        // basic validation
        var ses = _session.Get(stx); 
        Error.If(ses.IsAuthed, "already in authenticated session", @public: true, log: false);
        // !!! ToLower all emails in all Auth_ api endpoints
        req.Email = req.Email.ToLower();
        Error.FromValidationResult(AuthValidator.Email(req.Email));
        Error.FromValidationResult(AuthValidator.Pwd(req.Pwd));
        
        // start db tx
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var existing = await _db.Auths.SingleOrDefaultAsync(x => x.Email.Equals(req.Email) || x.NewEmail.Equals(req.Email));
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
                    PwdVersion = pwd.PwdVersion,
                    PwdSalt = pwd.PwdSalt,
                    PwdHash = pwd.PwdHash,
                    PwdIters = pwd.PwdIters
                };
                await _db.Auths.AddAsync(existing, stx.CancellationToken);
                await _db.SaveChangesAsync();
            }

            if (!existing.VerifyEmailCode.IsNullOrEmpty() && 
                (newCreated || 
                 (existing.VerifyEmailCodeCreatedOn.MinutesSince() > 10 && 
                  existing.ActivatedOn.IsZero())))
            {
                // if there is a verify email code and
                // we've just registered a new account
                // or the verify email was sent over 10 mins ago
                // and the account is not yet activated
                await _emailClient.SendEmailAsync(
                    "Confirm Email Address", 
                    $"<div><a href=\"https://localhost:9500/verify_email?email={req.Email}&code={existing.VerifyEmailCode}\">please click this link to verify your email address</a></div>", 
                    $"please use this link to verify your email address: https://localhost:9500/verify_email?email={req.Email}&code={existing.VerifyEmailCode}", 
                    "noreply@yolo.yolo", 
                    new List<string>(){req.Email});
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

    public override async Task<Nothing> Auth_VerifyEmail(Auth_VerifyEmailReq req, ServerCallContext stx)
    {
        // !!! ToLower all emails in all Auth_ api endpoints
        req.Email = req.Email.ToLower();
        Error.FromValidationResult(AuthValidator.Email(req.Email));
        
        // start db tx
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var auth = await _db.Auths.SingleOrDefaultAsync(x => x.Email.Equals(req.Email) || x.NewEmail.Equals(req.Email));
            Error.If(auth == null, "no matching record found", @public: true, log: false);
            Error.If(auth.NotNull().VerifyEmailCode != req.Code, "invalid email code", @public: true, log: false);
            if (!auth.NewEmail.IsNullOrEmpty() && auth.NewEmail == req.Email)
            {
                // verifying new email
                auth.Email = auth.NewEmail;
                auth.NewEmail = "";
            }
            else
            {
                // first account activation
                auth.ActivatedOn = DateTime.UtcNow;
            }

            auth.VerifyEmailCodeCreatedOn = DateTimeExts.Zero();
            auth.VerifyEmailCode = "";
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
        var ses = _session.Get(stx); 
        Error.If(ses.IsAuthed, "already in authenticated session", @public: true, log: false);
        // !!! ToLower all emails in all Auth_ api endpoints
        req.Email = req.Email.ToLower();
        Error.FromValidationResult(AuthValidator.Email(req.Email));
        
        // start db tx
        await using var tx = await _db.Database.BeginTransactionAsync();
        var auth = await _db.Auths.SingleOrDefaultAsync(x => x.Email.Equals(req.Email));
        Error.If(auth == null, "no matching record found", @public: true, log: false);
        Error.If(auth.NotNull().ActivatedOn.IsZero(), "account not verified, please check your emails for verification link", @public: true, log: false);
        RateLimitAuthAttempts(auth.NotNull());
        auth.LastSignInAttemptOn = DateTime.UtcNow;
        var pwdIsValid = Crypto.PwdIsValid(req.Pwd, auth);
        if (pwdIsValid)
        {
            auth.LastSignedInOn = DateTime.UtcNow;
            ses = _session.SignIn(stx, auth.Id, req.RememberMe);
        }
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        Error.If(!pwdIsValid, "no matching record found", @public: true, log: false);
        return new Auth_Session()
        {
            Id = ses.Id,
            IsAuthed = ses.IsAuthed
        };
    }

    public override Task<Auth_Session> Auth_SignOut(Nothing _, ServerCallContext stx)
    {
        // basic validation
        var ses = _session.Get(stx);
        if (ses.IsAuthed)
        {
            ses = _session.SignOut(stx);
        }
        return new Auth_Session()
        {
            Id = ses.Id,
            IsAuthed = ses.IsAuthed
        }.Task();
    }
    
    public override async Task<Nothing> Auth_SendResetPwdEmail(Auth_SendResetPwdEmailReq req, ServerCallContext stx)
    {
        // basic validation
        var ses = _session.Get(stx); 
        Error.If(ses.IsAuthed, "already in authenticated session", @public: true, log: false);
        // !!! ToLower all emails in all Auth_ api endpoints
        req.Email = req.Email.ToLower();
        Error.FromValidationResult(AuthValidator.Email(req.Email));
        
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
            await _emailClient.SendEmailAsync(
                "Reset Password", 
                $"<div><a href=\"https://localhost:9500/reset_pwd?email={req.Email}&code={existing.ResetPwdCode}\">please click this link to reset your password</a></div>", 
                $"please click this link to reset your password: https://localhost:9500/reset_pwd?email={req.Email}&code={existing.ResetPwdCode}", 
                "noreply@yolo.yolo", 
                new List<string>(){req.Email});
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
        Error.FromValidationResult(AuthValidator.Email(req.Email));
        Error.FromValidationResult(AuthValidator.Pwd(req.NewPwd));
        
        // start db tx
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var auth = await _db.Auths.SingleOrDefaultAsync(x => x.Email.Equals(req.Email));
            Error.If(auth == null, "no matching record found", @public: true, log: false);
            Error.If(auth.NotNull().ResetPwdCode != req.Code, "invalid reset pwd code", @public: true, log: false);
            var pwd = Crypto.HashPwd(req.NewPwd);
            auth.ResetPwdCodeCreatedOn = DateTimeExts.Zero();
            auth.ResetPwdCode = "";
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
    
    private const int AuthAttemptsRateLimit = 5;
    private static void RateLimitAuthAttempts(Auth auth)
    {
        Error.If(auth.LastSignInAttemptOn.SecondsSince() < AuthAttemptsRateLimit, $"auth attempts cannot be made more frequently than every {AuthAttemptsRateLimit} seconds", @public: true, log: false);
    }
}