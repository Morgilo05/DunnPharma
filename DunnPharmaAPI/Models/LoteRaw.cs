namespace DunnPharma.API.Models
{
    // Molde para los resultados del procedimiento almacenado.
    public class LoteRaw
    {
        public int IdLote { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string NombreLaboratorio { get; set; }
        public int PiezasDisponibles { get; set; }
        public string CodigoLote { get; set; }
        public decimal Costo { get; set; }
        public DateTime? FechaCaducidad { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public string Factura { get; set; }
    }
}