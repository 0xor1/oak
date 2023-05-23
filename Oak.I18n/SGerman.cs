using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly IReadOnlyDictionary<string, TemplatableString> German = new Dictionary<
        string,
        TemplatableString
    >()
    {
        { HomeHeader, new("Willkommen in Oak!") },
        {
            HomeBody,
            new(
                "Dabei handelt es sich um eine Projektverwaltung, bei der Sie Aufgaben in einer Baumstruktur wie einem Dateiverzeichnis organisieren. Jede Aufgabe enthält eine Zusammenfassung der wichtigen Statistiken der darunter liegenden Aufgaben, der geschätzten und angefallenen Zeit und Kosten usw."
            )
        },
        { Invalid, new("Ungültig") },
        { RpcUnknownEndpoint, new("Unbekannter RPC-Endpunkt") },
        { UnexpectedError, new("Ein unerwarteter Fehler ist aufgetreten") },
        { EntityNotFound, new("{{Name}} nicht gefunden") },
        { InsufficientPermission, new("Unzureichende Erlaubnis") },
        { ApiError, new("API-Fehler") },
        { MinMaxNullArgs, new("Sowohl das Min- als auch das Max-Argument sind null") },
        { MinMaxReversedArgs, new("Min. {{Min}}- und Max. {{Max}}-Werte sind nicht geordnet") },
        { BadRequest, new("Ungültige Anforderung") },
        { AuthInvalidEmail, new("Ungültige E-Mail") },
        { AuthInvalidPwd, new("Ungültiges Passwort") },
        { LessThan8Chars, new("Weniger als 8 Zeichen") },
        { NoLowerCaseChar, new("Kein Kleinbuchstabe") },
        { NoUpperCaseChar, new("Kein Großbuchstabe") },
        { NoDigit, new("Keine Ziffer") },
        { NoSpecialChar, new("Kein Sonderzeichen") },
        { AuthAlreadyAuthenticated, new("Bereits in authentifizierter Sitzung") },
        { AuthNotAuthenticated, new("Nicht in authentifizierter Sitzung") },
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
        { AuthFcmTopicInvalid, new("FCM-Thema ungültig Min: {{Min}}, Max: {{Max}}") },
        { AuthFcmTokenInvalid, new("Fcm-Token ungültig") },
        { AuthFcmNotEnabled, new("FCM nicht aktiviert") },
        { Home, new("Heim") },
        { L10n, new("Lokalisierung") },
        { ToggleLiveUpdates, new("Live-Updates umschalten") },
        { Live, new("Direkt:") },
        { On, new("An") },
        { Off, new("Aus") },
        { Or, new("oder") },
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
        {
            StringValidation,
            new("Ungültiger String {{Name}}, Min. {{Min}}, Max. {{Max}}, Regexes {{Regexes}}")
        },
        { OrgTooMany, new("Sie sind bereits Mitglied in zu vielen Orgs") },
        {
            TaskTooManyDescN,
            new(
                "Zu viele Nachkommen, um alle zu erhalten, nur gültig für Aufgaben mit 1000 oder weniger Nachkommen"
            )
        },
        { TaskCantMoveRootProjectNode, new("Stammprojektknoten kann nicht verschoben werden") },
        {
            TaskRecursiveLoopDetected,
            new("Die Move-Operation würde zu einer rekursiven Schleife führen")
        },
        {
            TaskMovePrevSibParentMismatch,
            new("Frühere Geschwister- und Eltern-IDs stimmen nicht überein")
        },
        {
            TaskDeleteProjectAttempt,
            new(
                "Der Projektknoten kann nicht vom Aufgabenendpunkt gelöscht werden. Verwenden Sie den Projektlöschendpunkt"
            )
        },
        {
            TaskTooManyDescNToDelete,
            new(
                "Eine Aufgabe mit {{Max}} oder mehr untergeordneten Aufgaben kann nicht gelöscht werden"
            )
        },
        { VItemInvalidTimeInc, new("Der Zeiteintrag muss 1 bis 1440 Minuten betragen") },
        { VItemInvalidCostInc, new("Der Kosteneintrag muss größer als 0 sein") },
        { New, new("Neu") },
        { Create, new("Erstellen") },
        { Name, new("Name") },
        { CreatedOn, new("Erstellt am") },
        { OrgMyOrgs, new("Meine Organisationen") },
        { OrgNoOrgs, new("Keine Organisationen") },
        { OrgNewOrg, new("Neue Organisation") },
        { OrgName, new("Name der Organisation") },
        { OrgYourName, new("Ihren Namen") },
        { OrgProjects, new("Projekte") },
        { OrgNoProjects, new("Keine Projekte") }
    };
}
