namespace DunnPharma.API.Models
{
    // Esta clase representa la estructura de datos "plana" que devuelve
    // el procedimiento almacenado 'sp_GetInventarioCompleto'.
    // EF Core la usará para mapear los resultados.
    public class InventarioItemRaw
    {
        public string NombreLaboratorio { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int TotalPiezas { get; set; }
        public int IdLote { get; set; }
        public string CodigoLote { get; set; }
        public int PiezasPorLote { get; set; }
        public decimal Costo { get; set; }
        public DateTime? FechaCaducidad { get; set; }
        public DateTime FechaEntrada { get; set; }
        public string UsuarioRegistro { get; set; }
        public string Factura { get; set; }
    }
}