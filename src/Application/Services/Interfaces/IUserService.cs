using Tienda_UCN_api.src.Application.DTO;
using Tienda_UCN_api.Src.Application.DTO.AuthDTO;

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
        Task<string> LoginAsync(LoginDTO loginDTO, HttpContext httpContext);

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="registerDTO">DTO que contiene la información del nuevo usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el mensaje de éxito del registro.</returns>
        Task<string> RegisterAsync(RegisterDTO registerDTO, HttpContext httpContext);
    }
}
