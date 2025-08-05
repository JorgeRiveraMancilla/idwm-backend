using System.IO.Compression;
using Microsoft.EntityFrameworkCore;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.src.Infrastructure.Data;
using Tienda_UCN_api.Src.Application.DTO.ProductDTO;
using Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces;

namespace Tienda_UCN_api.Src.Infrastructure.Repositories.Implements
{
    /// <summary>
    /// Implementación del repositorio de productos que interactúa con la base de datos.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        private readonly int _defaultPageSize;

        public ProductRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _defaultPageSize = _configuration.GetValue<int?>("Products:DefaultPageSize") ?? throw new ArgumentNullException("El tamaño de página por defecto no puede ser nulo.");
        }

        // <summary>
        /// Retorna una lista de productos para el administrador con los parámetros de búsqueda especificados.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con una lista de productos para el administrador y el conteo total de productos.</returns>
        public async Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForAdminAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images.OrderBy(i => i.CreatedAt).Take(1)) // Cargamos la URL de la imagen principal a la hora de crear el producto
                .AsNoTracking();

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();

                query = query.Where(p =>
                    p.Title.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Category.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Brand.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Status.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Price.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Stock.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize ?? _defaultPageSize)
                .Take(searchParams.PageSize ?? _defaultPageSize)
                .ToArrayAsync();

            return (products, totalCount);
        }

        /// <summary>
        /// Retorna una lista de productos para el cliente con los parámetros de búsqueda especificados.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con una lista de productos para el cliente y el conteo total de productos.</returns>
        public async Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForCustomerAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Products
                .Where(p => p.IsAvailable)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .AsNoTracking();

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();

                query = query.Where(p =>
                    p.Title.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Category.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Brand.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Status.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Price.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    p.Stock.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                );
            }

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize ?? _defaultPageSize)
                .Take(searchParams.PageSize ?? _defaultPageSize)
                .ToArrayAsync();

            return (products, totalCount);
        }
    }
}
