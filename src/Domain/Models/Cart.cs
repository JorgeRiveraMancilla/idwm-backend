namespace Tienda_UCN_api.src.Domain.Models
{
    public class Cart
    {
        /// <summary>
        /// Identificador único del carrito de compras.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Total del carrito de compras incluyendo descuentos.
        /// </summary>
        public string Total { get; set; } = string.Empty;

        /// <summary>
        /// Subtotal del carrito de compras sin descuentos.
        /// </summary>
        public string SubTotal { get; set; } = string.Empty;

        /// <summary>
        /// Usuario que realizó la compra.
        /// </summary>
        public User Buyer { get; set; } = null!;

        /// <summary>
        /// Lista de artículos en el carrito de compras.
        /// </summary>
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        /// <summary>
        /// Fecha de creación del carrito de compras.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de actualización del carrito de compras.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
