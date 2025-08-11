using Mapster;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.Src.Application.DTO.CartDTO;
using Tienda_UCN_api.Src.Application.Services.Interfaces;
using Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces;

namespace Tienda_UCN_api.Src.Application.Services.Implements
{
    /// <summary>
    /// Servicio para manejar operaciones del carrito de compras
    /// </summary>
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        ///<summary>
        /// Agrega un artículo al carrito de compras.
        ///</summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Cantidad del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public Task AddItemAsync(string buyerId, string productId, int quantity, string? userId = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Limpia el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto CartDTO.</returns>
        public async Task<CartDTO> ClearAsync(string buyerId, string? userId = null)
        {
            Cart cart = await _cartRepository.ClearAsync(buyerId, userId);
            return cart.Adapt<CartDTO>();
        }

        /// <summary>
        /// Crea un nuevo carrito o devuelve uno existente.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto CartDTO.</returns>
        public async Task<CartDTO> CreateOrGetAsync(string buyerId, string? userId = null)
        {
            Cart cart = await _cartRepository.CreateOrGetAsync(buyerId, userId);
            return cart.Adapt<CartDTO>();
        }

        /// <summary>
        /// Elimina un artículo del carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto CartDTO.</returns>
        public Task<CartDTO> RemoveItemAsync(string buyerId, string productId, string? userId = null)
        {
            throw new NotImplementedException();
        }
    }
}
