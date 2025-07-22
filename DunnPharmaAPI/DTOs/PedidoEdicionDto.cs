// ==========[ DunnPharma.API/DTOs/PedidoEdicionDto.cs ]==========
using DunnPharmaAPI.DTOs;
using DunnPharmaAPI.Models;

public class PedidoEdicionDto
{
    public Pedido Pedido { get; set; }
    public List<PedidoDetalle> Detalles { get; set; }

    public int IdPedido { get; set; }
    public int IdCliente { get; set; }
    public string NombreCliente { get; set; }
    public List<PedidoDetalleDto> Detalle { get; set; }
}