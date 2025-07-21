// ==========[ DunnPharma.API/DTOs/PedidoEdicionDto.cs ]==========
using DunnPharmaAPI.Models;

public class PedidoEdicionDto
{
    public Pedido Pedido { get; set; }
    public List<PedidoDetalle> Detalles { get; set; }
}