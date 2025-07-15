namespace DunnPharmaAPI.DTOs
{
    public class CardexDetalleDto
    {
        public string Producto { get; set; }
        public string Movimiento { get; set; }
        public int Piezas { get; set; }
        public decimal? Costo { get; set; }
        public decimal? PrecioVenta { get; set; }
        public string Factura { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Usuario { get; set; }
    }
}
