namespace DunnPharmaAPI.DTOs
{
    public class SurtidoProductoDto
    {
        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public int Piezas { get; set; }
        public int IdLote { get; set; }
        public decimal Costo { get; set; }
        public decimal PrecioVenta { get; set; }
        public string Factura { get; set; }
    }
}
