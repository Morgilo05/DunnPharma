namespace DunnPharmaAPI.DTOs
{
    public class InventarioReporteDto
    {
        public string Producto { get; set; }
        public string Lote { get; set; }
        public int Piezas { get; set; }
        public decimal Costo { get; set; }
        public DateTime FechaCaducidad { get; set; }
        public string Factura { get; set; }
    }
}
