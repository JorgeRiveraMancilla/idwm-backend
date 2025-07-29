using Microsoft.EntityFrameworkCore;
using Tienda_UCN_api.src.Infrastructure.Data;
using Tienda_UCN_api.Src.Domain.Models;
using Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces;

namespace Tienda_UCN_api.Src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Implementación del repositorio de códigos de verificación.
    /// </summary>
    public class VerificationCodeRepository : IVerificationCodeRepository
    {
        private readonly DataContext _context;

        public VerificationCodeRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Crea un nuevo código de verificación.
        /// </summary>
        /// <param name="verificationCode">El código de verificación a crear.</param>
        /// <returns>El código de verificación creado.</returns>
        public async Task<VerificationCode> CreateVerificationCodeAsync(VerificationCode verificationCode)
        {
            var existingCode = await _context.VerificationCodes.AnyAsync(vc => vc.Code == verificationCode.Code && vc.UserId == verificationCode.UserId);
            if (existingCode)
            {
                throw new InvalidOperationException("Ya existe un código de verificación con el mismo código y usuario.");
            }
            else
            {
                await _context.VerificationCodes.AddAsync(verificationCode);
                await _context.SaveChangesAsync();
                return verificationCode;
            }
        }
    }
}
