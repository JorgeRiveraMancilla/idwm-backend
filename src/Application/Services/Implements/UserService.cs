using Microsoft.AspNetCore.Identity;
using Serilog;
using Tienda_UCN_api.src.Application.DTO;
using Tienda_UCN_api.src.Application.Services.Interfaces;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.src.Infrastructure.Repositories.Implements;
using Tienda_UCN_api.src.Infrastructure.Repositories.Interfaces;
using Tienda_UCN_api.Src.Application.DTO.AuthDTO;
using Tienda_UCN_api.Src.Application.Services.Interfaces;
using Tienda_UCN_api.Src.Domain.Models;
using Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces;

namespace Tienda_UCN_api.src.Application.Services.Implements
{
    /// <summary>
    /// Implementación del servicio de usuarios.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeRepository _verificationCodeRepository;

        public UserService(ITokenService tokenService, IUserRepository userRepository, IEmailService emailService, IVerificationCodeRepository verificationCodeRepository)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _emailService = emailService;
            _verificationCodeRepository = verificationCodeRepository;
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

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="registerDTO">DTO que contiene la información del nuevo usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el mensaje de éxito del registro.</returns>
        public async Task<string> RegisterAsync(RegisterDTO registerDTO, HttpContext httpContext)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "IP desconocida";
            Log.Information($"Intento de registro de nuevo usuario: {registerDTO.Email} desde la IP: {ipAddress}");

            bool isRegistered = await _userRepository.ExistsByEmailAsync(registerDTO.Email);
            if (isRegistered)
            {
                Log.Warning($"El usuario con el correo {registerDTO.Email} ya está registrado.");
                throw new InvalidOperationException("El usuario ya está registrado.");
            }
            isRegistered = await _userRepository.ExistsByRutAsync(registerDTO.Rut);
            if (isRegistered)
            {
                Log.Warning($"El usuario con el RUT {registerDTO.Rut} ya está registrado.");
                throw new InvalidOperationException("El RUT ya está registrado.");
            }
            var user = new User
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                Rut = registerDTO.Rut,
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                BirthDate = registerDTO.BirthDate,
                PhoneNumber = registerDTO.PhoneNumber,
                Gender = (Gender)Enum.Parse(typeof(Gender), registerDTO.Gender)
            };
            var result = await _userRepository.CreateAsync(user, registerDTO.Password);
            if (!result)
            {
                Log.Warning($"Error al registrar el usuario: {registerDTO.Email}");
                throw new Exception("Error al registrar el usuario.");
            }
            Log.Information($"Registro exitoso para el usuario: {registerDTO.Email} desde la IP: {ipAddress}");
            string code = new Random().Next(100000, 999999).ToString();
            var verificationCode = new VerificationCode
            {
                UserId = user.Id,
                Code = code,
                CodeType = CodeType.EmailVerification,
            };
            var createdVerificationCode = await _verificationCodeRepository.CreateVerificationCodeAsync(verificationCode);
            Log.Information($"Código de verificación generado para el usuario: {registerDTO.Email} - Código: {createdVerificationCode.Code}");

            await _emailService.SendVerificationCodeEmailAsync(registerDTO.Email, createdVerificationCode.Code);
            Log.Information($"Se ha enviado un código de verificación al correo electrónico: {registerDTO.Email}");
            return "Se ha enviado un código de verificación a su correo electrónico.";
        }

        /// <summary>
        /// Verifica el correo electrónico del usuario.
        /// </summary>
        /// <param name="verifyEmailDTO">DTO que contiene el correo electrónico y el código de verificación.</param>
        /// <returns>Un string que representa el mensaje de éxito de la verificación.</returns>
        public async Task<string> VerifyEmailAsync(VerifyEmailDTO verifyEmailDTO)
        {
            User? user = await _userRepository.GetByEmailAsync(verifyEmailDTO.Email);
            if (user == null)
            {
                Log.Warning($"El usuario con el correo {verifyEmailDTO.Email} no existe.");
                throw new KeyNotFoundException("El usuario no existe.");
            }
            if (user.EmailConfirmed)
            {
                Log.Warning($"El usuario con el correo {verifyEmailDTO.Email} ya ha verificado su correo electrónico.");
                throw new InvalidOperationException("El correo electrónico ya ha sido verificado.");
            }
            CodeType codeType = CodeType.EmailVerification;

            VerificationCode? verificationCode = await _verificationCodeRepository.GetLatestVerificationCodeByUserIdAsync(user.Id, codeType);
            if (verificationCode == null)
            {
                Log.Warning($"No se encontró un código de verificación para el usuario: {verifyEmailDTO.Email}");
                throw new KeyNotFoundException("El código de verificación no existe.");
            }
            if (verificationCode.Code != verifyEmailDTO.VerificationCode || DateTime.UtcNow >= verificationCode.ExpiryDate)
            {
                int attempsCountUpdated = await _verificationCodeRepository.IncreaseVerificationCodeAttemptsAsync(user.Id, codeType);
                Log.Warning($"Código de verificación incorrecto o expirado para el usuario: {verifyEmailDTO.Email}. Intentos actuales: {attempsCountUpdated}");
                if (attempsCountUpdated >= 5)
                {
                    Log.Warning($"Se ha alcanzado el límite de intentos para el usuario: {verifyEmailDTO.Email}");
                    bool codeDeleteResult = await _verificationCodeRepository.DeleteVerificationCodeByUserIdAsync(user.Id, codeType);
                    if (codeDeleteResult)
                    {
                        Log.Warning($"Se ha eliminado el código de verificación para el usuario: {verifyEmailDTO.Email}");
                        bool userDeleteResult = await _userRepository.DeleteUserAsync(user.Id);
                        if (userDeleteResult)
                        {
                            Log.Warning($"Se ha eliminado el usuario: {verifyEmailDTO.Email}");
                            throw new ArgumentException("Se ha alcanzado el límite de intentos. El usuario ha sido eliminado.");
                        }
                    }
                }
                if (DateTime.UtcNow >= verificationCode.ExpiryDate)
                {
                    Log.Warning($"El código de verificación ha expirado para el usuario: {verifyEmailDTO.Email}");
                    throw new ArgumentException("El código de verificación ha expirado.");
                }
                else
                {
                    Log.Warning($"El código de verificación es incorrecto para el usuario: {verifyEmailDTO.Email}");
                    throw new ArgumentException($"El código de verificación es incorrecto, quedan {5 - attempsCountUpdated} intentos.");
                }
            }
            bool emailConfirmed = await _userRepository.ConfirmEmailAsync(user.Email!);
            if (emailConfirmed)
            {
                bool codeDeleteResult = await _verificationCodeRepository.DeleteVerificationCodeByUserIdAsync(user.Id, codeType);
                if (codeDeleteResult)
                {
                    Log.Warning($"Se ha eliminado el código de verificación para el usuario: {verifyEmailDTO.Email}");
                    await _emailService.SendWelcomeEmailAsync(user.Email!);
                    Log.Information($"El correo electrónico del usuario {verifyEmailDTO.Email} ha sido confirmado exitosamente.");
                    return "!Ya puedes iniciar sesión y disfrutar de todos los beneficios de Tienda UCN!";
                }
                throw new Exception("Error al confirmar el correo electrónico.");
            }
            throw new Exception("Error al verificar el correo electrónico.");
        }
    }
}
