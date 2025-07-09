namespace DunnPharmaAPI.DTOs
{
    public class SalidaProductoDto
    {
        public int IdProducto { get; set; }         // Producto que se retira
        public int Piezas { get; set; }             // Cantidad a retirar
        public int IdLote { get; set; }             // Lote desde el cual se retiran las piezas
        public string Movimiento { get; set; }      // "S" para venta, "M" para merma
        public decimal Costo { get; set; }          // Costo real del producto
        public decimal? PrecioVenta { get; set; }   // Precio de venta si aplica
        public string Factura { get; set; }         // Número de factura si aplica
        public string Nota { get; set; }            // Nota de justificación en caso de merma
    }
}
