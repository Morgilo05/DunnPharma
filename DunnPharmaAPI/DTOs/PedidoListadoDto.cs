// ==========[ DunnPharma.API/DTOs/PedidoListadoDto.cs ]==========
using DunnPharmaAPI.Models;

public class PedidoListadoDto
{
    public int IdPedido { get; set; }
    public string NombreCliente { get; set; }
    public decimal? Total { get; set; }
    public decimal? CostoTotal { get; set; }
    public decimal? UtilidadTotal { get; set; }
    public string UsuarioRegistro { get; set; }
    public DateTime? FechaRegistro { get; set; }
}

