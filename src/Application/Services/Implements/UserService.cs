using Microsoft.AspNetCore.Identity;
using Tienda_UCN_api.src.Application.DTO;
using Tienda_UCN_api.src.Application.Services.Interfaces;
using Tienda_UCN_api.src.Domain.Models;

namespace Tienda_UCN_api.src.Application.Services.Implements
{
    /// <summary>
    /// Implementación del servicio de usuarios.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;

        public UserService(ITokenService tokenService, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO que contiene las credenciales del usuario.</param
        /// <returns>Un string que representa el token JWT generado.</returns>
        public async Task<string> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email) ?? throw new UnauthorizedAccessException("Credenciales inválidas.");
            string roleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? throw new UnauthorizedAccessException("El usuario no tiene un rol asignado.");

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!result)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            // Generamos el token
            return _tokenService.GenerateToken(user, roleName, loginDTO.RememberMe);
        }
    }
}
