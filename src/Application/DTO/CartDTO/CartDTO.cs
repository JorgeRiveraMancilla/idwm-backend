namespace Tienda_UCN_api.Src.Application.DTO.CartDTO
{
    public class CartDTO
    {
        public required string BuyerId { get; set; }
        public required string? UserId { get; set; }
        public required List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
        public int SubTotalPrice => Items.Sum(item => item.SubTotalPrice);
        public int TotalPrice => Items.Sum(item => item.TotalPrice);
    }
}
