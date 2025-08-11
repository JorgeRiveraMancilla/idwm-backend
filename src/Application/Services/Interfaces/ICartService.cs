using Tienda_UCN_api.Src.Application.DTO.CartDTO;

namespace Tienda_UCN_api.Src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio del carrito de compras
    /// </summary>
    public interface ICartService
    {
        ///<summary>
        /// Agrega un artículo al carrito de compras.
        ///</summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Cantidad del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task AddItemAsync(string buyerId, string productId, int quantity, string? userId = null);

        /// <summary>
        /// Crea un nuevo carrito o devuelve uno existente.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto CartDTO.</returns>
        Task<CartDTO> CreateOrGetAsync(string buyerId, string? userId = null);

        /// <summary>
        /// Elimina un artículo del carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto CartDTO.</returns>
        Task<CartDTO> RemoveItemAsync(string buyerId, string productId, string? userId = null);

        /// <summary>
        /// Limpia el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto CartDTO.</returns>
        Task<CartDTO> ClearAsync(string buyerId, string? userId = null);

    }
}
