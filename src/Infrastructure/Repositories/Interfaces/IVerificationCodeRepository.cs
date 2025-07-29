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

        /// <summary>
        /// Obtiene el último código de verificación por ID de usuario y tipo de código.
        /// </summary>
        /// <param name="userId">El ID del usuario.</param>
        /// <param name="codeType">El tipo de código de verificación.</param>
        /// <returns>El último código de verificación encontrado, o null si no existe.</returns>
        Task<VerificationCode?> GetLatestVerificationCodeByUserIdAsync(int userId, CodeType codeType);

        /// <summary>
        /// Aumenta el contador de intentos de un código de verificación.
        /// </summary>
        /// <param name="userId">El ID del usuario.</param>
        /// <param name="codeType">El tipo de código de verificación.</param>
        /// <returns>El número de intentos incrementados.</returns>
        Task<int> IncreaseVerificationCodeAttemptsAsync(int userId, CodeType codeType);

        /// <summary>
        /// Elimina un código de verificación por ID de usuario y tipo de código.
        /// </summary>
        /// <param name="id">El ID del usuario.</param>
        /// <param name="codeType">El tipo de código de verificación.</param>
        /// <returns>True si se eliminó correctamente, false si no existía.</returns
        Task<bool> DeleteVerificationCodeByUserIdAsync(int id, CodeType codeType);
    }
}
