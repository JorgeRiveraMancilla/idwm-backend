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

        /// <summary>
        /// Crea un nuevo producto en el repositorio.
        /// </summary>
        /// <param name="product">El producto a crear.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el id del producto creado</returns>
        public async Task<int> CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        /// <summary>
        /// Crea o obtiene una marca por su nombre.
        /// </summary>
        /// <param name="brandName">El nombre de la marca.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con la marca creada o encontrada.</returns>
        public async Task<Brand> CreateOrGetBrandAsync(string brandName)
        {
            var brand = await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Name.ToLower() == brandName.ToLower());

            if (brand != null) { return brand; }
            brand = new Brand { Name = brandName };
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        /// <summary>
        /// Crea o obtiene una categoría por su nombre.
        /// </summary>
        /// <param name="categoryName">El nombre de la categoría.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con la categoría creada o encontrada.</returns>
        public async Task<Category> CreateOrGetCategoryAsync(string categoryName)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());

            if (category != null) { return category; }
            category = new Category { Name = categoryName };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        /// <summary>
        /// Retorna un producto específico por su ID.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.
                                        AsNoTracking().
                                        Where(p => p.Id == id && p.IsAvailable).
                                        Include(p => p.Category).
                                        Include(p => p.Brand).
                                        Include(p => p.Images)
                                        .FirstOrDefaultAsync();
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
                    p.Title.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Description.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Category.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Brand.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Status.ToString().Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Price.ToString().Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Stock.ToString().Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)
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
                .Include(p => p.Images.OrderBy(i => i.CreatedAt).Take(1))
                .AsNoTracking();

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();

                query = query.Where(p =>
                    p.Title.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Description.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Category.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Brand.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Status.ToString().Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Price.ToString().Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    p.Stock.ToString().Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)
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
