namespace DunnPharmaAPI.DTOs
{
    public class DevolucionProductoDto
    {
        public int IdProducto { get; set; }         // Producto devuelto
        public int IdCliente { get; set; }          // Cliente que devuelve
        public int IdPedido { get; set; }           // Pedido original
        public int IdLote { get; set; }             // Lote del producto vendido
        public int Piezas { get; set; }             // Piezas devueltas
        public string Factura { get; set; }         // Factura original
        public decimal Costo { get; set; }          // Costo de origen
        public decimal PrecioVenta { get; set; }    // Precio con que fue vendido
        public string Nota { get; set; }            // Motivo de devolución
    }
}
