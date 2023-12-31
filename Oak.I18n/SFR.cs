// Generated Code File, Do Not Edit.
// This file is generated with Common.I18nCodeGen.

using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, TemplatableString> FR_Strings = new Dictionary<
        string,
        TemplatableString
    >()
    {
        { Active, new("Actif") },
        { Add, new("Ajouter") },
        { Cancel, new("Annuler") },
        { ChildN, new("Enfants") },
        { Clear, new("Clair") },
        { Comment, new("Commentaire") },
        { Confirm, new("Confirmer") },
        { CopyToClipboardSuccess, new("Copié dans le presse-papier") },
        { Cost, new("Coût") },
        { CostEst, new("Prix estimé") },
        { CostInc, new("Coûts encourus") },
        { Create, new("Créer") },
        { CreatedOn, new("Créé sur") },
        { Currency, new("Monnaie") },
        { Days, new("Jours") },
        { DaysPerWeek, new("Jours par semaine") },
        { Delete, new("Supprimer") },
        { DescN, new("Descendance") },
        { Description, new("Description") },
        { Display, new("Afficher") },
        { DropAfter, new("Déposer après") },
        { DropIn, new("Placer à l'intérieur") },
        { Edit, new("Modifier") },
        { EndOn, new("Fin le") },
        { EntityNameMembers, new("{{Name}} Membres") },
        { False, new("Faux") },
        { File, new("Dossier") },
        { FileLimit, new("Limite de fichiers") },
        { FileN, new("Nombre de fichiers") },
        { FileSize, new("Taille du fichier") },
        { Home, new("Maison") },
        {
            HomeBody,
            new(
                "Oak est une application de gestion de projet, les tâches sont organisées en arborescences comme un répertoire de fichiers. Chaque tâche contient un résumé des statistiques de toutes les tâches en dessous, comme le temps estimé et engagé."
            )
        },
        { HomeHeader, new("Bienvenue à Chêne!") },
        { Hours, new("Heures") },
        { HoursPerDay, new("Heures par jour") },
        { Invite, new("Inviter") },
        { Loading, new("Chargement") },
        { Max, new("Max") },
        { Min, new("Min") },
        { Minutes, new("Minutes") },
        { Move, new("Se déplacer") },
        { Name, new("Nom") },
        { New, new("Nouveau") },
        { NoDescription, new("Pas de description") },
        { NotStarted, new("Pas commencé") },
        { Note, new("Note") },
        { NothingToSeeHere, new("Rien à voir ici.") },
        {
            OrgConfirmDeactivateMember,
            new("<p>Êtes-vous sûr de vouloir désactiver le membre <strong>{{Name}}</strong>?</p>")
        },
        {
            OrgConfirmDeleteOrg,
            new(
                "<p>Voulez-vous vraiment supprimer l'organisation <strong>{{Name}}</strong>?</p><p>Cette opération est irréversible.</p>"
            )
        },
        {
            OrgConfirmDeleteProject,
            new(
                "<p>Voulez-vous vraiment supprimer le projet <strong>{{Name}}</strong>?</p><p>Cette opération est irréversible.</p>"
            )
        },
        {
            OrgMemberInviteEmailHtml,
            new(
                "<p>Cher <strong>{{InviteeName}}</strong></p><p><strong>{{InvitedByName}}</strong> vous a invité à rejoindre l'organisation: <strong>{{OrgName} }</strong></p><p><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Veuillez cliquer sur ce lien pour vérifier votre adresse e-mail et rejoignez <strong>{{OrgName}}</strong></a></p>"
            )
        },
        { OrgMemberInviteEmailSubject, new("{{OrgName}} - Invitation à la gestion de projet") },
        {
            OrgMemberInviteEmailText,
            new(
                "Cher {{InviteeName}}\n\n{{InvitedByName}} vous a invité à rejoindre l'organisation: {{OrgName}}\n\nVeuillez cliquer sur ce lien pour vérifier votre adresse e-mail et rejoindre {{OrgName}}:\n\n{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
            )
        },
        { OrgMemberRole, new("Rôle") },
        { OrgMemberRoleAdmin, new("Administrateur") },
        { OrgMemberRoleOwner, new("Propriétaire") },
        { OrgMemberRolePerProject, new("Par projet") },
        { OrgMemberRoleReadAllProjects, new("Lire tous les projets") },
        { OrgMemberRoleWriteAllProjects, new("Écrire tous les projets") },
        { OrgMembers, new("Membres") },
        { OrgMyOrgs, new("Mes organisations") },
        { OrgName, new("Nom de l'organisme") },
        { OrgNameProjects, new("{{Name}} Projes") },
        { OrgNewMember, new("Nouveau membre") },
        { OrgNewOrg, new("Nouvelle organisation") },
        { OrgNewProject, new("Nouveau projet") },
        { OrgNoMembers, new("Aucun membre") },
        { OrgNoOrgs, new("Aucune organisation") },
        { OrgNoProjects, new("Aucune Projets") },
        { OrgProjects, new("Projets") },
        { OrgTooMany, new("Vous êtes déjà membre d'un trop grand nombre d'organisations") },
        { OrgUpdateMember, new("Mettre à jour le membre") },
        { OrgUpdateOrg, new("Mettre à jour l'organisation") },
        { OrgUpdateProject, new("Mettre à jour le projet") },
        { OrgYourName, new("votre nom") },
        { Parallel, new("Parallèle") },
        {
            ProjectFileLimitExceeded,
            new("La limite de fichiers de projet a dépassé {{FileLimit}}")
        },
        {
            ProjectInvalidDaysPerWeek,
            new("Les jours par semaine doivent être compris entre 1 et 7")
        },
        {
            ProjectInvalidHoursPerDay,
            new("Les heures par jour doivent être comprises entre 1 et 24")
        },
        { ProjectMemberRoleAdmin, new("Administrateur") },
        { ProjectMemberRoleWriter, new("Écrivain") },
        { ProjectMemberRoleReader, new("Lecteur") },
        { ProjectMembers, new("Membres du projet") },
        { Public, new("Public") },
        { Required, new("Requis") },
        { Sequential, new("Séquentiel") },
        { StartOn, new("Démarrer") },
        {
            StringValidation,
            new("Chaîne non valide {{Name}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
        },
        { SubCounts, new("Sous-comptes") },
        { Task, new("Tâche") },
        { TaskCantMoveRootProjectNode, new("Impossible de déplacer le nœud du projet racine") },
        {
            TaskConfirmDeleteComment,
            new("<p>Êtes-vous sûr de vouloir supprimer ce commentaire?</p>")
        },
        {
            TaskConfirmDeleteFile,
            new("<p>Êtes-vous sûr de vouloir supprimer le fichier <strong>{{Name}}</strong>?</p>")
        },
        {
            TaskConfirmDeleteTask,
            new(
                "<p>Êtes-vous sûr de vouloir supprimer la tâche <strong>{{Name}}</strong>?</p><p>Cette opération est irréversible.</p>"
            )
        },
        {
            TaskConfirmDeleteVitem,
            new("<p>Êtes-vous sûr de vouloir supprimer <strong>{{Valeur}}</strong>?</p>")
        },
        {
            TaskDeleteProjectAttempt,
            new(
                "Impossible de supprimer le nœud de projet du point de terminaison des tâches, utilisez le point de terminaison de suppression du projet"
            )
        },
        { TaskEditTask, new("Mettre à jour la tâche") },
        {
            TaskMovePrevSibParentMismatch,
            new(
                "Les précédents identifiants des frères et sœurs et des parents ne correspondent pas"
            )
        },
        { TaskMovingTask, new("Tâche de déplacement: <strong>{{Name}}</strong>") },
        { TaskNewCost, new("Nouveau coût") },
        { TaskNewTask, new("Nouvelle tâche") },
        { TaskNewTime, new("Nouvelle heure") },
        { TaskNoCosts, new("Aucun frais") },
        { TaskNoFiles, new("Aucun fichier") },
        { TaskNoTimes, new("Pas de temps") },
        {
            TaskRecursiveLoopDetected,
            new("L'opération de déplacement entraînerait une boucle récursive")
        },
        {
            TaskTooManyDescn,
            new(
                "Trop de descendants pour tous les obtenir, valide uniquement sur les tâches avec 1000 descendants ou moins"
            )
        },
        {
            TaskTooManyDescnToDelete,
            new("Impossible de supprimer une tâche avec {{Max}} ou plusieurs tâches descendantes")
        },
        { TaskUpdateCost, new("Coût de mise à jour") },
        { TaskUpdateTime, new("Temps de mise à jour") },
        { TaskUploadFile, new("Téléverser un fichier") },
        { Tasks, new("Tâches") },
        { Time, new("Temps") },
        { TimeEst, new("Temps estimée.") },
        { TimeInc, new("Temps engagé") },
        { TimeMin, new("Durée min.") },
        { TimerAlreadyExists, new("La minuterie existe déjà") },
        { TimerMaxTimers, new("Minuteries maximales par utilisateur: {{MaxTimers}}") },
        { Timers, new("Minuteries") },
        { True, new("Vrai") },
        { Unassigned, new("Non attribué") },
        { Upload, new("Télécharger") },
        { Uploading, new("Téléchargement") },
        { User, new("Utilisateur") },
        { VitemInvalidCostInc, new("L'entrée de coût doit être supérieure à 0") },
        {
            VitemInvalidTimeInc,
            new("L'entrée de temps doit être comprise entre 1 et 1440 minutes")
        },
        { Weeks, new("Semaines") },
        { Years, new("Ans") },
    };
}
