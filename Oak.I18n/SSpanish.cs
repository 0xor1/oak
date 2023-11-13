using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, TemplatableString> Spanish =
        new()
        {
            { HomeHeader, new("¡Bienvenido a Roble!") },
            {
                HomeBody,
                new(
                    "Esta es una aplicación de gestión de proyectos donde organizas tareas en una estructura de árbol como un directorio de archivos. Cada tarea ofrece un resumen de las estadísticas vitales de las tareas subyacentes, el tiempo y los costos estimados e incurridos, etc."
                )
            },
            { Home, new("Hogar") },
            {
                StringValidation,
                new("Cadena no válida {{Nombre}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
            },
            { CopyToClipboardSuccess, new("Copiado al portapapeles") },
            { NotStarted, new("No empezado") },
            { Uploading, new("Cargando") },
            { OrgTooMany, new("Ya eres miembro de demasiadas organizaciones") },
            { ProjectInvalidHoursPerDay, new("Las horas por día deben estar entre 1 y 24") },
            { ProjectInvalidDaysPerWeek, new("Los días de la semana deben estar entre 1 y 7") },
            {
                ProjectFileLimitExceeded,
                new("Se excedió el límite de archivos del proyecto {{FileLimit}}")
            },
            {
                TaskTooManyDescN,
                new(
                    "Demasiados descendientes para obtenerlos todos, solo válido en tareas con 1000 o menos descendientes"
                )
            },
            { TaskCantMoveRootProjectNode, new("No se puede mover el nodo del proyecto raíz") },
            {
                TaskRecursiveLoopDetected,
                new("La operación de movimiento daría como resultado un bucle recursivo")
            },
            {
                TaskMovePrevSibParentMismatch,
                new("Las identificaciones de padres y hermanos anteriores no coinciden")
            },
            {
                TaskDeleteProjectAttempt,
                new(
                    "No se puede eliminar el nodo del proyecto del punto final de tareas, use el punto final de eliminación del proyecto"
                )
            },
            {
                TaskTooManyDescNToDelete,
                new("No se puede eliminar una tarea con {{Max}} o más tareas descendientes")
            },
            { VItemInvalidTimeInc, new("La entrada de tiempo debe ser de 1 a 1440 minutos") },
            { VItemInvalidCostInc, new("La entrada de costo debe ser mayor que 0") },
            { Display, new("Mostrar") },
            { User, new("Usuario") },
            { Time, new("Tiempo") },
            { Cost, new("Costo") },
            { File, new("Archivo") },
            { SubCounts, new("Conteos secundarios") },
            { Minutes, new("Minutos") },
            { Hours, new("Horas") },
            { Days, new("Días") },
            { Weeks, new("Semanas") },
            { Years, new("Años") },
            { Unassigned, new("Sin asignar") },
            { Task, new("Tarea") },
            { Parallel, new("Paralelo") },
            { Sequential, new("Secuencial") },
            { Comment, new("Comentario") },
            { Description, new("Descripción") },
            { NoDescription, new("Sin descripción") },
            { NothingToSeeHere, new("Nada que ver aqui.") },
            { Loading, new("Cargando") },
            { Min, new("Mínimo") },
            { Max, new("Máx.") },
            { True, new("Verdadero") },
            { False, new("Falso") },
            { Required, new("Requerido") },
            { Public, new("Público") },
            { New, new("Nuevo") },
            { Edit, new("Editar") },
            { Create, new("Crear") },
            { Delete, new("Borrar") },
            { Cancel, new("Cancelar") },
            { Confirm, new("Confirmar") },
            { Name, new("Nombre") },
            { Currency, new("Divisa") },
            { CreatedOn, new("Creado en") },
            { HoursPerDay, new("Horas al día") },
            { DaysPerWeek, new("Días por semana") },
            { StartOn, new("Comienza en") },
            { EndOn, new("Finalizará el") },
            { Note, new("Nota") },
            { TimeMin, new("Tiempo mín.") },
            { TimeEst, new("Tiempo estimado") },
            { TimeInc, new("Tiempo incurrido") },
            { CostEst, new("Costo estimado") },
            { CostInc, new("Coste incurrido") },
            { FileN, new("Número de archivos") },
            { FileSize, new("Tamaño del archivo") },
            { ChildN, new("Niños") },
            { DescN, new("Descendientes") },
            { FileLimit, new("File Limit") },
            { Upload, new("Subir") },
            { OrgMyOrgs, new("Mis organizaciones") },
            { OrgNoOrgs, new("Sin organizaciones") },
            { OrgNewOrg, new("Nueva organización") },
            { OrgUpdateOrg, new("Actualizar organización") },
            {
                OrgConfirmDeleteOrg,
                new(
                    "<p>¿Está seguro de que desea eliminar la organización <strong>{{Name}}</strong>?</p><p>Esto no se puede deshacer.</p>"
                )
            },
            { OrgName, new("nombre de la organización") },
            { OrgYourName, new("Su nombre") },
            { OrgNameProjects, new("{{Name}} Proyectos") },
            { OrgProjects, new("Proyectos") },
            { OrgNoProjects, new("Sin Proyectos") },
            { OrgNewProject, new("Nuevo proyecto") },
            { OrgUpdateProject, new("Actualizar proyecto") },
            {
                OrgConfirmDeleteProject,
                new(
                    "<p>¿Está seguro de que desea eliminar el proyecto <strong>{{Name}}</strong>?</p><p>Esto no se puede deshacer.</p>"
                )
            },
            {
                OrgMemberInviteEmailSubject,
                new("{{OrgName}} - Invitación a la gestión de proyectos")
            },
            {
                OrgMemberInviteEmailHtml,
                new(
                    "<p>Estimado <strong>{{InviteeName}}</strong></p><p><strong>{{InvitedByName}}</strong> te ha invitado a unirte a la organización: <strong>{{OrgName} }</strong></p><p><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Haga clic en este enlace para verificar su dirección de correo electrónico y únete a <strong>{{OrgName}}</strong></a></p>"
                )
            },
            {
                OrgMemberInviteEmailText,
                new(
                    "Estimado, {{InviteeName}}\n\n{{InvitedByName}} lo ha invitado a unirse a la organización: {{OrgName}}\n\nHaga clic en este enlace para verificar su dirección de correo electrónico y unirse a {{OrgName}}:\n\n{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
                )
            },
            { TaskNewTask, new("Nueva tarea") },
            { TaskUpdateTask, new("Actualizar tarea") },
            {
                TaskConfirmDeleteTask,
                new(
                    "<p>¿Está seguro de que desea eliminar la tarea <strong>{{Name}}</strong>?</p><p>Esto no se puede deshacer.</p>"
                )
            },
            { TaskMovingTask, new("Tarea en movimiento: <strong>{{Name}}</strong>") },
            { TaskNewTime, new("Nuevo tiempo") },
            { TaskNewCost, new("Nuevo costo") },
            { TaskUpdateTime, new("Tiempo de actualizacion") },
            { TaskUpdateCost, new("Costo de actualización") },
            { TaskNoTimes, new("sin tiempos") },
            { TaskNoCosts, new("Sin Costos") },
            {
                TaskConfirmDeleteVItem,
                new("<p>¿Está seguro de que desea eliminar <strong>{{Value}}</strong>?</p>")
            },
            { TaskNoFiles, new("Sin archivos") },
            {
                TaskConfirmDeleteFile,
                new(
                    "<p>¿Está seguro de que desea eliminar el archivo <strong>{{Name}}</strong>?</p>"
                )
            },
            { TaskUploadFile, new("Subir archivo") }
        };
}
