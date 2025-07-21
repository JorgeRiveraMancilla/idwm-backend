using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Tienda_UCN_api.src.Domain.Models
{
    public class User : IdentityUser<int>
    {

        /// <summary>
        /// Identificador único del usuario chileno.
        /// </summary>
        public required string Rut { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public required string FirstName { get; set; }

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public required string LastName { get; set; }

        /// <summary>
        /// Género del usuario.
        /// </summary>
        public required string Gender { get; set; }

        /// <summary>
        /// Fecha de nacimiento del usuario.
        /// </summary>
        public required DateTime BirthDate { get; set; }

        /// <summary>
        /// Identificador del rol del usuario.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Rol del usuario.
        /// </summary>
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Fecha de registro del usuario.
        /// </summary>
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de actualización del usuario.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
