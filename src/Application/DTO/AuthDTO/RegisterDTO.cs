using System.ComponentModel.DataAnnotations;
using Tienda_UCN_api.Src.Application.Validators;

namespace Tienda_UCN_api.Src.Application.DTO.AuthDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ])[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica y contener al menos una mayúscula.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [MaxLength(20, ErrorMessage = "La contraseña debe tener como máximo 20 caracteres")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El Rut es obligatorio.")]
        [RegularExpression(@"^\d{7,8}-[0-9kK]", ErrorMessage = "El Rut no tiene un formato válido.")]
        [RutValidation(ErrorMessage = "El Rut no es válido.")]
        public required string Rut { get; set; }

        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$", ErrorMessage = "El Nombre solo puede contener carácteres del abecedario español.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener mínimo 2 letras.")]
        [MaxLength(20, ErrorMessage = "El nombre debe tener máximo 20 letras.")]
        public required string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s\-]+$", ErrorMessage = "El Apellido solo puede contener carácteres del abecedario español.")]
        [MinLength(2, ErrorMessage = "El apellido debe tener mínimo 2 letras.")]
        [MaxLength(20, ErrorMessage = "El apellido debe tener máximo 20 letras.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [BirthDateValidation]
        public required DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "El número de teléfono debe tener 9 dígitos.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "El género es obligatorio.")]
        [RegularExpression(@"^(Masculino|Femenino|Otro)$", ErrorMessage = "El género debe ser Masculino, Femenino u Otro.")]
        public required string Gender { get; set; }
    }
}
