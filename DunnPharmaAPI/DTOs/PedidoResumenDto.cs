namespace DunnPharmaAPI.DTOs
{
    public class PedidoResumenDto
    {
        public int IdPedido { get; set; }
        public string Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public string Estatus { get; set; }
        public decimal Total { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal UtilidadTotal { get; set; }
        public List<DetallePedidoDto> Productos { get; set; }
    }

    public class DetallePedidoDto
    {
        public string Producto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int PiezasPedidas { get; set; }
        public int PiezasSurtidas { get; set; }
        public decimal Subtotal => PrecioUnitario * PiezasSurtidas;
    }
}
