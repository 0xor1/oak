using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, TemplatableString> German =
        new()
        {
            { HomeHeader, new("Willkommen in Oak!") },
            {
                HomeBody,
                new(
                    "Dies ist eine Projektmanagement-App, mit der Sie Aufgaben in einer Baumstruktur wie einem Dateiverzeichnis organisieren. Jede Aufgabe enthält eine Zusammenfassung der wichtigen Statistiken der darunter liegenden Aufgaben, der geschätzten und angefallenen Zeit und Kosten usw."
                )
            },
            { Home, new("Heim") },
            {
                StringValidation,
                new("Ungültiger String {{Name}}, Min. {{Min}}, Max. {{Max}}, Regexes {{Regexes}}")
            },
            { CopyToClipboardSuccess, new("In die Zwischenablage kopiert") },
            { NotStarted, new("Nicht angefangen") },
            { Uploading, new("Hochladen") },
            { OrgTooMany, new("Sie sind bereits Mitglied in zu vielen Orgs") },
            {
                ProjectInvalidHoursPerDay,
                new("Die Stunden pro Tag müssen zwischen 1 und 24 liegen")
            },
            { ProjectInvalidDaysPerWeek, new("Die Tage pro Woche müssen zwischen 1 und 7 liegen") },
            { ProjectFileLimitExceeded, new("Projektdateilimit überschritten {{FileLimit}}") },
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
            { Display, new("Anzeige") },
            { User, new("Benutzer") },
            { Time, new("Zeit") },
            { Cost, new("Kosten") },
            { File, new("Datei") },
            { SubCounts, new("Sub-Zählungen") },
            { Minutes, new("Minuten") },
            { Hours, new("Stunden") },
            { Days, new("Tage") },
            { Weeks, new("Wochen") },
            { Years, new("Jahre") },
            { Unassigned, new("Nicht zugewiesen") },
            { Task, new("Aufgabe") },
            { Parallel, new("Parallel") },
            { Sequential, new("Sequentiell") },
            { Comment, new("Kommentar") },
            { Description, new("Beschreibung") },
            { NoDescription, new("Keine Beschreibung") },
            { NothingToSeeHere, new("Es gibt hier nichts zu sehen.") },
            { Loading, new("Wird geladen") },
            { Min, new("Min") },
            { Max, new("Max") },
            { True, new("Wahr") },
            { False, new("Falsch") },
            { Required, new("Erforderlich") },
            { Public, new("Öffentlich") },
            { New, new("Neu") },
            { Edit, new("Bearbeiten") },
            { Create, new("Erstellen") },
            { Delete, new("Löschen") },
            { Cancel, new("Stornieren") },
            { Confirm, new("Bestätigen") },
            { Name, new("Name") },
            { Currency, new("Währung") },
            { CreatedOn, new("Erstellt am") },
            { HoursPerDay, new("Stunden pro Tag") },
            { DaysPerWeek, new("Tage pro Woche") },
            { StartOn, new("Beginnen am") },
            { EndOn, new("Endet am") },
            { Note, new("Notiz") },
            { TimeMin, new("Zeitminimum") },
            { TimeEst, new("Zeitschätzung") },
            { TimeInc, new("Angefallene Zeit") },
            { CostEst, new("Geschätzte Kosten") },
            { CostInc, new("Entstandenen Kosten") },
            { FileN, new("Dateianzahl") },
            { ChildN, new("Kinder") },
            { DescN, new("Nachkommenschaft") },
            { FileSize, new("Dateigröße") },
            { FileLimit, new("Dateilimit") },
            { Upload, new("Hochladen") },
            { OrgMyOrgs, new("Meine Organisationen") },
            { OrgNoOrgs, new("Keine Organisationen") },
            { OrgNewOrg, new("Neue Organisation") },
            { OrgUpdateOrg, new("Organisation aktualisieren") },
            {
                OrgConfirmDeleteOrg,
                new(
                    "<p>Sind Sie sicher, dass Sie die Organisation <strong>{{Name}}</strong> löschen möchten?</p><p>Dies kann nicht rückgängig gemacht werden.</p>"
                )
            },
            { OrgName, new("Name der Organisation") },
            { OrgYourName, new("Ihren Namen") },
            { OrgNameProjects, new("{{Name}} Projekte") },
            { OrgProjects, new("Projekte") },
            { OrgNoProjects, new("Keine Projekte") },
            { OrgNewProject, new("Neues Projekt") },
            { OrgUpdateProject, new("Projekt aktualisieren") },
            {
                OrgConfirmDeleteProject,
                new(
                    "<p>Sind Sie sicher, dass Sie das Projekt <strong>{{Name}}</strong> löschen möchten?</p><p>Dies kann nicht rückgängig gemacht werden.</p>"
                )
            },
            { TaskNewTask, new("Neue Aufgabe") },
            { TaskUpdateTask, new("Update-Aufgabe") },
            {
                TaskConfirmDeleteTask,
                new(
                    "<p>Sind Sie sicher, dass Sie die Aufgabe <strong>{{Name}}</strong> löschen möchten?</p><p>Dies kann nicht rückgängig gemacht werden.</p>"
                )
            },
            { TaskMovingTask, new("Umzugsaufgabe: <strong>{{Name}}</strong>") },
            { TaskNewTime, new("Neue Zeit") },
            { TaskNewCost, new("Neue Kosten") },
            { TaskUpdateTime, new("Updatezeit") },
            { TaskUpdateCost, new("Aktualisierungskosten") },
            { TaskNoTimes, new("Keine Zeiten") },
            { TaskNoCosts, new("Keine Kosten") },
            {
                TaskConfirmDeleteVItem,
                new("<p>Sind Sie sicher, dass Sie <strong>{{Value}}</strong>?</p>")
            },
            { TaskNoFiles, new("Keine Dateien") },
            {
                TaskConfirmDeleteFile,
                new(
                    "<p>Sind Sie sicher, dass Sie die Datei <strong>{{Name}}</strong> löschen möchten?</p>"
                )
            },
            { TaskUploadFile, new("Datei hochladen") }
        };
}
