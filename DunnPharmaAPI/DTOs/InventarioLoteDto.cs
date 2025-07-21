// ==========[ DunnPharma.API/DTOs/InventarioLoteDto.cs ]==========

namespace DunnPharma.API.DTOs
{
    // DTO para representar el detalle de un lote específico en el inventario.
    public class InventarioLoteDto
    {
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

// ==========[ DunnPharma.API/DTOs/InventarioProductoDto.cs ]==========

namespace DunnPharma.API.DTOs
{
    // DTO para representar un producto en el inventario, incluyendo su total global
    // y el desglose de sus lotes.
    public class InventarioProductoDto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int TotalPiezas { get; set; }
        public List<InventarioLoteDto> Lotes { get; set; } = new List<InventarioLoteDto>();
    }
}


// ==========[ DunnPharma.API/DTOs/InventarioLaboratorioDto.cs ]==========
namespace DunnPharma.API.DTOs
{
    // DTO para agrupar los productos por laboratorio en el reporte de inventario.
    public class InventarioLaboratorioDto
    {
        public string NombreLaboratorio { get; set; }
        public List<InventarioProductoDto> Productos { get; set; } = new List<InventarioProductoDto>();
    }
}