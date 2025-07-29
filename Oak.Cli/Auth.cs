using Common.Shared;
using Common.Shared.Auth;
using IApi = Oak.Api.IApi;

namespace Oak.Cli;

public class Auth
{
    private readonly IApi _api;

    public Auth(IApi api)
    {
        _api = api;
    }

    /// <summary>
    /// Get your current session
    /// </summary>
    /// <param name="ctkn"></param>
    public async Task GetSession(CancellationToken ctkn = default)
    {
        var ses = await _api.Auth.GetSession(ctkn);
        Io.WriteYml(ses);
    }

    /// <summary>
    /// Register for a new account.
    /// </summary>
    /// <param name="email">-e, your email address</param>
    /// <param name="ctkn"></param>
    public async Task Register(string email, CancellationToken ctkn = default)
    {
        var pwd = Io.GetSensitiveValue("Enter Password: ");
        var confirmPwd = Io.GetSensitiveValue("Confirm Password: ");
        if (pwd != confirmPwd)
        {
            Console.WriteLine("Passwords do not match.");
        }
        await _api.Auth.Register(new Register(email, pwd), ctkn);
        Console.WriteLine(
            $"An email has been sent to {email}\nPlease use the provided link to complete account setup."
        );
    }

    /// <summary>
    /// Verify your email address
    /// </summary>
    /// <param name="email">-e, The email you used to register</param>
    /// <param name="code">-c, The code that was sent to your email address</param>
    /// <param name="ctkn"></param>
    public async Task VerifyEmail(string email, string code, CancellationToken ctkn = default)
    {
        await _api.Auth.VerifyEmail(new(email, code), ctkn);
    }

    /// <summary>
    /// Send a reset password email
    /// </summary>
    /// <param name="email">-e, The email address to send the reset password email to</param>
    /// <param name="ctkn"></param>
    public async Task SendResetPwdEmail(string email, CancellationToken ctkn = default)
    {
        await _api.Auth.SendResetPwdEmail(new(email), ctkn);
    }

    /// <summary>
    /// Reset your password
    /// </summary>
    /// <param name="email">-e, Your email address</param>
    /// <param name="code">-c, The reset password code</param>
    /// <param name="newPwd">-p, Your new password</param>
    /// <param name="ctkn"></param>
    public async Task ResetPwd(
        string email,
        string code,
        string newPwd,
        CancellationToken ctkn = default
    )
    {
        await _api.Auth.ResetPwd(new(email, code, newPwd), ctkn);
    }

    /// <summary>
    /// Send a magic link email
    /// </summary>
    /// <param name="email">-e, The email address to send the email to</param>
    /// <param name="rememberMe">-r, If you want the session to expire after browser tab closes</param>
    /// <param name="ctkn"></param>
    public async Task SendMagicLinkEmail(
        string email,
        bool rememberMe,
        CancellationToken ctkn = default
    )
    {
        await _api.Auth.SendMagicLinkEmail(new(email, rememberMe), ctkn);
    }

    /// <summary>
    /// Sign in using the code from a magic link email
    /// </summary>
    /// <param name="email">-e, The email the magic link was sent to</param>
    /// <param name="code">-c, The code from the magic link email</param>
    /// <param name="rememberMe">-r, If you want the session to expire after browser tab closes</param>
    /// <param name="ctkn"></param>
    public async Task MagicLinkSignIn(
        string email,
        string code,
        bool rememberMe,
        CancellationToken ctkn = default
    )
    {
        await _api.Auth.MagicLinkSignIn(new(email, code, rememberMe), ctkn);
    }

    /// <summary>
    /// Sign in to the app
    /// </summary>
    /// <param name="email">-e, The email of the account</param>
    /// <param name="rememberMe">-r, If you want the session to expire after browser tab closes</param>
    public async Task SignIn(string email, bool rememberMe = true)
    {
        var pwd = Io.GetSensitiveValue("Enter Password: ");
        Io.WriteYml(await _api.Auth.SignIn(new SignIn(email, pwd, rememberMe)));
    }

    /// <summary>
    /// Sign out from the app
    /// </summary>
    /// <param name="ctkn"></param>
    public async Task SignOut(CancellationToken ctkn = default) =>
        Io.WriteYml(await _api.Auth.SignOut(ctkn));

    /// <summary>
    /// Permanently delete your account
    /// </summary>
    /// <param name="ctkn"></param>
    public async Task Delete(CancellationToken ctkn = default) => await _api.Auth.Delete(ctkn);

    /// <summary>
    /// Set your l10n settings
    /// </summary>
    /// <param name="lang">-l, language code</param>
    /// <param name="dateFmt">-df, date format ymd dmy or mdy</param>
    /// <param name="timeFmt">-tf, time format like HH:mm</param>
    /// <param name="dateSeparator">-ds, date separator like - or /</param>
    /// <param name="thousandsSeparator">-ts, thousands separator like , </param>
    /// <param name="decimalSeparator">-decs, decimal separator like . </param>
    /// <param name="ctkn"></param>
    public async Task SetL10n(
        string lang,
        DateFmt dateFmt,
        string timeFmt,
        string dateSeparator,
        string thousandsSeparator,
        string decimalSeparator,
        CancellationToken ctkn = default
    )
    {
        var ses = await _api.Auth.SetL10n(
            new(lang, dateFmt, timeFmt, dateSeparator, thousandsSeparator, decimalSeparator),
            ctkn
        );
        Io.WriteYml(ses);
    }

    // these are only really relevant in a browser environment
    // public async Task FcmEnabled(FcmEnabled arg, CancellationToken ctkn = default) { }
    //
    // public async Task FcmRegister(FcmRegister arg, CancellationToken ctkn = default) { }
    //
    // public async Task FcmUnregister(FcmUnregister arg, CancellationToken ctkn = default) { }
}
