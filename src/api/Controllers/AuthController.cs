using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Tienda_UCN_api.src.Application.DTO;
using Tienda_UCN_api.src.Application.Services.Interfaces;

namespace Tienda_UCN_api.src.api.Controllers
{
    /// <summary>
    /// Controlador de autenticación.
    /// </summary>
    public class AuthController : BaseController
    {
        /// <summary>
        /// Servicio de usuarios.
        /// </summary>
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO que contiene las credenciales del usuario.</param
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var token = await _userService.LoginAsync(loginDTO, HttpContext);
            return Success(new { Token = token }, "Inicio de sesión exitoso");
        }
    }
}
