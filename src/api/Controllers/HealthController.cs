using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda_UCN_api.src.api.Controllers;

namespace Tienda_UCN_api.Src.API.Controllers
{
    /// <summary>
    /// Controlador para health checks y warm-up del servidor.
    /// </summary>
    public class HealthController : BaseController
    {
        /// <summary>
        /// Endpoint de health check para verificar que el servidor está activo.
        /// También se usa para "despertar" el servidor en Render Free Tier (cold starts).
        /// </summary>
        /// <returns>Estado del servidor.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetHealth()
        {
            return Ok(
                new
                {
                    status = "ok",
                    timestamp = DateTimeOffset.UtcNow.ToString("O"),
                    message = "Server is running"
                }
            );
        }
    }
}

