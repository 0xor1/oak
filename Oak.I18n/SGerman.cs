using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly IReadOnlyDictionary<string, TemplatableString> German = new Dictionary<
        string,
        TemplatableString
    >()
    {
        {
            Oak,
            new(
                "<h1>Hallo, Oak!</h1><p>Willkommen bei Ihrem neuen dotnet-Starterkit.</p><p>Sie finden:</p><ul><li>Client: eine Blazor-Wasm-App, die verwendet radzen ui library</li><li>Server: aspnet mit grpc api und Entity Framework db interface</li></ul>"
            )
        },
        { Invalid, new("Ungültig") },
        { AuthInvalidEmail, new("Ungültige E-Mail") },
        { AuthInvalidPwd, new("Ungültiges Passwort") },
        { LessThan8Chars, new("Weniger als 8 Zeichen") },
        { NoLowerCaseChar, new("Kein Kleinbuchstabe") },
        { NoUpperCaseChar, new("Kein Großbuchstabe") },
        { NoDigit, new("Keine Ziffer") },
        { NoSpecialChar, new("Kein Sonderzeichen") },
        { UnexpectedError, new("Ein unerwarteter Fehler ist aufgetreten") },
        { AuthAlreadyAuthenticated, new("Bereits in authentifizierter Sitzung") },
        { AuthNotAuthenticated, new("Nicht in authentifizierter Sitzung") },
        { NoMatchingRecord, new("Kein übereinstimmender Datensatz") },
        { AuthInvalidEmailCode, new("Ungültiger E-Mail-Code") },
        { AuthInvalidResetPwdCode, new("Ungültiger Passwort-Reset-Code") },
        {
            AuthAccountNotVerified,
            new("Konto nicht bestätigt, bitte überprüfen Sie Ihre E-Mails auf den Bestätigungslink")
        },
        {
            AuthAttemptRateLimit,
            new(
                "Authentifizierungsversuche können nicht häufiger als alle {{Seconds}} Sekunden durchgeführt werden"
            )
        },
        { AuthConfirmEmailSubject, new("e-Mail-Adresse bestätigen") },
        {
            AuthConfirmEmailHtml,
            new(
                "<div><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Bitte klicken Sie auf diesen Link, um Ihre E-Mail-Adresse zu bestätigen</a></div>"
            )
        },
        {
            AuthConfirmEmailText,
            new(
                "Bitte verwenden Sie diesen Link, um Ihre E-Mail-Adresse zu bestätigen: {{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
            )
        },
        { AuthResetPwdSubject, new("Passwort zurücksetzen") },
        {
            AuthResetPwdHtml,
            new(
                "<div><a href=\"{{BaseHref}}/reset_pwd?email={{Email}}&code={{Code}}\">klicken Sie bitte auf diesen Link, um Ihr Passwort zurückzusetzen</a></div>"
            )
        },
        {
            AuthResetPwdText,
            new(
                "Bitte klicken Sie auf diesen Link, um Ihr Passwort zurückzusetzen: {{BaseHref}}/reset_pwd?email={{Email}}&code={{Code}}"
            )
        },
        { ApiError, new("API-Fehler") },
        { Home, new("Heim") },
        { L10n, new("Lokalisierung") },
        { Language, new("Sprache") },
        { DateFmt, new("Datumsformat") },
        { TimeFmt, new("Zeitformat") },
        { Register, new("Registrieren") },
        { Registering, new("Registrieren") },
        {
            RegisterSuccess,
            new(
                "Bitte überprüfen Sie Ihre E-Mails auf einen Bestätigungslink, um die Registrierung abzuschließen."
            )
        },
        { SignIn, new("Anmelden") },
        { RememberMe, new("Mich erinnern") },
        { SigningIn, new("Anmelden") },
        { SignOut, new("Austragen") },
        { SigningOut, new("Abmelden") },
        { VerifyEmail, new("E-Mail bestätigen") },
        { Verifying, new("Überprüfung") },
        { VerifyingEmail, new("Überprüfung Ihrer E-Mail") },
        { VerifyEmailSuccess, new("Danke für das Verifizieren deiner E-Mail.") },
        { ResetPwd, new("Passwort zurücksetzen") },
        { Email, new("Email") },
        { Pwd, new("Passwort") },
        { ConfirmPwd, new("Bestätige das Passwort") },
        { PwdsDontMatch, new("Passwörter stimmen nicht überein") },
        { ResetPwdSuccess, new("Sie können sich jetzt mit Ihrem neuen Passwort anmelden.") },
        { Resetting, new("Zurücksetzen") },
        { SendResetPwdLink, new("Link zum Zurücksetzen des Passworts senden") },
        {
            SendResetPwdLinkSuccess,
            new(
                "Wenn diese E-Mail mit einem Konto übereinstimmt, wurde eine E-Mail zum Zurücksetzen Ihres Passworts gesendet."
            )
        },
        { Processing, new("wird bearbeitet") },
        { Send, new("Schicken") },
        { Success, new("Erfolg") },
        { Error, new("Fehler") },
        { Update, new("Aktualisieren") },
        { Updating, new("Aktualisierung") },
        { InsufficientPermission, new("Unzureichende Berechtigung") },
        { OrgTooMany, new("Sie sind bereits Mitglied in zu vielen Orgs") }
    };
}
