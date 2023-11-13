using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, TemplatableString> Italian =
        new()
        {
            { HomeHeader, new("Benvenuti in Quercia!") },
            {
                HomeBody,
                new(
                    "Questa è un'app di gestione dei progetti in cui organizzi le attività in una struttura ad albero come una directory di file. Ogni attività fornisce un riepilogo delle statistiche vitali delle attività sottostanti, tempi e costi stimati e sostenuti, ecc."
                )
            },
            { Home, new("Casa") },
            {
                StringValidation,
                new("Stringa non valida {{Name}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
            },
            { CopyToClipboardSuccess, new("Copiato negli appunti") },
            { NotStarted, new("Non iniziato") },
            { Uploading, new("Caricamento in corso") },
            { OrgTooMany, new("Sei già membro di troppe org") },
            {
                ProjectInvalidHoursPerDay,
                new("Le ore giornaliere devono essere comprese tra 1 e 24")
            },
            {
                ProjectInvalidDaysPerWeek,
                new("I giorni della settimana devono essere compresi tra 1 e 7")
            },
            { ProjectFileLimitExceeded, new("Limite del file di progetto superato {{FileLimit}}") },
            {
                TaskTooManyDescN,
                new(
                    "Troppi discendenti per ottenerli tutti, valido solo per attività con 1000 o meno discendenti"
                )
            },
            {
                TaskCantMoveRootProjectNode,
                new("Impossibile spostare il nodo principale del progetto")
            },
            {
                TaskRecursiveLoopDetected,
                new("L'operazione di spostamento risulterebbe in un ciclo ricorsivo")
            },
            {
                TaskMovePrevSibParentMismatch,
                new("Gli ID dei fratelli e dei genitori precedenti non corrispondono")
            },
            {
                TaskDeleteProjectAttempt,
                new(
                    "Impossibile eliminare il nodo del progetto dall'endpoint delle attività, utilizzare l'endpoint di eliminazione del progetto"
                )
            },
            {
                TaskTooManyDescNToDelete,
                new("Impossibile eliminare un'attività con {{Max}} o più attività discendenti")
            },
            {
                VItemInvalidTimeInc,
                new("L'inserimento dell'ora deve essere compreso tra 1 e 1440 minuti")
            },
            { VItemInvalidCostInc, new("La voce di costo deve essere maggiore di 0") },
            { Display, new("Schermo") },
            { User, new("Utente") },
            { Time, new("Tempo") },
            { Cost, new("Costo") },
            { File, new("File") },
            { SubCounts, new("Conteggi secondari") },
            { Minutes, new("Minuti") },
            { Hours, new("Ore") },
            { Days, new("Giorni") },
            { Weeks, new("Settimane") },
            { Years, new("Anni") },
            { Unassigned, new("Non assegnato") },
            { Task, new("Compito") },
            { Parallel, new("Parallelo") },
            { Sequential, new("Sequenziale") },
            { Comment, new("Commento") },
            { Description, new("Descrizione") },
            { NoDescription, new("Nessuna descrizione") },
            { NothingToSeeHere, new("Niente da vedere quì.") },
            { Loading, new("Caricamento") },
            { Min, new("Minimo") },
            { Max, new("Massimo") },
            { True, new("Vero") },
            { False, new("Falso") },
            { Required, new("Necessario") },
            { Public, new("Pubblico") },
            { New, new("Nuovo") },
            { Edit, new("Modificare") },
            { Create, new("Creare") },
            { Delete, new("Eliminare") },
            { Cancel, new("Annulla") },
            { Confirm, new("Confermare") },
            { Name, new("Nome") },
            { Currency, new("Valuta") },
            { CreatedOn, new("Creato") },
            { HoursPerDay, new("Ore al giorno") },
            { DaysPerWeek, new("Giorni alla settimana") },
            { StartOn, new("Inizia") },
            { EndOn, new("Fine su") },
            { Note, new("Nota") },
            { TimeMin, new("Tempo min.") },
            { TimeEst, new("Stima del tempo") },
            { TimeInc, new("Tempo impiegato") },
            { CostEst, new("Costo stimato") },
            { CostInc, new("Costo sostenuto") },
            { FileN, new("Conteggio file") },
            { FileSize, new("Dimensione del file") },
            { ChildN, new("Bambini") },
            { DescN, new("Discendenti") },
            { FileLimit, new("Limite file") },
            { Upload, new("Caricamento") },
            { Invite, new("Invitare") },
            { Active, new("Attivo") },
            { OrgMyOrgs, new("Le mie organizzazioni") },
            { OrgNoOrgs, new("Nessuna organizzazione") },
            { OrgNewOrg, new("Nuova Organizzazione") },
            { OrgUpdateOrg, new("Aggiorna organizzazione") },
            {
                OrgConfirmDeleteOrg,
                new(
                    "<p>Sei sicuro di voler eliminare l'organizzazione <strong>{{Name}}</strong>?</p><p>L'operazione non può essere annullata.</p>"
                )
            },
            { OrgName, new("Nome dell'Organizzazione") },
            { OrgYourName, new("Il tuo nome") },
            { OrgNameProjects, new("{{Name}} Progetti") },
            { OrgProjects, new("Progetti") },
            { OrgNoProjects, new("Nessuna Progetti") },
            { OrgNewProject, new("Nuovo progetto") },
            { OrgUpdateProject, new("Aggiorna progetto") },
            {
                OrgConfirmDeleteProject,
                new(
                    "<p>Sei sicuro di voler eliminare il progetto <strong>{{Name}}</strong>?</p><p>L'operazione non può essere annullata.</p>"
                )
            },
            { OrgMembers, new("Membri") },
            { OrgNoMembers, new("Nessun membro") },
            { OrgNewMember, new("Nuovo membro") },
            { OrgUpdateMember, new("Aggiorna membro") },
            {
                OrgConfirmDeactivateMember,
                new("<p>Sei sicuro di voler disattivare il membro <strong>{{Name}}</strong>?</p>")
            },
            { OrgMemberInviteEmailSubject, new("{{OrgName}} - Invito alla gestione del progetto") },
            {
                OrgMemberInviteEmailHtml,
                new(
                    "<p>Caro <strong>{{InviteeName}}</strong></p><p><strong>{{InvitedByName}}</strong> ti ha invitato a unirti all'organizzazione: <strong>{{OrgName} }</strong></p><p><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Fai clic su questo link per verificare il tuo indirizzo email e iscriviti a <strong>{{OrgName}}</strong></a></p>"
                )
            },
            {
                OrgMemberInviteEmailText,
                new(
                    "Gentile {{InviteeName}}\n\n{{InvitedByName}} ti ha invitato a unirti all'organizzazione: {{OrgName}}\n\nFai clic su questo collegamento per verificare il tuo indirizzo email e iscriverti a {{OrgName}}:\n\n{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
                )
            },
            { OrgMemberRole, new("Ruolo") },
            { OrgMemberRoleOwner, new("Proprietario") },
            { OrgMemberRoleAdmin, new("Ammin") },
            { OrgMemberRoleWriteAllProjects, new("Scrivi tutti i progetti") },
            { OrgMemberRoleReadAllProjects, new("Leggi tutti i progetti") },
            { OrgMemberRolePerProject, new("Per progetto") },
            { TaskNewTask, new("Nuovo compito") },
            { TaskUpdateTask, new("Attività di aggiornamento") },
            {
                TaskConfirmDeleteTask,
                new(
                    "<p>Sei sicuro di voler eliminare l'attività <strong>{{Name}}</strong>?</p><p>L'operazione non può essere annullata.</p>"
                )
            },
            { TaskMovingTask, new("Attività in movimento: <strong>{{Name}}</strong>") },
            { TaskNewTime, new("Nuovo tempo") },
            { TaskNewCost, new("Nuovo costo") },
            { TaskUpdateTime, new("Tempo di aggiornamento") },
            { TaskUpdateCost, new("Aggiorna costo") },
            { TaskNoTimes, new("Nessun tempo") },
            { TaskNoCosts, new("Nessun costo") },
            {
                TaskConfirmDeleteVItem,
                new("<p>Sei sicuro di voler eliminare <strong>{{Value}}</strong>?</p>")
            },
            { TaskNoFiles, new("Nessun documento") },
            {
                TaskConfirmDeleteFile,
                new("<p>Sei sicuro di voler eliminare il file <strong>{{Name}}</strong>?</p>")
            },
            { TaskUploadFile, new("Caricare un file") }
        };
}
