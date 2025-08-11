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
        /// <returns>Tarea que representa la operación asincrónica que retorna un objeto del tipo CartDTO.</returns>
        public async Task<CartDTO> AddItemAsync(string buyerId, int productId, int quantity, int? userId = null)
        {
            Cart cart = await _cartRepository.AddItemAsync(buyerId, productId, quantity, userId);
            return cart.Adapt<CartDTO>();
        }

        /// <summary>
        /// Asocia el carrito de compras con un usuario.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task AssociateWithUserAsync(string buyerId, int userId)
        {
            await _cartRepository.AssociateWithUserAsync(buyerId, userId);
        }

        /// <summary>
        /// Limpia el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto CartDTO.</returns>
        public async Task<CartDTO> ClearAsync(string buyerId, int? userId = null)
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
        public async Task<CartDTO> CreateOrGetAsync(string buyerId, int? userId = null)
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
        public async Task<CartDTO> RemoveItemAsync(string buyerId, int productId, int? userId = null)
        {
            Cart cart = await _cartRepository.RemoveItemAsync(buyerId, productId, userId);
            return cart.Adapt<CartDTO>();
        }

        /// <summary>
        /// Actualiza la cantidad de un artículo en el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Nueva cantidad del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto CartDTO.</returns>
        public async Task<CartDTO> UpdateItemQuantityAsync(string buyerId, int productId, int quantity, int? userId = null)
        {
            Cart cart = await _cartRepository.UpdateItemQuantityAsync(buyerId, productId, quantity, userId);
            return cart.Adapt<CartDTO>();
        }
    }
}
