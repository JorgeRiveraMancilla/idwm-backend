using Tienda_UCN_api.src.Domain.Models;

namespace Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio del carrito de compras
    /// </summary>
    public interface ICartRepository
    {
        /// <summary>
        /// Crea un nuevo carrito o devuelve uno existente.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        Task<Cart> CreateOrGetAsync(string buyerId, string? userId = null);

        /// <summary>
        /// Limpia el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        Task<Cart> ClearAsync(string buyerId, string? userId = null);

        /// <summary>
        /// Agrega un artículo al carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Cantidad del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        Task<Cart> AddItemAsync(string buyerId, string productId, int quantity, string? userId = null);

        /// <summary>
        /// Elimina un artículo del carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        Task<Cart> RemoveItemAsync(string buyerId, string productId, string? userId = null);
    }
}
