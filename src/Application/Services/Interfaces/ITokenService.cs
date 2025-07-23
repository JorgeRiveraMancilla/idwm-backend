using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda_UCN_api.src.Domain.Models;

namespace Tienda_UCN_api.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de generación de tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Genera un token JWT para el usuario proporcionado.
        /// </summary>
        /// <param name="user">El usuario para el cual se generará el token.</param>
        /// <returns>Un string que representa el token JWT generado.</returns>
        string GenerateToken(User user);
    }
}
