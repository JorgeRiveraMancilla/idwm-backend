using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda_UCN_api.src.api.Controllers;
using Tienda_UCN_api.src.Application.DTO;
using Tienda_UCN_api.Src.Application.DTO.ProductDTO;
using Tienda_UCN_api.Src.Application.Services.Interfaces;

namespace Tienda_UCN_api.Src.API.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones relacionadas con los productos.
    /// </summary>
    public class ProductController : BaseController
    {
        /// <summary>
        /// Controlador para manejar las operaciones relacionadas con los productos.
        /// </summary>
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Obtiene todos los productos para el administrador con los parámetros de búsqueda especificados.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una lista de productos filtrados para el administrador.</returns>
        [HttpGet("admin/products")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllForAdminAsync([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _productService.GetAllForAdminAsync(searchParams);
            if (result == null || !result.Products.Any()) { throw new KeyNotFoundException("No se encontraron productos."); }
            return Ok(new GenericResponse<ListedProductsForAdminDTO>("Productos obtenidos exitosamente", result));
        }

    }

}
