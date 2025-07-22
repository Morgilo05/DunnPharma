namespace DunnPharmaAPI.DTOs
{
    /// <summary>
    /// Representa un único producto dentro del cuerpo de una solicitud para crear o modificar un pedido.
    /// Este DTO (Data Transfer Object) contiene la información esencial que el backend
    /// necesita para procesar cada línea del pedido.
    /// </summary>
    public class PedidoItemDto
    {
        /// <summary>
        /// El identificador único del producto que se está pidiendo.
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// La cantidad de unidades que se están pidiendo de este producto.
        /// </summary>
        public int Piezas { get; set; }

        /// <summary>
        /// El precio por unidad al que se está vendiendo el producto en esta transacción específica.
        /// </summary>
        public decimal PrecioUnitario { get; set; }
    }
}