using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.Src.Application.DTO.ProductDTO;

namespace Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de productos, que define los métodos para interactuar con los datos de productos.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Retorna una lista de productos para el administrador con los parámetros de búsqueda especificados.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con una lista de productos para el administrador y el conteo total de productos.</returns>
        Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForAdminAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Retorna una lista de productos para el cliente con los parámetros de búsqueda especificados.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con una lista de productos para el cliente y el conteo total de productos.</returns>
        Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForCustomerAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Retorna un producto específico por su ID.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
        Task<Product?> GetByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo producto en el repositorio.
        /// </summary>
        /// <param name="product">El producto a crear.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el id del producto creado.</returns>
        Task<int> CreateAsync(Product product);

        /// <summary>
        /// Crea o obtiene una categoría por su nombre.
        /// </summary>
        /// <param name="categoryName">El nombre de la categoría.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con la categoría creada o encontrada.</returns>
        Task<Category> CreateOrGetCategoryAsync(string categoryName);

        /// <summary>
        /// Crea o obtiene una marca por su nombre.
        /// </summary>
        /// <param name="brandName">El nombre de la marca.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con la marca creada o encontrada.</returns>
        Task<Brand> CreateOrGetBrandAsync(string brandName);
    }
}
