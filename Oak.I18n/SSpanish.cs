using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly IReadOnlyDictionary<string, TemplatableString> Spanish = new Dictionary<
        string,
        TemplatableString
    >()
    {
        { HomeHeader, new("¡Bienvenido a Roble!") },
        {
            HomeBody,
            new(
                "Esta es una gestión de proyectos en la que organiza tareas en una estructura de árbol como un directorio de archivos. Cada tarea brinda un resumen de las estadísticas vitales de las tareas debajo de ella, tiempo y costos estimados e incurridos, etc."
            )
        },
        { Invalid, new("Inválido") },
        { RpcUnknownEndpoint, new("Extremo de RPC desconocido") },
        { UnexpectedError, new("Ocurrió un error inesperado") },
        { EntityNotFound, new("{{Name}} no encontrada") },
        { InsufficientPermission, new("Permiso insuficiente") },
        { ApiError, new("Error de API") },
        { MinMaxNullArgs, new("Los argumentos min y max son nulos") },
        { MinMaxReversedArgs, new("Los valores Min {{Min}} y Max {{Max}} están fuera de orden") },
        { BadRequest, new("Solicitud incorrecta") },
        { AuthInvalidEmail, new("Email inválido") },
        { AuthInvalidPwd, new("Contraseña invalida") },
        { LessThan8Chars, new("Menos de 8 caracteres") },
        { NoLowerCaseChar, new("Sin carácter en minúsculas") },
        { NoUpperCaseChar, new("Sin carácter en mayúscula") },
        { NoDigit, new("Sin dígito") },
        { NoSpecialChar, new("Sin carácter especial") },
        { AuthAlreadyAuthenticated, new("Ya en sesión autenticada") },
        { AuthNotAuthenticated, new("No en sesión autenticada") },
        { AuthInvalidEmailCode, new("Código de correo electrónico no válido") },
        { AuthInvalidResetPwdCode, new("Código de restablecimiento de contraseña no válido") },
        {
            AuthAccountNotVerified,
            new(
                "Cuenta no verificada, revise sus correos electrónicos para ver el enlace de verificación"
            )
        },
        {
            AuthAttemptRateLimit,
            new(
                "Los intentos de autenticación no se pueden realizar con más frecuencia que cada {{Seconds}} segundos"
            )
        },
        { AuthConfirmEmailSubject, new("Confirmar el correo") },
        {
            AuthConfirmEmailHtml,
            new(
                "<div><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Haga clic en este enlace para verificar su dirección de correo electrónico</a></div>"
            )
        },
        {
            AuthConfirmEmailText,
            new(
                "Utilice este enlace para verificar su dirección de correo electrónico: {{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
            )
        },
        { AuthResetPwdSubject, new("Restablecer la contraseña") },
        {
            AuthResetPwdHtml,
            new(
                "<div><a href=\"{{BaseHref}}/reset_pwd?email={{Email}}&code={{Code}}\">Haga clic en este enlace para restablecer su contraseña</a></div>"
            )
        },
        {
            AuthResetPwdText,
            new(
                "Haga clic en este enlace para restablecer su contraseña: {{BaseHref}}/reset_pwd?email={{Email}}&code={{Code}}"
            )
        },
        { AuthFcmTopicInvalid, new("Tema de Fcm no válido Min: {{Min}}, Max: {{Max}}") },
        { AuthFcmTokenInvalid, new("Token de Fcm no válido") },
        { AuthFcmNotEnabled, new("Fcm no habilitado") },
        { Home, new("Hogar") },
        { L10n, new("Localización") },
        { ToggleLiveUpdates, new("Alternar actualizaciones en vivo") },
        { Live, new("Vivo:") },
        { On, new("En") },
        { Off, new("De") },
        { Or, new("o") },
        { Language, new("Idioma") },
        { DateFmt, new("Formato de fecha") },
        { TimeFmt, new("Formato de tiempo") },
        { Register, new("Registro") },
        { Registering, new("Registrarse") },
        {
            RegisterSuccess,
            new(
                "Revise sus correos electrónicos para obtener un enlace de confirmación para completar el registro."
            )
        },
        { SignIn, new("Iniciar sesión") },
        { RememberMe, new("Acuérdate de mí") },
        { SigningIn, new("Iniciando sesión") },
        { SignOut, new("Desconectar") },
        { SigningOut, new("Cerrando sesión") },
        { VerifyEmail, new("Verificar correo electrónico") },
        { Verifying, new("Verificando") },
        { VerifyingEmail, new("Verificando tu correo electrónico") },
        { VerifyEmailSuccess, new("Gracias por verificar tu e-mail.") },
        { ResetPwd, new("Restablecer la contraseña") },
        { Email, new("Correo electrónico") },
        { Pwd, new("Contraseña") },
        { ConfirmPwd, new("Confirmar Contraseña") },
        { PwdsDontMatch, new("Las contraseñas no coinciden") },
        { ResetPwdSuccess, new("Ahora puede iniciar sesión con su nueva contraseña.") },
        { Resetting, new("Restablecer") },
        { SendResetPwdLink, new("Enviar enlace de restablecimiento de contraseña") },
        {
            SendResetPwdLinkSuccess,
            new(
                "Si este correo electrónico coincide con una cuenta, se habrá enviado un correo electrónico para restablecer su contraseña."
            )
        },
        { Processing, new("Procesando") },
        { Send, new("Enviar") },
        { Success, new("Éxito") },
        { Error, new("Error") },
        { Update, new("Actualizar") },
        { Updating, new("Actualizando") },
        {
            StringValidation,
            new("Cadena no válida {{Nombre}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
        },
        { OrgTooMany, new("Ya eres miembro de demasiadas organizaciones") },
        { ProjectInvalidHoursPerDay, new("Las horas por día deben estar entre 1 y 24") },
        { ProjectInvalidDaysPerWeek, new("Los días de la semana deben estar entre 1 y 7") },
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
        { OrgMyOrgs, new("Mis organizaciones") },
        { OrgNoOrgs, new("Sin organizaciones") },
        { OrgNewOrg, new("Nueva organización") },
        { OrgName, new("nombre de la organización") },
        { OrgYourName, new("Su nombre") },
        { OrgProjects, new("Proyectos") },
        { OrgNoProjects, new("Sin Proyectos") },
        { OrgNewProject, new("Nuevo proyecto") },
        { TaskNewTask, new("Nueva tarea") },
        { TaskUpdateTask, new("Actualizar tarea") },
        {
            TaskConfirmDeleteTask,
            new(
                "<p>¿Está seguro de que desea eliminar la tarea <strong>{{Name}}</strong>?</p><p>Esto no se puede deshacer.</p>"
            )
        }
    };
}
