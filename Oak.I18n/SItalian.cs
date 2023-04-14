using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly IReadOnlyDictionary<string, TemplatableString> Italian = new Dictionary<
        string,
        TemplatableString
    >()
    {
        {
            Oak,
            new(
                "<h1>Ciao, Oak!</h1><p>Benvenuto nel tuo nuovo starter kit dotnet.</p><p>Troverai:</p><ul><li>Client: un'app blazor wasm che utilizza Libreria radzen ui</li><li>Server: aspnet con interfaccia grpc api e entità framework db</li></ul>"
            )
        },
        { Invalid, new("Non valido") },
        { AuthInvalidEmail, new("E-mail non valido") },
        { AuthInvalidPwd, new("Password non valida") },
        { LessThan8Chars, new("Meno di 8 caratteri") },
        { NoLowerCaseChar, new("Nessun carattere minuscolo") },
        { NoUpperCaseChar, new("Nessun carattere maiuscolo") },
        { NoDigit, new("Nessuna cifra") },
        { NoSpecialChar, new("Nessun carattere speciale") },
        { UnexpectedError, new("Si è verificato un errore imprevisto") },
        { AuthAlreadyAuthenticated, new("Già in sessione autenticata") },
        { NoMatchingRecord, new("Nessun record corrispondente") },
        { AuthInvalidEmailCode, new("Codice e-mail non valido") },
        { AuthInvalidResetPwdCode, new("Codice di reimpostazione della password non valido") },
        {
            AuthAccountNotVerified,
            new("Account non verificato, controlla le tue e-mail per il link di verifica")
        },
        {
            AuthAttemptRateLimit,
            new(
                "I tentativi di autenticazione non possono essere effettuati più frequentemente di ogni {{Seconds}} secondi"
            )
        },
        { AuthConfirmEmailSubject, new("Conferma l'indirizzo e-mail") },
        {
            AuthConfirmEmailHtml,
            new(
                "<div><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Fai clic su questo link per verificare il tuo indirizzo email</a></div>"
            )
        },
        {
            AuthConfirmEmailText,
            new(
                "Utilizza questo link per verificare il tuo indirizzo email: {{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
            )
        },
        { AuthResetPwdSubject, new("Resetta la password") },
        {
            AuthResetPwdHtml,
            new(
                "<div><a href=\"{{BaseHref}}/reset_pwd?email={{Email}}&code={{Code}}\">Fai clic su questo link per reimpostare la tua password</a></div>"
            )
        },
        {
            AuthResetPwdText,
            new(
                "Fai clic su questo link per reimpostare la tua password: {{BaseHref}}/reset_pwd?email={{Email}}&code={{Code}}"
            )
        },
        { ApiError, new("Errore API") },
        { Home, new("Casa") },
        { L10n, new("Localizzazione") },
        { Language, new("Lingua") },
        { DateFmt, new("Formato data") },
        { TimeFmt, new("Formato orario") },
        { Register, new("Registrati") },
        { Registering, new("Registrazione") },
        {
            RegisterSuccess,
            new("Controlla le tue e-mail per un link di conferma per completare la registrazione.")
        },
        { SignIn, new("Registrazione") },
        { RememberMe, new("Ricordati di me") },
        { SigningIn, new("Registrarsi") },
        { SignOut, new("Disconnessione") },
        { SigningOut, new("Disconnessione") },
        { VerifyEmail, new("Verifica Email") },
        { Verifying, new("Verifica") },
        { VerifyingEmail, new("Verifica della tua email") },
        { VerifyEmailSuccess, new("Grazie per aver verificato la tua email.") },
        { ResetPwd, new("Resetta la password") },
        { Email, new("E-mail") },
        { Pwd, new("Parola d'ordine") },
        { ConfirmPwd, new("Conferma password") },
        { PwdsDontMatch, new("Le password non corrispondono") },
        { ResetPwdSuccess, new("Ora puoi accedere con la tua nuova password.") },
        { Resetting, new("Ripristino") },
        { SendResetPwdLink, new("Invia collegamento per reimpostare la password") },
        {
            SendResetPwdLinkSuccess,
            new(
                "Se questa e-mail corrisponde a un account, sarà stata inviata un'e-mail per reimpostare la password."
            )
        },
        { Processing, new("in lavorazione") },
        { Send, new("Inviare") },
        { Success, new("Successo") },
        { Error, new("Errore") },
        { Update, new("Aggiornamento") },
        { Updating, new("In aggiornamento") }
    };
}
