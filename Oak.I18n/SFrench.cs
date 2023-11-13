using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, TemplatableString> French =
        new()
        {
            { HomeHeader, new("Bienvenue à Chêne!") },
            {
                HomeBody,
                new(
                    "Il s'agit d'une application de gestion de projet dans laquelle vous organisez les tâches dans une arborescence comme un répertoire de fichiers. Chaque tâche donne un résumé des statistiques vitales des tâches en dessous, du temps et des coûts estimés et engagés, etc."
                )
            },
            { Home, new("Maison") },
            {
                StringValidation,
                new("Chaîne non valide {{Name}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
            },
            { CopyToClipboardSuccess, new("Copié dans le presse-papier") },
            { NotStarted, new("Pas commencé") },
            { Uploading, new("Téléchargement") },
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
                ProjectFileLimitExceeded,
                new("La limite de fichiers de projet a dépassé {{FileLimit}}")
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
                new(
                    "Impossible de supprimer une tâche avec {{Max}} ou plusieurs tâches descendantes"
                )
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
            { Upload, new("Télécharger") },
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
            { OrgMemberInviteEmailSubject, new("{{OrgName}} - Invitation à la gestion de projet") },
            {
                OrgMemberInviteEmailHtml,
                new(
                    "<p>Cher <strong>{{InviteeName}}</strong></p><p><strong>{{InvitedByName}}</strong> vous a invité à rejoindre l'organisation: <strong>{{OrgName} }</strong></p><p><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Veuillez cliquer sur ce lien pour vérifier votre adresse e-mail et rejoignez <strong>{{OrgName}}</strong></a></p>"
                )
            },
            {
                OrgMemberInviteEmailText,
                new(
                    "Cher {{InviteeName}}\n\n{{InvitedByName}} vous a invité à rejoindre l'organisation: {{OrgName}}\n\nVeuillez cliquer sur ce lien pour vérifier votre adresse e-mail et rejoindre {{OrgName}}:\n\n{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
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
                new(
                    "<p>Êtes-vous sûr de vouloir supprimer le fichier <strong>{{Name}}</strong>?</p>"
                )
            },
            { TaskUploadFile, new("Téléverser un fichier") }
        };
}
