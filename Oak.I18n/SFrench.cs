using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly IReadOnlyDictionary<string, TemplatableString> French = new Dictionary<
        string,
        TemplatableString
    >()
    {
        { HomeHeader, new("Bienvenue à Chêne !") },
        {
            HomeBody,
            new(
                "Il s'agit d'une gestion de projet où vous organisez les tâches dans une arborescence comme un répertoire de fichiers. Chaque tâche donne un résumé des statistiques vitales des tâches en dessous, du temps et des coûts estimés et encourus, etc."
            )
        },
        { Invalid, new("Invalide") },
        { RpcUnknownEndpoint, new("Point de terminaison RPC inconnu") },
        { UnexpectedError, new("Une erreur inattendue est apparue") },
        { EntityNotFound, new("{{Name}} introuvable") },
        { InsufficientPermission, new("Permission insuffisante") },
        { ApiError, new("Erreur API") },
        { MinMaxNullArgs, new("Les arguments min et max sont nuls") },
        { MinMaxReversedArgs, new("Les valeurs Min {{Min}} et Max {{Max}} ne sont pas ordonnées") },
        { BadRequest, new("Mauvaise demande") },
        { AuthInvalidEmail, new("Email invalide") },
        { AuthInvalidPwd, new("Mot de passe incorrect") },
        { LessThan8Chars, new("Moins de 8 caractères") },
        { NoLowerCaseChar, new("Pas de caractère minuscule") },
        { NoUpperCaseChar, new("Pas de caractère majuscule") },
        { NoDigit, new("Aucun chiffre") },
        { NoSpecialChar, new("Aucun caractère spécial") },
        { AuthAlreadyAuthenticated, new("Déjà en session authentifiée") },
        { AuthNotAuthenticated, new("Pas en session authentifiée") },
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
        { AuthFcmTopicInvalid, new("Sujet Fcm invalide Min: {{Min}}, Max: {{Max}}") },
        { AuthFcmTokenInvalid, new("Jeton Fcm invalide") },
        { AuthFcmNotEnabled, new("FCM non activé") },
        { Home, new("Maison") },
        { L10n, new("Localisation") },
        { ToggleLiveUpdates, new("Basculer les mises à jour en direct") },
        { Live, new("En direct:") },
        { On, new("Sur") },
        { Off, new("Désactivé") },
        { Or, new("ou") },
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
        { Updating, new("Mise à jour") },
        {
            StringValidation,
            new("Chaîne non valide {{Name}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
        },
        { OrgTooMany, new("Vous êtes déjà membre d'un trop grand nombre d'organisations") },
        {
            ProjectInvalidHoursPerDay,
            new("Les heures par jour doivent être comprises entre 1 et 24")
        },
        {
            ProjectInvalidDaysPerWeek,
            new("Les jours par semaine doivent être compris entre 1 et 7")
        },
        {
            TaskTooManyDescN,
            new(
                "Trop de descendants pour tous les obtenir, valide uniquement sur les tâches avec 1000 descendants ou moins"
            )
        },
        { TaskCantMoveRootProjectNode, new("Impossible de déplacer le nœud du projet racine") },
        {
            TaskRecursiveLoopDetected,
            new("L'opération de déplacement entraînerait une boucle récursive")
        },
        {
            TaskMovePrevSibParentMismatch,
            new(
                "Les précédents identifiants des frères et sœurs et des parents ne correspondent pas"
            )
        },
        {
            TaskDeleteProjectAttempt,
            new(
                "Impossible de supprimer le nœud de projet du point de terminaison des tâches, utilisez le point de terminaison de suppression du projet"
            )
        },
        {
            TaskTooManyDescNToDelete,
            new("Impossible de supprimer une tâche avec {{Max}} ou plusieurs tâches descendantes")
        },
        {
            VItemInvalidTimeInc,
            new("L'entrée de temps doit être comprise entre 1 et 1440 minutes")
        },
        { VItemInvalidCostInc, new("L'entrée de coût doit être supérieure à 0") },
        { Display, new("Afficher") },
        { User, new("Utilisateur") },
        { Time, new("Temps") },
        { Cost, new("Coût") },
        { File, new("Dossier") },
        { SubCounts, new("Sous-comptes") },
        { Minutes, new("Minutes") },
        { Hours, new("Heures") },
        { Days, new("Jours") },
        { Weeks, new("Semaines") },
        { Years, new("Ans") },
        { Unassigned, new("Non attribué") },
        { Task, new("Tâche") },
        { Parallel, new("Parallèle") },
        { Sequential, new("Séquentiel") },
        { Comment, new("Commentaire") },
        { Description, new("Description") },
        { NoDescription, new("Pas de description") },
        { NothingToSeeHere, new("Rien à voir ici.") },
        { Loading, new("Chargement") },
        { Min, new("Min") },
        { Max, new("Max") },
        { True, new("Vrai") },
        { False, new("Faux") },
        { Required, new("Requis") },
        { Public, new("Public") },
        { New, new("Nouveau") },
        { Edit, new("Modifier") },
        { Create, new("Créer") },
        { Delete, new("Supprimer") },
        { Cancel, new("Annuler") },
        { Confirm, new("Confirmer") },
        { Name, new("Nom") },
        { Currency, new("Monnaie") },
        { CreatedOn, new("Créé sur") },
        { HoursPerDay, new("Heures par jour") },
        { DaysPerWeek, new("Jours par semaine") },
        { StartOn, new("Démarrer") },
        { EndOn, new("Fin le") },
        { Note, new("Note") },
        { TimeMin, new("Durée min.") },
        { TimeEst, new("Temps estimée.") },
        { TimeInc, new("Temps engagé") },
        { CostEst, new("Prix estimé") },
        { CostInc, new("Coûts encourus") },
        { FileN, new("Nombre de fichiers") },
        { FileSize, new("Taille du fichier") },
        { ChildN, new("Enfants") },
        { DescN, new("Descendance") },
        { FileLimit, new("Limite de fichiers") },
        { OrgMyOrgs, new("Mes organisations") },
        { OrgNoOrgs, new("Aucune organisation") },
        { OrgNewOrg, new("Nouvelle organisation") },
        { OrgUpdateOrg, new("Mettre à jour l'organisation") },
        {
            OrgConfirmDeleteOrg,
            new(
                "<p>Voulez-vous vraiment supprimer l'organisation <strong>{{Name}}</strong>?</p><p>Cette opération est irréversible.</p>"
            )
        },
        { OrgName, new("Nom de l'organisme") },
        { OrgYourName, new("votre nom") },
        { OrgNameProjects, new("{{Name}} Projes") },
        { OrgProjects, new("Projets") },
        { OrgNoProjects, new("Aucune Projets") },
        { OrgNewProject, new("Nouveau projet") },
        { OrgUpdateProject, new("Mettre à jour le projet") },
        {
            OrgConfirmDeleteProject,
            new(
                "<p>Voulez-vous vraiment supprimer le projet <strong>{{Name}}</strong>?</p><p>Cette opération est irréversible.</p>"
            )
        },
        { TaskNewTask, new("Nouvelle tâche") },
        { TaskUpdateTask, new("Mettre à jour la tâche") },
        {
            TaskConfirmDeleteTask,
            new(
                "<p>Êtes-vous sûr de vouloir supprimer la tâche <strong>{{Name}}</strong>?</p><p>Cette opération est irréversible.</p>"
            )
        },
        { TaskMovingTask, new("Tâche de déplacement: <strong>{{Name}}</strong>") },
        { TaskNewTime, new("Nouvelle heure") },
        { TaskNewCost, new("Nouveau coût") },
        { TaskUpdateTime, new("Temps de mise à jour") },
        { TaskUpdateCost, new("Coût de mise à jour") },
        { TaskNoTimes, new("Pas de temps") },
        { TaskNoCosts, new("Aucun frais") },
        {
            TaskConfirmDeleteVItem,
            new("<p>Êtes-vous sûr de vouloir supprimer <strong>{{Valeur}}</strong>?</p>")
        },
        { TaskNoFiles, new("Aucun fichier") },
        {
            TaskConfirmDeleteFile,
            new("<p>Êtes-vous sûr de vouloir supprimer le fichier <strong>{{Name}}</strong>?</p>")
        }
    };
}
