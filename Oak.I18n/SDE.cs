// Generated Code File, Do Not Edit.
// This file is generated with Common.I18nCodeGen.

using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, TemplatableString> DE_Strings = new Dictionary<
        string,
        TemplatableString
    >()
    {
        { Active, new("Aktiv") },
        { Add, new("Hinzufügen") },
        { Cancel, new("Stornieren") },
        { ChildN, new("Kinder") },
        { Clear, new("Klar") },
        { Comment, new("Kommentar") },
        { Confirm, new("Bestätigen") },
        { CopyToClipboardSuccess, new("In die Zwischenablage kopiert") },
        { Cost, new("Kosten") },
        { CostEst, new("Geschätzte Kosten") },
        { CostInc, new("Entstandenen Kosten") },
        { Create, new("Erstellen") },
        { CreatedOn, new("Erstellt am") },
        { Currency, new("Währung") },
        { Days, new("Tage") },
        { DaysPerWeek, new("Tage pro Woche") },
        { Delete, new("Löschen") },
        { DescN, new("Nachkommenschaft") },
        { Description, new("Beschreibung") },
        { Display, new("Anzeige") },
        { DropAfter, new("Danach fallenlassen") },
        { DropIn, new("Drinnen legen") },
        { Edit, new("Bearbeiten") },
        { EndOn, new("Endet am") },
        { EntityNameMembers, new("{{Name}} Mitglieder") },
        { False, new("Falsch") },
        { File, new("Datei") },
        { FileLimit, new("Dateilimit") },
        { FileN, new("Dateianzahl") },
        { FileSize, new("Dateigröße") },
        { Home, new("Heim") },
        {
            HomeBody,
            new(
                "Dies ist eine Projektmanagement-App, mit der Sie Aufgaben in einer Baumstruktur wie einem Dateiverzeichnis organisieren. Jede Aufgabe enthält eine Zusammenfassung der wichtigen Statistiken der darunter liegenden Aufgaben, der geschätzten und angefallenen Zeit und Kosten usw."
            )
        },
        { HomeHeader, new("Willkommen in Oak!") },
        { Hours, new("Stunden") },
        { HoursPerDay, new("Stunden pro Tag") },
        { Invite, new("Einladen") },
        { Loading, new("Wird geladen") },
        { Max, new("Max") },
        { Min, new("Min") },
        { Minutes, new("Minuten") },
        { Move, new("Bewegen") },
        { Name, new("Name") },
        { New, new("Neu") },
        { NoDescription, new("Keine Beschreibung") },
        { NotStarted, new("Nicht angefangen") },
        { Note, new("Notiz") },
        { NothingToSeeHere, new("Es gibt hier nichts zu sehen.") },
        {
            OrgConfirmDeactivateMember,
            new(
                "<p>Sind Sie sicher, dass Sie das Mitglied <strong>{{Name}}</strong> deaktivieren möchten?</p>"
            )
        },
        {
            OrgConfirmDeleteOrg,
            new(
                "<p>Sind Sie sicher, dass Sie die Organisation <strong>{{Name}}</strong> löschen möchten?</p><p>Dies kann nicht rückgängig gemacht werden.</p>"
            )
        },
        {
            OrgConfirmDeleteProject,
            new(
                "<p>Sind Sie sicher, dass Sie das Projekt <strong>{{Name}}</strong> löschen möchten?</p><p>Dies kann nicht rückgängig gemacht werden.</p>"
            )
        },
        {
            OrgMemberInviteEmailHtml,
            new(
                "<p>Lieber <strong>{{InviteeName}}</strong></p><p><strong>{{InvitedByName}}</strong> hat Sie eingeladen, der Organisation beizutreten: <strong>{{OrgName} </strong></p><p><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Bitte klicken Sie auf diesen Link, um Ihre zu bestätigen E-Mail-Adresse und treten Sie <strong>{{OrgName}}</strong></a></p> bei"
            )
        },
        { OrgMemberInviteEmailSubject, new("{{OrgName}} – Projektmanagement-Einladung") },
        {
            OrgMemberInviteEmailText,
            new(
                "Lieber {{InviteeName}}\n\n{{InvitedByName}} hat Sie eingeladen, der Organisation beizutreten: {{OrgName}}\n\nBitte klicken Sie auf diesen Link, um Ihre E-Mail-Adresse zu bestätigen und {{OrgName}} beizutreten:\n\n{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
            )
        },
        { OrgMemberRole, new("Rolle") },
        { OrgMemberRoleAdmin, new("Administrator") },
        { OrgMemberRoleOwner, new("Eigentümer") },
        { OrgMemberRolePerProject, new("Pro Projekt") },
        { OrgMemberRoleReadAllProjects, new("Alle Projekte lesen") },
        { OrgMemberRoleWriteAllProjects, new("Schreiben Sie alle Projekte") },
        { OrgMembers, new("Mitglieder") },
        { OrgMyOrgs, new("Meine Organisationen") },
        { OrgName, new("Name der Organisation") },
        { OrgNameProjects, new("{{Name}} Projekte") },
        { OrgNewMember, new("Neues Mitglied") },
        { OrgNewOrg, new("Neue Organisation") },
        { OrgNewProject, new("Neues Projekt") },
        { OrgNoMembers, new("Keine Mitglieder") },
        { OrgNoOrgs, new("Keine Organisationen") },
        { OrgNoProjects, new("Keine Projekte") },
        { OrgProjects, new("Projekte") },
        { OrgTooMany, new("Sie sind bereits Mitglied in zu vielen Orgs") },
        { OrgUpdateMember, new("Mitglied aktualisieren") },
        { OrgUpdateOrg, new("Organisation aktualisieren") },
        { OrgUpdateProject, new("Projekt aktualisieren") },
        { OrgYourName, new("Ihren Namen") },
        { Parallel, new("Parallel") },
        { ProjectFileLimitExceeded, new("Projektdateilimit überschritten {{FileLimit}}") },
        { ProjectInvalidDaysPerWeek, new("Die Tage pro Woche müssen zwischen 1 und 7 liegen") },
        { ProjectInvalidHoursPerDay, new("Die Stunden pro Tag müssen zwischen 1 und 24 liegen") },
        { ProjectMemberRoleAdmin, new("Administrator") },
        { ProjectMemberRoleWriter, new("Schriftsteller") },
        { ProjectMemberRoleReader, new("Leser") },
        { ProjectMembers, new("Projektmitglieder") },
        { Public, new("Öffentlich") },
        { Required, new("Erforderlich") },
        { Sequential, new("Sequentiell") },
        { StartOn, new("Beginnen am") },
        {
            StringValidation,
            new("Ungültiger String {{Name}}, Min. {{Min}}, Max. {{Max}}, Regexes {{Regexes}}")
        },
        { SubCounts, new("Sub-Zählungen") },
        { Task, new("Aufgabe") },
        { TaskCantMoveRootProjectNode, new("Stammprojektknoten kann nicht verschoben werden") },
        { TaskConfirmDeleteComment, new("<p>Sind Sie sicher") },
        {
            TaskConfirmDeleteFile,
            new(
                "<p>Sind Sie sicher, dass Sie die Datei <strong>{{Name}}</strong> löschen möchten?</p>"
            )
        },
        {
            TaskConfirmDeleteTask,
            new(
                "<p>Sind Sie sicher, dass Sie die Aufgabe <strong>{{Name}}</strong> löschen möchten?</p><p>Dies kann nicht rückgängig gemacht werden.</p>"
            )
        },
        {
            TaskConfirmDeleteVitem,
            new("<p>Sind Sie sicher, dass Sie <strong>{{Value}}</strong>?</p>")
        },
        {
            TaskDeleteProjectAttempt,
            new(
                "Der Projektknoten kann nicht vom Aufgabenendpunkt gelöscht werden. Verwenden Sie den Projektlöschendpunkt"
            )
        },
        { TaskEditTask, new("Update-Aufgabe") },
        {
            TaskMovePrevSibParentMismatch,
            new("Frühere Geschwister- und Eltern-IDs stimmen nicht überein")
        },
        { TaskMovingTask, new("Umzugsaufgabe: <strong>{{Name}}</strong>") },
        { TaskNewCost, new("Neue Kosten") },
        { TaskNewTask, new("Neue Aufgabe") },
        { TaskNewTime, new("Neue Zeit") },
        { TaskNoCosts, new("Keine Kosten") },
        { TaskNoFiles, new("Keine Dateien") },
        { TaskNoTimes, new("Keine Zeiten") },
        {
            TaskRecursiveLoopDetected,
            new("Die Move-Operation würde zu einer rekursiven Schleife führen")
        },
        {
            TaskTooManyDescn,
            new(
                "Zu viele Nachkommen, um alle zu erhalten, nur gültig für Aufgaben mit 1000 oder weniger Nachkommen"
            )
        },
        {
            TaskTooManyDescnToDelete,
            new(
                "Eine Aufgabe mit {{Max}} oder mehr untergeordneten Aufgaben kann nicht gelöscht werden"
            )
        },
        { TaskUpdateCost, new("Aktualisierungskosten") },
        { TaskUpdateTime, new("Updatezeit") },
        { TaskUploadFile, new("Datei hochladen") },
        { Tasks, new("Aufgaben") },
        { Time, new("Zeit") },
        { TimeEst, new("Zeitschätzung") },
        { TimeInc, new("Angefallene Zeit") },
        { TimeMin, new("Zeitminimum") },
        { True, new("Wahr") },
        { Unassigned, new("Nicht zugewiesen") },
        { Upload, new("Hochladen") },
        { Uploading, new("Hochladen") },
        { User, new("Benutzer") },
        { VitemInvalidCostInc, new("Der Kosteneintrag muss größer als 0 sein") },
        { VitemInvalidTimeInc, new("Der Zeiteintrag muss 1 bis 1440 Minuten betragen") },
        { Weeks, new("Wochen") },
        { Years, new("Jahre") },
    };
}
