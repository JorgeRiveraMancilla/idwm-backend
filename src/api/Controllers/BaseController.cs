using Microsoft.AspNetCore.Mvc;

namespace Tienda_UCN_api.src.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Crea una respuesta exitosa estándar.
        /// </summary>
        protected IActionResult Success<T>(T data, string message = "Operación exitosa")
        {
            return Ok(new
            {
                message,
                data
            });
        }

        /// <summary>
        /// Crea una respuesta de recurso creado.
        /// </summary>
        protected IActionResult Created<T>(T data, string message = "Recurso creado exitosamente")
        {
            return StatusCode(201, new
            {
                message,
                data
            });
        }

    }
}
