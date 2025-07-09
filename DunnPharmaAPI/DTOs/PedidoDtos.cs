namespace DunnPharma.API.DTOs
{
    /// <summary>Resumen simplificado para listado.</summary>
    public class PedidoResumenDto
    {
        public int IdPedido { get; set; }
        public string? Cliente { get; set; }
        public decimal? Total { get; set; }
        public decimal? CostoTotal { get; set; }
        public decimal? UtilidadTotal { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string? UsuarioRegistro { get; set; }
    }

    /// <summary>Detalle editable de un pedido.</summary>
    public class DetallePedidoDto
    {
        public int IdProducto { get; set; }
        public int Piezas { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    /// <summary>Cuerpo para la edición.</summary>
    public class EditarPedidoDto
    {
        public IList<DetallePedidoDto> Detalles { get; set; } = [];
        public string UsuarioEdicion { get; set; } = string.Empty;
    }
}
