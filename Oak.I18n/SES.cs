// Generated Code File, Do Not Edit.
// This file is generated with Common.I18nCodeGen.

using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, TemplatableString> ES_Strings = new Dictionary<
        string,
        TemplatableString
    >()
    {
        { Active, new("Activo") },
        { Add, new("Agregar") },
        { Cancel, new("Cancelar") },
        { ChildN, new("Niños") },
        { Clear, new("Claro") },
        { Comment, new("Comentario") },
        { Confirm, new("Confirmar") },
        { CopyToClipboardSuccess, new("Copiado al portapapeles") },
        { Cost, new("Costo") },
        { CostEst, new("Costo estimado") },
        { CostInc, new("Coste incurrido") },
        { Create, new("Crear") },
        { CreatedOn, new("Creado en") },
        { Currency, new("Divisa") },
        { Days, new("Días") },
        { DaysPerWeek, new("Días por semana") },
        { Delete, new("Borrar") },
        { DescN, new("Descendientes") },
        { Description, new("Descripción") },
        { Display, new("Mostrar") },
        { DropAfter, new("Soltar después") },
        { DropIn, new("Colocar dentro") },
        { Edit, new("Editar") },
        { EndOn, new("Finalizará el") },
        { EntityNameMembers, new("{{Name}} Miembros") },
        { False, new("Falso") },
        { File, new("Archivo") },
        { FileLimit, new("File Limit") },
        { FileN, new("Número de archivos") },
        { FileSize, new("Tamaño del archivo") },
        { Home, new("Hogar") },
        {
            HomeBody,
            new(
                "Esta es una aplicación de gestión de proyectos donde organizas tareas en una estructura de árbol como un directorio de archivos. Cada tarea ofrece un resumen de las estadísticas vitales de las tareas subyacentes, el tiempo y los costos estimados e incurridos, etc."
            )
        },
        { HomeHeader, new("¡Bienvenido a Roble!") },
        { Hours, new("Horas") },
        { HoursPerDay, new("Horas al día") },
        { Invite, new("Invitar") },
        { Loading, new("Cargando") },
        { Max, new("Máx.") },
        { Min, new("Mínimo") },
        { Minutes, new("Minutos") },
        { Move, new("Mover") },
        { Name, new("Nombre") },
        { New, new("Nuevo") },
        { NoDescription, new("Sin descripción") },
        { NotStarted, new("No empezado") },
        { Note, new("Nota") },
        { NothingToSeeHere, new("Nada que ver aqui.") },
        {
            OrgConfirmDeactivateMember,
            new(
                "<p>¿Estás seguro de que deseas desactivar el miembro <strong>{{Name}}</strong>?</p>"
            )
        },
        {
            OrgConfirmDeleteOrg,
            new(
                "<p>¿Está seguro de que desea eliminar la organización <strong>{{Name}}</strong>?</p><p>Esto no se puede deshacer.</p>"
            )
        },
        {
            OrgConfirmDeleteProject,
            new(
                "<p>¿Está seguro de que desea eliminar el proyecto <strong>{{Name}}</strong>?</p><p>Esto no se puede deshacer.</p>"
            )
        },
        {
            OrgMemberInviteEmailHtml,
            new(
                "<p>Estimado <strong>{{InviteeName}}</strong></p><p><strong>{{InvitedByName}}</strong> te ha invitado a unirte a la organización: <strong>{{OrgName} }</strong></p><p><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Haga clic en este enlace para verificar su dirección de correo electrónico y únete a <strong>{{OrgName}}</strong></a></p>"
            )
        },
        { OrgMemberInviteEmailSubject, new("{{OrgName}} - Invitación a la gestión de proyectos") },
        {
            OrgMemberInviteEmailText,
            new(
                "Estimado, {{InviteeName}}\n\n{{InvitedByName}} lo ha invitado a unirse a la organización: {{OrgName}}\n\nHaga clic en este enlace para verificar su dirección de correo electrónico y unirse a {{OrgName}}:\n\n{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
            )
        },
        { OrgMemberRole, new("Role") },
        { OrgMemberRoleAdmin, new("Administrador") },
        { OrgMemberRoleOwner, new("Dueño") },
        { OrgMemberRolePerProject, new("Por proyecto") },
        { OrgMemberRoleReadAllProjects, new("Leer todos los proyectos") },
        { OrgMemberRoleWriteAllProjects, new("Escribir todos los proyectos") },
        { OrgMembers, new("Miembros") },
        { OrgMyOrgs, new("Mis organizaciones") },
        { OrgName, new("nombre de la organización") },
        { OrgNameProjects, new("{{Name}} Proyectos") },
        { OrgNewMember, new("Nuevo miembro") },
        { OrgNewOrg, new("Nueva organización") },
        { OrgNewProject, new("Nuevo proyecto") },
        { OrgNoMembers, new("Sin miembros") },
        { OrgNoOrgs, new("Sin organizaciones") },
        { OrgNoProjects, new("Sin Proyectos") },
        { OrgProjects, new("Proyectos") },
        { OrgTooMany, new("Ya eres miembro de demasiadas organizaciones") },
        { OrgUpdateMember, new("Actualizar miembro") },
        { OrgUpdateOrg, new("Actualizar organización") },
        { OrgUpdateProject, new("Actualizar proyecto") },
        { OrgYourName, new("Su nombre") },
        { Parallel, new("Paralelo") },
        {
            ProjectFileLimitExceeded,
            new("Se excedió el límite de archivos del proyecto {{FileLimit}}")
        },
        { ProjectInvalidDaysPerWeek, new("Los días de la semana deben estar entre 1 y 7") },
        { ProjectInvalidHoursPerDay, new("Las horas por día deben estar entre 1 y 24") },
        { ProjectMemberRoleAdmin, new("Administrador") },
        { ProjectMemberRoleWriter, new("Escritor") },
        { ProjectMemberRoleReader, new("Lector") },
        { ProjectMembers, new("Miembros del proyecto") },
        { Public, new("Público") },
        { Required, new("Requerido") },
        { Sequential, new("Secuencial") },
        { StartOn, new("Comienza en") },
        {
            StringValidation,
            new("Cadena no válida {{Nombre}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
        },
        { SubCounts, new("Conteos secundarios") },
        { Task, new("Tarea") },
        { TaskCantMoveRootProjectNode, new("No se puede mover el nodo del proyecto raíz") },
        {
            TaskConfirmDeleteComment,
            new("<p> ¿Estás seguro de que quieres eliminar este comentario? </p>")
        },
        {
            TaskConfirmDeleteFile,
            new("<p>¿Está seguro de que desea eliminar el archivo <strong>{{Name}}</strong>?</p>")
        },
        {
            TaskConfirmDeleteTask,
            new(
                "<p>¿Está seguro de que desea eliminar la tarea <strong>{{Name}}</strong>?</p><p>Esto no se puede deshacer.</p>"
            )
        },
        {
            TaskConfirmDeleteVitem,
            new("<p>¿Está seguro de que desea eliminar <strong>{{Value}}</strong>?</p>")
        },
        {
            TaskDeleteProjectAttempt,
            new(
                "No se puede eliminar el nodo del proyecto del punto final de tareas, use el punto final de eliminación del proyecto"
            )
        },
        { TaskEditTask, new("Actualizar tarea") },
        {
            TaskMovePrevSibParentMismatch,
            new("Las identificaciones de padres y hermanos anteriores no coinciden")
        },
        { TaskMovingTask, new("Tarea en movimiento: <strong>{{Name}}</strong>") },
        { TaskNewCost, new("Nuevo costo") },
        { TaskNewTask, new("Nueva tarea") },
        { TaskNewTime, new("Nuevo tiempo") },
        { TaskNoCosts, new("Sin Costos") },
        { TaskNoFiles, new("Sin archivos") },
        { TaskNoTimes, new("sin tiempos") },
        {
            TaskRecursiveLoopDetected,
            new("La operación de movimiento daría como resultado un bucle recursivo")
        },
        {
            TaskTooManyDescn,
            new(
                "Demasiados descendientes para obtenerlos todos, solo válido en tareas con 1000 o menos descendientes"
            )
        },
        {
            TaskTooManyDescnToDelete,
            new("No se puede eliminar una tarea con {{Max}} o más tareas descendientes")
        },
        { TaskUpdateCost, new("Costo de actualización") },
        { TaskUpdateTime, new("Tiempo de actualizacion") },
        { TaskUploadFile, new("Subir archivo") },
        { Tasks, new("Tareas") },
        { Time, new("Tiempo") },
        { TimeEst, new("Tiempo estimado") },
        { TimeInc, new("Tiempo incurrido") },
        { TimeMin, new("Tiempo mín.") },
        { True, new("Verdadero") },
        { Unassigned, new("Sin asignar") },
        { Upload, new("Subir") },
        { Uploading, new("Cargando") },
        { User, new("Usuario") },
        { VitemInvalidCostInc, new("La entrada de costo debe ser mayor que 0") },
        { VitemInvalidTimeInc, new("La entrada de tiempo debe ser de 1 a 1440 minutos") },
        { Weeks, new("Semanas") },
        { Years, new("Años") },
    };
}
