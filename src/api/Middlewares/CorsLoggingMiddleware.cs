using Serilog;

namespace Tienda_UCN_api.Src.API.Middlewares
{
    /// <summary>
    /// Middleware para logging detallado de peticiones CORS, especialmente preflight (OPTIONS)
    /// </summary>
    public class CorsLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public CorsLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var isPreflight = request.Method == "OPTIONS";

            // Logging detallado de la petición
            Log.Information(
                "=== CORS Request Debug ==="
                    + "\nMethod: {Method}"
                    + "\nPath: {Path}"
                    + "\nIsPreflight: {IsPreflight}"
                    + "\nOrigin: {Origin}"
                    + "\nAccess-Control-Request-Method: {RequestMethod}"
                    + "\nAccess-Control-Request-Headers: {RequestHeaders}"
                    + "\nAuthorization Header Present: {HasAuth}"
                    + "\nContent-Type: {ContentType}",
                request.Method,
                request.Path,
                isPreflight,
                request.Headers["Origin"].ToString(),
                request.Headers["Access-Control-Request-Method"].ToString(),
                request.Headers["Access-Control-Request-Headers"].ToString(),
                !string.IsNullOrEmpty(request.Headers["Authorization"].ToString()),
                request.ContentType
            );

            // Logging de todos los headers relevantes
            var relevantHeaders = new[]
            {
                "Origin",
                "Access-Control-Request-Method",
                "Access-Control-Request-Headers",
                "Authorization",
                "Content-Type",
                "Accept",
                "Referer",
                "User-Agent",
            };

            Log.Debug("Relevant Headers:");
            foreach (var headerName in relevantHeaders)
            {
                var headerValue = request.Headers[headerName].ToString();
                if (!string.IsNullOrEmpty(headerValue))
                {
                    Log.Debug("  {HeaderName}: {HeaderValue}", headerName, headerValue);
                }
            }

            // Continuar con el siguiente middleware
            await _next(context);

            // Logging de la respuesta
            var response = context.Response;
            Log.Information(
                "=== CORS Response Debug ==="
                    + "\nStatus Code: {StatusCode}"
                    + "\nAccess-Control-Allow-Origin: {AllowOrigin}"
                    + "\nAccess-Control-Allow-Credentials: {AllowCredentials}"
                    + "\nAccess-Control-Allow-Methods: {AllowMethods}"
                    + "\nAccess-Control-Allow-Headers: {AllowHeaders}"
                    + "\nAccess-Control-Expose-Headers: {ExposeHeaders}",
                response.StatusCode,
                response.Headers["Access-Control-Allow-Origin"].ToString(),
                response.Headers["Access-Control-Allow-Credentials"].ToString(),
                response.Headers["Access-Control-Allow-Methods"].ToString(),
                response.Headers["Access-Control-Allow-Headers"].ToString(),
                response.Headers["Access-Control-Expose-Headers"].ToString()
            );
        }
    }
}
