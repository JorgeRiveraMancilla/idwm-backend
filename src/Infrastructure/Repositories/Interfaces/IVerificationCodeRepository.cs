using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda_UCN_api.Src.Domain.Models;

namespace Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de códigos de verificación.
    /// </summary>
    public interface IVerificationCodeRepository
    {
        /// <summary>
        /// Crea un nuevo código de verificación.
        /// </summary>
        /// <param name="verificationCode">El código de verificación a crear.</param>
        /// <returns>El código de verificación creado.</returns>
        Task<VerificationCode> CreateVerificationCodeAsync(VerificationCode verificationCode);
    }
}
