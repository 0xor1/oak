using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly IReadOnlyDictionary<string, TemplatableString> Italian = new Dictionary<
        string,
        TemplatableString
    >()
    {
        { HomeHeader, new("Benvenuti in Quercia!") },
        {
            HomeBody,
            new(
                "Questa è una gestione del progetto in cui organizzi le attività in una struttura ad albero come una directory di file. Ogni attività fornisce un riepilogo delle statistiche vitali delle attività sottostanti, tempi e costi stimati e sostenuti, ecc."
            )
        },
        { Invalid, new("Non valido") },
        { RpcUnknownEndpoint, new("Endpoint RPC sconosciuto") },
        { UnexpectedError, new("Si è verificato un errore imprevisto") },
        { EntityNotFound, new("{{Name}} non trovata") },
        { InsufficientPermission, new("Autorizzazione insufficiente") },
        { ApiError, new("Errore API") },
        { MinMaxNullArgs, new("Entrambi gli argomenti min e max sono nulli") },
        { MinMaxReversedArgs, new("I valori Min {{Min}} e Max {{Max}} non sono ordinati") },
        { BadRequest, new("Brutta richiesta") },
        { AuthInvalidEmail, new("E-mail non valido") },
        { AuthInvalidPwd, new("Password non valida") },
        { LessThan8Chars, new("Meno di 8 caratteri") },
        { NoLowerCaseChar, new("Nessun carattere minuscolo") },
        { NoUpperCaseChar, new("Nessun carattere maiuscolo") },
        { NoDigit, new("Nessuna cifra") },
        { NoSpecialChar, new("Nessun carattere speciale") },
        { AuthAlreadyAuthenticated, new("Già in sessione autenticata") },
        { AuthNotAuthenticated, new("Non in sessione autenticata") },
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
        { AuthFcmTopicInvalid, new("Argomento Fcm non valido Min: {{Min}}, Max: {{Max}}") },
        { AuthFcmTokenInvalid, new("Token Fcm non valido") },
        { AuthFcmNotEnabled, new("Fcm non abilitato") },
        { Home, new("Casa") },
        { L10n, new("Localizzazione") },
        { ToggleLiveUpdates, new("Attiva/disattiva gli aggiornamenti in tempo reale") },
        { Live, new("Vivo:") },
        { On, new("SU") },
        { Off, new("Spento") },
        { Or, new("O") },
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
        { NotStarted, new("Non iniziato") },
        { Uploading, new("Caricamento in corso") },
        { Success, new("Successo") },
        { Error, new("Errore") },
        { Update, new("Aggiornamento") },
        { Updating, new("In aggiornamento") },
        {
            StringValidation,
            new("Stringa non valida {{Name}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
        },
        { CopyToClipboardSuccess, new("Copiato negli appunti") },
        { OrgTooMany, new("Sei già membro di troppe org") },
        { ProjectInvalidHoursPerDay, new("Le ore giornaliere devono essere comprese tra 1 e 24") },
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
