using System.ComponentModel.DataAnnotations;

namespace Tienda_UCN_api.Src.Application.DTO.AuthDTO
{
    public class VerifyEmailDTO
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El código de verificación es obligatorio.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "El código de verificación debe tener 6 dígitos.")]
        public required string VerificationCode { get; set; }
    }
}
