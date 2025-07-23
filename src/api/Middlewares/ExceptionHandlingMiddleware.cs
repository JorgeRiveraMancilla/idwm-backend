using System.Security;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Tienda_UCN_api.src.Infrastructure.Middlewares
{
    /// <summary>
    /// Middleware para el manejo de excepciones en la aplicación.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Método que se invoca en cada solicitud HTTP.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pasamos al siguiente middleware en la cadena si no hay excepciones
                await _next(context);
            }
            catch (Exception ex)
            {
                //Capturamos excepciones no controladas y generación de un ID de seguimiento único
                var traceId = Guid.NewGuid().ToString();
                context.Response.Headers["trace-id"] = traceId;

                var (statusCode, title) = MapExceptionToStatus(ex);

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = ex.Message,
                    Instance = context.Request.Path,
                };

                Log.Error(ex, "Excepción no controlada. Trace ID: {TraceId}", traceId);
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = statusCode;

                // Serializamos el objeto ProblemDetails a JSON y lo escribimos en la respuesta
                var json = JsonSerializer.Serialize(problem);
                // Acá se escribe la respuesta al cliente
                await context.Response.WriteAsync(json);
            }
        }


        private (int, string) MapExceptionToStatus(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException _ => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                ArgumentNullException _ => (StatusCodes.Status400BadRequest, "Bad Request"),
                KeyNotFoundException _ => (StatusCodes.Status404NotFound, "Not Found"),
                InvalidOperationException _ => (StatusCodes.Status409Conflict, "Conflict"),
                FormatException _ => (StatusCodes.Status400BadRequest, "Bad Request"),
                SecurityException _ => (StatusCodes.Status403Forbidden, "Forbidden"),
                ArgumentException _ => (StatusCodes.Status400BadRequest, "Bad Request"),
                JsonException _ => (StatusCodes.Status400BadRequest, "Bad Request"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };
        }
    }
}
