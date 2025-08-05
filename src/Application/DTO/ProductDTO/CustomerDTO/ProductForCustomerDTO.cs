using Tienda_UCN_api.src.Domain.Models;

namespace Tienda_UCN_api.Src.Application.DTO.ProductDTO.CustomerDTO
{
    public class ProductForCustomerDTO
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public required string Price { get; set; }
        public required int Stock { get; set; }
        public required string StockIndicator { get; set; }
        public required string CategoryName { get; set; }
        public required string BrandName { get; set; }
        public required string StatusName { get; set; }
    }
}
