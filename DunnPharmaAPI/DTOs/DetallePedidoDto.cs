namespace DunnPharmaAPI.DTOs
{
    public class DetallePedidoDto
    {
        public string Producto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int PiezasPedidas { get; set; }
        public int PiezasSurtidas { get; set; }
        public decimal Subtotal => PrecioUnitario * PiezasSurtidas;
    }
}
