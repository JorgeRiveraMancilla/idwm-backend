namespace Tienda_UCN_api.Src.Application.DTO.CartDTO
{
    public class CartDTO
    {
        public required string BuyerId { get; set; }
        public required int? UserId { get; set; }
        public required List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
        public int SubTotalPrice { get; set; }
        public int TotalPrice { get; set; }
    }
}
