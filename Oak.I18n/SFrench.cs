using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly IReadOnlyDictionary<string, TemplatableString> French = new Dictionary<
        string,
        TemplatableString
    >()
    {
        {
            Oak,
            new(
                "<h1>Bonjour, Oak !</h1><p>Bienvenue dans votre nouveau kit de démarrage dotnet.</p><p>Vous trouverez :</p><ul><li>Client : une application blazor wasm utilisant bibliothèque radzen ui</li><li>Serveur : aspnet avec grpc api et interface de base de données du cadre d'entité</li></ul>"
            )
        },
        { Invalid, new("Invalide") },
        { AuthInvalidEmail, new("Email invalide") },
        { AuthInvalidPwd, new("Mot de passe incorrect") },
        { LessThan8Chars, new("Moins de 8 caractères") },
        { NoLowerCaseChar, new("Pas de caractère minuscule") },
        { NoUpperCaseChar, new("Pas de caractère majuscule") },
        { NoDigit, new("Aucun chiffre") },
        { NoSpecialChar, new("Aucun caractère spécial") },
        { UnexpectedError, new("Une erreur inattendue est apparue") },
        { AuthAlreadyAuthenticated, new("Déjà en session authentifiée") },
        { NoMatchingRecord, new("Aucun enregistrement correspondant") },
        { AuthInvalidEmailCode, new("Code e-mail invalide") },
        { AuthInvalidResetPwdCode, new("Code de mot de passe de réinitialisation invalide") },
        {
            AuthAccountNotVerified,
            new("Compte non vérifié, veuillez vérifier vos e-mails pour le lien de vérification")
        },
        {
            AuthAttemptRateLimit,
            new(
                "Les tentatives d'authentification ne peuvent pas être effectuées plus fréquemment que toutes les {{Seconds}} secondes"
            )
        },
        { AuthConfirmEmailSubject, new("Confirmez votre adresse email") },
        {
            AuthConfirmEmailHtml,
            new(
                "<div><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Veuillez cliquer sur ce lien pour vérifier votre adresse e-mail</a></div>"
            )
        },
        {
            AuthConfirmEmailText,
            new(
                "Veuillez utiliser ce lien pour vérifier votre adresse e-mail: {{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
            )
        },
        { AuthResetPwdSubject, new("Réinitialiser le mot de passe") },
        {
            AuthResetPwdHtml,
            new(
                "<div><a href=\"{{BaseHref}}/reset_pwd?email={{Email}}&code={{Code}}\">Veuillez cliquer sur ce lien pour réinitialiser votre mot de passe</a></div>"
            )
        },
        {
            AuthResetPwdText,
            new(
                "Veuillez cliquer sur ce lien pour réinitialiser votre mot de passe: {{BaseHref}}/reset_pwd?email={{Email}}&code={{Code}}"
            )
        },
        { ApiError, new("Erreur API") },
        { Home, new("Maison") },
        { L10n, new("Localisation") },
        { Language, new("Langue") },
        { DateFmt, new("Format de date") },
        { TimeFmt, new("Format de l'heure") },
        { Register, new("Enregistrer") },
        { Registering, new("Enregistrement") },
        {
            RegisterSuccess,
            new(
                "Veuillez vérifier vos e-mails pour un lien de confirmation pour terminer l'inscription."
            )
        },
        { SignIn, new("S'identifier") },
        { RememberMe, new("Souviens-toi de moi") },
        { SigningIn, new("Connectez-vous") },
        { SignOut, new("Se déconnecter") },
        { SigningOut, new("Déconnecter") },
        { VerifyEmail, new("Vérifier les courriels") },
        { Verifying, new("Vérification") },
        { VerifyingEmail, new("Vérification de votre e-mail") },
        { VerifyEmailSuccess, new("Merci d'avoir vérifié votre adresse e-mail.") },
        { ResetPwd, new("Réinitialiser le mot de passe") },
        { Email, new("E-mail") },
        { Pwd, new("Mot de passe") },
        { ConfirmPwd, new("Confirmez le mot de passe") },
        { PwdsDontMatch, new("Les mots de passe ne correspondent pas") },
        {
            ResetPwdSuccess,
            new("Vous pouvez maintenant vous connecter avec votre nouveau mot de passe.")
        },
        { Resetting, new("Réinitialisation") },
        { SendResetPwdLink, new("Envoyer le lien de réinitialisation du mot de passe") },
        {
            SendResetPwdLinkSuccess,
            new(
                "Si cet e-mail correspond à un compte, un e-mail vous aura été envoyé pour réinitialiser votre mot de passe."
            )
        },
        { Processing, new("Traitement") },
        { Send, new("Envoyer") },
        { Success, new("Succès") },
        { Error, new("Erreur") },
        { Update, new("Mise à jour") },
        { Updating, new("Mise à jour") }
    };
}
