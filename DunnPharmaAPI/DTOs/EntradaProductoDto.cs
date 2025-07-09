namespace DunnPharmaAPI.DTOs
{
    public class EntradaProductoDto
    {
        public int IdProducto { get; set; }             // Producto que ingresa
        public int Piezas { get; set; }                 // Número de piezas
        public string CodigoLote { get; set; }          // Lote o folio de producción
        public decimal Costo { get; set; }              // Costo por pieza
        public DateTime FechaCaducidad { get; set; }    // Fecha de vencimiento
        public string Factura { get; set; }             // Número de factura
    }
}
