namespace Tienda_UCN_api.Src.Application.DTO.CartDTO
{
    public class CartItemDTO
    {
        public required int ProductId { get; set; }
        public required string ProductTitle { get; set; }
        public required string ProductImageUrl { get; set; }
        public required int Price { get; set; }
        public required int Quantity { get; set; }

        public required int Discount { get; set; }

        public int SubTotalPrice => Price * Quantity;
        public int TotalPrice => (int)(Price * Quantity * (1 - Discount / 100m));
    }
}
