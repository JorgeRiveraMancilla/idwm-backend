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
        Task<Cart> CreateOrGetAsync(string buyerId, int? userId = null);

        /// <summary>
        /// Limpia el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        Task<Cart> ClearAsync(string buyerId, int? userId = null);

        /// <summary>
        /// Agrega un artículo al carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Cantidad del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        Task<Cart> AddItemAsync(string buyerId, int productId, int quantity, int? userId = null);

        /// <summary>
        /// Elimina un artículo del carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        Task<Cart> RemoveItemAsync(string buyerId, int productId, int? userId = null);

        /// <summary>
        /// Asocia un carrito de compras con un usuario.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task AssociateWithUserAsync(string buyerId, int userId);

        /// <summary>
        /// Actualiza la cantidad de un artículo en el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Nueva cantidad del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        Task<Cart> UpdateItemQuantityAsync(string buyerId, int productId, int quantity, int? userId = null);
    }
}
