using Microsoft.EntityFrameworkCore;
using Serilog;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.src.Infrastructure.Data;
using Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces;

namespace Tienda_UCN_api.Src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Repositorio para manejar operaciones del carrito de compras
    /// </summary>
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;

        public CartRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Agrega un artículo al carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Cantidad del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        public Task<Cart> AddItemAsync(string buyerId, string productId, int quantity, string? userId = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Limpia el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        public async Task<Cart> ClearAsync(string buyerId, string? userId = null)
        {
            Cart? cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.UserId == userId);
            if (cart == null)
            {
                Log.Information("El carrito no existe para el comprador especificado {BuyerId}.", buyerId);
                throw new InvalidOperationException("El carrito no existe para el comprador especificado.");
            }
            cart.CartItems.Clear();
            await _context.SaveChangesAsync();
            return cart;
        }

        /// <summary>
        /// Crea un nuevo carrito o devuelve uno existente.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        public async Task<Cart> CreateOrGetAsync(string buyerId, string? userId = null)
        {
            Cart? cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    BuyerId = buyerId,
                    UserId = userId
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }

        /// <summary>
        /// Elimina un artículo del carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        public Task<Cart> RemoveItemAsync(string buyerId, string productId, string? userId = null)
        {
            throw new NotImplementedException();
        }
    }
}
