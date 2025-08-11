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
        private readonly IProductRepository _productRepository;

        public CartRepository(DataContext dataContext, IProductRepository productRepository)
        {
            _context = dataContext;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Agrega un artículo al carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Cantidad del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        public async Task<Cart> AddItemAsync(string buyerId, int productId, int quantity, int? userId = null)
        {
            Cart? cart = await FindAsync(buyerId, userId);
            Product? product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                Log.Information("El producto con ID {ProductId} no existe.", productId);
                throw new KeyNotFoundException("El producto no existe.");
            }
            if (product.Stock < quantity)
            {
                Log.Information("El producto con ID {ProductId} no tiene suficiente stock.", productId);
                throw new KeyNotFoundException("No hay suficiente stock del producto.");
            }
            if (cart == null)
            {
                Log.Information("Creando nuevo carrito para buyerId: {BuyerId}", buyerId);
                cart = new Cart
                {
                    BuyerId = buyerId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.Images)
                    .FirstAsync(c => c.Id == cart.Id);
            }
            var existingProduct = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingProduct != null)
            {
                existingProduct.Quantity += quantity;
                Log.Information("Actualizando cantidad del producto {ProductId} en el carrito.", productId);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    ProductId = product.Id,
                    Product = product,
                    CartId = cart.Id,
                    Quantity = quantity,
                };

                cart.CartItems.Add(newCartItem);
                Log.Information("Nuevo producto agregado al carrito. ProductId: {ProductId}, Cantidad: {Quantity}", productId, quantity);
            }
            RecalculateCartTotals(cart);
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            Log.Information("Carrito guardado exitosamente. CartId: {CartId}", cart.Id);

            return cart;
        }

        /// <summary>
        /// Limpia el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        public async Task<Cart> ClearAsync(string buyerId, int? userId = null)
        {
            Cart? cart = await FindAsync(buyerId, userId);
            if (cart == null)
            {
                Log.Information("El carrito no existe para el comprador especificado {BuyerId}.", buyerId);
                throw new KeyNotFoundException("El carrito no existe para el comprador especificado.");
            }
            cart.CartItems.Clear();
            RecalculateCartTotals(cart);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            Log.Information("Carrito limpiado exitosamente. CartId: {CartId}", cart.Id);
            return cart;
        }

        /// <summary>
        /// Crea un nuevo carrito o devuelve uno existente.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        public async Task<Cart> CreateOrGetAsync(string buyerId, int? userId = null)
        {
            Cart? cart = await FindAsync(buyerId, userId);

            if (cart == null)
            {
                if (userId.HasValue)
                {
                    var existingUserCart = await _context.Carts
                        .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                                .ThenInclude(p => p.Images)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    if (existingUserCart != null)
                    {
                        Log.Information("Se encontró carrito existente del usuario durante CreateOrGet. UserId: {UserId}", userId);
                        return existingUserCart;
                    }
                }

                cart = new Cart
                {
                    BuyerId = buyerId,
                    UserId = userId,
                    SubTotal = 0,
                    Total = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.Images).FirstAsync(c => c.Id == cart.Id);

                Log.Information("Nuevo carrito creado para buyerId: {BuyerId}, userId: {UserId}", buyerId, userId);
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
        public async Task<Cart> RemoveItemAsync(string buyerId, int productId, int? userId = null)
        {
            Cart? cart = await FindAsync(buyerId, userId);
            if (cart == null)
            {
                Log.Information("El carrito no existe para el comprador especificado {BuyerId}.", buyerId);
                throw new KeyNotFoundException("El carrito no existe para el comprador especificado.");
            }

            CartItem? itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToRemove == null)
            {
                Log.Information("El artículo no existe en el carrito para el comprador especificado {BuyerId}.", buyerId);
                throw new KeyNotFoundException("El artículo no existe en el carrito.");
            }

            cart.CartItems.Remove(itemToRemove); //eliminamos de la collection de cart
            _context.CartItems.Remove(itemToRemove); //eliminamos de la base de datos
            RecalculateCartTotals(cart);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return cart;
        }

        /// <summary>
        /// Asocia un carrito de compras con un usuario.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task AssociateWithUserAsync(string buyerId, int userId)
        {
            Cart? cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.Images).FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.UserId == null);
            if (cart == null)
            {
                Log.Information("No hay carrito para asociar con buyerId: {BuyerId}", buyerId);
                return;
            }
            var existingUserCart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == userId);

            if (existingUserCart != null)
            {
                foreach (var anonymousItem in cart.CartItems.ToList())
                {
                    var existingItem = existingUserCart.CartItems.FirstOrDefault(i => i.ProductId == anonymousItem.ProductId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity += anonymousItem.Quantity;
                    }
                    else
                    {
                        anonymousItem.CartId = existingUserCart.Id;
                        existingUserCart.CartItems.Add(anonymousItem);
                    }
                }

                RecalculateCartTotals(existingUserCart);
                existingUserCart.UpdatedAt = DateTime.UtcNow;

                _context.Carts.Remove(cart);

                Log.Information("Carritos fusionados. Carrito anónimo {AnonymousCartId} → Carrito usuario {UserCartId}", cart.Id, existingUserCart.Id);
            }
            else
            {
                cart.UserId = userId;
                cart.UpdatedAt = DateTime.UtcNow;
                Log.Information("Carrito anónimo asociado con usuario. BuyerId: {BuyerId} → UserId: {UserId}", buyerId, userId);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza la cantidad de un artículo en el carrito de compras.
        /// </summary>
        /// <param name="buyerId">ID del comprador.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Nueva cantidad del producto.</param>
        /// <param name="userId">ID del usuario si es que está autenticado</param>
        /// <returns>Tarea que representa la operación asincrónica retornando un objeto Cart.</returns>
        public async Task<Cart> UpdateItemQuantityAsync(string buyerId, int productId, int quantity, int? userId = null)
        {
            Cart? cart = await FindAsync(buyerId, userId);
            if (cart == null)
            {
                Log.Information("El carrito no existe para el comprador especificado {BuyerId}.", buyerId);
                throw new KeyNotFoundException("El carrito no existe para el comprador especificado.");
            }
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                Log.Information("El producto no existe para el ID especificado {ProductId}.", productId);
                throw new KeyNotFoundException("El producto no existe para el ID especificado.");
            }
            var itemToUpdate = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (itemToUpdate == null)
            {
                throw new KeyNotFoundException("Producto del carrito no encontrado");
            }
            if (product.Stock < quantity)
            {
                Log.Information("El producto con ID {ProductId} no tiene suficiente stock para la cantidad solicitada.", productId);
                throw new ArgumentException("Stock insuficiente");
            }
            itemToUpdate.Quantity = quantity;
            cart.UpdatedAt = DateTime.UtcNow;
            RecalculateCartTotals(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        /// <summary>
        /// Recalcula los totales del carrito.
        /// </summary>
        private static void RecalculateCartTotals(Cart cart)
        {
            if (!cart.CartItems.Any())
            {
                cart.SubTotal = 0;
                cart.Total = 0;
                return;
            }

            cart.SubTotal = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            var totalWithDiscounts = cart.CartItems.Sum(ci =>
            {
                var itemTotal = ci.Product.Price * ci.Quantity;
                var discount = ci.Product.Discount;
                return (int)(itemTotal * (1 - (decimal)discount / 100));
            });

            cart.Total = totalWithDiscounts;

            Log.Information("Totales recalculados. SubTotal: {SubTotal}, Total: {Total}", cart.SubTotal, cart.Total);
        }

        /// <summary>
        /// Busca carrito usando estrategia: primero por userId, luego por buyerId.
        /// </summary>
        /// <param name="buyerId">El id de la cookie</param>
        /// <param name="userId">El id del usuario si es que está autenticado.</param>
        /// <returns>El carrito o null si no lo encuentras</returns>
        private async Task<Cart?> FindAsync(string buyerId, int? userId)
        {
            Cart? cart = null;

            if (userId.HasValue)
            {
                cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.Images).FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    Log.Information("Carrito encontrado por userId: {UserId}", userId);
                    if (cart.BuyerId != buyerId)
                    {
                        cart.BuyerId = buyerId;
                        cart.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        Log.Information("BuyerId actualizado en carrito existente. UserId: {UserId}, NuevoBuyerId: {BuyerId}", userId, buyerId);
                    }
                    return cart;
                }
            }

            cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.Images).FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.UserId == null);

            if (cart != null)
            {
                Log.Information("Carrito anónimo encontrado por buyerId: {BuyerId}", buyerId);

                if (userId.HasValue)
                {
                    cart.UserId = userId;
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    Log.Information("Carrito anónimo asociado con usuario. BuyerId: {BuyerId} → UserId: {UserId}", buyerId, userId);
                }
            }

            return cart;
        }
    }
}
