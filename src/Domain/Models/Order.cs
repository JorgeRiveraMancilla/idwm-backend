namespace Tienda_UCN_api.src.Domain.Models
{
    public class Order
    {
        /// <summary>
        /// Identificador único del pedido.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código del pedido.
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Total del pedido con descuentos.
        /// </summary>
        public required int Total { get; set; }

        /// <summary>
        /// Total del pedido sin descuentos.
        /// </summary>
        public required int SubTotal { get; set; }

        /// <summary>
        /// Lista de artículos del pedido.
        /// </summary>
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        /// <summary>
        /// Fecha de creación del pedido.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de actualización del pedido.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
