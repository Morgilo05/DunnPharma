namespace DunnPharmaAPI.DTOs
{
    public class PedidoDetalleDto
    {
        public int IdCliente { get; set; }              // Cliente que realiza el pedido
        public List<ItemPedidoDto> Productos { get; set; } // Lista de productos solicitados

        public int IdDetalle { get; set; }
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; }
        public int? Piezas { get; set; }
        public decimal? PrecioUnitario { get; set; }
    }

    
}
