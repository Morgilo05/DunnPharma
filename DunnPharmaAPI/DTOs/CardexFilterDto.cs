// ==========[ DunnPharma.API/DTOs/CardexFilterDto.cs ]==========

namespace DunnPharma.API.DTOs
{
    // DTO para recibir los parámetros de filtro desde la aplicación cliente.
    public class CardexFilterDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdProducto { get; set; }
        public int? IdCliente { get; set; }
        public char? TipoMovimiento { get; set; }
        public string? UsuarioRegistro { get; set; }
    }
}



