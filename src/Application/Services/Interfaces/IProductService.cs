using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.Src.Application.DTO.ProductDTO;

namespace Tienda_UCN_api.Src.Application.Services.Interfaces
{
    public interface IProductService
    {

        /// <summary>
        /// Retorna todos los productos para el administrador según los parámetros de búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una lista de productos filtrados para el administrador.</returns>
        Task<ListedProductsForAdminDTO> GetAllForAdminAsync(SearchParamsDTO searchParams);
    }
}
