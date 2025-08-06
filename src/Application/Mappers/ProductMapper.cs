using Mapster;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.Src.Application.DTO.ProductDTO;
using Tienda_UCN_api.Src.Application.DTO.ProductDTO.CustomerDTO;

namespace Tienda_UCN_api.Src.Application.Mappers
{
    public class ProductMapper
    {
        private readonly IConfiguration _configuration;
        private readonly string? _defaultImageURL;
        private readonly int _fewUnitsAvailable;
        private readonly string? _soldOutMessage;
        private readonly string? _fewUnitsMessage;
        private readonly string? _inStockMessage;


        public ProductMapper(IConfiguration configuration)
        {
            _configuration = configuration;
            _defaultImageURL = _configuration.GetValue<string>("Products:DefaultImageUrl") ?? throw new InvalidOperationException("La URL de la imagen por defecto no puede ser nula.");
            _fewUnitsAvailable = _configuration.GetValue<int?>("Products:FewUnitsAvailable") ?? throw new InvalidOperationException("La configuración 'FewUnitsAvailable' no puede ser nula.");
            _soldOutMessage = _configuration.GetValue<string>("Products:SoldOutMessage") ?? throw new InvalidOperationException("La configuración 'SoldOutMessage' no puede ser nula.");
            _fewUnitsMessage = _configuration.GetValue<string>("Products:FewUnitsMessage") ?? throw new InvalidOperationException("La configuración 'FewUnitsMessage' no puede ser nula.");
            _inStockMessage = _configuration.GetValue<string>("Products:InStockMessage") ?? throw new InvalidOperationException("La configuración 'InStockMessage' no puede ser nula.");
        }

        public void ConfigureAllMappings()
        {
            ConfigureProductMappings();
        }

        public void ConfigureProductMappings()
        {
            TypeAdapterConfig<Product, ProductDetailDTO>.NewConfig()
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.ImagesURL, src => src.Images.Count() != 0 ? src.Images.Select(i => i.ImageUrl).ToList() : new List<string> { _defaultImageURL! })
                .Map(dest => dest.Price, src => src.Price.ToString("C"))
                .Map(dest => dest.Discount, src => src.Discount)
                .Map(dest => dest.Stock, src => src.Stock)
                .Map(dest => dest.StockIndicator, src => GetStockIndicator(src.Stock))
                .Map(dest => dest.CategoryName, src => src.Category.Name)
                .Map(dest => dest.BrandName, src => src.Brand.Name)
                .Map(dest => dest.StatusName, src => src.Status);

            TypeAdapterConfig<Product, ProductForCustomerDTO>.NewConfig()
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.MainImageURL, src => src.Images.FirstOrDefault() != null ? src.Images.First().ImageUrl : _defaultImageURL)
                .Map(dest => dest.Price, src => src.Price.ToString("C"))
                .Map(dest => dest.Discount, src => src.Discount);

            TypeAdapterConfig<Product, ProductForAdminDTO>.NewConfig()
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.MainImageURL, src => src.Images.FirstOrDefault() != null ? src.Images.First().ImageUrl : _defaultImageURL)
                .Map(dest => dest.Price, src => src.Price.ToString("C"))
                .Map(dest => dest.Stock, src => src.Stock)
                .Map(dest => dest.StockIndicator, src => GetStockIndicator(src.Stock))
                .Map(dest => dest.CategoryName, src => src.Category.Name)
                .Map(dest => dest.BrandName, src => src.Brand.Name)
                .Map(dest => dest.StatusName, src => src.Status);
        }

        /// <summary>
        /// Obtiene el indicador de stock basado en la cantidad disponible.
        /// </summary>
        /// <param name="stock">Stock del producto</param>
        /// <returns>Retorna el mensaje adecuado</returns>
        private string GetStockIndicator(int stock)
        {
            if (stock == 0) { return _soldOutMessage!; }
            if (stock <= _fewUnitsAvailable) { return _fewUnitsMessage!; }
            return _inStockMessage!;
        }
    }
}
