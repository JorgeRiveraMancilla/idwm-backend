using Microsoft.AspNetCore.Identity;
using Serilog;
using Tienda_UCN_api.src.Application.DTO;
using Tienda_UCN_api.src.Application.Services.Interfaces;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.src.Infrastructure.Repositories.Implements;
using Tienda_UCN_api.src.Infrastructure.Repositories.Interfaces;

namespace Tienda_UCN_api.src.Application.Services.Implements
{
    /// <summary>
    /// Implementación del servicio de usuarios.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public UserService(ITokenService tokenService, UserRepository userRepository)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO que contiene las credenciales del usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el token JWT generado.</returns>
        public async Task<string> LoginAsync(LoginDTO loginDTO, HttpContext httpContext)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "IP desconocida";
            var user = await _userRepository.GetByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                Log.Warning($"Intento de inicio de sesión fallido para el usuario: {loginDTO.Email} desde la IP: {ipAddress}");
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            if (!user.EmailConfirmed)
            {
                Log.Warning($"Intento de inicio de sesión fallido para el usuario: {loginDTO.Email} desde la IP: {ipAddress} - Correo no confirmado.");
                throw new InvalidOperationException("El correo electrónico del usuario no ha sido confirmado.");
            }

            var result = await _userRepository.CheckPasswordAsync(user, loginDTO.Password);
            if (!result)
            {
                Log.Warning($"Intento de inicio de sesión fallido para el usuario: {loginDTO.Email} desde la IP: {ipAddress}");
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            string roleName = await _userRepository.GetUserRoleAsync(user) ?? throw new InvalidOperationException("El usuario no tiene un rol asignado.");

            // Generamos el token
            Log.Information($"Inicio de sesión exitoso para el usuario: {loginDTO.Email} desde la IP: {ipAddress}");
            return _tokenService.GenerateToken(user, roleName, loginDTO.RememberMe);
        }
    }
}
