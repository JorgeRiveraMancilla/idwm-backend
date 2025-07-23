using Tienda_UCN_api.src.Application.DTO;

namespace Tienda_UCN_api.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de usuarios.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO que contiene las credenciales del usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el token JWT generado.</returns>
        Task<string> Login(LoginDTO loginDTO, HttpContext httpContext);
    }
}
