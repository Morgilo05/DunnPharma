namespace DunnPharmaAPI.DTOs
{
    public class VentaReporteDto
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public string Producto { get; set; }
        public int Piezas { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => PrecioUnitario * Piezas;
        public decimal CostoUnitario { get; set; }
        public decimal Utilidad => (PrecioUnitario - CostoUnitario) * Piezas;
    }
}
