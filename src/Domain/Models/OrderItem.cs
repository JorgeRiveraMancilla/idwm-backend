namespace Tienda_UCN_api.src.Domain.Models
{
    public class OrderItem
    {
        /// <summary>
        /// Identificador único del artículo del pedido.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Cantidad del artículo en el pedido.
        /// </summary>
        public required int Quantity { get; set; }

        /// <summary>
        /// Precio del artículo en el momento del pedido.
        /// </summary>
        public required int PriceAtMoment { get; set; }

        /// <summary>
        /// Descuento aplicado al artículo.
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// Identificador del producto asociado al artículo del pedido.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Producto asociado al artículo del pedido.
        /// </summary>
        public Product Product { get; set; } = null!;

        /// <summary>
        /// Identificador del pedido al que pertenece el artículo.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Pedido al que pertenece el artículo.
        /// </summary>
        public Order Order { get; set; } = null!;
    }
}
