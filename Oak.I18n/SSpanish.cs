using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly IReadOnlyDictionary<string, TemplatableString> Spanish = new Dictionary<
        string,
        TemplatableString
    >()
    {
        {
            Oak,
            new(
                "<h1>¡Hola, Oak!</h1><p>Bienvenido a su nuevo kit de inicio de dotnet.</p><p>Encontrará:</p><ul><li>Cliente: una aplicación blazor wasm usando biblioteca radzen ui</li><li>Servidor: aspnet con grpc api e interfaz db del marco de la entidad</li></ul>"
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
        { OrgTooMany, new("Ya eres miembro de demasiadas organizaciones") }
    };
}
