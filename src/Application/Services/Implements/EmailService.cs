using Resend;
using Serilog;
using Tienda_UCN_api.Src.Application.Services.Interfaces;

namespace Tienda_UCN_api.Src.Application.Services.Implements
{
    /// <summary>
    /// Servicio para enviar correos electrónicos de verificación.
    /// </summary>
    public class EmailService(
        IResend resend,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment
    ) : IEmailService
    {
        private readonly IResend _resend = resend;
        private readonly IConfiguration _configuration = configuration;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

        /// <summary>
        /// Envía un código de verificación al correo electrónico del usuario.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <param name="code">El código de verificación a enviar.</param>
        public async Task SendVerificationCodeEmailAsync(string email, string code)
        {
            var htmlBody = await LoadTemplate("VerificationCode", code);

            var message = new EmailMessage
            {
                To = email,
                Subject =
                    _configuration["EMAIL_CONFIGURATION:VERIFICATION_SUBJECT"]
                    ?? throw new ArgumentNullException(
                        "El asunto del correo de verificación no puede ser nulo."
                    ),
                From =
                    _configuration["EMAIL_CONFIGURATION:FROM"]
                    ?? throw new ArgumentNullException(
                        "La configuración de 'From' no puede ser nula."
                    ),
                HtmlBody = htmlBody,
            };
            await _resend.EmailSendAsync(message);
        }

        /// <summary>
        /// Envía un correo electrónico de bienvenida al usuario.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        public async Task SendWelcomeEmailAsync(string email)
        {
            var htmlBody = await LoadTemplate("Welcome", null);

            var message = new EmailMessage
            {
                To = email,
                Subject =
                    _configuration["EMAIL_CONFIGURATION:WELCOME_SUBJECT"]
                    ?? throw new ArgumentNullException(
                        "El asunto del correo de bienvenida no puede ser nulo."
                    ),
                From =
                    _configuration["EMAIL_CONFIGURATION:FROM"]
                    ?? throw new ArgumentNullException(
                        "La configuración de 'From' no puede ser nula."
                    ),
                HtmlBody = htmlBody,
            };

            await _resend.EmailSendAsync(message);
        }

        /// <summary>
        /// Carga una plantilla de correo electrónico desde el sistema de archivos y reemplaza el marcador de código.
        /// </summary>
        /// <param name="templateName">El nombre de la plantilla sin extensión.</param>
        /// <param name="code">El código a insertar en la plantilla.</param>
        /// <returns>El contenido HTML de la plantilla con el código reemplazado.</returns>
        private async Task<string> LoadTemplate(string templateName, string? code)
        {
            var fileName = $"{templateName}.html";
            var baseDirectory = AppContext.BaseDirectory;
            var contentRootPath = _hostEnvironment.ContentRootPath ?? baseDirectory;

            Log.Information(
                "Buscando plantilla de email: {TemplateName}. ContentRootPath: {ContentRootPath}, BaseDirectory: {BaseDirectory}",
                templateName,
                contentRootPath,
                baseDirectory
            );

            // Intentar múltiples rutas posibles para compatibilidad entre desarrollo y producción
            var possiblePaths = new[]
            {
                // Ruta con minúsculas (preferida para Linux/Docker)
                Path.Combine(contentRootPath, "src", "Application", "Templates", "Email", fileName),
                // Ruta con mayúsculas (backward compatibility)
                Path.Combine(contentRootPath, "Src", "Application", "Templates", "Email", fileName),
                // Ruta relativa desde BaseDirectory (fallback)
                Path.Combine(baseDirectory, "src", "Application", "Templates", "Email", fileName),
                Path.Combine(baseDirectory, "Src", "Application", "Templates", "Email", fileName),
            };

            string? templatePath = null;
            foreach (var path in possiblePaths)
            {
                Log.Debug("Verificando ruta de plantilla: {Path}", path);
                if (File.Exists(path))
                {
                    templatePath = path;
                    Log.Information("Plantilla encontrada en: {Path}", path);
                    break;
                }
            }

            if (templatePath == null)
            {
                var errorMessage =
                    $"No se pudo encontrar la plantilla de email '{templateName}'. "
                    + $"Rutas intentadas: {string.Join(", ", possiblePaths)}. "
                    + $"ContentRootPath: {contentRootPath}, BaseDirectory: {baseDirectory}";

                Log.Error(errorMessage);
                throw new FileNotFoundException(errorMessage);
            }

            try
            {
                string html;
                using (var stream = File.OpenRead(templatePath))
                using (var reader = new StreamReader(stream))
                {
                    html = await reader.ReadToEndAsync();
                }
                var result = code != null ? html.Replace("{{CODE}}", code) : html;
                Log.Debug("Plantilla '{TemplateName}' cargada exitosamente", templateName);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(
                    ex,
                    "Error al leer la plantilla de email '{TemplateName}' desde: {Path}",
                    templateName,
                    templatePath
                );
                throw;
            }
        }
    }
}
