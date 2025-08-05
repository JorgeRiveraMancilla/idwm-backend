using Serilog;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.Src.Application.DTO.ProductDTO;
using Tienda_UCN_api.Src.Application.Services.Interfaces;
using Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces;

namespace Tienda_UCN_api.Src.Application.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IConfiguration _configuration;
        private readonly int _fewUnitsAvailable;

        public ProductService(IProductRepository productRepository, IConfiguration configuration)
        {
            _productRepository = productRepository;
            _configuration = configuration;
            _fewUnitsAvailable = int.Parse(_configuration["Products:FewUnitsAvailable"] ?? throw new InvalidOperationException("La configuración 'FewUnitsAvailable' no está definida."));
        }

        /// <summary>
        /// Retorna todos los productos para el administrador según los parámetros de búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una lista de productos filtrados para el administrador.</returns>
        public async Task<ListedProductsForAdminDTO> GetAllForAdminAsync(SearchParamsDTO searchParams)
        {
            Log.Information("Obteniendo productos para administrador con parámetros de búsqueda: {@SearchParams}", searchParams);
            var (products, totalCount) = await _productRepository.GetAllForAdminAsync(searchParams);
            var totalPages = (int)Math.Ceiling((double)totalCount / searchParams.PageSize);
            int currentPage = searchParams.PageNumber;
            int pageSize = searchParams.PageSize;
            if (currentPage < 1 || currentPage > totalPages)
            {
                throw new ArgumentOutOfRangeException("El número de página está fuera de rango.");
            }
            Log.Information("Total de productos encontrados: {TotalCount}, Total de páginas: {TotalPages}, Página actual: {CurrentPage}, Tamaño de página: {PageSize}", totalCount, totalPages, currentPage, pageSize);

            // Convertimos los productos filtrados a DTOs para la respuesta
            return new ListedProductsForAdminDTO
            {
                Products = products.Select(product => new ProductForAdminDTO
                {
                    Title = product.Title,
                    MainImageURL = product.Images.FirstOrDefault()?.ImageUrl, // Asumimos que siempre hay al menos una imagen (en la creación del producto se exige)
                    Price = product.Price.ToString("C"), // Formateo a moneda
                    Stock = product.Stock,
                    StockIndicator = product.Stock > _fewUnitsAvailable ? "Con Stock" : "Pocas unidades", //este mensaje es netamente informativo, no es necesario dejarlo en appsettings.json
                    CategoryName = product.Category.Name,
                    BrandName = product.Brand.Name,
                    StatusName = product.Status.ToString(),
                    UpdatedAt = product.UpdatedAt
                }).ToList(),
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = currentPage,
                PageSize = pageSize
            };

        }
    }
}
