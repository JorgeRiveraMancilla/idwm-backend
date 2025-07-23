using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Tienda_UCN_api.src.Application.Services.Interfaces;
using Tienda_UCN_api.src.Domain.Models;

namespace Tienda_UCN_api.src.Application.Services.Implements
{
    /// <summary>
    /// Implementación del servicio de generación de tokens JWT.
    /// </summary>
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration), "La configuración no puede ser nula");
        }

        /// <summary>
        /// Genera un token JWT para el usuario proporcionado.
        /// </summary>
        /// <param name="user">El usuario para el cual se generará el token.</param>
        /// <returns>Un string que representa el token JWT generado.</returns>
        public string GenerateToken(User user)
        {

            try
            {
                // Listamos los claims que queremos incluir en el token (solo las necesarias, no todas las propiedades del usuario)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email ?? throw new ArgumentNullException(nameof(user.Email), "El email del usuario no puede ser nulo")),
                    new Claim(ClaimTypes.Role, user.Role?.Name ?? throw new ArgumentNullException(nameof(user.Role), "El rol del usuario no puede ser nulo"))
                };

                // Extraemos la clave desde la configuración de appsettings
                string jwtSecret = _configuration["JWTSecret"] ?? throw new InvalidOperationException("La clave secreta JWT no está configurada.");

                // Creamos la clave de seguridad
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret));

                // Creamos las credenciales de firma, ojo la clave debe ser lo suficientemente larga y segura (256 bits mínimo para HMACSHA256)
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Creamos el token
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(24),
                    signingCredentials: creds
                );

                // Serializamos el token a string
                Log.Information("Token JWT generado exitosamente para el usuario {UserId}", user.Id);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al generar el token JWT para el usuario {UserId}", user.Id);
                throw new InvalidOperationException("Error al generar el token JWT", ex);
            }

        }
    }
}
