namespace DunnPharma.API.DTOs
{
    // DTO que representa un solo registro en el reporte del Cardex.
    public class CardexItemDto
    {
        public int IdCardex { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string? TipoMovimiento { get; set; }
        public string? NombreProducto { get; set; }
        public string? NombreCliente { get; set; }
        public int? Piezas { get; set; }
        public string? CodigoLote { get; set; }
        public decimal? Costo { get; set; }
        public decimal? PrecioVenta { get; set; }
        public string? Nota { get; set; }
        public string? UsuarioRegistro { get; set; }
    }
}