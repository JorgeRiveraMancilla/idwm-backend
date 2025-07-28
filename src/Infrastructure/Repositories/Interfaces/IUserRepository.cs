using Microsoft.AspNetCore.Identity;
using Tienda_UCN_api.src.Domain.Models;

namespace Tienda_UCN_api.src.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="id">Id del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un usuario por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Obtiene un usuario por su RUT.
        /// </summary>
        /// <param name="rut">RUT del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        Task<User?> GetByRutAsyncWithoutTracking(string rut);

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="user">Usuario a crear</param>
        /// <returns>Resultado de la creación del usuario</returns>
        Task<IdentityResult> CreateAsync(User user);

        /// <summary>
        /// Cambia la contraseña de un usuario.
        /// </summary>
        /// <param name="user">Usuario al que se le cambiará la contraseña</param
        /// <param name="currentPassword">Contraseña actual del usuario</param>
        /// <param name="newPassword">Nueva contraseña para el usuario</param>
        /// <returns>Resultado de la operación de cambio de contraseña</returns>
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

        /// <summary>
        /// Verifica si la contraseña proporcionada es correcta para el usuario.
        /// </summary>
        /// <param name="user">Usuario al que se le verificará la contraseña</param>
        /// <param name="password">Contraseña a verificar</param>
        /// <returns>True si la contraseña es correcta, false en caso contrario</returns>
        Task<bool> CheckPasswordAsync(User user, string password);

        /// <summary>
        /// Obtiene el rol del usuario.
        /// </summary>
        /// <param name="user">Usuario del cual se desea obtener el rol</param>
        /// <returns>Nombre del rol del usuario</returns>
        Task<string> GetUserRoleAsync(User user);
    }
}
