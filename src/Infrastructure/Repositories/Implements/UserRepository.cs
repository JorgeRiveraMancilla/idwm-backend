using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.src.Infrastructure.Data;
using Tienda_UCN_api.src.Infrastructure.Repositories.Interfaces;

namespace Tienda_UCN_api.src.Infrastructure.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        public UserRepository(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Cambia la contraseña de un usuario.
        /// </summary>
        /// <param name="user">Usuario al que se le cambiará la contraseña</param
        /// <param name="currentPassword">Contraseña actual del usuario</param>
        /// <param name="newPassword">Nueva contraseña para el usuario</param>
        /// <returns>Resultado de la operación de cambio de contraseña</returns>
        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        /// <summary>
        /// Verifica si la contraseña proporcionada es correcta para el usuario.
        /// </summary>
        /// <param name="user">Usuario al que se le verificará la contraseña</param>
        /// <param name="password">Contraseña a verificar</param>
        /// <returns>True si la contraseña es correcta, false en caso contrario</returns>
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="user">Usuario a crear</param>
        /// <returns>Resultado de la creación del usuario</returns>
        public async Task<IdentityResult> CreateAsync(User user)
        {
            return await _userManager.CreateAsync(user);
        }

        /// <summary>
        /// Obtiene un usuario por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="id">Id del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        /// <summary>
        /// Obtiene un usuario por su RUT.
        /// </summary>
        /// <param name="rut">RUT del usuario</param>
        /// <returns>Usuario encontrado o nulo</returns>
        public async Task<User?> GetByRutAsyncWithoutTracking(string rut)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Rut == rut);
        }

        /// <summary>
        /// Obtiene el rol del usuario.
        /// </summary>
        /// <param name="user">Usuario del cual se desea obtener el rol</param>
        /// <returns>Nombre del rol del usuario</returns>
        public async Task<string> GetUserRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault()!;
        }
    }
}
